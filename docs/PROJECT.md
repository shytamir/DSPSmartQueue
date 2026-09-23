# Project status and steering

## Authority

This is the sole authoritative record of current phase, epic/story status,
implementation readiness, and acceptance. Update those facts here only. The
[roadmap](management/ROADMAP.md) is the entry point for future work definitions;
the [kickoff](KICKOFF.md) defines the product contract. Other documents provide
instructions or dated evidence, not parallel status records. Current owner
instructions take precedence.

## Current phase

**Maintenance mode.** There is no active epic, story, implementation work, or
pending validation gate. The game-update documentation, minor-version promotion,
package validation, tagging, and store publication are complete. New work requires
owner direction; historical plans do not authorize implementation.

## 2026-09-24 closeout

- Recorded support for DSP **0.10.35.29057** and the owner's successful validation.
- Promoted MINOR from 0 to 1; the unchanged workflow's BuildNumber 1 produces
  **1.1.1**. Runtime implementation is unchanged.
- Verified local and hosted builds, downloaded package contents, version fields,
  and artifact hashes. Release evidence is recorded below.
- The owner confirmed that version 1.1 was tagged and published. Remote tag
  [`1.1`](https://github.com/shytamir/DSPSmartQueue/tree/1.1) resolves to
  `f2055a57c9fdf0fe882de2d01bbb35486948a9ef`; Thunderstore lists package **1.1.1**
  as active. Its public download is byte-identical to the verified CI ZIP below.
- Archived the completed prototype roadmap and restored the live roadmap to a
  planning placeholder. All work in this closeout is done.

The closeout date is 2026-09-24 in Europe/Madrid. Compatibility and promotion
validation recorded on 2026-09-23 retain their original dates.

## Current game compatibility

Dyson Sphere Program **0.10.35.29057** is fully supported by the existing plugin
without code changes. On 2026-09-23, the owner reported rerunning all existing
validations after the game update and confirmed that everything works as is.
This records owner validation, including runtime acceptance; no agent-run gameplay
or new automated test run is claimed by this documentation update.

The installed game's `Updates/Versions.txt` identifies `0.10.35.29057`, dated
2026-09-23; static inspection of `GameConfig` confirms the `0.10.35` default.
Read-only hashing on the same date identified the updated managed references:

| Reference | SHA256 |
| --- | --- |
| `Assembly-CSharp.dll` | `e75d3fe4b6a9ca822766189f826ba3a8348dfb7e301aa37ff6779db29a83fd8d` |
| `UnityEngine.UI.dll` | `c5acc25ebd597b586d323af506fb88e760322550940f1a3b0ea4fc8143fcb26c` |

The other six references in the saved local preparation baseline are unchanged.
The [2026-09-09 preparation evidence](REFERENCE-BASELINE.md), prototype procedure,
implementation records, and release artifact hashes retain their original identities.

## Published release

The published 1.1.1 package at tagged source revision `f2055a57c9fdf0fe882de2d01bbb35486948a9ef`
passed [CI run 35922336188](https://github.com/shytamir/DSPSmartQueue/actions/runs/35922336188):
zero-warning compilation, offline queue/hook/identity checks, ZIP inspection, and
all nine package rejection checks. The downloaded public artifact and separate
build evidence were independently inspected on 2026-09-23.

| Version surface | Verified value |
| --- | --- |
| Actions artifact | `DSPSmartQueue-1.1.1` |
| Release ZIP version, manifest, BepInEx plugin metadata | `1.1.1` |
| DLL assembly and file versions | `1.1.1.0` |
| DLL product version, build and inspection evidence | `1.1.1` |

The downloaded DLL is byte-identical to the zero-warning local build against the
updated game references identified above. Offline logic and real-game hook metadata
checks also passed for the downloaded DLL. Both evidence files identify the clean
tagged revision; all recorded ZIP, entry, DLL, and build-evidence hashes match.

- ZIP: `DSPSmartQueue-1.1.1-f2055a57c9fd-7F86077D1E2E.zip`.
- ZIP SHA256: `7F86077D1E2ECEED1247074483776195EFBE285BEC0E7786FE8ED57805CE54C3`.
- DLL SHA256: `66C3F51EC6203A75A60488A57B198928C00666489BB2DAD242FCFD5F6DACB47E`.

The release is available from [Thunderstore](https://thunderstore.io/c/dyson-sphere-program/p/DSPSmartQueue/DSPSmartQueue/).
Its public download was verified on 2026-09-24 against the CI ZIP identified above.
The public ZIP contains exactly manifest,
player README, icon, license, and plugin DLL. Build evidence is uploaded separately.
Shims and internal documents are excluded. The earlier owner game-update validation
is recorded above; these promotion checks add no new agent-run gameplay claim.

The initial promotion at `3e5b7bc8d7c0857f74676070d7912ee5a047146e` also passed
[CI run 35921719655](https://github.com/shytamir/DSPSmartQueue/actions/runs/35921719655).
Its earlier ZIP `DSPSmartQueue-1.1.1-3e5b7bc8d7c0-6C47F962178E.zip` had SHA256
`6C47F962178E8F0FE0D46760CC2E6E8AD0A7E03DA26A935C7697CFD1592BE1F4` and the same DLL.

## Historical 1.0.1 correction

Thunderstore received 1.0.0 but rejected its community listing. The owner relayed
moderator feedback that the runtime `BindingChecks` class triggered their filters
and that binding failures should surface as errors. The 1.0.1 correction removed
that startup gate and its fixtures; patch installation failures are rethrown after
cleanup. Queue presentation, click identity, and recovery behavior were unchanged.
Offline hook metadata checks remain in the test executable, which is not shipped.

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

This historical artifact is superseded by the verified 1.1.1 promotion above.

The rejected 1.0.0 submission was byte-identical to the inspected artifact from
[run 34313606162](https://github.com/shytamir/DSPSmartQueue/actions/runs/34313606162)
(SHA256 `644D0A72FEB93AAA907F93EDBE388F1B739B6FB156D8B9552CE201C67925592F`).
Its successful format checks did not establish Thunderstore moderation acceptance.
It is superseded by this correction and should not be resubmitted unchanged.

## Historical prototype completion

| Epic | Status |
| --- | --- |
| SQ-1: Safe integrated queue adapter | Complete; owner accepted |
| SQ-2: Identified prototype delivery and feasibility | Complete; owner accepted |

| Story | Status | Completion evidence |
| --- | --- | --- |
| [SQ-1.1](archive/2026-09-24-prototype-roadmap.md#sq-11--minimal-plugin-foundation) | Complete | [Compilation, metadata and negative checks](implementation/SQ-1.1.md) |
| [SQ-1.2](archive/2026-09-24-prototype-roadmap.md#sq-12--identify-visible-requests) | Complete | [Read-only mapping and focused checks](implementation/SQ-1.2.md) |
| [SQ-1.3](archive/2026-09-24-prototype-roadmap.md#sq-13--present-and-operate-the-same-task) | Complete | [Integration, boundary checks and limits](implementation/SQ-1.3.md) |
| [SQ-1.4](archive/2026-09-24-prototype-roadmap.md#sq-14--recover-and-restore-native-consistency) | Complete | [Recovery, cleanup checks and live observation handoff](implementation/SQ-1.4.md) |
| [SQ-2.1](archive/2026-09-24-prototype-roadmap.md#sq-21--repeatable-identified-prototype-build) | Complete | [Build identity, clean checkout and failure checks](implementation/SQ-2.1.md) |
| [SQ-2.2](archive/2026-09-24-prototype-roadmap.md#sq-22--prototype-package-and-owner-procedure) | Complete | [Package inspection and procedure coverage](implementation/SQ-2.2.md) |
| [SQ-2.3](archive/2026-09-24-prototype-roadmap.md#sq-23--owner-feasibility-result) | Complete | [Owner acceptance report](#owner-acceptance) |

The two-epic, seven-story prototype delivery was owner accepted. Its archived
definitions and completion evidence are retained above for reference.
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

The owner completed runtime validation, tagging, and store publication. No release
action remains pending. Future installation, gameplay validation, and publication
remain owner actions unless separately authorized; repository pushes alone do not
authorize a new release.
