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
prototype delivery. **SQ-1.1 is active and ready to implement.** Implementation
begins in the next work session after this planning update is published to main;
this update does not perform implementation.

Repository scaffolding and local preparation are complete. The
[preparation evidence](REFERENCE-BASELINE.md) records static inspection and toolkit
checks; [local development](LOCAL-DEVELOPMENT.md) documents reusable commands.
No plugin implementation, plugin compilation, or live feasibility validation has
been completed. The GitHub workflow produces an incomplete scaffold artifact,
not an installable package.

## Work tracking

| Epic | Status |
| --- | --- |
| SQ-1: Safe integrated queue adapter | Active; implementation not started |
| SQ-2: Identified prototype delivery and feasibility | Planned |

| Story | Status | Completion evidence |
| --- | --- | --- |
| [SQ-1.1](management/ROADMAP.md#sq-11--minimal-plugin-foundation) | Active; ready to implement | None yet |
| [SQ-1.2](management/ROADMAP.md#sq-12--identify-visible-requests) | Planned | None yet |
| [SQ-1.3](management/ROADMAP.md#sq-13--present-and-operate-the-same-task) | Planned | None yet |
| [SQ-1.4](management/ROADMAP.md#sq-14--recover-and-restore-native-consistency) | Planned | None yet |
| [SQ-2.1](management/ROADMAP.md#sq-21--repeatable-identified-prototype-build) | Planned | None yet |
| [SQ-2.2](management/ROADMAP.md#sq-22--prototype-package-and-owner-procedure) | Planned | None yet |
| [SQ-2.3](management/ROADMAP.md#sq-23--owner-feasibility-result) | Planned | None yet |

Record concise evidence against each story's definition of done before marking it
complete. Activation identifies the next work; it is not completion evidence.
Choose the plugin identity, target framework, and exact binding signatures within
SQ-1.1. No further planning document is required to begin that bounded story.

## Delivery boundary

Agents perform repository implementation, static inspection, and offline checks.
The owner installs and executes the plugin for live observations. Prototype
feasibility is unestablished. GitHub release and Thunderstore publication are not
authorized; pushing repository work to main does not change that boundary.
