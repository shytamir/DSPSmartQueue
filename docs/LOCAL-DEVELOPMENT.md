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

## Hosted build and reference validation

GitHub Actions runs `./scripts/New-Package.ps1 -Hosted -BuildNumber 1`, then
`./tests/PackageChecks.ps1` against the resulting ZIP before uploading it.
`Initialize-CIReferences.ps1` downloads checksum-pinned BepInEx 5.4.17 and public
Unity 2022.3.62 modules. `build/References` supplies only the game and Unity UI
signatures used by the plugin and binding checks. Shim methods throw; these are
compile references, not an emulated game, and must never be shipped or executed.
Build evidence explicitly identifies the selected reference mode.

After changing referenced APIs, validate hosted declarations against the selected
real local references with `./tests/ReferenceParity.ps1`. It builds both ways,
runs the respective checks, and requires byte-identical plugin DLLs. A mismatch
requires investigation, not updating an expected hash. Neither path runs the game.
Offline checks alone remain available with:

```powershell
dotnet run --project tests/FoundationChecks -c Release -- --logic-only
```

## Release package

Download `DSPSmartQueue-<version>` from the successful GitHub Actions run identified
in PROJECT.md. Extract the release ZIP from GitHub's artifact download wrapper;
that inner ZIP is the Thunderstore upload. Keep the separate `build-evidence`
artifact for verification. The workflow uses VERSION major/minor and the workflow-specified BuildNumber patch.

For local development, `./scripts/New-Package.ps1` builds against real references;
`-Hosted` exercises the CI path. Both require a clean committed checkout and inspect
the actual ZIP before naming it. Local output is for verification; release delivery
comes from GitHub Actions.

The public ZIP contains exactly:

- `manifest.json`
- `README.md`
- `icon.png` (decoded 256x256 PNG)
- `LICENSE`
- `BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll`

Only the ZIP is for publication. Its adjacent `.build.json` and `.inspection.json`
retain source/reference identity, verification results, and hashes for maintainers.
Owner procedures, management docs, references, and caches are excluded from the
ZIP. `package.pending.zip` is temporary. Previous artifacts are retained unchanged;
PROJECT.md identifies which release is suitable for publication.

Inspect and exercise the package regression checks using the matching checkout
and the verified DLL/build.json directory:

```powershell
./scripts/Test-Package.ps1 -Path '<ZIP path>' -BuildDirectory '<verified build directory>'
./tests/PackageChecks.ps1 -Path '<ZIP path>' -BuildDirectory '<verified build directory>'
```

Changed package documents or templates are rejected. Windows PowerShell 7 and
System.Drawing decode the PNG; the inspector does not execute the plugin.
The package follows [Thunderstore's package rules](https://wiki.thunderstore.io/mods/creating-a-package)
and [BepInEx folder routing](https://wiki.thunderstore.io/mods/packaging-your-mods).
CI runs compilation, offline metadata/logic checks, ZIP inspection, and package regression checks.
