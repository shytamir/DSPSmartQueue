using System;
using System.Reflection;
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
                var errors = BindingChecks.Validate(
                    Assembly.Load("Assembly-CSharp"), Assembly.Load("UnityEngine.UI"),
                    Assembly.Load("UnityEngine.CoreModule"), typeof(int).Assembly);
                if (errors.Count != 0)
                {
                    Logger.LogError("Incompatible queue UI; plugin disabled: " + string.Join("; ", errors));
                    enabled = false;
                    return;
                }

                harmony = new Harmony(PluginGuid);
                QueueHooks.Start(harmony, Logger);
                Logger.LogInfo("Queue presentation and guarded native input enabled.");
            }
            catch (Exception exception)
            {
                Logger.LogError("Unable to validate queue bindings; plugin disabled: " + exception);
                if (harmony != null) harmony.UnpatchSelf();
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (harmony == null) return;
            try { QueueHooks.Stop(harmony); }
            catch (Exception exception) { Logger.LogError("Native queue restoration failed; input guard retained: " + exception); }
        }
    }
}
