# Project status and steering

## Authority

This is the sole authoritative record of current phase, epic/story status,
implementation readiness, and acceptance. Update those facts here only. The
[roadmap](management/ROADMAP.md) defines work, exclusions, and completion criteria;
the [kickoff](KICKOFF.md) defines the product contract. Other documents provide
instructions or dated evidence, not parallel status records. Current owner
instructions take precedence.

## Current phase

The two-epic, seven-story prototype delivery was owner accepted. Thunderstore
received 1.0.0 but rejected its community listing. The owner relayed moderator
feedback that the runtime `BindingChecks` class triggered their filters and that
binding failures should surface as errors.

The 1.0.1 correction removes that startup gate and its fixtures. Patch installation
failures are rethrown after cleanup. Queue presentation, click identity, and recovery
behavior are unchanged. Offline hook metadata checks remain in the test executable,
which is not shipped. The prior owner acceptance is historical; no new runtime
acceptance or moderator approval has been reported for this correction.

## Release readiness

Local validation of 1.0.1 passed: zero-warning hosted and real-reference builds,
offline queue/hook/identity checks, and byte-identical DLLs across both builds.
[CI run 34315112469](https://github.com/shytamir/DSPSmartQueue/actions/runs/34315112469)
passed compilation, offline checks, ZIP inspection, and package rejection tests.
The downloaded `DSPSmartQueue-1.0.1` artifact was independently inspected: exactly
five public files, no `BindingChecks` in the DLL, and a DLL identical to the local
real-reference build. Real-game metadata checks also passed for the CI DLL.

- ZIP: `DSPSmartQueue-1.0.1-6ba698bb5ac0-4579573F7F9C.zip`.
- ZIP SHA256: `4579573F7F9C8363432BF9B05AF22CE41BE724F39C17CC5CA941A8C5F22B8F78`.
- DLL SHA256: `4FA5F7BF4600D41731103D61D3CC46132E26A20C9C4C7F3DBB89F49D958E5551`.

Download the artifact from that run and extract its contained release ZIP. Delivery remains a GitHub Actions
artifact; agents do not install the plugin, run the game, or publish to Thunderstore.
The public ZIP contains exactly manifest, player README, icon, license, and plugin
DLL. Build evidence is uploaded separately. Shims and internal documents are excluded.

The rejected 1.0.0 submission was byte-identical to the inspected artifact from
[run 34313606162](https://github.com/shytamir/DSPSmartQueue/actions/runs/34313606162)
(SHA256 `644D0A72FEB93AAA907F93EDBE388F1B739B6FB156D8B9552CE201C67925592F`).
Its successful format checks did not establish Thunderstore moderation acceptance.
It is superseded by this correction and should not be resubmitted unchanged.

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

## Historical prototype handoff

- Procedure: [owner feasibility procedure](OWNER-PROCEDURE.md), enclosed in that historical ZIP only.
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

The owner handles runtime validation and resubmission of the corrected CI artifact.
The [Thunderstore listing](https://thunderstore.io/c/dyson-sphere-program/p/DSPSmartQueue/DSPSmartQueue/)
was rejected; approval has not been reported.
