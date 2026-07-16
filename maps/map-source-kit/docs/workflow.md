# Workflow

## Normal Change Loop

1. Create a small task for one system.
2. Edit text source in `src/war3map.j`.
3. Build with `tools/build.ps1`.
4. Test `builds/MyMap_test.w3x` in Warcraft III.
5. Review the text diff.
6. Keep the change only if behavior matches or the intentional behavior change is documented.

## Feature Flag Pattern

Use a boolean flag for replacement systems, defaulting off.

Example:

```jass
boolean udg_use_new_spawns = false
```

Recommended behavior:

- Flag off: old system runs exactly as before.
- Flag on: new system runs, and only the replaced old triggers are disabled.
- Add a host-only chat command later for test toggles.
- Add verbose debug output for in-game observability.

Current migration commands:

```text
-newspawns   toggles udg_use_new_spawns
-migverbose  toggles udg_migration_verbose
-migration   shows current migration flag status
```

Only the detected host or an admin level 90+ player can use these commands.
The flags do not replace gameplay behavior yet; they are switchboard infrastructure for future feature-flagged work.

## Testing Rule

Never replace more than one gameplay system per test pass.

For each change:

- Test map start.
- Test the old behavior with the flag off.
- Test the new behavior with the flag on.
- Watch for duplicate timers, duplicate shops, duplicate units, or missing cleanup.
- Test multiplayer-sensitive logic carefully.

## World Editor Rule

Do not save GUI trigger edits over this source without re-extracting and merging the generated script. World Editor can regenerate `war3map.j`.
