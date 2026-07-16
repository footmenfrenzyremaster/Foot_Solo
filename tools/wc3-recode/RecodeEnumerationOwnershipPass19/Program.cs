using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-18.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "enumeration_ownership_pass_19.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-19.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "enumeration-ownership-pass-19", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));

var replacementFunctions = new[]
{
    "InitializeUnitIndexer",
    "Trig_Cripple_Wave_Actions",
    "Trig_Mirror_Image_Finish_Actions",
    "Trig_ITEM_Purge_the_Dead_Func002002003",
    "Trig_ITEM_Purge_the_Dead_Func005A",
    "Trig_ITEM_Purge_the_Dead_Actions",
    "Trig_ITEM_Scroll_of_Invisibility_Actions",
    "Trig_ITEM_Scroll_of_Inner_Fire_Actions",
    "Trig_Upgrades_level_Func002A",
    "Trig_Upgrades_level_Actions",
    "Trig_player_destroyed_Func003Func001C"
};

var transientEnumerations = new[]
{
    new EnumerationPatch("Trig_init_Actions", "call ForGroupBJ(GetUnitsOfPlayerAll(ConvertedPlayer(udg_X)), function Trig_init_Func032Func001Func021002)"),
    new EnumerationPatch("Trig_init_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(ConvertedPlayer(udg_X), 'h01G'), function Trig_init_Func091Func001A)"),
    new EnumerationPatch("Trig_Taunt_Turtle_Upgrade_Actions", "call ForGroupBJ(GetUnitsInRectAll(GetPlayableMapRect()), function Trig_Taunt_Turtle_Upgrade_Func003A)"),
    new EnumerationPatch("Trig_swap_execute_Actions", "call ForGroupBJ(GetUnitsOfPlayerMatching(udg_temp_player, Condition(function Trig_swap_execute_Func006Func004Func010001002)), function Trig_swap_execute_Func006Func004Func010A)"),
    new EnumerationPatch("Trig_swap_execute_Actions", "call ForGroupBJ(GetUnitsOfPlayerMatching(ConvertedPlayer(udg_temp_playernumber_ally_1), Condition(function Trig_swap_execute_Func006Func004Func012001002)), function Trig_swap_execute_Func006Func004Func012A)"),
    new EnumerationPatch("Trig_swap_execute_Actions", "call ForGroupBJ(GetUnitsOfPlayerMatching(ConvertedPlayer(udg_temp_playernumber), Condition(function Trig_swap_execute_Func007Func005Func008001002)), function Trig_swap_execute_Func007Func005Func008A)"),
    new EnumerationPatch("Trig_swap_execute_Actions", "call ForGroupBJ(GetUnitsOfPlayerMatching(ConvertedPlayer(udg_temp_playernumber_ally_2), Condition(function Trig_swap_execute_Func007Func005Func010001002)), function Trig_swap_execute_Func007Func005Func010A)"),
    new EnumerationPatch("Trig_set_unit_info_Actions", "call ForGroupBJ(GetUnitsSelectedAll(udg_temp_player), function Trig_set_unit_info_Func004A)"),
    new EnumerationPatch("Trig_set_unit_scale_Actions", "call ForGroupBJ(GetUnitsSelectedAll(udg_temp_player), function Trig_set_unit_scale_Func026A)"),
    new EnumerationPatch("Trig_set_unit_level_Actions", "call ForGroupBJ(GetUnitsSelectedAll(udg_temp_player), function Trig_set_unit_level_Func030A)"),
    new EnumerationPatch("Trig_remove_showroom_heros_Actions", "call ForGroupBJ(GetUnitsInRectOfPlayer(gg_rct_HEROshowroom, Player(PLAYER_NEUTRAL_PASSIVE)), function Trig_remove_showroom_heros_Func003A)"),
    new EnumerationPatch("Trig_Tentacle_Mass_Root_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(udg_temp_player, 'o018'), function Trig_Tentacle_Mass_Root_Func005A)"),
    new EnumerationPatch("Trig_HERO_Archangel_Smite_Actions", "call ForGroupBJ(GetUnitsInRangeOfLocMatching(udg_temp_AoE_array[udg_abilitylevel], udg_temp_point, Condition(function Trig_HERO_Archangel_Smite_Func017Func002001003)), function Trig_HERO_Archangel_Smite_Func017Func002A)"),
    new EnumerationPatch("Trig_HERO_Blooddancer_Illusions_summon_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(udg_temp_player, 'n02T'), function Trig_HERO_Blooddancer_Illusions_summon_Func007A)"),
    new EnumerationPatch("Trig_HERO_Blooddancer_Illusions_sacrifice_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(udg_temp_player, 'O00J'), function Trig_HERO_Blooddancer_Illusions_sacrifice_Func005A)"),
    new EnumerationPatch("Trig_HERO_Blooddancer_Illusions_sacrifice_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(udg_temp_player, 'O00G'), function Trig_HERO_Blooddancer_Illusions_sacrifice_Func006A)"),
    new EnumerationPatch("Trig_HERO_Blooddancer_Illusions_die_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(udg_temp_player, 'O00G'), function Trig_HERO_Blooddancer_Illusions_die_Func001Func004Func005A)"),
    new EnumerationPatch("Trig_HERO_Ships_Doc_wards_Actions", "call ForGroupBJ(GetUnitsOfPlayerAndTypeId(udg_temp_player, 'o00W'), function Trig_HERO_Ships_Doc_wards_Func007A)")
};

const string obsoleteUpgradeHelper = "Trig_Upgrades_level_Func002Func009Func004A";

ValidateOriginal(script, transientEnumerations);

foreach (var name in replacementFunctions)
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);

foreach (var patch in transientEnumerations)
    script = InsertDestroyFlag(script, patch);

script = ReplaceOnce(script,
    "    call DestroyGroup(udg_temp_unitgroup)\n    call RemoveLocation(udg_temp_point)\n    call DestroyGroup(udg_temp_unitgroup)\n    call DestroyGroup(udg_temp_unitgroup2)\n    call DestroyGroup(udg_temp_unitgroup3)",
    "    call DestroyGroup(udg_temp_unitgroup)\n    call DestroyGroup(udg_temp_unitgroup2)\n    call DestroyGroup(udg_temp_unitgroup3)\n    set udg_temp_unitgroup=null\n    set udg_temp_unitgroup2=null\n    set udg_temp_unitgroup3=null\n    set udg_temp_point=null");

script = ReplaceOnce(script,
    "    call ForForce(GetPlayersAllies(GetOwningPlayer(GetTriggerUnit())), function Trig_player_destroyed_Func003A)",
    "    set udg_temp_playergroup=GetPlayersAllies(GetOwningPlayer(GetTriggerUnit()))\n    call ForForce(udg_temp_playergroup, function Trig_player_destroyed_Func003A)\n    call DestroyForce(udg_temp_playergroup)\n    set udg_temp_playergroup=null");

script = RemoveFunction(script, obsoleteUpgradeHelper);

ValidateReplacement(script, transientEnumerations, obsoleteUpgradeHelper);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script, transientEnumerations.Length), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script, IReadOnlyList<EnumerationPatch> patches)
{
    Require(ExtractFunction(script, "InitializeUnitIndexer").Text, "Filter(function IndexUnit)");
    Require(ExtractFunction(script, "Trig_Cripple_Wave_Actions").Text, "RemoveLocation(udg_QS_Point[0])");
    Require(ExtractFunction(script, "Trig_Mirror_Image_Finish_Actions").Text, "RemoveLocation(udg_MIC_Point[udg_MIC_Loop])");
    Require(ExtractFunction(script, "Trig_ITEM_Purge_the_Dead_Func002002003").Text, "GetEnumUnit()");
    Require(ExtractFunction(script, "Trig_ITEM_Purge_the_Dead_Actions").Text, "GetUnitsInRangeOfLocMatching(800.00, GetSpellTargetLoc()");
    Require(ExtractFunction(script, "Trig_ITEM_Scroll_of_Invisibility_Actions").Text, "DestroyGroup(udg_temp_unitgroup2)");
    Require(ExtractFunction(script, "Trig_ITEM_Scroll_of_Inner_Fire_Actions").Text, "DestroyGroup(udg_temp_unitgroup2)");
    Require(ExtractFunction(script, "Trig_Upgrades_level_Actions").Text, "ForForce(GetPlayersAllies(GetTriggerPlayer())");
    Require(ExtractFunction(script, "Trig_player_destroyed_Func003Func001C").Text, "CountUnitsInGroup(GetUnitsOfPlayerMatching");
    foreach (var patch in patches)
    {
        var function = ExtractFunction(script, patch.FunctionName).Text;
        if (Regex.Matches(function, Regex.Escape(patch.Call)).Count != 1)
            throw new InvalidOperationException($"Expected one transient enumeration in {patch.FunctionName}: {patch.Call}");
    }
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializers in pass 18.");
}

static void ValidateReplacement(string script, IReadOnlyList<EnumerationPatch> patches, string obsoleteUpgradeHelper)
{
    var indexer = ExtractFunction(script, "InitializeUnitIndexer").Text;
    Require(indexer, "local boolexpr initialFilter=Filter(function IndexUnit)");
    Require(indexer, "call DestroyBoolExpr(initialFilter)");

    var cripple = ExtractFunction(script, "Trig_Cripple_Wave_Actions").Text;
    Require(cripple, "RemoveLocation(udg_QS_Point_Copy[0])");
    if (cripple.Contains("RemoveLocation(udg_QS_Point[", StringComparison.Ordinal))
        throw new InvalidOperationException("Cripple Wave still removes Searing Bullet's point array.");

    var mirror = ExtractFunction(script, "Trig_Mirror_Image_Finish_Actions").Text;
    Require(mirror, "set udg_MIC_Point[udg_MIC_Loop]=null");
    if (mirror.Contains("RemoveLocation(udg_MIC_Point[udg_MIC_Loop])", StringComparison.Ordinal))
        throw new InvalidOperationException("Mirror Image still double-removes MissileCreate's finish point.");

    var purgeFilter = ExtractFunction(script, "Trig_ITEM_Purge_the_Dead_Func002002003").Text;
    Require(purgeFilter, "GetFilterUnit()");
    if (purgeFilter.Contains("GetEnumUnit()", StringComparison.Ordinal))
        throw new InvalidOperationException("Purge the Dead still evaluates enumeration state inside its filter.");
    var purge = ExtractFunction(script, "Trig_ITEM_Purge_the_Dead_Actions").Text;
    Require(purge, "GetUnitsInRangeOfLocMatching(800.00, udg_temp_point");

    foreach (var name in new[] { "Trig_ITEM_Scroll_of_Invisibility_Actions", "Trig_ITEM_Scroll_of_Inner_Fire_Actions" })
    {
        var function = ExtractFunction(script, name).Text;
        if (function.Contains("DestroyGroup(udg_temp_unitgroup2)", StringComparison.Ordinal))
            throw new InvalidOperationException($"{name} still destroys an unowned shared group.");
    }

    var upgrades = ExtractFunction(script, "Trig_Upgrades_level_Actions").Text;
    Require(upgrades, "local force alliedPlayers=GetPlayersAllies(GetTriggerPlayer())");
    Require(upgrades, "PingMinimapLocForForceEx(alliedPlayers");
    Require(upgrades, "call DestroyForce(alliedPlayers)");
    var upgradeCallback = ExtractFunction(script, "Trig_Upgrades_level_Func002A").Text;
    Require(upgradeCallback, "local group affectedUnits=null");
    Require(upgradeCallback, "set affectedUnits=GetUnitsOfPlayerAndTypeId(GetEnumPlayer(), 'hgtw')");

    var baseCheck = ExtractFunction(script, "Trig_player_destroyed_Func003Func001C").Text;
    Require(baseCheck, "local boolean hasTownHall=FirstOfGroup(townHalls) != null");
    Require(baseCheck, "call DestroyGroup(townHalls)");

    foreach (var patch in patches)
    {
        var function = ExtractFunction(script, patch.FunctionName).Text;
        var flagged = $"set bj_wantDestroyGroup=true\n{GetIndent(function, patch.Call)}{patch.Call}";
        if (!function.Contains(flagged, StringComparison.Ordinal))
            throw new InvalidOperationException($"Transient group is not marked for destruction in {patch.FunctionName}: {patch.Call}");
    }

    var lines = script.Split('\n');
    for (var i = 0; i < lines.Length; i++)
    {
        if (!lines[i].Contains("ForGroupBJ(GetUnits", StringComparison.Ordinal)) continue;
        var protectedByFlag = Enumerable.Range(Math.Max(0, i - 4), Math.Min(4, i))
            .Any(index => lines[index].Contains("set bj_wantDestroyGroup=true", StringComparison.Ordinal));
        if (!protectedByFlag)
            throw new InvalidOperationException($"Unowned inline group remains at output line {i + 1}: {lines[i].Trim()}");
    }

    if (Regex.IsMatch(script, $@"(?m)^function\s+{Regex.Escape(obsoleteUpgradeHelper)}\s+takes"))
        throw new InvalidOperationException("Obsolete upgrade group-copy helper still exists.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Trigger initializer count changed.");
    var names = Regex.Matches(script, @"(?m)^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
        .Select(match => match.Groups[1].Value).ToList();
    if (names.GroupBy(name => name, StringComparer.Ordinal).Any(group => group.Count() > 1))
        throw new InvalidOperationException("Duplicate function names found.");
    var endCount = Regex.Matches(script, @"(?m)^endfunction\s*$").Count;
    if (endCount != names.Count)
        throw new InvalidOperationException($"Function/endfunction balance changed: functions={names.Count}, endfunctions={endCount}.");
}

static string InsertDestroyFlag(string script, EnumerationPatch patch)
{
    var function = ExtractFunction(script, patch.FunctionName);
    var indent = GetIndent(function.Text, patch.Call);
    var oldLine = indent + patch.Call;
    var newLines = indent + "set bj_wantDestroyGroup=true\n" + oldLine;
    var replacement = ReplaceOnce(function.Text, oldLine, newLines);
    return script.Remove(function.Start, function.Length).Insert(function.Start, replacement);
}

static string GetIndent(string function, string call)
{
    var match = Regex.Match(function, $@"(?m)^(?<indent>[ \t]*){Regex.Escape(call)}[ \t]*$");
    if (!match.Success)
        throw new InvalidOperationException($"Could not locate exact call indentation: {call}");
    return match.Groups["indent"].Value;
}

static string RemoveFunction(string script, string name)
{
    var function = ExtractFunction(script, name);
    var start = function.Start;
    var length = function.Length;
    if (start > 0 && script[start - 1] == '\n')
    {
        start--;
        length++;
    }
    return script.Remove(start, length);
}

static string ReplaceFunction(string script, string name, string replacement)
{
    var function = ExtractFunction(script, name);
    return script.Remove(function.Start, function.Length).Insert(function.Start, ToLf(replacement).TrimEnd('\n'));
}

static string ReplaceOnce(string text, string oldValue, string newValue)
{
    if (Regex.Matches(text, Regex.Escape(oldValue)).Count != 1)
        throw new InvalidOperationException($"Expected one exact replacement: {oldValue}");
    return text.Replace(oldValue, newValue, StringComparison.Ordinal);
}

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^[ \t]*function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\n.*?^[ \t]*endfunction[ \t]*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value);
}

static void Require(string text, string marker)
{
    if (!text.Contains(marker, StringComparison.Ordinal))
        throw new InvalidOperationException($"Expected reviewed marker: {marker}");
}

static string ToLf(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
static string ToCrlf(string text) => ToLf(text).Replace("\n", "\r\n", StringComparison.Ordinal);

static string BuildReport(string inputPath, string outputPath, string script, int transientGroupCount)
{
    var b = new StringBuilder();
    b.AppendLine("# Enumeration Ownership Pass 19");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Confirmed Warcraft's matching-group BJ wrappers destroy their supplied boolexpr filters.");
    b.AppendLine("- Reused and destroyed the Unit Indexer's one-shot initial-enumeration filter.");
    b.AppendLine($"- Added explicit ownership to {transientGroupCount} anonymous ForGroupBJ enumerations.");
    b.AppendLine("- Repaired Cripple Wave point cleanup and Mirror Image missile-location ownership.");
    b.AppendLine("- Repaired Purge the Dead filtering, target-point reuse, and corpse effect cleanup.");
    b.AppendLine("- Removed two stale shared-group destroys from item scrolls and duplicate init cleanup.");
    b.AppendLine("- Rebuilt allied upgrade propagation and base-destruction enumeration with local ownership.");
    b.AppendLine("- Removed one obsolete upgrade group-copy helper while preserving all 499 trigger initializers.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record EnumerationPatch(string FunctionName, string Call);
sealed record FunctionSlice(int Start, int Length, string Text);
