# SQ-1.3 implementation evidence

Historical evidence recorded 2026-09-09. The implementation description and build
results below describe this story's slice at that time. Current status is tracked
only in [PROJECT.md](../PROJECT.md).

## Integration

QueueHooks installs the native click prefix/finalizer before the presentation
prefix/finalizer and activates them together. Private-field injection signatures
are checked against the real metadata. Native helper MethodInfo/FieldInfo bindings
and the icon selector are cached rather than looked up each frame.

SetBufferData's prefix writes only queue icon/hover arrays, quantity labels/root
styling, and the root count. It clears unused entries across the original buffer
length and rebuilds hover from the geometric slot and the new mapped icons.
The original method still performs all four uploads, including the untouched recipe
buffers. Readiness is committed only after that upload returns without exception.

The click prefix accepts native left/right events only, resolves the displayed
reference against the current native list, temporarily translates mouseQueueIndex,
and lets the original method run once. Empty filtered slots, changed queue instances,
and disappeared targets are consumed and refreshed. Reentrant input is blocked
while the index is translated. The finalizer restores the geometric index before
refresh; it preserves native exceptions and never replays a handled action.

QueueStrip is the production presentation/input boundary linked into offline tests.
The Unity shell applies each slot's quantity, root color, and active state directly.
No replacement crafting, selection, cancellation, material handling, sound,
countdown, progress, or recipe-buffer logic was introduced.

## Checks and results

The local build passed with zero warnings/errors against selected real references.
All existing checks and the added boundary checks passed:

- Icon order, quantities, full buffer-tail clearing, mapped hover, outside/empty
  hover, and no input before successful upload commitment.
- Distinct references with equal recipe/data; completion and preceding cancellation
  shifting the native index after display.
- Normal and exceptional index restoration, preservation of the original exception,
  and no repeated native action; reentrant attempts are rejected.
- Disappeared targets and empty filtered slots even when the corresponding raw
  queue index exists; changed list instances are rejected.
- Failed icon projection and uncommitted uploads block input; successful refresh
  re-enables mapped input.
- Real-metadata verification of Harmony native argument/field injections and
  prefix/finalizer state type agreement. No game/plugin assembly was loaded into
  the executing test context.

Boundary inspection reconfirmed native _OnUpdate reads MechaForge.totalTime and
tasks[0] progress before SetBufferData; OnQueueMouseDown owns recipe selection,
CancelTask, CalculateExtra, and audio. The plugin does not replace those operations.
Offline dispatch tests use a stand-in original action; Harmony execution and Unity
rendering have not been exercised in a running game.

Output: only DSPSmartQueue.dll, version 0.1.0, 16,896 bytes; SHA256:

`bce65498c8887ec20c928e467050b4b4c04ce76ddc63ee7df0a500f4646a6d73`

## Verification limits

This slice included basic refresh and shutdown guards. Comprehensive failure
recovery and lifecycle cleanup were outside its verification scope. The offline
checks did not establish live feasibility.
