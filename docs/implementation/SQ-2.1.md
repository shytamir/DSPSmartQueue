# SQ-2.1 build verification evidence

Recorded 2026-09-09. Current status is tracked only in [PROJECT.md](../PROJECT.md).

The existing local command was extended to produce a verified DLL and `build.json`.
The record captured version, revision, working-tree status, source file hashes,
reference hashes, SDK version, check coverage, and DLL hash without machine paths.
Compilation used staging; failure invalidated the handoff output. Source and
reference identity were checked again before promotion to catch changes during
verification. The local and scaffold versions used one shared VERSION reader.

Verification against the selected real references used SDK 10.0.302:

- A disposable clean Git checkout, without local-tool configuration or restored
  packages, built using explicit reference paths and BuildNumber 42. Compilation
  had zero warnings/errors; all offline and real-metadata checks passed. Its record
  reported a clean revision, version 0.1.42, and the independently checked DLL hash.
- A dirty build recorded its changed source path and matching content hash.
- Invalid version/build-number input, missing references, compiler errors, and a
  failing check each returned nonzero and removed a seeded previous DLL/evidence
  pair. Faults were introduced only in the disposable checkout.
- The reference-free check mode passed the production mapping, input, recovery,
  and binding fixtures. Hosted CI used that mode and gated its scaffold job on it.

The build record accompanies each local DLL; this document does not assign an old
hash to future builds. Hosted checks do not compile the plugin or inspect real
game metadata. No Harmony execution, Unity rendering, installed validation, or
byte-for-byte reproducibility was established by these checks.
