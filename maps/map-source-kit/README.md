# WC3 Map Source Kit

This kit makes the map workable for Codex, Claude, and GitHub by giving them text files to edit and review.

## Source Of Truth

`src/war3map.j` is the editable gameplay script source.

`basemap/MyMap_DEV.w3x` is the binary shell for terrain, object data, imports, regions, doodads, and other non-script map data.

Do not use World Editor GUI triggers as the source of truth at the same time. If World Editor saves GUI trigger changes, it can regenerate `war3map.j` and overwrite script changes.

## Layout

```text
basemap/
  MyMap_DEV.w3x              Binary map shell.
src/
  war3map.j                  Editable compiled JASS script.
tools/
  build.ps1                  Builds a playable test map.
  extract.ps1                Re-extracts script/reference files from a map.
  ReplaceWar3MapScript/      Small MPQ patching tool.
docs/
  architecture.md            How the current map systems fit together.
  workflow.md                PR/test/merge loop.
  system-inventory.md        Trigger/system map from inspection.
extracted-reference/
  war3map.wtg/wct/wts/etc.   Reference files from the current map.
builds/
  MyMap_test.w3x             Generated test map output.
```

## Build

From this folder:

```powershell
.\tools\build.ps1
```

If Windows blocks local PowerShell scripts, use:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\build.ps1
```

That copies `basemap/MyMap_DEV.w3x`, replaces its internal `war3map.j` with `src/war3map.j`, and writes:

```text
builds/MyMap_test.w3x
```

## Working Rule

For agent changes:

1. Edit `src/war3map.j` or clearly documented modules that are concatenated into it later.
2. Build `builds/MyMap_test.w3x`.
3. Test in Warcraft III.
4. Review the text diff before accepting the change.

Preserve behavior first. Optimize second. Change one system per test pass.

## Hero And Spell Catalog

The generated hero/spell reference is:

```text
docs/hero-spell-catalog.md
docs/hero-spell-catalog.json
```

Regenerate it with:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\GenerateHeroSpellCatalog.ps1
```

The catalog combines hero stats from `src/war3map.j`, hero spell lists from `war3map.w3u`, ability stats from `war3map.w3a`, and readable ability names from `war3map.wts`.
