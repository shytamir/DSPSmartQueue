# SQ-1.2 implementation evidence

Evidence recorded 2026-09-09. Current story status is authoritative only in
[PROJECT.md](../PROJECT.md).

## Implementation

`VisibleRequests<T>` reads an existing task list and keeps at most fourteen slots,
each containing the original task reference, recipe ID, and native display quantity.
One array is allocated per mapper and reused. All roots contribute to TotalCount;
only visible roots require recipe/quantity lookup. No list or task is cloned,
modified, merged, deduplicated, or retained as history.

Four read selectors are the offline test seam. `ForgeRequests.Create()` connects
them directly to ForgeTask fields and LDB recipe result counts. The quantity rule
was checked against the cached native ActiveQueueText: single output multiplies
operations by output count; multiple outputs or an absent recipe keep operations.

Rebuild clears old slots first. A failed rebuild clears partial results and
rethrows; Clear also releases task references and resets counts. This is local
mapping hygiene, not UI recovery or session integration. No UI/input hook or plugin
activation behavior was added; those belong to subsequent stories.

## Verification

`scripts/Build-Local.ps1` passed with zero compiler warnings/errors against the
selected real references. It ran the existing foundation checks and the new checks
linked to the production mapper, without loading game/plugin assemblies into the
executing test context.

The mapping checks covered:

- Empty and child-only queues; mixed roots/children; explicitly requested components;
  root detection by any negative parent index.
- Distinct same-recipe task references and a native-style merged task/count change.
- Single-output multiplication, multiple-output operation counts, absent recipes.
- 13, 14, 15, and 17 roots interspersed with children: ordered first-fourteen
  references/quantities, total root count, and no off-screen recipe lookups.
- A read-only input list plus before/after identity and field snapshots proving
  tasks, list order, and recipe result arrays stayed unchanged.
- Full-to-short, full-to-child-only, and populated-to-empty rebuilds; empty slots
  held no stale task reference, recipe ID, or quantity.
- Repeated Clear and failure after partial mapping both left an empty result.

The output directory contained only DSPSmartQueue.dll, version 0.1.0, 11,264 bytes:

`e938901030cee6a7a94141b5cdea45581560e51bc20bf5830018e81ce6a7f3dd`

The native selector adapter was compiled and inspected, not executed. Runtime UI
integration and owner feasibility are not established by these offline checks.
