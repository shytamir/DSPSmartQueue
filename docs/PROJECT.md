# Project status and steering

## Authority

This is the sole authoritative record of current phase, epic/story status,
implementation readiness, and acceptance. Update those facts here only. The
[roadmap](management/ROADMAP.md) defines work, exclusions, and completion criteria;
the [kickoff](KICKOFF.md) defines the product contract. Other documents provide
instructions or dated evidence, not parallel status records. Current owner
instructions take precedence.

## Current phase

The two-epic, seven-story prototype delivery is complete. The owner accepted the
identified handoff after passing all live observations and cleanup. No planned
stories remain open. The owner runtime-validated version 0.9.0 with the final icon
and approved first publication after the final review. The publishing namespace
is DSPSmartQueue; the owner is creating that team. Publication has not occurred.

Repository scaffolding and local preparation are complete. The
[preparation evidence](REFERENCE-BASELINE.md) records static inspection and toolkit
checks; [local development](LOCAL-DEVELOPMENT.md) documents reusable commands.
The adapter compiles and its offline binding, mapping, interaction, and lifecycle/
recovery checks passed for the handoff build. Owner-reported live feasibility
validation passed. The GitHub workflow runs offline logic/binding fixtures; private
package creation uses the local real-reference build. Installation and live
observations were performed by the owner, not the agent.

## Release readiness

Pre-release review on 2026-09-09 covered runtime boundaries, build identity,
package contents, approved player-facing copy, and CI. VERSION is 0.9 (local
package version 0.9.0; a supplied build number replaces the patch component).

- The approved package README and manifest tagline were applied.
- The pinned checkout action was updated to Node 24 to resolve the hosted Node 20
  deprecation warning; stale repository overview wording was removed.
- Clean-checkout real-reference compilation passed with zero warnings/errors,
  along with offline logic and real-metadata checks and ZIP inspection. Package
  probes rejected extra/missing/duplicate entries, altered DLL/icon/version, and
  dirty build evidence. Runtime source, tests, and build scripts were unchanged
  from the accepted prototype.
- The owner-supplied icon replaced the placeholder. Its PNG format and 256x256
  dimensions were verified; the original file was copied unchanged.
- The management pass removed obsolete hook-validation wording. PROJECT.md remains
  the sole current status record; dated prototype evidence and acceptance retain
  their original scope. The owner subsequently approved first publication.

The original acceptance below belongs to the 0.1.0 prototype. The owner separately
reported another successful runtime validation for the final-icon 0.9.0 candidate
on 2026-09-09. Final archive inspection passed; the declared BepInEx dependency was
available, the repository was public, and hosted checks passed for the same source
([run 34311531714](https://github.com/shytamir/DSPSmartQueue/actions/runs/34311531714)).

Publish the existing validated ZIP, without rebuilding:

- Namespace/package/version: `DSPSmartQueue-DSPSmartQueue-0.9.0`.
- ZIP: `artifacts/package/DSPSmartQueue-0.9.0-d63cba1e8097-9278B9FD1344.zip`.
- Source: `d63cba1e809754f2c6de2483b1536263d9ede54f`.
- ZIP SHA256: `9278B9FD1344E9E32C88F60799BB59E92239D266F2A09031CEC3768C49D1FDB6`.
- DLL SHA256: `282CEBD93269F95F3512F1CD2ADB7B6A71C78891A135F81DB9424757305922C5`.

The enclosed build.json records automated build-time evidence; its runtimeValidated
flag is false because the build command never runs the game. The later owner
acceptance is recorded here without rewriting the validated archive.

## Work tracking

| Epic | Status |
| --- | --- |
| SQ-1: Safe integrated queue adapter | Complete; owner accepted |
| SQ-2: Identified prototype delivery and feasibility | Complete; owner accepted |

| Story | Status | Completion evidence |
| --- | --- | --- |
| [SQ-1.1](management/ROADMAP.md#sq-11--minimal-plugin-foundation) | Complete | [Compilation, metadata and negative checks](implementation/SQ-1.1.md) |
| [SQ-1.2](management/ROADMAP.md#sq-12--identify-visible-requests) | Complete | [Read-only mapping and focused checks](implementation/SQ-1.2.md) |
| [SQ-1.3](management/ROADMAP.md#sq-13--present-and-operate-the-same-task) | Complete | [Integration, boundary checks and limits](implementation/SQ-1.3.md) |
| [SQ-1.4](management/ROADMAP.md#sq-14--recover-and-restore-native-consistency) | Complete | [Recovery, cleanup checks and live observation handoff](implementation/SQ-1.4.md) |
| [SQ-2.1](management/ROADMAP.md#sq-21--repeatable-identified-prototype-build) | Complete | [Build identity, clean checkout and failure checks](implementation/SQ-2.1.md) |
| [SQ-2.2](management/ROADMAP.md#sq-22--prototype-package-and-owner-procedure) | Complete | [Package inspection and procedure coverage](implementation/SQ-2.2.md) |
| [SQ-2.3](management/ROADMAP.md#sq-23--owner-feasibility-result) | Complete | [Owner acceptance report](#owner-acceptance) |

Record concise evidence against each story's definition of done before marking it
complete. Activation identifies the next work; it is not completion evidence.
Plugin identity is defined in `src/DSPSmartQueue/Plugin.cs`; target framework and
initial binding verification are recorded in SQ-1.1's evidence.

## Owner handoff

- Procedure: [owner feasibility procedure](OWNER-PROCEDURE.md), also enclosed in the ZIP.
- Local artifact: `artifacts/package/DSPSmartQueue-0.1.0-639767658c41-B257B3F2B504.zip`.
- Inspection record: the adjacent `DSPSmartQueue-0.1.0-639767658c41-B257B3F2B504.zip.inspection.json`.
- Version: `0.1.0`; clean source revision: `639767658c417a676626b05c4c8ac2757e9c3809`.
- ZIP SHA256: `B257B3F2B50411CD346EDF69F4794E67E9FF3F199577FB25CE78BA6D511C3B87`.
- DLL SHA256: `A6AD76921FD7EC94B8F0A69A3C742CF01493CD5B32A24E0787803E9D6D669F04`.

This identifies the accepted handoff, not a future rebuild. Source changes after
that revision do not alter the archived ZIP.

## Owner acceptance

On 2026-09-09, the owner reported the following results for the identified handoff
and explicitly confirmed: "Live acceptance passes, the story is done."

| Observation | Owner result |
| --- | --- |
| Display (step 1) | Pass |
| Native continuity and hover (step 2) | Pass |
| Selection and recipe grid (step 3) | Pass |
| Out-of-order cancellation and feedback (step 4) | Pass |
| Selection after advancement (step 5) | Pass |
| Produce, keyboard controls, reopening (step 6) | Pass |
| Subsequent session (step 7) | Pass |
| Cleanup without plugin | Pass |

No failures were observed. The owner also marked package identity, build identity,
and environment checks as passing. The reply did not restate the full game build,
other-mod list, or handcraft upgrades; no specific values are inferred here.
Acceptance applies to this handoff under the owner's tested conditions, not a
broader compatibility matrix. The automated evidence for the same source and DLL
is linked under SQ-2.2 and retained with the package.

## Delivery boundary

Agents perform repository implementation, static inspection, and offline checks.
The owner approved first Thunderstore publication of the identified 0.9.0 ZIP
under the DSPSmartQueue team. Team creation and upload are outstanding. No public
release URL or publication success has been recorded.
