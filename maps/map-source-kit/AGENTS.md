# Agent Instructions

This is a Warcraft III custom map source kit.

## Hard Rules

- Treat `src/war3map.j` as the script source of truth.
- Treat `basemap/MyMap_DEV.w3x` as the binary shell.
- Do not edit GUI triggers in World Editor and then overwrite `src/war3map.j` without explicitly extracting and reviewing the new script.
- Keep changes small enough to test in-game.
- Use feature flags for replacement systems.
- Default new systems off until tested.
- Preserve current behavior before optimizing.
- Update `docs/workflow.md`, `docs/system-inventory.md`, or relevant notes when changing system boundaries.

## Review Focus

When reviewing changes, look for:

- desync risk
- unbounded periodic loops
- full-map unit scans
- leaks from groups, locations, timers, effects, and forces
- changed mode-selection behavior
- hero/tavern behavior regressions
- old and new systems both running accidentally

## Build

Use:

```powershell
.\tools\build.ps1
```

The output map is:

```text
builds/MyMap_test.w3x
```

