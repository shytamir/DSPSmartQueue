using System;
using BepInEx;
using HarmonyLib;

namespace DSPSmartQueue
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion.Value)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "smartqueue";
        public const string PluginName = "DSP Smart Queue";
        private Harmony harmony;

        private void Awake()
        {
            try
            {
                harmony = new Harmony(PluginGuid);
                QueueHooks.Start(harmony, Logger);
                Logger.LogInfo("Queue presentation and guarded native input enabled.");
            }
            catch
            {
                Stop();
                throw;
            }
        }

        private void OnDestroy()
        {
            Stop();
        }

        private void OnDisable()
        {
            Stop();
        }

        private void Stop()
        {
            if (harmony == null) return;
            try { QueueHooks.Stop(harmony); harmony = null; }
            catch (Exception exception) { Logger.LogError("Native queue restoration failed; input guard retained: " + exception); }
        }
    }
}
