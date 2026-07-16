# System Inventory

This inventory is based on inspection of the extracted `war3map.j`.

## Scale

- About 499 `InitTrig_...` trigger initializers.
- Many are generated GUI triggers or library/support triggers.
- The migration should be system-based, not raw-trigger-count based.

## High-Value Clusters

### Tavern / Hero Selection

Relevant triggers:

- `set_hero_numbers`
- `set_hero_stats`
- `Start_SD`
- `Start_AP`
- `Start_AR`
- `TP_hero`
- `Hero_Skills`
- `restore_visibility_to_tavern`
- `move_unit_hero_showroom`
- `remove_timer_repick`

Why it matters:

- Central player-facing flow.
- Lots of static data and repeated stock logic.
- Good candidate for feature-flagged rewrite.

### Mode Selection

Relevant triggers:

- `MODE_GUI_show`
- `MODE_GUI_execute`
- `MODE_set_pub_game_default`
- `MODE_set_pub_game_custom`
- `MODE_set_pro_mode`
- `MODE_set_inhouse`
- `MODE_new`
- `Start_Vote`
- `Vote_SD_button`
- `Vote_AP_button`
- `Vote_AR_button`
- `Vote_Timer_Expires`

Why it matters:

- Controls which systems start.
- Should be migrated only after tavern/hero selection is documented.

### Spawns

Relevant triggers:

- `set_spawn_variables`
- `set_spawn_rates`
- `set_base`
- `eight_start_timer`
- `eight`
- `five`
- `ten`
- `eight_new`
- `ten_new`
- `twelve_new`
- `No_double_tech`
- `check_if_team_active`
- `Kill_units_of_inactive_player`

Why it matters:

- Runtime performance-sensitive.
- Uses timers and repeated production logic.
- Already had individual spawn-rate work.

Current migration infrastructure:

- `udg_use_new_spawns` defaults to `false`.
- `udg_migration_verbose` defaults to `false`.
- `-newspawns`, `-migverbose`, and `-migration` are registered in `InitTrig_migration_flags`.
- These flags are not wired into spawn replacement behavior yet.

### Hot Periodic Spell Systems

Examples:

- hook loops
- knockback loops
- projectile/missile loops
- pounce/shockwave/gust loops
- wall countdowns
- high-frequency fake-shop visibility logic

Why it matters:

- These are likely performance hot paths.
- Audit active-instance tracking and timer shutdown.

## Performance Audit Signals

Observed in generated script:

- Many timer/periodic registrations.
- Many group/unit scan patterns.
- Many location creation patterns.

These are not automatically bugs because generated code and BJ helpers can obscure cleanup, but they are the right places to audit first.
