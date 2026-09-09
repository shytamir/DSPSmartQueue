# DSPSmartQueue

A minimal BepInEx mod for Dyson Sphere Program, intended for distribution as a
Thunderstore package. It makes the Replicator queue easier to control by showing
the products the player requested and hiding automatically queued intermediates.

See [PROJECT.md](docs/PROJECT.md), the authoritative record of project status,
active work, and delivery readiness.

## Intended behavior

The existing 14-slot strip shows the first 14 outstanding action-product tasks in
native order, packed from the left. Explicitly requested components remain visible;
the same components queued automatically for another product are hidden.

```text
Native queue:  [component] [component] [Product A] [component] [Product B]
Visible strip: [Product A] [Product B]
```

Each entry retains its native recipe icon and remaining quantity. Left-click
selects its recipe and right-click cancels its native task through the original
handlers. Separate requests remain separate unless DSP itself merges them. The
queue count includes action products beyond the visible 14 slots.

DSP still owns crafting order, material allocation, cancellation, dependent work,
and material returns. The whole-queue countdown and actual executing-task progress
remain native, including when a hidden intermediate is crafting. Recipe selection,
Produce controls, keyboard behavior, and feedback retain native behavior.

The queue retains its full capacity. Reordering, scrolling, paging, additional
slots, separate panels, and mod-owned save data are outside scope.

## Project records

- [Kickoff](docs/KICKOFF.md): complete intended behavior and prototype acceptance contract.
- [Roadmap](docs/management/ROADMAP.md): epic/story scope and completion criteria.
- [Agent instructions](AGENTS.md): repository working conventions.
- [Local development](docs/LOCAL-DEVELOPMENT.md): toolkit activation and cached inspection.
- [Reference baseline](docs/REFERENCE-BASELINE.md): static findings for planning.

For live evaluation, agents provide compilation and deterministic checks; the
owner performs installation and in-game validation using the owner procedure.
Consult PROJECT.md for acceptance and release readiness.

## Versioning and package artifacts

The [Build release package workflow](https://github.com/shytamir/DSPSmartQueue/actions/workflows/build.yml)
compiles, checks, and packages each push to `main`; it also supports manual dispatch.
Download the `DSPSmartQueue-<version>` artifact from a successful run and extract
its contained release ZIP for Thunderstore. The separate `build-evidence` artifact
is for maintainers and is not part of the public package.

VERSION supplies major/minor components; the release workflow specifies the patch component with BuildNumber.
The ZIP filename includes version, source revision, and a hash prefix. Hosted builds
use pinned public dependencies and minimal compile-only game/UI declarations;
none of those references are shipped. See [local development](docs/LOCAL-DEVELOPMENT.md)
for real-reference validation and [PROJECT.md](docs/PROJECT.md) for release readiness.
Repository pushes do not publish releases or install anything.

## License

[Apache License 2.0](LICENSE).
