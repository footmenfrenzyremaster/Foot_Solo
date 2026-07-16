$ErrorActionPreference = "Stop"

$projectRoot = "C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799"
$scriptPath = Join-Path $projectRoot "extracted\799W-tester\files\war3map.j"
$spawnLua = Join-Path $projectRoot "src\data\spawn_units.lua"
$spawnCatalog = Join-Path $projectRoot "docs\spawn_unit_catalog.md"
$spawnSystem = Join-Path $projectRoot "docs\spawn_system.md"
$changelog = Join-Path $projectRoot "docs\changelog.md"

$lines = Get-Content -LiteralPath $scriptPath

function Find-FunctionRange($name) {
    $start = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "function\s+$name\s+takes") {
            $start = $i
            break
        }
    }
    if ($start -lt 0) {
        throw "Function not found: $name"
    }
    for ($i = $start + 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^endfunction\s*$') {
            return @($start, $i)
        }
    }
    throw "Function end not found: $name"
}

function Parse-JassValue($text) {
    $text = $text.Trim()
    if ($text -match "^'([^']+)'$") {
        return $matches[1]
    }
    if ($text -match '^[0-9]+\.[0-9]+$') {
        return [double]$text
    }
    if ($text -match '^-?[0-9]+$') {
        return [int]$text
    }
    return $text
}

function Lua-String($value) {
    if ($null -eq $value) { return "nil" }
    return '"' + ([string]$value -replace '\\', '\\' -replace '"', '\"') + '"'
}

function Lua-Number($value) {
    if ($null -eq $value) { return "nil" }
    if ($value -is [double] -or $value -is [float]) {
        return $value.ToString("0.########", [Globalization.CultureInfo]::InvariantCulture)
    }
    return [string]$value
}

function Timer-Group($index) {
    if (($index -ge 0 -and $index -le 13) -or ($index -ge 31 -and $index -le 43)) {
        return @{ name = "eight"; seconds = 8.00; activation = "active from spawnTimer"; mode = "legacy"; initially_enabled = $true }
    }
    if (($index -ge 14 -and $index -le 20) -or ($index -ge 44 -and $index -le 50)) {
        return @{ name = "five"; seconds = 6.25; activation = "enabled by Tech_Delay_T1 when version == 5"; mode = "legacy"; initially_enabled = $false }
    }
    if ($index -ge 21 -and $index -le 30) {
        return @{ name = "ten"; seconds = 8.75; activation = "enabled by Tech_Delay_T1 when version == 5"; mode = "legacy"; initially_enabled = $false }
    }
    return @{ name = $null; seconds = $null; activation = $null; mode = $null; initially_enabled = $null }
}

function Balanced-Group($index) {
    if ($index -ge 0 -and $index -le 3) {
        return @{ name = "eight_new"; seconds = 8.00; activation = "disabled and not enabled in extracted script" }
    }
    if ($index -ge 20 -and $index -le 35) {
        return @{ name = "ten_new"; seconds = 10.00; activation = "disabled and not enabled in extracted script" }
    }
    if ($index -ge 40 -and $index -le 43) {
        return @{ name = "twelve_new"; seconds = 12.00; activation = "disabled and not enabled in extracted script" }
    }
    return @{ name = $null; seconds = $null; activation = $null }
}

$spawnUnits = @{}
$currentName = $null
$sourceIndex = @{}
$range = Find-FunctionRange "Trig_set_spawn_variables_Actions"
foreach ($line in $lines[$range[0]..$range[1]]) {
    if ($line -match '^\s*//\s*(.+?)\s*$') {
        $comment = $matches[1].Trim()
        if ($comment -match '^\[(\d+)\]\s+(.+?)\s+\(from\s+(\d+)\)') {
            $currentName = $matches[2].Trim()
            $sourceIndex[[int]$matches[1]] = [int]$matches[3]
        } elseif ($comment -and $comment -notmatch '^(=|\*|-|\[|Base buildings)') {
            $currentName = $comment
        }
        continue
    }

    if ($line -match 'set\s+udg_(spawn_unit|unit_HP|unit_armor_type|unit_armor|unit_att_base|unit_att_num_dice|unit_att_dice_sides|unit_att_CD|unit_bounty_base|unit_bounty_num_dice|unit_bounty_dice_sides|unit_range|unit_level)\[(\d+)\]=(.+)$') {
        $field = $matches[1]
        $index = [int]$matches[2]
        $value = Parse-JassValue $matches[3]
        if (-not $spawnUnits.ContainsKey($index)) {
            $spawnUnits[$index] = [ordered]@{
                index = $index
                name = $currentName
            }
        }
        if (-not $spawnUnits[$index].name -and $currentName) {
            $spawnUnits[$index].name = $currentName
        }
        $spawnUnits[$index][$field] = $value
    }
}

$spawnRates = @{}
$range = Find-FunctionRange "Trig_set_spawn_rates_Actions"
foreach ($line in $lines[$range[0]..$range[1]]) {
    if ($line -match 'set\s+udg_unit_spawnrate\[(\d+)\]=(.+)$') {
        $spawnRates[[int]$matches[1]] = Parse-JassValue $matches[2]
    }
}

$baseTypes = @{}
$baseNames = @{}
$currentBaseName = $null
$range = Find-FunctionRange "Trig_set_base_Actions"
foreach ($line in $lines[$range[0]..$range[1]]) {
    if ($line -match '^\s*//\s*(.+?)\s*$') {
        $comment = $matches[1].Trim()
        if ($comment -match '^\d+\s+(.+)$') {
            $currentBaseName = $matches[1].Trim()
        } elseif ($comment -match '^\[(\d+)\]\s+(.+)$') {
            $currentBaseName = $matches[2].Trim()
        } elseif ($comment -and $comment -notmatch '^(=|\*|-|Base buildings)') {
            $currentBaseName = $comment
        }
        continue
    }
    if ($line -match "set\s+udg_base_type\[(\d+)\]='([^']+)'") {
        $idx = [int]$matches[1]
        $baseTypes[$idx] = $matches[2]
        if ($currentBaseName) {
            $baseNames[$idx] = $currentBaseName
        }
    }
}

$ordered = $spawnUnits.Keys | Sort-Object | ForEach-Object { $spawnUnits[$_] }
if ($ordered.Count -ne 51) {
    throw "Expected 51 spawn unit entries, parsed $($ordered.Count)"
}

$lua = New-Object System.Collections.Generic.List[string]
$lua.Add("-- Spawn unit catalog extracted from war3map.j.")
$lua.Add("--")
$lua.Add("-- The live map does not rely on Object Editor stats for spawned units.")
$lua.Add("-- Spawn triggers create units, then overwrite HP, armor, damage, range, level,")
$lua.Add("-- attack cooldown, bounty, and defense type from these indexed arrays.")
$lua.Add("")
$lua.Add("local SpawnUnits = {")
foreach ($u in $ordered) {
    $idx = [int]$u.index
    $timer = Timer-Group $idx
    $balanced = Balanced-Group $idx
    $lua.Add("    [$idx] = {")
    $lua.Add("        name = $(Lua-String $u.name),")
    $lua.Add("        rawcode = $(Lua-String $u.spawn_unit),")
    $lua.Add("        source_index = $(if ($sourceIndex.ContainsKey($idx)) { $sourceIndex[$idx] } else { "nil" }),")
    $lua.Add("        base_rawcode = $(Lua-String $(if ($baseTypes.ContainsKey($idx)) { $baseTypes[$idx] } else { $null })),")
    $lua.Add("        base_name = $(Lua-String $(if ($baseNames.ContainsKey($idx)) { $baseNames[$idx] } else { $null })),")
    $lua.Add("        timer_group = $(Lua-String $timer.name),")
    $lua.Add("        timer_seconds = $(Lua-Number $timer.seconds),")
    $lua.Add("        timer_activation = $(Lua-String $timer.activation),")
    $lua.Add("        balanced_timer_group = $(Lua-String $balanced.name),")
    $lua.Add("        balanced_timer_seconds = $(Lua-Number $balanced.seconds),")
    $lua.Add("        configured_spawnrate = $(Lua-Number $(if ($spawnRates.ContainsKey($idx)) { $spawnRates[$idx] } else { $null })),")
    $lua.Add("        hp = $(Lua-Number $u.unit_HP),")
    $lua.Add("        armor_type = $(Lua-Number $u.unit_armor_type),")
    $lua.Add("        armor = $(Lua-Number $u.unit_armor),")
    $lua.Add("        attack_base = $(Lua-Number $u.unit_att_base),")
    $lua.Add("        attack_dice = $(Lua-Number $u.unit_att_num_dice),")
    $lua.Add("        attack_sides = $(Lua-Number $u.unit_att_dice_sides),")
    $lua.Add("        attack_cooldown = $(Lua-Number $u.unit_att_CD),")
    $lua.Add("        range = $(Lua-Number $u.unit_range),")
    $lua.Add("        level = $(Lua-Number $u.unit_level),")
    $lua.Add("        bounty_base = $(Lua-Number $u.unit_bounty_base),")
    $lua.Add("        bounty_dice = $(Lua-Number $u.unit_bounty_num_dice),")
    $lua.Add("        bounty_sides = $(Lua-Number $u.unit_bounty_dice_sides),")
    $lua.Add("    },")
}
$lua.Add("}")
$lua.Add("")
$lua.Add("return SpawnUnits")
Set-Content -LiteralPath $spawnLua -Value $lua -Encoding ASCII

$md = New-Object System.Collections.Generic.List[string]
$md.Add("# Spawn Unit Catalog")
$md.Add("")
$md.Add("Generated from `extracted/799W-tester/files/war3map.j`.")
$md.Add("")
$md.Add("Important: spawned unit stats are applied by trigger after creation. Object Editor changes to HP/damage/armor/range/bounty can be overwritten by this system.")
$md.Add("")
$md.Add("Total spawn entries: $($ordered.Count)")
$md.Add("")
$md.Add("| # | Spawn Unit | Rawcode | Base Rawcode | Timer | Seconds | HP | Armor | Damage | CD | Range | Level | Bounty |")
$md.Add("|---:|---|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|")
foreach ($u in $ordered) {
    $idx = [int]$u.index
    $timer = Timer-Group $idx
    $baseRaw = if ($baseTypes.ContainsKey($idx)) { $baseTypes[$idx] } else { "" }
    $bounty = "{0}+{1}d{2}" -f $u.unit_bounty_base, $u.unit_bounty_num_dice, $u.unit_bounty_dice_sides
    $md.Add("| $idx | $($u.name) | $($u.spawn_unit) | $baseRaw | $($timer.name) | $($timer.seconds) | $($u.unit_HP) | $($u.unit_armor) | $($u.unit_att_base) | $($u.unit_att_CD) | $($u.unit_range) | $($u.unit_level) | $bounty |")
}
Set-Content -LiteralPath $spawnCatalog -Value $md -Encoding ASCII

$sys = @"
# Spawn System Notes

Readable source: `extracted/799W-tester/files/war3map.j`

## Why This System Is Hard To Tune

The map does not simply let spawned units use their Object Editor stats.

The spawn triggers create a unit from `udg_spawn_unit[index]`, then immediately overwrite many fields:

- max HP
- current HP
- armor
- defense type
- base damage
- attack dice
- attack sides
- attack cooldown
- attack range
- unit level
- bounty values

That means changing a spawned unit in Object Editor may do nothing if the trigger later overwrites that field.

## Current Legacy Spawn Groups

| Trigger | Active | Timing | Spawn indexes |
|---|---|---:|---|
| `eight` | yes | `udg_spawnTimer`, 8.00 seconds | `0-13`, `31-43` |
| `five` | disabled at init, enabled by `Tech_Delay_T1` when `udg_version == 5` | 6.25 seconds | `14-20`, `44-50` |
| `ten` | disabled at init, enabled by `Tech_Delay_T1` when `udg_version == 5` | 8.75 seconds | `21-30` |
| `eight_mass_bonus` | registered, dormant until `udg_spawnMassTimer` starts | 20.00 seconds during premass bonus | `0`, then `31-43` path exists |

## Current Balanced/New Spawn Groups

These triggers exist but are disabled in the extracted script and no `EnableTrigger` call was found for them.

| Trigger | Timing | Spawn indexes |
|---|---:|---|
| `eight_new` | 8.00 seconds | `0-3` |
| `ten_new` | 10.00 seconds | `20-35` |
| `twelve_new` | 12.00 seconds | `40-43` |

## Better Direction

Use `src/data/spawn_units.lua` as the readable source of truth.

Future change path:

1. Edit one row in `src/data/spawn_units.lua`.
2. Generate the matching `udg_spawn_unit`, `udg_unit_HP`, `udg_unit_armor`, damage, range, level, and bounty assignments.
3. Keep timer grouping explicit per unit instead of hiding it inside index ranges.
4. Eventually replace duplicate trigger bodies with one generic spawn function.

This would make changes like "nerf Dark Knight HP" or "move Rifleman to a different timer" a one-row edit instead of a hunt through generated JASS.
"@
Set-Content -LiteralPath $spawnSystem -Value $sys -Encoding ASCII

$changelogText = Get-Content -LiteralPath $changelog -Raw
if ($changelogText -notmatch "spawn unit catalog") {
    $changelogText = $changelogText -replace "(?m)^## Unreleased\s*", "## Unreleased`r`n`r`n- Added spawn unit catalog and spawn system notes.`r`n- Added `src/data/spawn_units.lua` as readable per-unit spawn data.`r`n"
    Set-Content -LiteralPath $changelog -Value $changelogText -Encoding ASCII
}

Write-Output "Generated $($ordered.Count) spawn unit entries."
