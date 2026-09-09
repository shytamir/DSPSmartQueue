using System.Reflection;
using DSPSmartQueue;

bool logicOnly = args.Length == 1 && args[0] == "--logic-only";
if (!logicOnly && args.Length != 3) throw new ArgumentException("Expected --logic-only or ManagedPath, DependencyPath, and plugin DLL path.");
VisibleRequestsChecks.Run();
QueueStripChecks.Run();
QueueRecoveryChecks.Run();
var errors = new List<string>();
BindingChecks.RequireField(errors, typeof(Fixture), "Value", typeof(int), false);
BindingChecks.RequireMethod(errors, typeof(Fixture), "Click", typeof(void), true, new[] { typeof(int) });
if (errors.Count != 0) throw new Exception("Valid fixture rejected.");

void MustReject(Action<List<string>> check)
{
    var failure = new List<string>();
    check(failure);
    if (failure.Count != 1 || string.IsNullOrWhiteSpace(failure[0]))
        throw new Exception("Invalid binding did not report exactly one incompatibility.");
}
MustReject(e => BindingChecks.RequireField(e, typeof(Fixture), "Missing", typeof(int), false));
MustReject(e => BindingChecks.RequireField(e, typeof(Fixture), "Value", typeof(string), false));
MustReject(e => BindingChecks.RequireField(e, typeof(Fixture), "Value", typeof(int), true));
MustReject(e => BindingChecks.RequireField(e, typeof(Fixture), "Shared", typeof(int), false));
MustReject(e => BindingChecks.RequireField(e, typeof(Fixture), "ReadOnly", typeof(int), false));
MustReject(e => BindingChecks.RequireField(e, typeof(DerivedFixture), "Value", typeof(int), false));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "Missing", typeof(void), true, Type.EmptyTypes));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "Click", typeof(int), true, new[] { typeof(int) }));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "Click", typeof(void), true, new[] { typeof(string) }));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "StaticClick", typeof(void), true, Type.EmptyTypes));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "PublicClick", typeof(void), true, Type.EmptyTypes));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "GenericClick", typeof(void), true, Type.EmptyTypes));
MustReject(e => BindingChecks.RequireMethod(e, typeof(Fixture), "WidenedClick", typeof(void), true, new[] { typeof(int) }));
try
{
    BindingChecks.Validate(typeof(Fixture).Assembly, typeof(Fixture).Assembly, typeof(Fixture).Assembly, typeof(int).Assembly);
    throw new Exception("Missing game type accepted.");
}
catch (TypeLoadException) { }

if (logicOnly)
{
    Console.WriteLine("PASS: offline logic and binding fixtures. Real-reference compilation, metadata, Harmony execution, and Unity rendering were not checked.");
    return;
}

// MetadataLoadContext never loads game code into the executing runtime.
var paths = Directory.GetFiles(args[0], "*.dll").Concat(Directory.GetFiles(args[1], "*.dll"))
    .Append(Path.GetFullPath(args[2]))
    // Prefer the selected game's references over any older copies in offline artifacts.
    .GroupBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToArray();
using var metadata = new MetadataLoadContext(new PathAssemblyResolver(paths), "mscorlib");
var game = metadata.LoadFromAssemblyPath(Path.Combine(args[0], "Assembly-CSharp.dll"));
var ui = metadata.LoadFromAssemblyPath(Path.Combine(args[0], "UnityEngine.UI.dll"));
var unity = metadata.LoadFromAssemblyPath(Path.Combine(args[0], "UnityEngine.CoreModule.dll"));
var core = metadata.LoadFromAssemblyPath(Path.Combine(args[0], "mscorlib.dll"));
var actualErrors = BindingChecks.Validate(game, ui, unity, core);
if (actualErrors.Count != 0) throw new Exception(string.Join(Environment.NewLine, actualErrors));
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
Console.WriteLine("PASS: valid fixture, 13 invalid member cases, missing type, supplied reference metadata signatures, plugin identity/version; no game/plugin execution load.");

class Fixture
{
    public int Value = 0;
    public static int Shared = 0;
    public readonly int ReadOnly = 0;
    private void Click(int value) { }
    private static void StaticClick() { }
    public void PublicClick() { }
    private void GenericClick<T>() { }
    private void WidenedClick(long value) { }
}
class DerivedFixture : Fixture { }
