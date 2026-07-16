# Changelog

## 2026-07-09

- Created script-as-source map kit using `src/war3map.j` and `basemap/MyMap_DEV.w3x`.
- Added build tooling that injects `src/war3map.j` into a copied `.w3x`.
- Verified extracted built-map script matches `src/war3map.j`.
- Added first migration flags:
  - `udg_use_new_spawns`, default `false`
  - `udg_migration_verbose`, default `false`
  - `-newspawns`
  - `-migverbose`
  - `-migration`
- Migration flags are host/admin-only and do not replace gameplay behavior yet.
- Added generated hero/spell catalog with 91 heroes and ability stat rows.
- Added `tools\ObjectDumpJson` and `tools\GenerateHeroSpellCatalog.ps1` for repeatable object-data extraction.
