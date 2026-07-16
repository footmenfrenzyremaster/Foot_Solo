$ErrorActionPreference = "Stop"

$projectRoot = "C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799"
$scriptPath = Join-Path $projectRoot "extracted\799W-tester\files\war3map.j"
$heroesLua = Join-Path $projectRoot "src\data\heroes.lua"
$catalogMd = Join-Path $projectRoot "docs\hero_catalog.md"
$changelog = Join-Path $projectRoot "docs\changelog.md"

$lines = Get-Content -LiteralPath $scriptPath

$rawcodes = @{}
foreach ($line in $lines) {
    if ($line -match "set\s+udg_hero_type\[(\d+)\]='([^']+)'") {
        $rawcodes[[int]$matches[1]] = $matches[2]
    }
}

$statsStart = 0
$statsEnd = $lines.Count - 1
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match 'function\s+Trig_set_hero_stats_Actions') {
        $statsStart = $i
        break
    }
}
for ($i = $statsStart; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match 'call\s+DestroyTrigger\(GetTriggeringTrigger\(\)\)') {
        $statsEnd = $i
        break
    }
}

$heroes = @{}
$currentName = $null
foreach ($line in $lines[$statsStart..$statsEnd]) {
    if ($line -match '^\s*//(.+?)\s*$') {
        $candidate = $matches[1].Trim()
        if ($candidate -and $candidate -notmatch '^(=|\*|-|Trigger:|===========================================================================)') {
            $currentName = $candidate
        }
        continue
    }

    if ($line -match 'set\s+udg_hero_(category|STR_base|STR_inc|AGI_base|AGI_inc|INT_base|INT_inc|move_base|turnrate)\[(\d+)\]=([0-9.]+)') {
        $field = $matches[1]
        $index = [int]$matches[2]
        $valueText = $matches[3]
        if (-not $heroes.ContainsKey($index)) {
            $heroes[$index] = [ordered]@{
                index = $index
                name = $currentName
                rawcode = if ($rawcodes.ContainsKey($index)) { $rawcodes[$index] } else { $null }
            }
        }
        if (-not $heroes[$index].name -and $currentName) {
            $heroes[$index].name = $currentName
        }
        $value = if ($valueText.Contains(".")) { [double]$valueText } else { [int]$valueText }
        $heroes[$index][$field] = $value
    }
}

function To-Key($name) {
    $key = $name.ToUpperInvariant()
    $key = $key -replace "[^A-Z0-9]+", "_"
    $key = $key.Trim("_")
    if (-not $key) { $key = "HERO" }
    if ($key[0] -match '[0-9]') { $key = "HERO_$key" }
    return $key
}

function Lua-String($value) {
    if ($null -eq $value) { return "nil" }
    return '"' + ($value -replace '\\', '\\' -replace '"', '\"') + '"'
}

function Lua-Number($value) {
    if ($null -eq $value) { return "nil" }
    if ($value -is [double] -or $value -is [float]) {
        return $value.ToString("0.########", [Globalization.CultureInfo]::InvariantCulture)
    }
    return [string]$value
}

$orderedHeroes = $heroes.Keys | Sort-Object | ForEach-Object { $heroes[$_] }
if ($orderedHeroes.Count -ne 91) {
    throw "Expected 91 heroes, parsed $($orderedHeroes.Count)"
}

$usedKeys = @{}
$lua = New-Object System.Collections.Generic.List[string]
$lua.Add("-- Hero rawcodes and balance metadata extracted from war3map.j.")
$lua.Add("--")
$lua.Add("-- Generated from the original map's `Trig_set_hero_stats_Actions` and")
$lua.Add("-- `udg_hero_type` assignments. Verify in-game before using for live balance changes.")
$lua.Add("")
$lua.Add("local Heroes = {")

foreach ($hero in $orderedHeroes) {
    $key = To-Key $hero.name
    if ($usedKeys.ContainsKey($key)) {
        $usedKeys[$key] += 1
        $key = "$key`_$($usedKeys[$key])"
    } else {
        $usedKeys[$key] = 1
    }

    $lua.Add("    $key = {")
    $lua.Add("        index = $($hero.index),")
    $lua.Add("        name = $(Lua-String $hero.name),")
    $lua.Add("        rawcode = $(Lua-String $hero.rawcode),")
    $lua.Add("        category = $(Lua-Number $hero.category),")
    $lua.Add("        strength_base = $(Lua-Number $hero.STR_base),")
    $lua.Add("        strength_gain = $(Lua-Number $hero.STR_inc),")
    $lua.Add("        agility_base = $(Lua-Number $hero.AGI_base),")
    $lua.Add("        agility_gain = $(Lua-Number $hero.AGI_inc),")
    $lua.Add("        intelligence_base = $(Lua-Number $hero.INT_base),")
    $lua.Add("        intelligence_gain = $(Lua-Number $hero.INT_inc),")
    $lua.Add("        move_speed = $(Lua-Number $hero.move_base),")
    $lua.Add("        turn_rate = $(Lua-Number $hero.turnrate),")
    $lua.Add("        weight = 10,")
    $lua.Add("    },")
}

$lua.Add("}")
$lua.Add("")
$lua.Add("return Heroes")
Set-Content -LiteralPath $heroesLua -Value $lua -Encoding ASCII

$md = New-Object System.Collections.Generic.List[string]
$md.Add("# Hero Catalog")
$md.Add("")
$md.Add("Generated from `extracted/799W-tester/files/war3map.j`.")
$md.Add("")
$md.Add("Total heroes: $($orderedHeroes.Count)")
$md.Add("")
$md.Add("| # | Rawcode | Name | Category | STR | STR+ | AGI | AGI+ | INT | INT+ | Move | Turn |")
$md.Add("|---:|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|")
foreach ($hero in $orderedHeroes) {
    $md.Add("| $($hero.index) | $($hero.rawcode) | $($hero.name) | $($hero.category) | $($hero.STR_base) | $($hero.STR_inc) | $($hero.AGI_base) | $($hero.AGI_inc) | $($hero.INT_base) | $($hero.INT_inc) | $($hero.move_base) | $($hero.turnrate) |")
}
Set-Content -LiteralPath $catalogMd -Value $md -Encoding ASCII

$changelogText = Get-Content -LiteralPath $changelog -Raw
if ($changelogText -notmatch "Generated 91-hero Lua catalog") {
    $changelogText = $changelogText -replace "(?m)^## Unreleased\s*", "## Unreleased`r`n`r`n- Generated 91-hero Lua catalog from extracted trigger script.`r`n- Added readable `docs/hero_catalog.md` table.`r`n"
    Set-Content -LiteralPath $changelog -Value $changelogText -Encoding ASCII
}

Write-Output "Generated $($orderedHeroes.Count) heroes."
