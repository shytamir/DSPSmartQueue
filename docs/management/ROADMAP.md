# Roadmap

This roadmap defines the first prototype delivery. Current phase, activation,
progress, and acceptance are tracked only in [PROJECT.md](../PROJECT.md).
The [kickoff](../KICKOFF.md) supplies the product behavior and live acceptance
contract; this document defines bounded implementation work.

## Delivery scope and exclusions

Deliver one BepInEx queue presentation adapter, an identified prototype package,
and a concise owner feasibility procedure. Prefer native task identity, handlers,
lifecycle, logging, and refresh operations over replacement mechanisms.

The following are outside this delivery:

- Queue mutation, scheduling, reprioritization, reordering, recipe optimization,
  alternative crafting, material allocation, refunds, or dependency cancellation.
- Scrolling, paging, extra slots, separate panels, new hotkeys, drag-and-drop,
  filtering controls, configurable themes, or changes to other Replicator controls.
- Synthetic request IDs, parallel queue models, request history, persistence,
  save migration, undo/redo, or replay of player actions.
- Custom dependency injection, service containers, event buses, configuration
  frameworks, plugin hosts, or generalized mod-support libraries.
- Unity projects, asset bundles, custom shaders, animations, or elaborate artwork
  pipelines. A simple package icon is included in SQ-2.2.
- Telemetry, analytics, continuous performance monitoring, benchmarking programs,
  broad compatibility matrices, multiplayer support, or integration with other mods.
- Complete game API substitutes, general-purpose reference generators, build farms,
  installers, auto-updaters, signing infrastructure, or automated publication.
- Agent installation/execution of the plugin or modification of deployed game/mod
  files. Live validation belongs to the owner.
- GitHub releases, Thunderstore publication, and support for additional game,
  Unity, or BepInEx versions beyond the selected prototype baseline.

Tests and recovery checks accompany the behavior they exercise. A newly discovered
requirement outside these bounds needs an explicit scope decision; it must not be
absorbed as an incidental implementation improvement.

## SQ-1: Safe integrated queue adapter

**Value:** Players identify, select, and cancel their requested products while DSP
continues to own crafting and material handling.

### SQ-1.1 — Minimal plugin foundation

**Story:** As the maintainer, I want a minimal plugin that compiles against the
selected runtime so that integration starts from a known technical baseline.

**Scope:** Choose plugin identity and target framework; create one plugin project
using BepInEx lifecycle/logging; configure external references; identify and check
the required binding signatures; establish one local build command.

**Out of scope:** Queue rendering or input patches, task mapping, runtime injection,
package completion, CI expansion, settings UI, and speculative bindings for future
features. Establish the entry point here; owned hook activation/recovery belongs
to SQ-1.3 and SQ-1.4.

**Definition of done:**

- One documented command produces the plugin DLL against the selected real game,
  Unity, BepInEx, and Harmony references.
- Missing references fail with an actionable message and nonzero exit status.
- Plugin identity and target selection are explicit in the implementation.
- Required members have exact signature checks suitable for initialization;
  missing/mismatched bindings report incompatibility without applying UI changes.
- The deliverable contains no copied game/runtime dependency assemblies.
- No installed plugin execution is claimed as evidence of this story.

### SQ-1.2 — Identify visible requests

**Story:** As a player, I want the strip to represent my outstanding requests
accurately so that intermediates do not obscure the work I can control.

**Scope:** Read native tasks, select the first 14 roots in order, retain references,
calculate the total root count and native quantities, and reuse bounded slot storage.

**Out of scope:** New queue ownership, task cloning, synthetic IDs, deduplication,
custom merging, cached history, UI hooks, and material or recipe simulation. A
small test seam is sufficient; do not recreate the game domain for testing.

**Definition of done:**

- Deterministic checks cover empty/mixed queues, explicit component requests,
  exactly 14 roots, and more than 14 roots with intervening children.
- Order and task reference identity are preserved; same-recipe requests remain
  separate and a native merged task remains one entry.
- Single-output quantities apply the native multiplier; multiple-output recipes
  retain operation counts. Counts include roots outside the viewport.
- Checks establish that source tasks/list are not modified and that unused slots
  cannot retain references from an earlier mapping.
- Tests run without loading game assemblies into their executing host.

### SQ-1.3 — Present and operate the same task

**Story:** As a player, I want each product's appearance and mouse actions to refer
to the same request so that selecting or cancelling affects the intended work.

**Scope:** Integrate icons, quantities, root styling, count, and hover with guarded
native input. Resolve the displayed reference's current index immediately before
the original handler. Enable filtered display and guarded input together.

**Out of scope:** Replacement selection/cancellation/audio, appended listeners that
allow raw-index actions first, alternative countdown/progress sources, recipe-buffer
rewrites, additional input gestures, and retrying partially handled clicks.

**Definition of done:**

- Populated slots' icon, quantity, hover, and target agree; unused presentation
  entries are cleared and empty slots consume task actions.
- Valid left/right input reaches the original handler once with the resolved index.
- Focused checks cover preceding completion/cancellation, duplicate recipes,
  disappeared targets, empty slots, and exceptional handler exits.
- Temporary index changes are restored on normal and exceptional exits; stale
  targets consume input and request refresh instead of falling through.
- Hover is reconciled against the current mapping, including native update ordering.
- Queue countdown, executing-task progress, recipe buffers, other controls, and
  native feedback retain their original sources/handlers, verified by focused
  boundary checks and inspection. The owner confirms live behavior in SQ-2.3.
- Compilation passes against selected references. Filtered presentation is never
  intentionally activated without its matching input guard.

### SQ-1.4 — Recover and restore native consistency

**Story:** As a player, I want the queue to remain usable across reopening, session
changes, and adapter failures so that the mod cannot leave misleading controls.

**Scope:** Coordinate readiness, activation, recovery, mapping invalidation, and
owned hook teardown using the native lifecycle and refresh operations where viable.

**Out of scope:** A generic fault-management framework, background retry loops,
save rollback, mod hot-reload support, disabling other mods, or removing patches
owned by another Harmony instance.

**Definition of done:**

- Failed initialization preserves native presentation/input; failure after
  activation cannot leave filtered visuals paired with raw-index input.
- Failure checks include partial presentation changes and verify restoration of
  a consistent native view before raw native task input resumes.
- Uncertain clicks during recovery are consumed and never retried.
- Focused checks cover close/reopen, queue/session replacement, repeated cleanup,
  and shutdown; none can reuse stale task mappings.
- Teardown removes only owned hooks and clears owned transient references.
- Remaining live lifecycle observations are carried into the owner procedure,
  without treating deterministic checks as runtime acceptance.

## SQ-2: Identified prototype delivery and feasibility

**Value:** The owner evaluates a specific build with trustworthy automated evidence
and a short, repeatable live procedure.

### SQ-2.1 — Repeatable identified prototype build

**Story:** As the maintainer, I want one command to build and check an identified
prototype so that a reported result can be traced to source and references.

**Scope:** Extend the SQ-1.1 command with focused checks, versioning, source/reference
identity, output hashing, and the minimum useful hosted CI role.

**Out of scope:** A second build system, byte-for-byte reproducibility guarantees,
complete hosted game API emulation, proprietary assembly distribution, and broad
toolchain matrices. Hosted pure-logic checks plus a real-reference local build are
an acceptable solution; CI need not compile the plugin using substitutes.

**Definition of done:**

- A clean checkout with documented local prerequisites builds the DLL and runs
  the focused checks through one command.
- Build/check failures return nonzero and cannot present stale output as a new
  successful deliverable.
- Evidence identifies version, source revision, working-tree modifications if any,
  relevant reference hashes, and the resulting DLL hash.
- The owner handoff build compiles against the selected real references.
- Hosted checks accurately state their coverage and limitations; version identity
  follows VERSION plus the build-number convention.

### SQ-2.2 — Prototype package and owner procedure

**Story:** As the owner, I want an inspected package and concrete instructions so
that I can evaluate the prototype without reconstructing the developer's setup.

**Scope:** Complete the scaffold for private handoff: plugin, metadata, README,
license, valid simple icon, package inspection, and one concise feasibility procedure.

**Out of scope:** Public release, marketing assets, branded artwork exploration,
installer development, automated deployment, broad gameplay tutorials, or requiring
the owner to repeat deterministic tests already covered offline.

**Definition of done:**

- Inspection checks expected ZIP paths/content, metadata, valid icon dimensions,
  consistent versioning, and the identified DLL hash.
- No game/runtime dependency assemblies, caches, credentials, or local paths are
  included. The owner receives the exact inspected artifact.
- The procedure names concrete recipes, quantities, starting materials, unlocks,
  and setup that exercise intermediates followed by multiple requests.
- It covers every kickoff live acceptance criterion, including native continuity,
  queue advancement, explicit components, reopening, and a subsequent session.
- Installation and cleanup identify exactly what the owner adds/removes without
  overwriting unrelated mods or saves.
- Results record build identity, pass/fail observations, and reproducible failure
  details. Deterministic coverage is supplied as build evidence, not owner homework.

### SQ-2.3 — Owner feasibility result

**Story:** As the owner, I want a clear result from the agreed observations so that
I can decide whether the prototype meets its intended behavior.

**Scope:** Owner executes the procedure; record evidence for the identified build;
route failures to the affected story and recheck the correction and impacted behavior.

**Out of scope:** Agent-run live validation, assuming silence means acceptance,
unrelated refinements discovered during play, exhaustive compatibility testing,
and release publication.

**Definition of done:**

- Every kickoff acceptance criterion has an explicit result for the final build.
- Required automated checks pass for that same build; observations from earlier
  builds are not silently attributed to it.
- No failed/unobserved criterion is represented as passing; feasibility is recorded
  only when all criteria pass under documented conditions.
- Failed observations return only affected scope for correction and proportionate
  recheck. Test conditions and remaining compatibility limits accompany the result.
- The authoritative status record links the evidence and distinguishes owner
  acceptance from publication authorization.

## Execution order

SQ-1.1 -> SQ-1.2 -> SQ-1.3 -> SQ-1.4 -> SQ-2.1 -> SQ-2.2 -> SQ-2.3.
Tests accompany their implementation story; procedure drafting can happen as the
integration takes shape. Target framework, binding details, and CI reference
strategy are bounded story decisions, not additional preparatory epics.
