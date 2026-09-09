# Project status and steering

## Authority

This is the sole authoritative record of current phase, epic/story status,
implementation readiness, and acceptance. Update those facts here only. The
[roadmap](management/ROADMAP.md) defines work, exclusions, and completion criteria;
the [kickoff](KICKOFF.md) defines the product contract. Other documents provide
instructions or dated evidence, not parallel status records. Current owner
instructions take precedence.

## Current phase

Planning is complete. The two-epic, seven-story roadmap is approved for the first
prototype delivery. SQ-1 implementation and SQ-2.1 are complete. SQ-2.2 is next
planned work and has not been started.

Repository scaffolding and local preparation are complete. The
[preparation evidence](REFERENCE-BASELINE.md) records static inspection and toolkit
checks; [local development](LOCAL-DEVELOPMENT.md) documents reusable commands.
The adapter compiles and its offline binding, mapping, interaction, and lifecycle/
recovery checks pass. Live feasibility validation has not been completed. The GitHub
workflow passes offline logic/binding fixtures and produces an incomplete scaffold
artifact, not an installable package. The local build emits an identified, verified
DLL and evidence record; packaging and the owner procedure remain SQ-2.2.

## Work tracking

| Epic | Status |
| --- | --- |
| SQ-1: Safe integrated queue adapter | Implementation complete; live feasibility remains SQ-2.3 |
| SQ-2: Identified prototype delivery and feasibility | In progress |

| Story | Status | Completion evidence |
| --- | --- | --- |
| [SQ-1.1](management/ROADMAP.md#sq-11--minimal-plugin-foundation) | Complete | [Compilation, metadata and negative checks](implementation/SQ-1.1.md) |
| [SQ-1.2](management/ROADMAP.md#sq-12--identify-visible-requests) | Complete | [Read-only mapping and focused checks](implementation/SQ-1.2.md) |
| [SQ-1.3](management/ROADMAP.md#sq-13--present-and-operate-the-same-task) | Complete | [Integration, boundary checks and limits](implementation/SQ-1.3.md) |
| [SQ-1.4](management/ROADMAP.md#sq-14--recover-and-restore-native-consistency) | Complete | [Recovery, cleanup checks and live observation handoff](implementation/SQ-1.4.md) |
| [SQ-2.1](management/ROADMAP.md#sq-21--repeatable-identified-prototype-build) | Complete | [Build identity, clean checkout and failure checks](implementation/SQ-2.1.md) |
| [SQ-2.2](management/ROADMAP.md#sq-22--prototype-package-and-owner-procedure) | Planned | None yet |
| [SQ-2.3](management/ROADMAP.md#sq-23--owner-feasibility-result) | Planned | None yet |

Record concise evidence against each story's definition of done before marking it
complete. Activation identifies the next work; it is not completion evidence.
Plugin identity is defined in `src/DSPSmartQueue/Plugin.cs`; target framework and
initial binding verification are recorded in SQ-1.1's evidence.
No further planning document is required before the next bounded story.

## Delivery boundary

Agents perform repository implementation, static inspection, and offline checks.
The owner installs and executes the plugin for live observations. Prototype
feasibility is unestablished. GitHub release and Thunderstore publication are not
authorized; pushing repository work to main does not change that boundary.
