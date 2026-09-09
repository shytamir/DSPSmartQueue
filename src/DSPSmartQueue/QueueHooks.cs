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
        private static bool running;
        private static ManualLogSource log;
        private static UIReplicatorWindow window;
        private static QueueStrip<ForgeTask> strip;

        internal static void Start(Harmony harmony, ManualLogSource logger)
        {
            log = logger;
            // No filtered upload can occur before guarded input is installed.
            harmony.Patch(AccessTools.Method(typeof(UIReplicatorWindow), "OnQueueMouseDown"),
                prefix: new HarmonyMethod(typeof(QueueHooks), nameof(ClickPrefix)),
                finalizer: new HarmonyMethod(typeof(QueueHooks), nameof(ClickFinalizer)));
            harmony.Patch(Upload, prefix: new HarmonyMethod(typeof(QueueHooks), nameof(PresentPrefix)),
                finalizer: new HarmonyMethod(typeof(QueueHooks), nameof(PresentFinalizer)));
            running = true;
        }

        private static void PresentPrefix(UIReplicatorWindow __instance, List<ForgeTask> ___taskQueue,
            uint[] ___queueIndexArray, uint[] ___queueStateArray, Text[] ___queueNumTexts,
            int ___mouseQueueIndex, out bool __state)
        {
            __state = false;
            if (!running) return;
            if (strip != null && strip.ClickInProgress) return;
            if (!ReferenceEquals(window, __instance))
            {
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
            catch (Exception error) { log.LogError("Queue presentation failed; task clicks blocked: " + error); }
            // The original always uploads its recipe buffers; timing/progress are untouched.
        }

        private static Exception PresentFinalizer(UIReplicatorWindow __instance, bool __state, Exception __exception)
        {
            if (running && ReferenceEquals(window, __instance))
            {
                if (__state && __exception == null) strip.Commit();
                else strip.Invalidate();
            }
            return __exception;
        }

        private static bool ClickPrefix(UIReplicatorWindow __instance, BaseEventData evtData,
            List<ForgeTask> ___taskQueue, ref int ___mouseQueueIndex,
            out QueueStrip<ForgeTask>.ClickState __state)
        {
            __state = default(QueueStrip<ForgeTask>.ClickState);
            if (!running) return strip == null; // During failed shutdown, never expose raw input over filtered UI.
            var pointer = evtData as PointerEventData;
            if (pointer == null || (pointer.button != PointerEventData.InputButton.Left &&
                                    pointer.button != PointerEventData.InputButton.Right)) return false;
            try
            {
                if (ReferenceEquals(window, __instance) && strip.BeginClick(___taskQueue,
                    ___mouseQueueIndex, ref ___mouseQueueIndex, out __state)) return true;
            }
            catch (Exception error) { log.LogError("Queue input resolution failed: " + error); }
            evtData.Use();
            Refresh(__instance);
            return false;
        }

        private static Exception ClickFinalizer(UIReplicatorWindow __instance, ref int ___mouseQueueIndex,
            QueueStrip<ForgeTask>.ClickState __state, Exception __exception)
        {
            // Restore even if the original threw, before doing anything that can itself fail.
            if (__state.Translated)
            {
                strip.EndClick(ref ___mouseQueueIndex, __state);
                if (__exception == null) Refresh(__instance);
            }
            return __exception; // Do not swallow or replay native failures.
        }

        private static void Refresh(UIReplicatorWindow target)
        {
            if (strip != null && strip.ClickInProgress) return;
            try { Upload.Invoke(target, null); }
            catch (Exception error)
            {
                if (strip != null) strip.Invalidate();
                log.LogError("Queue refresh failed; task clicks blocked: " + error);
            }
        }

        internal static void Stop(Harmony harmony)
        {
            running = false;
            // Restore the native strip before removing its matching input guard.
            if (window != null && strip != null)
            {
                var tasks = (List<ForgeTask>)Tasks.GetValue(window);
                RefreshIcons.Invoke(window, null);
                for (int i = 0; i < VisibleRequests<ForgeTask>.Capacity; i++)
                    (i < tasks.Count ? ActivateText : DeactivateText).Invoke(window, new object[] { i });
                window.queueCountText.text = tasks.Count.ToString();
                TestHover.Invoke(window, null);
                Upload.Invoke(window, null);
            }
            harmony.UnpatchSelf();
            strip = null;
            window = null;
        }
    }
}
