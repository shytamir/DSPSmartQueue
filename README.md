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

The intended first delivery is a prototype DLL and a concise owner feasibility
procedure. Agents perform compilation and deterministic checks; the owner performs
installation and in-game validation. Feasibility and publication remain separate.

## Versioning and package artifacts

`.github/workflows/build.yml` runs offline checks on pushes to `main` and manual
dispatch. Private packages are built locally against the selected real references:

```powershell
./scripts/New-PrototypePackage.ps1 -BuildNumber 0
```

VERSION supplies major/minor components; the build number supplies patch (CI uses
its run number). The command requires committed source, runs the build/checks, and
writes an inspected ZIP with an adjacent inspection record under `artifacts/package/`.
Its filename includes version, source revision, and a hash prefix. See
[local development](docs/LOCAL-DEVELOPMENT.md) for prerequisites and
[owner procedure](docs/OWNER-PROCEDURE.md) for private evaluation.

Consult PROJECT.md for the exact handoff and acceptance state. Repository pushes
and local packaging do not publish releases or install anything.

## License

[Apache License 2.0](LICENSE).
