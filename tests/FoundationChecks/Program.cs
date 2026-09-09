using System.Reflection;
using DSPSmartQueue;

bool logicOnly = args.Length == 1 && args[0] == "--logic-only";
if (!logicOnly && args.Length != 3) throw new ArgumentException("Expected --logic-only or ManagedPath, DependencyPath, and plugin DLL path.");
VisibleRequestsChecks.Run();
QueueStripChecks.Run();
QueueRecoveryChecks.Run();
if (logicOnly)
{
    Console.WriteLine("PASS: offline queue logic. Real-reference compilation, metadata, Harmony execution, and Unity rendering were not checked.");
    return;
}

// MetadataLoadContext never loads game code into the executing runtime.
var paths = Directory.GetFiles(args[0], "*.dll").Concat(Directory.GetFiles(args[1], "*.dll"))
    .Append(Path.GetFullPath(args[2]))
    // Prefer the selected game's references over any older copies in offline artifacts.
    .GroupBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToArray();
using var metadata = new MetadataLoadContext(new PathAssemblyResolver(paths), "mscorlib");
var game = metadata.LoadFromAssemblyPath(Path.Combine(args[0], "Assembly-CSharp.dll"));
var plugin = metadata.LoadFromAssemblyPath(Path.GetFullPath(args[2]));
HookBindingChecks.Run(game, plugin);
var entry = plugin.GetType("DSPSmartQueue.Plugin", true)!;
var bepInEx = metadata.LoadFromAssemblyName("BepInEx");
if (entry.BaseType != bepInEx.GetType("BepInEx.BaseUnityPlugin", true)) throw new Exception("Wrong plugin base.");
var attribute = entry.GetCustomAttributesData().Single(a => a.AttributeType == bepInEx.GetType("BepInEx.BepInPlugin", true));
if (!Equals(attribute.ConstructorArguments[0].Value, entry.GetField("PluginGuid")!.GetRawConstantValue()) ||
    !Equals(attribute.ConstructorArguments[1].Value, entry.GetField("PluginName")!.GetRawConstantValue()) ||
    (string?)attribute.ConstructorArguments[2].Value != plugin.GetName().Version!.ToString(3))
    throw new Exception("Plugin metadata/version mismatch.");
if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name is "Assembly-CSharp" or "DSPSmartQueue"))
    throw new Exception("Game or plugin loaded into execution context.");
Console.WriteLine("PASS: supplied hook metadata signatures and plugin identity/version; no game/plugin execution load.");
