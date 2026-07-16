# WC3-799 Map Project

This folder is a Git/Codex-friendly workspace for the Warcraft III map `799W-tester`.

## Layout

```text
WC3-799/
  map/                 Packed map backup
  releases/            Versioned tester builds
  extracted/           Disassembled map files for reference
  src/                 Readable data and generated JASS systems
  docs/                Notes, bugs, changelog, migration docs
  tools/               Utility scripts for extraction/build work
  src/jass/            Script source baselines and migration candidates
  AGENTS.md            Instructions for coding agents
```

## Current Map

Original packed map:

```text
map/799W-tester.w3x
```

Injected individual-spawn test build:

```text
releases/799W-tester-individual-spawn.w3x
```

Cleaned JASS test builds:

```text
releases/799W-tester-cleaned-jass.w3x
releases/799W-tester-individual-spawn-cleaned-jass.w3x
```

Current recode test build:

```text
releases/799W-tester-recode-pass-21.w3x
```

This build includes the individual spawn scheduler, generated hero and spawn-unit data, repaired spell systems, the compact item-limit system, twelve-choice tavern support, Red-only setup, and the six-level Guardian Force Field rewrite. Pass 19 repairs transient ownership and several spells/upgrades. Pass 20 replaces the player-leave, abandoned-base, team-defeat, visibility, and victory lifecycle. Pass 21 fixes cross-team control cleanup, defeated-player vision persistence, and stale votekick/autopool dialogs found while recovering Claude's unfinished review.

Extracted reference files:

```text
extracted/799W-tester/files/
```

Important extracted files:

- `war3map.j`: generated JASS trigger script
- `war3map.wtg`: World Editor trigger data
- `war3map.wct`: custom trigger text
- `war3map.w3u`: unit object data
- `war3map.w3a`: ability object data
- `war3map.w3t`: item object data
- `war3map.imp`: imported file list

## Spell Breakdowns

Basic hero spell draft:

```text
docs/basic_hero_spell_breakdowns.md
docs/basic_hero_spell_cards.html
src/data/basic_hero_spells.json
```

## Workflow

1. Keep `map/799W-tester.w3x` as the stable packed backup.
2. Use `extracted/` to inspect current triggers, object data, imports, and generated script.
3. Record findings in `docs/`.
4. Build new systems in `src/` before migrating them into the map.
5. Save playable builds in `releases/` with versioned names.
6. Verify packed script changes by extracting `war3map.j` from the release map and comparing it to the generated script.

## JASS Recode Track

Current plan:

```text
docs/whole_map_recode_plan.md
```

Current source baseline:

```text
src/jass/war3map.original.j
```

Current generated systems:

```text
src/systems/individual_spawn_rates.jass
src/systems/hero_setup.generated.jass
src/systems/spawn_unit_data.generated.jass
src/systems/page_system.jass
src/systems/spell_cleanup_pass_5.jass
src/systems/glaive_loop_pass_6.jass
src/systems/item_limits_pass_8.jass
src/systems/red_mode_setup_pass_10.jass
src/systems/startup_repairs_pass_11.jass
src/systems/mode_pruning_pass_12.jass
src/systems/runtime_location_cleanup_pass_13.jass
src/systems/startup_dead_code_pass_14.jass
src/systems/local_handle_cleanup_pass_15.jass
src/systems/spell_lifecycle_cleanup_pass_16.jass
src/systems/claude_review_fixes_pass_17.jass
src/systems/guardian_force_field_pass_18.jass
src/systems/enumeration_ownership_pass_19.jass
src/systems/player_lifecycle_pass_20.jass
src/systems/player_lifecycle_review_pass_21.jass
```

## First Migration Target

The tavern and hero-pool systems are good first candidates for Lua migration because they involve arrays, randomization, uniqueness, category weights, and hero stock updates.
