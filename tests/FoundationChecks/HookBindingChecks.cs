using System.Reflection;

internal static class HookBindingChecks
{
    internal static void Run(Assembly game, Assembly plugin)
    {
        var window = game.GetType("UIReplicatorWindow", true)!;
        var hooks = plugin.GetType("DSPSmartQueue.QueueHooks", true)!;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (var pair in new[] { ("PresentPrefix", "PresentFinalizer", "SetBufferData"), ("ClickPrefix", "ClickFinalizer", "OnQueueMouseDown") })
        {
            var original = window.GetMethod(pair.Item3, flags)!;
            var prefix = hooks.GetMethod(pair.Item1, flags)!;
            var finalizer = hooks.GetMethod(pair.Item2, flags)!;
            foreach (var hook in new[] { prefix, finalizer })
            {
                if (!hook.IsStatic) throw new Exception("Harmony hook must be static.");
                foreach (var parameter in hook.GetParameters())
                {
                    var name = parameter.Name!;
                    var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType()! : parameter.ParameterType;
                    if (name.StartsWith("___", StringComparison.Ordinal))
                    {
                        var field = window.GetField(name[3..], flags);
                        if (field == null || field.IsStatic || type != field.FieldType)
                            throw new Exception("Invalid Harmony field injection: " + name);
                    }
                    else if (name == "__instance")
                    {
                        if (type != window) throw new Exception("Invalid instance injection.");
                    }
                    else if (!name.StartsWith("__", StringComparison.Ordinal))
                    {
                        if (!original.GetParameters().Any(p => p.Name == name && p.ParameterType == type))
                            throw new Exception("Invalid native argument injection: " + name);
                    }
                }
            }
            var writtenState = prefix.GetParameters().Single(p => p.Name == "__state");
            var readState = finalizer.GetParameters().Single(p => p.Name == "__state");
            if (!writtenState.IsOut || writtenState.ParameterType.GetElementType() != readState.ParameterType)
                throw new Exception("Prefix/finalizer state types disagree.");
            if (finalizer.ReturnType.FullName != "System.Exception") throw new Exception("Finalizer must preserve native exceptions.");
        }
        Console.WriteLine("PASS: Harmony hook native argument/field injection and prefix/finalizer state signatures against real metadata.");
    }
}
