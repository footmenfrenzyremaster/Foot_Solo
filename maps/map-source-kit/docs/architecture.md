# Architecture Notes

The map is currently driven by a generated JASS script extracted as `src/war3map.j`.

## Current Major System Clusters

- Initialization: global setup, version setup, colors, locations, teams, base values.
- Mode selection: host/vote flow, SD/AP/AR selection, public/private/pro/inhouse variants.
- Tavern and hero selection: hero rawcode tables, stat tables, tavern stock, random/all-pick/same-draft behavior, repick timer, showroom behavior.
- Spawns and income: base setup, spawn timers, tech-gated spawn groups, individual spawn-rate experiments.
- KOTH: hill ownership, gold income, hill reset, floating text, board updates.
- Items: item limits, combo limits, scroll/rune item triggers, wall/conversion items.
- Hero spells: many periodic movement/projectile/channel loops.
- Admin/debug/chat commands: host/admin commands, debug commands, version infrastructure.
- Observer/UI: multiboard, camera/zoom, observer visibility, mode dialogs.

## Current Migration Priority

1. Stabilize script-as-source workflow.
2. Document trigger clusters.
3. Add feature flags and debug observability.
4. Migrate low-risk data tables.
5. Migrate one gameplay system at a time.

## Migration Flags

The first script-side migration infrastructure has been added:

- `udg_use_new_spawns`
- `udg_migration_verbose`
- `InitTrig_migration_flags`

These support host/admin chat commands for toggling and inspecting future replacement systems. They intentionally do not change spawn behavior yet.

## Critical Workflow Decision

Once `src/war3map.j` is source of truth, World Editor should be used mainly for terrain, objects, imports, regions, and visual map editing. GUI trigger edits become risky unless they are intentionally re-extracted and merged back into `src/war3map.j`.

Alternative workflows:

- Full script-as-source: recommended for agent PRs.
- GUI master: agents provide snippets, user pastes into World Editor, repo is documentation/support only.
- Save-as-folder: check current World Editor support; this may provide a better native loose-file workflow.
