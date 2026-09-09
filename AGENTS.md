# Working on DSPSmartQueue

## Read first

Read this file and `docs/PROJECT.md` before making changes. Use `docs/KICKOFF.md`
for the product contract and `docs/management/ROADMAP.md` for planning status.
Current owner instructions take precedence. Reference repositories and archives
are examples, not authority.

## Scope and product boundaries

The repository is in initial buildup before its first planning session. Scaffolding
does not authorize implementation or release. Keep the roadmap a placeholder until
planning occurs and keep steering records concise.

- Build a small BepInEx presentation adapter for the existing 14-slot queue strip.
- Preserve the native queue, crafting, countdown, and executing-task progress.
  No queue mutation, save data, new panels, or viewport expansion.
- Identify action products by `ForgeTask.parentTaskIndex < 0`. Map slots to task
  references and resolve current native indices at click time, never by recipe ID.
- Keep icon, quantity, hover, and action target consistent. Consume empty, stale,
  or uncertain clicks; valid input reaches the original handler exactly once.
- Enable and disable presentation and input together. Restore a consistent native
  view on failure, clear mappings at session changes, and remove hooks on shutdown.
- Treat kickoff hook locations as candidates requiring prototype validation.

## Local work

Static inspection of selected game managed assemblies is allowed. Do not install
or execute the plugin, launch the game for validation, or modify installed game/mod
files. The owner performs installed and in-game validation. Do not load game
assemblies into an executing test host merely to inspect metadata.

Use DSPRecipeTracker as a read-only packaging/test reference and the local BepInEx
checkout as a dependency reference. Do not transfer their architecture, identities,
release state, or project-specific governance. Do not commit or redistribute game
assemblies, decompiled game code, credentials, local absolute paths, or tool caches.

Prefer PowerShell 7, .NET SDK, Git, and ripgrep. Batch independent reads and cache
targeted static inspection by assembly hash. Keep machine configuration and
agent-only tools in ignored `artifacts/.runtime-tools/` or a project-specific
directory in the shared untracked resource area. Add reusable build/check commands
as needed. Introduce a Unity project or extra toolchain only for a concrete need.

When implementation is authorized, test filtering, ordering, quantities, identity,
input recovery, and cleanup independently of Unity where possible. Compile against
the selected real references before owner handoff. Hosted reference substitutes
cannot establish game compatibility. Distinguish static checks, compilation,
automated tests, owner observations, and publication.

## Git and packaging

Check status before edits, preserve unrelated changes, and stage reviewed named
paths only. Commit and push when requested; do not force-push. For sandbox ownership
errors, use per-command `git -c safe.directory=<repo>` rather than global changes.

`VERSION` contains manually edited `MAJOR` and `MINOR` integers. The scaffold
workflow appends the GitHub run number as patch version. Its artifact is explicitly
incomplete. Package completion, GitHub releases, and Thunderstore publication
require later scope decisions.
