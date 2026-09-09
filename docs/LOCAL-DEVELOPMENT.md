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

Output is `artifacts/build/DSPSmartQueue.dll` with `build.json`. The JSON records
VERSION plus BuildNumber, Git revision and working-tree status, source file hashes,
reference DLL hashes, SDK version, checks performed, and the output DLL hash. Paths
in the record are repository-relative or reference filenames; machine paths are
not included. Reference hashes cover the DLLs available to compilation and metadata
resolution in the selected directories. Keep those directories stable during a build.

Each invocation invalidates the previous deliverable before input validation.
Compilation uses `artifacts/build-staging/`; only a successful build and all checks
promote the DLL and evidence to `artifacts/build/`. A failure removes the deliverable
pair and returns nonzero. Staging files are not handoff artifacts. Run one build at
a time per checkout, without concurrent source/reference edits. Dirty builds are
identified explicitly; use a clean committed checkout for owner handoff.

The command rejects missing
references, unexpected output files, or failed binding, metadata, mapping, input,
recovery, or lifecycle checks. It never installs or runs the plugin. Recorded
verification evidence is linked from PROJECT.md.

Hosted CI uses the same offline logic and binding fixtures without game files:

```powershell
dotnet run --project tests/FoundationChecks -c Release -- --logic-only
```

This mode does not compile the plugin or verify real-reference metadata, Harmony
execution, or Unity rendering. CI runs those offline checks.
`scripts/Get-BuildVersion.ps1` supplies the same version rule to the local
build, package, and CI workflow; it rejects values outside assembly-version limits.

## Private handoff package

From a clean committed checkout, run `./scripts/New-PrototypePackage.ps1` with the
same reference and BuildNumber parameters as Build-Local. It builds and verifies
against real references, creates a ZIP, then checks its allowlist, source content,
manifest/version, decoded 256x256 PNG, and DLL/evidence hashes before naming the
handoff artifact. No proprietary references are packaged. Only the inspected ZIP
and its adjacent `.inspection.json` under `artifacts/package/` are handoff files;
`prototype.pending.zip` is temporary. Earlier identified packages remain unchanged.

To inspect an existing ZIP against its retained local build evidence:

```powershell
./scripts/Test-PrototypePackage.ps1 -Path '<ZIP path>' -BuildDirectory '<directory containing verified DLL and build.json>'
```

Use the matching source checkout as well; changed package documents or templates
are rejected. The inspector does not execute the DLL. Windows PowerShell 7 and
System.Drawing decode the PNG; no Unity project or artwork toolchain is required.
The package layout follows [Thunderstore's package rules](https://wiki.thunderstore.io/mods/creating-a-package)
and [BepInEx folder routing](https://wiki.thunderstore.io/mods/packaging-your-mods).
CI runs offline checks only; packages require the local real-reference command.