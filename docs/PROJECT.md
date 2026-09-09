# Project status and steering

## Authority

This is the sole authoritative record of current phase, epic/story status,
implementation readiness, and acceptance. Update those facts here only. The
[roadmap](management/ROADMAP.md) defines work, exclusions, and completion criteria;
the [kickoff](KICKOFF.md) defines the product contract. Other documents provide
instructions or dated evidence, not parallel status records. Current owner
instructions take precedence.

## Current phase

The two-epic, seven-story prototype delivery is complete and owner accepted.
The first public release is 1.0.0, under DSPSmartQueue-DSPSmartQueue. The owner
will publish it. Publication has not been reported.

The owner runtime-validated the final-icon 0.9.0 candidate. The subsequent 1.0.0
promotion changed version identity only. Public-package correction and verification
are in progress; no new runtime behavior is being introduced.

## Release readiness

Earlier package checks incorrectly retained the private handoff's owner procedure
and build evidence. Those ZIPs, including the initial 1.0.0 ZIP from `fa2cd58`, are
superseded for publication. The old instruction to publish 0.9.0 is withdrawn.

The public ZIP contract is exactly manifest, player README, icon, license, and
plugin DLL. Build/inspection JSON stays alongside the ZIP for maintainers. The
owner procedure and all management documents remain in the repository. Publication
uses only the corrected ZIP identified here after verification.

The owner supplied the icon and approved the README/tagline. The README's viewport
wording was corrected to state that requests beyond the first 14 remain queued.
Release scripts and their regression checks enforce the public package boundary.
The GitHub workflow runs offline checks; real-reference compilation and ZIP checks
run locally. No agent-installed or agent-run gameplay is claimed.

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

The owner will publish the corrected 1.0.0 package. Agents perform repository
work and offline verification; installation and gameplay remain owner actions.
No public release URL or publication success has been recorded.