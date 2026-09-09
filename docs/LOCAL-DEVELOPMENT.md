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
