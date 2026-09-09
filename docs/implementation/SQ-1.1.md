# SQ-1.1 implementation evidence

Evidence recorded 2026-09-09. Story status is tracked only in
[PROJECT.md](../PROJECT.md).

## Decisions and implementation

- Plugin GUID: `shytamir.dspsmartqueue`; display name: `DSP Smart Queue`;
  assembly: `DSPSmartQueue.dll`.
- Plugin target: `net472`, C# 7.3. This follows the reference mod's target and was
  verified by compilation against the selected game and BepInEx 5.4.17 references.
  It is not a claim of in-game compatibility.
- `Microsoft.NETFramework.ReferenceAssemblies` 1.0.3 supplies compile-time framework
  references; dependencies are external references with copying disabled.
- BepInEx `Awake` and its logger provide initialization and diagnostics. Initialization
  checks six method signatures and twelve field signatures, including visibility,
  instance/static shape, parameter/return types, field type and mutability.
- Type resolution and validation run inside the initialization exception boundary.
  Missing or mismatched bindings log incompatibility and disable the component.
  No queue patches, UI writes, task mapping, or hook activation were implemented.
- The build command derives the plugin attribute and assembly versions from VERSION
  plus BuildNumber (local default 0). Broader provenance/CI work belongs to SQ-2.1.

## Verification

`scripts/Build-Local.ps1` produced version `0.1.0` with zero compiler warnings/errors.
The output directory contained only `DSPSmartQueue.dll` (8,704 bytes), SHA256:

`d0cdf2113a579f99f4d3aa8cba5254ebb05d8798bade4d4ddebb401cc2a335e6`

The FoundationChecks executable links the production signature checker. It passed:

- A valid fixture and thirteen invalid member cases: missing, wrong field type,
  wrong visibility, static/readonly/inherited field, missing method, wrong return
  or parameter type, static/public method where private is required, generic method,
  and a widening parameter conversion that the default reflection binder can accept.
- Missing required game type produced an explicit TypeLoadException diagnostic.
- The same signature checks passed against the real selected assemblies in a
  MetadataLoadContext, using the [preparation baseline](../REFERENCE-BASELINE.md).
- Plugin metadata inspection confirmed the BepInEx base class, GUID, display name,
  and agreement between the attribute and assembly version.
- The executing test context contained neither the game assembly nor the plugin.
  Metadata inspection did not instantiate the plugin or execute inspected types.
- Separate child-process calls with missing managed and dependency references
  returned nonzero and named the missing Assembly-CSharp.dll or BepInEx.dll.

The build used SDK 10.0.302 in desktop context. Sandbox SDK-directory permissions
blocked compilation there; desktop compilation and metadata-only checks succeeded.
The test host targets net10.0 with MetadataLoadContext 10.0.0; it is not a runtime
dependency of the plugin. No game installation or live validation was performed.

## Reproduction

Run `./scripts/Build-Local.ps1` after configuring local reference directories, or
supply explicit paths as documented in [local development](../LOCAL-DEVELOPMENT.md).
The command compiles, checks the output allowlist, and runs the focused checks;
failures return nonzero. Generated sources, binaries, and restored packages are
ignored. The deliverable is a foundation DLL, not the completed queue adapter.
