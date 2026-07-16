$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$scriptPath = Join-Path $root "src\war3map.j"
$wtsPath = Join-Path $root "extracted-reference\war3map.wts"
$unitsJsonPath = Join-Path $root "work-units.json"
$abilitiesJsonPath = Join-Path $root "work-abilities.json"
$outputMd = Join-Path $root "docs\hero-spell-catalog.md"
$outputJson = Join-Path $root "docs\hero-spell-catalog.json"

if (-not (Test-Path -LiteralPath $unitsJsonPath) -or -not (Test-Path -LiteralPath $abilitiesJsonPath)) {
    $dumpTool = Join-Path $root "tools\ObjectDumpJson\ObjectDumpJson.csproj"
    dotnet run --project $dumpTool -- unit (Join-Path $root "extracted-reference\war3map.w3u") $unitsJsonPath
    if ($LASTEXITCODE -ne 0) { throw "Unit object dump failed." }
    dotnet run --project $dumpTool -- ability (Join-Path $root "extracted-reference\war3map.w3a") $abilitiesJsonPath
    if ($LASTEXITCODE -ne 0) { throw "Ability object dump failed." }
}

$lines = Get-Content -LiteralPath $scriptPath
$unitDump = Get-Content -LiteralPath $unitsJsonPath -Raw | ConvertFrom-Json
$abilityDump = Get-Content -LiteralPath $abilitiesJsonPath -Raw | ConvertFrom-Json

$unitEntries = @{}
foreach ($table in $unitDump.Tables) {
    foreach ($entry in $table.Entries) {
        if ($entry.NewId -and $entry.NewId -notmatch "^\x00+$") {
            $unitEntries[$entry.NewId] = $entry
        }
    }
}

$abilityEntries = @{}
foreach ($table in $abilityDump.Tables) {
    foreach ($entry in $table.Entries) {
        if ($entry.NewId -and $entry.NewId -notmatch "^\x00+$") {
            $abilityEntries[$entry.NewId] = $entry
        } elseif ($entry.OldId -and $entry.OldId -notmatch "^\x00+$") {
            $abilityEntries[$entry.OldId] = $entry
        }
    }
}

$abilityNames = @{}
foreach ($line in Get-Content -LiteralPath $wtsPath) {
    if ($line -match '//\s+Abilities:\s+([A-Za-z0-9]{4})\s+\(([^)]+)\),') {
        $code = $matches[1]
        $name = $matches[2]
        if (-not $abilityNames.ContainsKey($code)) {
            $abilityNames[$code] = $name
        }
    }
}

$fallbackAbilityNames = @{
    void = "Placeholder / no ability"
    AHhb = "Holy Light"; AHds = "Divine Shield"; AHad = "Devotion Aura"; AHre = "Resurrection"
    AHbz = "Blizzard"; AHab = "Brilliance Aura"; AHwe = "Summon Water Elemental"; AHmt = "Mass Teleport"
    AHfs = "Flame Strike"; AHbn = "Banish"; AHdr = "Siphon Mana"; AHpx = "Phoenix"
    AHtb = "Storm Bolt"; AHtc = "Thunder Clap"; AHbh = "Bash"; AHav = "Avatar"
    AOwk = "Wind Walk"; AOcr = "Critical Strike"; AOmi = "Mirror Image"; AOww = "Bladestorm"
    AOfs = "Feral Spirit"; AOsf = "Far Sight"; AOcl = "Chain Lightning"; AOeq = "Earthquake"
    AOsh = "Shockwave"; AOae = "Endurance Aura"; AOws = "War Stomp"; AOre = "Reincarnation"
    AOhw = "Healing Wave"; AOhx = "Hex"; AOvd = "Serpent Ward"; AOsw = "Big Bad Voodoo"
    AUdc = "Death Coil"; AUdp = "Death Pact"; AUau = "Unholy Aura"; AUan = "Animate Dead"
    AUfn = "Frost Nova"; AUfu = "Frost Armor"; AUdr = "Dark Ritual"; AUdd = "Death and Decay"
    AUav = "Vampiric Aura"; AUsl = "Sleep"; AUcs = "Carrion Swarm"; AUin = "Inferno"
    AUim = "Impale"; AUts = "Spiked Carapace"; AUcb = "Carrion Beetles"; AUls = "Locust Swarm"
    AEer = "Entangling Roots"; AEdt = "Force of Nature"; AEah = "Thorns Aura"; AEtq = "Tranquility"
    AEst = "Scout"; AHfa = "Searing Arrows"; AEar = "Trueshot Aura"; AEsf = "Starfall"
    AEmb = "Mana Burn"; AEim = "Immolation"; AEev = "Evasion"; AEme = "Metamorphosis"
    AEbl = "Blink"; AEsb = "Shadow Strike"; AEfk = "Fan of Knives"; AEsv = "Vengeance"
    ANbf = "Breath of Fire"; ANdh = "Drunken Haze"; ANdb = "Drunken Brawler"; ANef = "Storm, Earth, and Fire"
    ANrf = "Rain of Fire"; ANht = "Howl of Terror"; ANca = "Cleaving Attack"; ANdo = "Doom"
    ANsg = "Summon Bear"; ANsq = "Summon Quilbeast"; ANsw = "Summon Hawk"; ANst = "Stampede"
    ANsi = "Silence"; ANba = "Black Arrow"; ANdr = "Life Drain"; ANch = "Charm"
    ANso = "Soul Burn"; ANlm = "Lava Spawn"; ANic = "Incinerate"; ANvc = "Volcano"
    ANhs = "Healing Spray"; ANcr = "Chemical Rage"; ANab = "Acid Bomb"; ANtm = "Transmute"
    ANsy = "Pocket Factory"; ANcs = "Cluster Rockets"; ANeg = "Engineering Upgrade"; ANrg = "Robo-Goblin"
}

$baseHeroAbilities = @{
    Hpal = @("AHhb", "AHds", "AHad", "AHre")
    Hamg = @("AHbz", "AHab", "AHwe", "AHmt")
    Hblm = @("AHfs", "AHbn", "AHdr", "AHpx")
    Hmkg = @("AHtb", "AHtc", "AHbh", "AHav")
    Obla = @("AOwk", "AOcr", "AOmi", "AOww")
    Ofar = @("AOfs", "AOsf", "AOcl", "AOeq")
    Otch = @("AOsh", "AOae", "AOws", "AOre")
    Oshd = @("AOhw", "AOhx", "AOvd", "AOsw")
    Udea = @("AUdc", "AUdp", "AUau", "AUan")
    Ulic = @("AUfn", "AUfu", "AUdr", "AUdd")
    Udre = @("AUav", "AUsl", "AUcs", "AUin")
    Ucrl = @("AUim", "AUts", "AUcb", "AUls")
    Ekee = @("AEer", "AEdt", "AEah", "AEtq")
    Emoo = @("AEst", "AHfa", "AEar", "AEsf")
    Edem = @("AEmb", "AEim", "AEev", "AEme")
    Ewar = @("AEbl", "AEsb", "AEfk", "AEsv")
    Npbm = @("ANbf", "ANdh", "ANdb", "ANef")
    Nplh = @("ANrf", "ANht", "ANca", "ANdo")
    Nbst = @("ANsg", "ANsq", "ANsw", "ANst")
    Nbrn = @("ANsi", "ANba", "ANdr", "ANch")
    Nfir = @("ANso", "ANlm", "ANic", "ANvc")
    Nalc = @("ANhs", "ANcr", "ANab", "ANtm")
    Ntin = @("ANsy", "ANcs", "ANeg", "ANrg")
    Nngs = @("ANfl", "ANfa", "ANms", "ANto")
}

$rawcodes = @{}
foreach ($line in $lines) {
    if ($line -match "set\s+udg_hero_type\[(\d+)\]='([^']+)'") {
        $idx = [int]$matches[1]
        if ($idx -le 91) {
            $rawcodes[$idx] = $matches[2]
        }
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
        if ($candidate -and $candidate -notmatch '^(=|\*|-|Trigger:)') {
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
                rawcode = $rawcodes[$index]
            }
        }
        $heroes[$index][$field] = if ($valueText.Contains(".")) { [double]$valueText } else { [int]$valueText }
    }
}

function Join-LevelValues($mods, [string]$field) {
    $values = @{}
    foreach ($mod in $mods | Where-Object { $_.Id -eq $field -and $null -ne $_.Level -and $_.Level -gt 0 }) {
        $values[[int]$mod.Level] = $mod.Value
    }
    if ($values.Count -eq 0) { return $null }
    $max = ($values.Keys | Measure-Object -Maximum).Maximum
    $parts = for ($i = 1; $i -le $max; $i++) {
        if ($values.ContainsKey($i)) { [string]$values[$i] } else { "-" }
    }
    return ($parts -join "/")
}

function Ability-Name([string]$code, $entry) {
    if ($abilityNames.ContainsKey($code)) { return $abilityNames[$code] }
    if ($fallbackAbilityNames.ContainsKey($code)) { return $fallbackAbilityNames[$code] }
    if ($entry -and $abilityNames.ContainsKey($entry.OldId)) { return $abilityNames[$entry.OldId] }
    if ($entry -and $fallbackAbilityNames.ContainsKey($entry.OldId)) { return $fallbackAbilityNames[$entry.OldId] }
    if ($entry) { return "Base " + $entry.OldId }
    return "Unknown"
}

function Ability-Summary([string]$code) {
    $entry = $abilityEntries[$code]
    $mods = if ($entry) { @($entry.Mods) } else { @() }
    $levelMod = $mods | Where-Object Id -eq "alev" | Select-Object -First 1
    $levels = if ($levelMod) { [int]$levelMod.Value } elseif ($mods.Count -gt 0) { [int](($mods | Where-Object { $_.Level -gt 0 } | Measure-Object Level -Maximum).Maximum) } else { $null }

    $common = [ordered]@{
        cooldown = Join-LevelValues $mods "acdn"
        mana = Join-LevelValues $mods "amcs"
        range = Join-LevelValues $mods "aran"
        area = Join-LevelValues $mods "aare"
        duration = Join-LevelValues $mods "adur"
        hero_duration = Join-LevelValues $mods "ahdu"
    }

    $skip = @("alev","alsk","acdn","amcs","aran","aare","adur","ahdu","atar","abuf","aeff","aart","arac","aher","arlv","areq","ahky","auhk","atp1","aub1","aret","arut","anam","ansf","abpx","abpy","aubx","auby")
    $dataFields = @()
    $dataIds = $mods | Where-Object { $_.Level -gt 0 -and $skip -notcontains $_.Id } | Select-Object -ExpandProperty Id -Unique
    foreach ($id in $dataIds | Sort-Object) {
        $joined = Join-LevelValues $mods $id
        if ($joined) {
            $dataFields += [ordered]@{ field = $id; values = $joined }
        }
    }

    [ordered]@{
        rawcode = $code
        base_rawcode = if ($entry) { $entry.OldId } else { $null }
        name = Ability-Name $code $entry
        levels = $levels
        cooldown = $common.cooldown
        mana = $common.mana
        range = $common.range
        area = $common.area
        duration = $common.duration
        hero_duration = $common.hero_duration
        data_fields = $dataFields
        object_data_found = [bool]$entry
    }
}

$catalog = @()
foreach ($index in ($heroes.Keys | Sort-Object)) {
    $hero = $heroes[$index]
    $unitEntry = $unitEntries[$hero.rawcode]
    $uhabMod = if ($unitEntry) { $unitEntry.Mods | Where-Object Id -eq "uhab" | Select-Object -First 1 } else { $null }
    $source = "object uhab"
    if ($uhabMod -and $uhabMod.Value) {
        $spellCodes = @($uhabMod.Value -split "," | ForEach-Object { $_.Trim() } | Where-Object { $_ })
    } elseif ($unitEntry -and $baseHeroAbilities.ContainsKey($unitEntry.OldId)) {
        $spellCodes = $baseHeroAbilities[$unitEntry.OldId]
        $source = "inherited from base " + $unitEntry.OldId
    } else {
        $spellCodes = @()
        $source = "missing"
    }

    if ($hero.rawcode -eq "O00G") {
        $spellCodes += @("A0EA", "A0EK", "A07X", "A078", "A07D")
        $source = $source + " + trigger-detected Blooddancer handlers"
    }

    $catalog += [ordered]@{
        index = $hero.index
        name = $hero.name
        rawcode = $hero.rawcode
        base_rawcode = if ($unitEntry) { $unitEntry.OldId } else { $null }
        category = $hero.category
        strength = "$($hero.STR_base) + $($hero.STR_inc)"
        agility = "$($hero.AGI_base) + $($hero.AGI_inc)"
        intelligence = "$($hero.INT_base) + $($hero.INT_inc)"
        move_speed = $hero.move_base
        turn_rate = $hero.turnrate
        spell_source = $source
        spells = @($spellCodes | ForEach-Object { Ability-Summary $_ })
    }
}

$catalog | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $outputJson -Encoding UTF8

$md = New-Object System.Collections.Generic.List[string]
$md.Add("# Hero Spell Catalog")
$md.Add("")
$md.Add("Generated from `src/war3map.j`, `war3map.w3u`, `war3map.w3a`, and `war3map.wts`.")
$md.Add("")
$md.Add("Notes:")
$md.Add("- Hero base stats come from `Trig_set_hero_stats_Actions`.")
$md.Add("- Spell lists come from unit `uhab` object data when present.")
$md.Add("- If `uhab` is not present, the catalog uses the inherited Warcraft III base hero spell list and marks it as inherited.")
$md.Add("- Ability numbers are raw object-editor fields. `cooldown`, `mana`, `range`, `area`, `duration`, and `hero_duration` are shown as slash-separated values by level.")
$md.Add("- `data_fields` are raw ability data fields such as `Htc1` or `Ucs3`; these usually contain the actual damage, bonus, count, or scaling values, depending on the base ability.")
$md.Add("")

foreach ($hero in $catalog) {
    $tick = [char]96
    $md.Add("## $($hero.index). $($hero.name) ($tick$($hero.rawcode)$tick)")
    $md.Add("")
    $md.Add("Base: $tick$($hero.base_rawcode)$tick | Category: $($hero.category) | STR: $($hero.strength) | AGI: $($hero.agility) | INT: $($hero.intelligence) | Move: $($hero.move_speed) | Turn: $($hero.turn_rate)")
    $md.Add("")
    $md.Add("Spell source: $($hero.spell_source)")
    $md.Add("")
    $md.Add("| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |")
    $md.Add("|---:|---|---|---|---:|---|---|---|---|---|---|---|")
    $slot = 1
    foreach ($spell in $hero.spells) {
        $dataText = if ($spell.data_fields.Count -gt 0) {
            (($spell.data_fields | Select-Object -First 8 | ForEach-Object { "$($_.field)=$($_.values)" }) -join "; ")
        } else {
            ""
        }
        if ($spell.data_fields.Count -gt 8) {
            $dataText += "; ..."
        }
        $md.Add("| $slot | $tick$($spell.rawcode)$tick | $($spell.name) | $tick$($spell.base_rawcode)$tick | $($spell.levels) | $($spell.cooldown) | $($spell.mana) | $($spell.range) | $($spell.area) | $($spell.duration) | $($spell.hero_duration) | $dataText |")
        $slot++
    }
    while ($slot -le 4) {
        $md.Add("| $slot |  | Missing / not in `uhab` |  |  |  |  |  |  |  |  |  |")
        $slot++
    }
    $md.Add("")
}

Set-Content -LiteralPath $outputMd -Value $md -Encoding UTF8

Write-Host "Wrote $outputMd"
Write-Host "Wrote $outputJson"
