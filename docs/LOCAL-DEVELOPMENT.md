# Local preparation

For current status and active work, see the authoritative [PROJECT.md](PROJECT.md).

Run from the repository using PowerShell 7. First initialization needs two local
directories; these are saved only under ignored `artifacts/.runtime-tools/`:

```powershell
./scripts/Initialize-LocalTools.ps1 -ManagedPath '<game Managed directory>' -DependencyPath '<offline BepInEx and Harmony DLL directory>'
```

Subsequent sessions use one command:

```powershell
. ./scripts/Enter-LocalTools.ps1
```

Activation checks PowerShell, .NET SDK, Git, ripgrep, the pinned ILSpy executable,
and reference hashes. It adds the local tool directory to this session's PATH
without persistent environment changes. It does not download anything. Re-running
initialization skips an already installed matching ILSpy version and refreshes the
baseline; review reported reference drift before deliberately doing so.

Inspect a type statically, then search the returned local file:

```powershell
$source = ./scripts/Inspect-Type.ps1 -Type UIReplicatorWindow
rg -n 'SetBufferData|OnQueueMouseDown' $source
./scripts/Inspect-Type.ps1 -Assembly 0Harmony.dll -Type HarmonyLib.Harmony
```

Inspection caches by assembly/reference content and decompiler version. Cached
decompilation remains ignored and must not be published. The script reads DLLs;
it does not execute the inspected types. Dependency-directory DLLs participate in
the cache key, so use a dedicated offline reference directory when configuring a
new machine.

The preparation installed [ILSpy CLI 11.0.0.9375](https://www.nuget.org/packages/ilspycmd/11.0.0.9375),
which targets .NET 10. Tool binaries, NuGet configuration/packages, machine paths,
the full reference baseline, and cached inspection stay in the ignored directory.
Preparation created no Unity project, plugin build, or test project. Product build
commands and target selection are defined in the roadmap's implementation stories.

The initial install encountered sandbox NuGet TLS authentication failure; the same
command succeeded in the desktop context. No certificate validation bypass was
used. Normal activation and static inspection subsequently succeeded in the sandbox.

## Build and run offline checks

```powershell
./scripts/Build-Local.ps1
```

The command uses the saved local reference paths. Without local-tool initialization,
provide `-ManagedPath '<game Managed directory>'` and
`-DependencyPath '<offline BepInEx and Harmony DLL directory>'` explicitly.
`-BuildNumber 0` is the local default; VERSION supplies major/minor components.

Use .NET SDK 10 with its runtime installed (verified with 10.0.302). The plugin
targets net472/C# 7.3; the metadata-only check executable targets net10.0. First
restore requires NuGet access for pinned framework references and MetadataLoadContext.
If the desktop sandbox denies SDK discovery or NuGet authentication, run the same
command in the approved desktop context; do not bypass certificate checks.

Output is `artifacts/build/DSPSmartQueue.dll`. The command rejects missing
references, unexpected output files, or failed signature/metadata/mapping/input checks. It never
installs or runs the plugin. See [SQ-1.1 evidence](implementation/SQ-1.1.md) for the
recorded foundation verification and [SQ-1.2 evidence](implementation/SQ-1.2.md)
for mapping checks. [SQ-1.3 evidence](implementation/SQ-1.3.md) describes the
presentation/input boundary checks and their runtime limits.
[SQ-1.4 evidence](implementation/SQ-1.4.md) covers recovery and lifecycle checks.
Consult PROJECT.md
for current delivery readiness.
