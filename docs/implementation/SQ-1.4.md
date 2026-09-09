# SQ-1.4 implementation evidence

Historical evidence recorded 2026-09-09. The implementation description and build
results below describe this story's slice at that time. Current status is tracked
only in [PROJECT.md](../PROJECT.md).

## Recovery and lifecycle behavior

QueueRecovery disables projection before restoring native presentation. QueueHooks
uses the game's RefreshQueueIcons, ActiveQueueText/DeactiveQueueText,
TestMouseQueueIndex, and SetBufferData, plus the native task count. Raw input is
allowed only after restoration completes. No crafting or player action is replayed.

If restoration fails, the native ManualBehaviour._Close operation closes the
Replicator; actual inactive/hidden state is checked before allowing native input.
Static inspection established that this operation calls native close handling and
deactivates the window. An inactive-but-visible window is not treated as restored.
The native close signature and active property are included in initialization checks.

An integration failure disables projection for the remaining plugin lifetime;
there is no per-frame retry or automatic reactivation. If both restoration and
closure fail, input remains blocked and the error is logged. Task mappings are
cleared; the affected window and guard are retained for safe explicit teardown,
rather than falsely declaring recovery or exposing raw indices. This exceptional
case cannot guarantee usability when the native window itself cannot recover.

Prefixes on open, close, free, and destroy clear transient mapping. Click state
retains its originating mapper so an in-flight native handler still restores its
temporary index after lifecycle invalidation. Reopening requires a fresh successful
presentation; queue-reference checks reject an earlier session's list.

Plugin disable/destroy restores or closes the native view before calling Harmony's
owner-scoped UnpatchSelf. Successful teardown releases the mapping, window, logger,
and plugin Harmony reference. Repeated cleanup is safe. Failed initialization uses
the same cleanup path without altering an untouched native view. If safe restoration
and closure both fail, teardown deliberately retains guards until safe removal is
possible, instead of removing protection from a potentially stale visible strip.

## Verification

The local build passed with zero warnings/errors, including all existing checks.
New tests ran the production recovery gate and mapping cleanup over offline fixtures:

- Partial installation cleanup without native UI writes.
- Raw input blocked throughout native restoration, including partial UI changes.
- Restoration failure followed by successful closure before raw input resumes.
- Double failure: guard retained, transient task references cleared, explicit later
  teardown succeeds, and repeated cleanup is safe.
- No repeated recovery after success and no player-action replay path.
- Close/reopen and session replacement reject stale mappings; all fourteen task
  slots and both counts are cleared.
- Close during an in-flight click still permits original-index restoration without
  making the cleared map ready again.
- Real metadata confirms lifecycle hook shape and native close/readiness signatures.

Inspection verified that hook removal calls UnpatchSelf on the plugin's Harmony
instance, not unscoped UnpatchAll. Actual detour removal and native window operations
were not executed: tests use callbacks for those runtime operations. Game/plugin
assemblies remained metadata-only in the test host.

Output: only DSPSmartQueue.dll, version 0.1.0, 19,968 bytes; SHA256:

`209bd52725fe164682b8f584274b3d108307c02b4254d1ef430e6378bba2cef1`

## Live observations to carry into SQ-2.2's procedure

- Close and reopen the Replicator after queue advancement: no old task target or
  highlight should survive; the refreshed strip must remain usable.
- Enter a subsequent session with a different queue and confirm fresh presentation
  and selection, along with the kickoff's native controls/timing observations.
- Follow the owner's normal exit/removal/restart cleanup and confirm native queue
  behavior without the plugin. This does not require a hot-reload mechanism.

Failure injection, reference identity, and cleanup were checked offline. The
recorded verification did not include these live observations.
