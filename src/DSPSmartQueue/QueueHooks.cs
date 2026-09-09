using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DSPSmartQueue
{
    internal static class QueueHooks
    {
        private static readonly Func<int, uint> IconForRecipe = id => GameMain.iconSet.recipeIconIndex[id];
        private static readonly MethodInfo Upload = AccessTools.Method(typeof(UIReplicatorWindow), "SetBufferData");
        private static readonly MethodInfo RefreshIcons = AccessTools.Method(typeof(UIReplicatorWindow), "RefreshQueueIcons");
        private static readonly MethodInfo ActivateText = AccessTools.Method(typeof(UIReplicatorWindow), "ActiveQueueText");
        private static readonly MethodInfo DeactivateText = AccessTools.Method(typeof(UIReplicatorWindow), "DeactiveQueueText");
        private static readonly MethodInfo TestHover = AccessTools.Method(typeof(UIReplicatorWindow), "TestMouseQueueIndex");
        private static readonly FieldInfo Tasks = AccessTools.Field(typeof(UIReplicatorWindow), "taskQueue");
        private static readonly QueueRecovery recovery = new QueueRecovery();
        private static ManualLogSource log;
        private static UIReplicatorWindow window;
        private static QueueStrip<ForgeTask> strip;

        private struct ClickContext
        {
            internal QueueStrip<ForgeTask> Owner;
            internal QueueStrip<ForgeTask>.ClickState State;
        }

        internal static void Start(Harmony harmony, ManualLogSource logger)
        {
            log = logger;
            // No filtered upload can occur before guarded input is installed.
            harmony.Patch(AccessTools.Method(typeof(UIReplicatorWindow), "OnQueueMouseDown"),
                prefix: new HarmonyMethod(typeof(QueueHooks), nameof(ClickPrefix)),
                finalizer: new HarmonyMethod(typeof(QueueHooks), nameof(ClickFinalizer)));
            foreach (var name in new[] { "_OnOpen", "_OnClose", "_OnFree", "_OnDestroy" })
                harmony.Patch(AccessTools.Method(typeof(UIReplicatorWindow), name),
                    prefix: new HarmonyMethod(typeof(QueueHooks), nameof(ResetPrefix)));
            harmony.Patch(Upload, prefix: new HarmonyMethod(typeof(QueueHooks), nameof(PresentPrefix)),
                finalizer: new HarmonyMethod(typeof(QueueHooks), nameof(PresentFinalizer)));
            recovery.Activate();
        }

        private static void PresentPrefix(UIReplicatorWindow __instance, List<ForgeTask> ___taskQueue,
            uint[] ___queueIndexArray, uint[] ___queueStateArray, Text[] ___queueNumTexts,
            int ___mouseQueueIndex, out bool __state)
        {
            __state = false;
            if (!recovery.ProjectionEnabled) return;
            if (strip != null && strip.ClickInProgress) return;
            if (!ReferenceEquals(window, __instance))
            {
                ClearMapping();
                window = __instance;
                strip = new QueueStrip<ForgeTask>(ForgeRequests.Create());
            }
            strip.Invalidate();
            try
            {
                if (___queueNumTexts == null || ___queueNumTexts.Length != VisibleRequests<ForgeTask>.Capacity)
                    throw new InvalidOperationException("Expected fourteen queue quantity labels.");
                strip.Prepare(___taskQueue, ___queueIndexArray, ___queueStateArray, ___mouseQueueIndex, IconForRecipe);
                for (int i = 0; i < ___queueNumTexts.Length; i++)
                {
                    var label = ___queueNumTexts[i];
                    bool populated = i < strip.Requests.VisibleCount;
                    label.text = populated ? strip.Requests.GetSlot(i).Quantity.ToString() : "";
                    if (populated) label.color = __instance.mainTaskTextColor;
                    label.gameObject.SetActive(populated);
                }
                __instance.queueCountText.text = strip.Requests.TotalCount.ToString();
                __state = true;
            }
            catch (Exception error) { Recover(__instance, error); }
            // The original always uploads its recipe buffers; timing/progress are untouched.
        }

        private static Exception PresentFinalizer(UIReplicatorWindow __instance, bool __state, Exception __exception)
        {
            if (recovery.ProjectionEnabled && ReferenceEquals(window, __instance))
            {
                if (__state && __exception == null) strip.Commit();
                else if (__exception != null) Recover(__instance, __exception);
                else strip.Invalidate();
            }
            return __exception;
        }

        private static bool ClickPrefix(UIReplicatorWindow __instance, BaseEventData evtData,
            List<ForgeTask> ___taskQueue, ref int ___mouseQueueIndex,
            out ClickContext __state)
        {
            __state = default(ClickContext);
            if (!recovery.ProjectionEnabled)
            {
                if (!recovery.NativeInputAllowed && evtData != null) evtData.Use();
                return recovery.NativeInputAllowed;
            }
            var pointer = evtData as PointerEventData;
            if (pointer == null || (pointer.button != PointerEventData.InputButton.Left &&
                                    pointer.button != PointerEventData.InputButton.Right)) return false;
            try
            {
                if (ReferenceEquals(window, __instance))
                {
                    __state.Owner = strip;
                    if (strip.BeginClick(___taskQueue, ___mouseQueueIndex, ref ___mouseQueueIndex, out __state.State))
                        return true;
                }
            }
            catch (Exception error) { Recover(__instance, error); }
            evtData.Use();
            Refresh(__instance);
            return false;
        }

        private static Exception ClickFinalizer(UIReplicatorWindow __instance, ref int ___mouseQueueIndex,
            ClickContext __state, Exception __exception)
        {
            // Restore even if the original threw, before doing anything that can itself fail.
            if (__state.State.Translated)
            {
                __state.Owner.EndClick(ref ___mouseQueueIndex, __state.State);
                if (__exception != null) Recover(__instance, __exception);
                else if (ReferenceEquals(window, __instance) && ReferenceEquals(strip, __state.Owner)) Refresh(__instance);
            }
            return __exception; // Do not swallow or replay native failures.
        }

        private static void Refresh(UIReplicatorWindow target)
        {
            if (!recovery.ProjectionEnabled) return;
            if (strip != null && strip.ClickInProgress) return;
            try { Upload.Invoke(target, null); }
            catch (Exception error)
            {
                Recover(target, error);
            }
        }

        internal static void Stop(Harmony harmony)
        {
            var target = window;
            try { recovery.Stop(() => RestoreNative(target), () => CloseNative(target), harmony.UnpatchSelf, ClearMapping); }
            catch { window = target; throw; }
            window = null;
            log = null;
        }

        private static void ResetPrefix(UIReplicatorWindow __instance)
        {
            if (ReferenceEquals(window, __instance))
            {
                ClearMapping();
                if (recovery.ProjectionEnabled || recovery.NativeInputAllowed) window = null;
            }
        }

        private static void ClearMapping()
        {
            if (strip != null) strip.Clear();
            strip = null;
        }

        private static void Recover(UIReplicatorWindow target, Exception error)
        {
            // Recovery is attempted once per failure transition, not on every frame/click.
            if (!recovery.ProjectionEnabled) return;
            try
            {
                recovery.Recover(() => RestoreNative(target), () => CloseNative(target));
                log.LogError("Queue adapter disabled; native UI restored or closed: " + error);
            }
            catch (Exception recoveryFailure) { log.LogError(recoveryFailure); }
            finally
            {
                ClearMapping();
                window = recovery.NativeInputAllowed ? null : target;
            }
        }

        private static void RestoreNative(UIReplicatorWindow target)
        {
            // Closed/destroyed native windows cannot expose stale controls.
            if (target != null && !target.active && target.gameObject.activeSelf)
                throw new InvalidOperationException("Inactive native queue window is still visible.");
            if (target != null && target.active)
            {
                var tasks = (List<ForgeTask>)Tasks.GetValue(target);
                RefreshIcons.Invoke(target, null);
                for (int i = 0; i < VisibleRequests<ForgeTask>.Capacity; i++)
                    (i < tasks.Count ? ActivateText : DeactivateText).Invoke(target, new object[] { i });
                target.queueCountText.text = tasks.Count.ToString();
                TestHover.Invoke(target, null);
                Upload.Invoke(target, null);
            }
        }

        private static void CloseNative(UIReplicatorWindow target)
        {
            if (target == null) return;
            target._Close();
            if (target.active || target.gameObject.activeSelf)
                throw new InvalidOperationException("Native queue window did not close.");
        }
    }
}
