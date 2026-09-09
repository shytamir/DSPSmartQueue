# Replicator Queue: Project Kickoff

This document defines the product contract. Current status and active work are
tracked exclusively in [PROJECT.md](PROJECT.md).

## Objective

Make the Replicator queue easier to control by showing the products the player requested and hiding their automatically queued intermediates.

The mod uses the existing 14-slot strip. Removing intermediate entries makes room for more actionable requests without enlarging the interface or changing crafting behavior.

```text
Underlying queue: [component] [component] [Product A] [component] [Product B]
Visible strip:    [Product A] [Product B]
```

An **action product** is a top-level queued task. Explicitly requested gears remain visible; gears automatically queued to construct a machine are hidden.

## Product behavior

| Element | Requirement |
| --- | --- |
| Visible entries | Show the first 14 outstanding action-product tasks in native order, packed from the left. |
| Icons and quantities | Show each displayed task's native recipe icon and remaining quantity. |
| Left-click | Select the displayed task's recipe through the original handler. |
| Right-click | Cancel the displayed task through the original handler. |
| Queue count | Show the number of outstanding action-product tasks, including those beyond the visible 14. |
| Queue countdown | Preserve the native whole-queue countdown. |
| Operation progress | Preserve native progress for the actual executing task, including a hidden intermediate. |
| Other controls | Preserve native recipe selection, Produce controls, keyboard behavior, and feedback. |

Separate requests remain separate even when they use the same recipe. If DSP merges requests itself, the display reflects the resulting native task. Empty slots do not accept task actions.

The underlying queue retains its full capacity. More than 14 action products can still exceed the viewport. Reordering, scrolling, paging, additional slots, and separate panels are outside the project scope.

## Native ownership

DSP remains responsible for crafting order, material allocation, task completion, cancellation, and material returns. The mod changes queue presentation and translates player input to the corresponding native task.

Native cancellation may also remove later work that depended on surplus intermediate output. Preserve that behavior and its normal feedback.

The whole-queue countdown continues to use `MechaForge.totalTime`, including hidden intermediates and off-screen work. The progress image continues to read the real executing task. Filtering the visible entries must not change either source.

## Task identity and input

Use `ForgeTask.parentTaskIndex < 0` to identify action products. Retain each displayed task's reference identity and resolve its current native queue index when clicked.

Earlier tasks can finish between rendering and clicking, shifting later indices. Recipe IDs are also insufficient to identify a request when several tasks use the same recipe.

The following rules apply to every populated slot:

- Icon, quantity, hover treatment, and click target represent the same task.
- A valid click reaches the original handler once, using that task's current index.
- If the displayed task has disappeared, consume the click and refresh.
- Empty slots and unresolved targets never fall through to raw-index handling.

Preserve DSP's quantity convention: for a single-output recipe, display remaining operations multiplied by the recipe's output count; for a multi-output recipe, retain the native operation-count display.

## Implementation approach

Build one plugin with a small visible-slot mapping and narrow hooks into the queue UI. Keep filtering and identity checks testable independently of Unity. Cache required bindings and reuse presentation storage.

The initial integration candidates are:

| Method | Intended use |
| --- | --- |
| `UIReplicatorWindow.SetBufferData()` | Reconcile queue icons, quantities, count, and hover before the native buffer upload. Preserve recipe buffers and timing. |
| `UIReplicatorWindow.OnQueueMouseDown(BaseEventData)` | Resolve the displayed task, translate the input index for the original handler, and restore the UI index on both normal and exceptional exits. |

Static inspection establishes these methods as candidate boundaries. The prototype must confirm that they preserve the complete display and interaction contract. An appended click listener is insufficient because the native raw-index action could already have executed.

Read the forge queue without replacing or modifying its list, task objects, or parent links. Player-requested cancellation runs through the original handler. No mod-owned queue history or save data is required.

Display and input enable and disable together. If integration fails, restore a consistent native view and native input. Consume uncertain clicks during recovery and never retry a partially handled action. Clear transient mappings at session changes and remove the plugin's hooks on shutdown.

## First delivery

Deliver one working prototype DLL and a concise owner feasibility procedure.

Before handoff, compile against the selected game references and complete focused checks for:

- Root filtering, native order, explicit component requests, and the 14-slot boundary.
- Duplicate recipes and native quantity conventions.
- Task completion and cancellation shifting indices between display and input.
- Disappeared targets, empty slots, exceptional exits, and session cleanup.

The owner then performs one session covering the remaining live observations:

| Observation | Acceptance criteria |
| --- | --- |
| Display | A request requiring intermediates, followed by two different requests, appears as three action products in the correct order. Each quantity belongs to its displayed product. |
| Actions | Left-click selects the displayed recipe. Out-of-order right-click cancels the displayed request through native behavior. Selection remains correct after the queue advances. |
| Native continuity | Countdown and operation progress remain native while intermediates craft. Explicitly requested components remain visible. Produce controls, reopening the Replicator, and a subsequent session behave correctly. |

The procedure must identify the build, a concrete recipe/material setup, installation and cleanup steps, and a simple result format. The owner performs installed and in-game validation; agents do not install or execute the plugin. Deterministic checks are not reassigned as human workload.

A failed observation returns only the affected scope for correction and recheck. Feasibility is established when the identified build satisfies all acceptance criteria under the tested conditions. Publication and broader compatibility remain separate decisions.

## Working principles

Preserve native behavior, verify exact game surfaces, test the new logic, and keep cleanup explicit. Use owner observations to resolve live presentation and interaction questions. Introduce additional architecture or tooling only when the implementation requires it.

The technical basis was static inspection of `ForgeTask`, `MechaForge`, and `UIReplicatorWindow`. Delivery scope and completion criteria are defined in the [roadmap](management/ROADMAP.md).
