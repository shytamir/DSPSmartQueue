using System;
using System.Reflection;
using BepInEx;

namespace DSPSmartQueue
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion.Value)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "shytamir.dspsmartqueue";
        public const string PluginName = "DSP Smart Queue";

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

                Logger.LogInfo("Queue binding signatures validated. Foundation only; no queue hooks applied.");
            }
            catch (Exception exception)
            {
                Logger.LogError("Unable to validate queue bindings; plugin disabled: " + exception);
                enabled = false;
            }
        }
    }
}
