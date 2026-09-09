# Owner feasibility procedure

Historical prototype procedure. Package paths and enclosed evidence below refer
to the original private handoff, not the public release ZIP.

Current readiness and acceptance are tracked only in the repository's
[PROJECT.md](https://github.com/shytamir/DSPSmartQueue/blob/main/docs/PROJECT.md).
This procedure supplies observations for SQ-2.3; installing or accepting the
prototype remains the owner's decision.

## Identify and prepare

Keep the supplied ZIP and its adjacent inspection JSON together. Record the ZIP
filename/hash from that inspection and version, source revision, and DLL hash from
the enclosed `build.json`. Do not substitute a later rebuild during the observations.
The supplied automated evidence covers compilation against real references,
metadata, mapping, input recovery, and lifecycle fixtures. You need not repeat it.

Use an early-game test save with **Electromagnetism** and **Basic Assembling**
unlocked, and ordinary handcraft speed. Confirm the Replicator offers Assembling
Machine Mk.I, Tesla Tower, and Gear. Record the game's full displayed version,
BepInEx version, handcraft-speed upgrades, and any other enabled mods. The compilation
baseline was game managed data reporting 0.10.34 and BepInEx 5.4.17; the full game
build number was not established statically. A different setup is a compatibility
observation, not evidence for the selected baseline.

Start with an empty crafting queue, charged Icarus with fuel, and at least five
free inventory slots. Put **500 Iron Ingots, 100 Copper Ingots, and 100 Magnets** in
Icarus's inventory. Put all Gears, Circuit Boards, and Magnetic Coils into storage;
disable personal logistics supply of those intermediates for this observation.
These quantities include reserve for the follow-up requests. Leave research idle
so it does not consume setup materials. Save under a new test-save name.

The recipe setup uses 4 Iron Ingots + 8 Gears + 4 Circuit Boards per assembler,
and 2 Iron Ingots + 1 Magnetic Coil per Tesla tower. With the intermediate inventory
empty, native recursive crafting supplies those parts. Recipe/unlock sources:
[Assembler](https://dyson-sphere-program.fandom.com/wiki/Assembler),
[Basic Assembling](https://dyson-sphere-program.fandom.com/wiki/Basic_Assembling),
[Tesla Tower](https://5gamers.com/en/dyson-sphere-program/building/tesla-tower).
These are reference data, not observations from the installed build.

## Native baseline and installation

1. With Smart Queue absent, load the test save and queue **10 Assembling Machine
   Mk.I**, then **10 Tesla Towers**, then **10 Gears**, each with one Produce click
   at quantity 10. Note the native countdown and progress while intermediate parts
   craft, plus ordinary recipe selection, Produce, and feedback. A short recording
   makes comparison easier. Reload the saved empty-queue setup afterward.
2. Exit the game fully. Extract the ZIP to a temporary folder outside the game.
   In the BepInEx root you actually launch (the selected profile for a mod manager,
   or the game root for a manual installation), add only
   `BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll` from the extracted package.
   Create that dedicated directory if absent. If it already exists, stop and set
   aside the old Smart Queue copy rather than overwrite it; avoid two copies of
   plugin GUID `smartqueue`. Do not copy anything into `core` or `patchers`.
3. Launch through that same profile/installation and load the saved setup. Check
   the BepInEx log for DSP Smart Queue initialization and no adapter-disable error.
   Keep the extracted README, procedure, and evidence outside the plugin directory.
   Do not install a different loader merely for this test; report a loader mismatch.

## Live observations

Use the native quantity buttons to set 10 and click Produce once per request.
Perform steps 1–4 while the assembler batch is still queued. If upgraded crafting
finishes too quickly, reload the setup and repeat; do not infer a pass from a
missing observation.

| Step | Action | Expected observation |
| --- | --- | --- |
| 1 — Display | Queue 10 assemblers, 10 Tesla towers, then 10 gears, in that order. | Three entries: assembler, tower, gear; count 3. Intermediate gears/circuits/coils are hidden, but the explicit gear request is visible. Initial quantities are 10; remaining quantities decrease only with their own production. |
| 2 — Native continuity | Watch while hidden intermediate parts craft; compare with the native baseline. | Whole-queue countdown continues and executing-task progress moves while assembler quantity can remain unchanged. No frozen timer, substituted root progress, or corrupted recipe area. Hover follows the icon under the pointer. |
| 3 — Selection | Left-click each visible entry, then choose another recipe through the ordinary recipe grid. | Each click selects that displayed recipe with normal feedback; the ordinary recipe controls remain usable. |
| 4 — Out-of-order cancel | Right-click the middle Tesla tower entry while assemblers remain ahead of it. | The tower request disappears once; assemblers and the explicit gear request remain. Count becomes 2, remaining entries pack left, and native material return/feedback occurs once. Record unexpected dependent removals rather than assuming independent cancellation. |
| 5 — Advancement | Let the assembler request finish, then left-click the shifted gear entry promptly. | The remaining gear entry selects Gear, with correct hover/quantity after native indices and visible positions have advanced. If it finishes first, repeat steps 1–4 from the saved setup and watch the transition. |
| 6 — Controls and reopening | Queue 10 assemblers again, then 10 gears. Close/reopen the Replicator with your usual keyboard binding while work remains. Select the visible gear request. | Produce and keyboard controls behave normally. Reopening shows fresh quantities, highlight, and selection targets, without stale icons or errors. |
| 7 — Subsequent session | Return to the menu, reload the original saved setup, and queue 5 Tesla towers followed by 5 gears. Open the Replicator and select both. | Only the new requests appear, in order, with fresh counts and correct selections; nothing from the earlier queue survives. |

Pause and report any disabled adapter, exception, misleading target, duplicate
action, or stuck UI. Keep the log and a short description of the exact queue and
click. Recovery failure injection, boundary sizes, and duplicate-reference races
are automated checks, not additional manual tasks.

## Cleanup

Exit the game fully. Remove only the DLL you added from
`BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll`; remove its dedicated directory
only if empty. If you set aside an earlier copy, restore it only after completing
the without-plugin check. Leave every other mod and all saves intact. Keep logs
and the separate test save until you have recorded the result.

Restart the same profile without Smart Queue and load the original test setup.
Queue 10 assemblers and confirm native intermediates and controls return without
Smart Queue initialization/errors. Record cleanup as pass/fail/unobserved.

## Result to return

Copy this block into your reply and replace each dash. Use **pass**, **fail**, or
**unobserved** for every observation; an unobserved item does not establish feasibility.

```text
ZIP filename / SHA256:
Version / source revision / DLL SHA256:
Game full version / BepInEx / other mods / handcraft upgrades:
Display (step 1): -
Native continuity and hover (step 2): -
Selection and recipe grid (step 3): -
Out-of-order cancellation and feedback (step 4): -
Selection after advancement (step 5): -
Produce, keyboard controls, reopening (step 6): -
Subsequent session (step 7): -
Cleanup without plugin: -
Failures: step, queue before click, clicked icon/button, expected/actual, log or recording.
```
