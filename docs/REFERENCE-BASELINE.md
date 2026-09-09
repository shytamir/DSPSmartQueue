# Preparation evidence

Static inspection on 2026-09-09. This historical snapshot established planning
inputs, not compilation or runtime feasibility. Current status is authoritative
only in [PROJECT.md](PROJECT.md). Paths and complete reference hashes were saved in
ignored `artifacts/.runtime-tools/baseline.json`; decompiled source is ignored too.

## Selected inputs

| Input | Observed identity |
| --- | --- |
| Game Assembly-CSharp.dll | SHA256 `ae0ba95f75bd879a62aa4ce253b2ab78eaa4fb3c7c595f5e1fee75ebe0e0ef85`; assembly version `0.0.0.0` |
| GameConfig | Embedded default game version `0.10.34`; exact runtime build number not established by this inspection |
| Local BepInEx source | Clean checkout at tag `v5.4.17`, commit `3c3153598b530fb85e623ba226c8cca1b3f4d262` |
| Offline BepInEx.dll | Assembly `5.4.17.0`; SHA256 `dc1cb6b58b962bda5aaa1d6b5f9ae14ec174f61836a1a1f96c1a040c7e8381f7` |
| Offline 0Harmony.dll | Assembly `2.5.5.0`; SHA256 `7bd2bd6f87c1758047def40f2f0f024c877456ce7c01d68031358ee0c615d850` |
| Installed Unity editor | `2022.3.62f3_96770f904ca7`; inspected file metadata, not launched |
| Preparation tools | PowerShell `7.6.5`, .NET SDK `10.0.302`, ILSpy `11.0.0.9375` |

Dependency DLLs were read from DSPRecipeTracker's offline artifacts, not installed
game mod directories. BepInEx's hash matches the reference mod's pinned package
workflow checksum. Harmony's version matches the v5.4.17 source project dependency;
its hash is a local baseline, not independently verified package provenance.
Unity references and mscorlib came from the selected game's Managed directory.
The eight recorded references were initial inspection inputs, not a final build
reference closure. Dependency resolution during compilation was not checked.

## Confirmed surfaces and implications

Evidence locators below refer to the named types and methods in the hashed inputs;
they can be regenerated using the local inspection command.

| Surface | Confirmed behavior and planning implication |
| --- | --- |
| ForgeTask constructors, Produce | Reference class; roots start with parentTaskIndex -1. count is remaining operations and decreases on production. Reference identity is available. |
| MechaForge.tasks, AddTaskIterate | List of task references; generated children receive parent indices. Some consecutive same-recipe requests merge into the existing last task. Reflect native merges. Initial list capacity 256 is not evidence of a hard queue limit. |
| MechaForge.GameTick, RemoveTask_Direct, RemoveTaskRange_Direct | Completion/removal shifts indices and repairs parent indices. Resolve a retained task reference immediately before input; never retain only its old index. |
| UIReplicatorWindow._OnCreate, RefreshQueueIcons | 14 quantity labels and first-14 icon rendering; index/state storage arrays have 120 elements. Preserve storage layout and clear unused presentation entries. |
| ActiveQueueText | Remaining count multiplied by ResultCounts[0] only for a single output; root/child text treatments differ. Displayed roots need root treatment. |
| _OnUpdate | Mouse/hover calculation precedes current icon refresh. Native labels, count, countdown, and progress are updated before SetBufferData. Reconcile hover against the new slot map rather than preserving a stale highlight. |
| SetBufferData | Private instance void method with no arguments; uploads queue state/index AND recipe state/index buffers. A prefix can reconcile queue presentation; the original recipe uploads must still run. |
| OnQueueMouseDown(BaseEventData) | Private instance void method; reads mouseQueueIndex. Left-click calls native recipe selection plus sound. Right-click calls CancelTask, CalculateExtra, and sound. Guard the original invocation and restore the temporary index even on exceptions. |
| TestMouseQueueIndex | Computes a geometric index across all 14 slots; populated icon controls hover, but the click handler bounds against the underlying task count. Empty filtered slots must explicitly consume input. |
| _OnUpdate and MechaForge.GameTick | Countdown uses totalTime accumulated across all tasks. Progress uses tasks[0].tick / tickSpend. Neither source should be filtered. |
| MechaForge.CancelTask | Returns served materials, removes associated work, recalculates surplus, and can recursively cancel later dependent work; preserves native feedback. |
| _OnOpen, _OnClose, _OnFree, _OnDestroy | Window open rebinds the forge/list; close clears UI state; free/destroy release presentation resources. Mapping invalidation needs explicit lifecycle handling. SetBufferData is also called during open before the first regular update. |
| BepInEx.BaseUnityPlugin | MonoBehaviour base, BepInPlugin metadata requirement, logging/config services. No need to introduce a separate runtime host. |
| HarmonyLib.Harmony | Patch supports prefix, postfix, and finalizer; UnpatchSelf provides owner-scoped teardown. Available API surface supports the proposed guarded input approach, but patch execution has not been tested. |

## Questions identified by the inspection

No contradiction with the kickoff was found. The two primary hooks were plausible
candidates, with additional lifecycle hooks likely needed. Static inspection did
not establish event ordering in a running session, visual consistency after queue
advancement, or successful recovery after a partial presentation/buffer failure.
These observations informed the integrated prototype stories and their checks.

The inspection proposed net472 based on the reference mod; it did not approve a
target or establish compilation. It recommended a modern .NET host for pure logic
tests without loading game assemblies. Hosted compile-reference strategy, plugin
identity, final reference closure, and owner recipe/material setup were carried
into the roadmap as bounded story decisions.

Preparation checks passed: initialization, download-free activation, reference
hash verification, targeted decompilation, repeated-type cache reuse, initialization
without reinstalling, and rejection of a deliberately mismatched reference hash
(original baseline restored and rechecked). No plugin
was built, installed, or executed, and no game or reference-repository files changed.
