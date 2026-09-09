# SQ-2.2 package verification evidence

Recorded 2026-09-09. Current handoff identity, readiness, and acceptance are tracked
only in [PROJECT.md](../PROJECT.md).

The local package command built against real references and produced a ZIP with
exactly seven files: manifest, package README, license, a decoded 256x256 PNG,
owner procedure, build evidence, and the plugin in its dedicated BepInEx plugins
folder. The inspector matched package documents to recorded source hashes and the
DLL to its verified build hash. No game/runtime dependency assemblies, caches,
credentials, or machine paths were included. The icon was drawn from simple
geometric shapes and visually inspected; no game artwork was copied.

A disposable clean checkout passed compilation with zero warnings/errors, all
existing offline/metadata checks, package creation, and package inspection. Negative
probes rejected an extra dependency DLL, missing procedure, duplicate ZIP entry,
altered DLL, invalid icon, changed manifest version, and dirty build evidence.
Only the disposable package/check records were modified for these probes.

The procedure used three concrete requests with sourced recipes and unlocks,
starting inventory, native comparison, installation, observation/result rows, and
removal. Cached inspection of UIReplicatorWindow confirmed the native quantity
control's maximum of 10. Its observations covered every kickoff live criterion;
fixture tests and failure injection were not assigned to the owner.

Hosted CI was limited to offline checks and version validation. Private packaging
used the real-reference local command; the metadata-only scaffold artifact was
removed. No package upload, release, installation, or live validation was performed.

Handoff source `6397676` also passed [hosted run 34308089166](https://github.com/shytamir/DSPSmartQueue/actions/runs/34308089166).
