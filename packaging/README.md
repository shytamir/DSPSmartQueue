# DSP Smart Queue

Smart Queue shows requested products in the Replicator's existing 14-slot strip,
hiding automatically queued intermediates. Left-click selects the displayed
recipe; right-click cancels that native task. Crafting, material handling,
countdown, and executing-task progress remain owned by the game.

Use this package for private prototype evaluation. See [OWNER-PROCEDURE.md](OWNER-PROCEDURE.md)
for setup, installation, observations, and removal. `build.json` identifies the
exact source, references, DLL, and automated checks. Current project status and
acceptance are recorded only in the repository's
[PROJECT.md](https://github.com/shytamir/DSPSmartQueue/blob/main/docs/PROJECT.md).

Requires BepInEx 5.4.17. Install only the enclosed
`BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll` into the corresponding folder of
your selected game or mod-manager profile while the game is closed. Do not replace
other mods, loader files, or saves. The package includes no loader dependencies.

The mod adds no panels, scrolling, queue reordering, settings, or save data.
No public release or broader compatibility claim accompanies this handoff.

Licensed under Apache License 2.0; see LICENSE.
