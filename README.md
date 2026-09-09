# DSPSmartQueue

A planned minimal BepInEx mod for Dyson Sphere Program, distributed as a
Thunderstore package. It makes the Replicator queue easier to control by showing
the products the player requested and hiding automatically queued intermediates.

**Status: initial repository buildup, before the first planning session. No plugin
or installable package exists yet.**

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
- [Project steering](docs/PROJECT.md): current phase and authority boundaries.
- [Roadmap](docs/management/ROADMAP.md): placeholder pending planning.
- [Agent instructions](AGENTS.md): repository working conventions.

The intended first delivery is a prototype DLL and a concise owner feasibility
procedure. Agents perform compilation and deterministic checks; the owner performs
installation and in-game validation. Feasibility and publication remain separate.

## Packaging scaffold

`.github/workflows/build.yml` runs on pushes to `main` and manual dispatch. It reads
`MAJOR=0` and `MINOR=1` from `VERSION` and appends the workflow run number, producing
a `0.1.<run>` scaffold ZIP as a GitHub Actions artifact.

The ZIP contains draft manifest metadata, this README, the license, and a notice
listing unfinished package work. It lacks a plugin DLL and icon and is not an
installable or publishable Thunderstore package. Compilation, package validation,
final artwork, and publication are not implemented yet.

## License

[Apache License 2.0](LICENSE).
