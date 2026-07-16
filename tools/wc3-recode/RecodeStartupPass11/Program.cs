using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-10.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "startup_repairs_pass_11.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-11.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "startup-repairs-pass-11", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
ValidateOriginal(script);

script = ReplaceFunction(script, "Trig_Start_game_Func035Func002A", ExtractFunction(source, "Trig_Start_game_Func035Func002A").Text);
script = PatchFunction(script, "Trig_Start_game_Actions", function =>
{
    function = ReplaceOnce(function, "    call TimerDialogDisplayBJ(true, udg_timer_window_colossal)\n", "");
    function = ReplaceOnce(function,
        "        set udg_timer_window_colossal=GetLastCreatedTimerDialogBJ()\n",
        "        set udg_timer_window_colossal=GetLastCreatedTimerDialogBJ()\n        call TimerDialogDisplayBJ(true, udg_timer_window_colossal)\n");
    function = ReplaceOnce(function, "        call DestroyTrigger(GetTriggeringTrigger())\n", "");
    function = ReplaceOnce(function,
        "    call ForGroupBJ(udg_temp_unitgroup, function Trig_Start_game_Func029A)\n",
        "    call ForGroupBJ(udg_temp_unitgroup, function Trig_Start_game_Func029A)\n    call DestroyGroup(udg_temp_unitgroup)\n");
    function = ReplaceOnce(function, "        exitwhen udg_X > 6\n", "        exitwhen udg_X > 9\n");
    return function;
});

script = PatchFunction(script, "Trig_Start_SD_Actions", function =>
{
    function = ReplaceOnce(function,
        "        set udg_hero_category_group[udg_X_category]=CreateGroup()\n",
        "        call GroupClear(udg_hero_category_group[udg_X_category])\n");
    function = ReplaceOnce(function,
        "        call ForGroupBJ(GetUnitsOfTypeIdAll(udg_hero_type[udg_X]), function Trig_Start_SD_Func025Func001A)\n",
        "        set udg_temp_unitgroup2=GetUnitsOfTypeIdAll(udg_hero_type[udg_X])\n        call ForGroupBJ(udg_temp_unitgroup2, function Trig_Start_SD_Func025Func001A)\n        call DestroyGroup(udg_temp_unitgroup2)\n");
    function = ReplaceOnce(function,
        "    call DisplayTimedTextToForce(GetPlayersAll(), 10.00, I2S(udg_total_hero_count))\n    call DisplayTimedTextToForce(GetPlayersAll(), 10.00, I2S(udg_numberofSDcategories))\n    call DisplayTimedTextToForce(GetPlayersAll(), 10.00, I2S(CountUnitsInGroup(udg_hero_category_group[1])))\n",
        "");
    function = ReplaceOnce(function,
        "            call ForGroupBJ(GetRandomSubGroup(1, udg_hero_category_group[udg_X_category]), function Trig_Start_SD_Func038Func006Func002A)\n",
        "            set udg_temp_unitgroup2=GetRandomSubGroup(1, udg_hero_category_group[udg_X_category])\n            call ForGroupBJ(udg_temp_unitgroup2, function Trig_Start_SD_Func038Func006Func002A)\n            call DestroyGroup(udg_temp_unitgroup2)\n");
    function = ReplaceOnce(function,
        "    call DestroyGroup(udg_temp_unitgroup)    set udg_temp_unitgroup=GetUnitsInRectOfPlayer(gg_rct_ALL_HERO, Player(PLAYER_NEUTRAL_PASSIVE))\n",
        "    call DestroyGroup(udg_temp_unitgroup)\n    set udg_temp_unitgroup=GetUnitsInRectOfPlayer(gg_rct_ALL_HERO, Player(PLAYER_NEUTRAL_PASSIVE))\n");
    function = ReplaceOnce(function,
        "    call DestroyGroup(udg_temp_unitgroup)\n    call DestroyGroup(udg_temp_unitgroup2)\n    call DestroyGroup(udg_temp_unitgroup3)\n    call DestroyTrigger(GetTriggeringTrigger())\n",
        "    call DestroyTrigger(GetTriggeringTrigger())\n");
    return function;
});

foreach (var name in new[]
{
    "Trig_Start_AP_Func006Func002A",
    "Trig_Start_AP_Func006Func003A",
    "Trig_Start_AR_Func008Func002A",
    "Trig_Start_AR_Func008Func003A"
})
{
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);
}

script = PatchFunction(script, "Trig_Start_AP_Actions", function =>
{
    function = ReplaceOnce(function,
        "    call RemoveLocation(udg_temp_point)\n",
        "    call DestroyGroup(udg_temp_unitgroup2)\n    call DestroyGroup(udg_temp_unitgroup3)\n");
    function = ReplaceOnce(function, "        exitwhen udg_X > 8\n", "        exitwhen udg_X > 9\n");
    function = ReplaceOnce(function, "        exitwhen udg_X > 93\n", "        exitwhen udg_X > udg_total_hero_count\n");
    return function;
});

script = RemoveFunction(script, "Trig_Start_AR_Func022Func001Func005A");
script = ReplaceFunction(script, "Trig_Start_AR_Actions", ExtractFunction(source, "Trig_Start_AR_Actions").Text);

script = PatchFunction(script, "Trig_repick_command_Actions", function => ReplaceOnce(function,
    "        call DestroyGroup(udg_temp_unitgroup)\n        call DestroyForce(udg_temp_playergroup)\n",
    "        call DestroyGroup(udg_temp_unitgroup)\n"));

ValidateReplacement(script);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    var required = new[]
    {
        "call DestroyGroup(udg_temp_unitgroup)    set udg_temp_unitgroup=GetUnitsInRectOfPlayer",
        "set udg_hero_category_group[udg_X_category]=CreateGroup()",
        "ForGroupBJ(GetRandomSubGroup(1, udg_hero_category_group[udg_X_category])",
        "exitwhen udg_X > 93",
        "set udg_random_hero[udg_X]=GetUnitTypeId(GroupPickRandomUnit(udg_temp_unitgroup))",
        "function Trig_Start_AR_Func022Func001Func005A",
        "exitwhen udg_X > 6\n        call RemoveLocation(udg_tavern[udg_X])"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Reviewed pass-10 marker missing: {marker}");
    }

    if (Regex.Matches(ExtractFunction(script, "Trig_Start_game_Actions").Text, @"DestroyTrigger\(GetTriggeringTrigger\(\)\)").Count != 2)
        throw new InvalidOperationException("Expected the reviewed duplicate Start_game trigger destruction.");
    if (Regex.Matches(ExtractFunction(script, "Trig_repick_command_Actions").Text, @"DestroyForce\(udg_temp_playergroup\)").Count != 2)
        throw new InvalidOperationException("Expected the reviewed duplicate repick force destruction.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializer entry points in pass 10.");
}

static void ValidateReplacement(string script)
{
    var required = new[]
    {
        "local location startLocation=GetPlayerStartLocationLoc(GetEnumPlayer())",
        "call TimerDialogDisplayBJ(true, udg_timer_window_colossal)",
        "call GroupClear(udg_hero_category_group[udg_X_category])",
        "set udg_temp_unitgroup2=GetUnitsOfTypeIdAll(udg_hero_type[udg_X])",
        "set udg_temp_unitgroup2=GetRandomSubGroup(1, udg_hero_category_group[udg_X_category])",
        "exitwhen udg_X > 9\n        call CreateTextTagUnitBJ",
        "exitwhen udg_X > udg_total_hero_count\n        call AddUnitToStockBJ(udg_hero_type[udg_X], udg_real_shop[21]",
        "set udg_random_hero[udg_X]=GetUnitTypeId(udg_temp_unit)",
        "call CreateNUnitsAtLoc(1, udg_random_hero[udg_X], ConvertedPlayer(udg_X), udg_loc_tavern, bj_UNIT_FACING)",
        "// Draw without replacement. The stored type is exactly the hero granted."
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Pass-11 replacement marker missing: {marker}");
    }

    var forbidden = new[]
    {
        "call DestroyGroup(udg_temp_unitgroup)    set udg_temp_unitgroup",
        "set udg_hero_category_group[udg_X_category]=CreateGroup()",
        "call ForGroupBJ(GetUnitsOfTypeIdAll(udg_hero_type[udg_X])",
        "call ForGroupBJ(GetRandomSubGroup(1, udg_hero_category_group[udg_X_category])",
        "call DisplayTimedTextToForce(GetPlayersAll(), 10.00, I2S(udg_total_hero_count))",
        "exitwhen udg_X > 93",
        "set udg_random_hero[udg_X]=GetUnitTypeId(GroupPickRandomUnit(udg_temp_unitgroup))",
        "function Trig_Start_AR_Func022Func001Func005A"
    };
    foreach (var marker in forbidden)
    {
        if (script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Removed pass-10 marker still present: {marker}");
    }

    var startGame = ExtractFunction(script, "Trig_Start_game_Actions").Text;
    if (Regex.Matches(startGame, @"DestroyTrigger\(GetTriggeringTrigger\(\)\)").Count != 1)
        throw new InvalidOperationException("Start_game must retire its trigger exactly once.");
    if (Regex.Matches(startGame, @"TimerDialogDisplayBJ\(true, udg_timer_window_colossal\)").Count != 1 ||
        !ExtractFunction(startGame, "Trig_Start_game_Actions").Text.Contains("if ( Trig_Start_game_Func006C() ) then", StringComparison.Ordinal))
        throw new InvalidOperationException("Colossal timer dialog placement was not preserved as expected.");

    var repick = ExtractFunction(script, "Trig_repick_command_Actions").Text;
    if (Regex.Matches(repick, @"DestroyForce\(udg_temp_playergroup\)").Count != 1)
        throw new InvalidOperationException("Repick temporary force must be destroyed exactly once.");

    var names = Regex.Matches(script, @"(?m)^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
        .Select(match => match.Groups[1].Value)
        .ToList();
    var duplicates = names.GroupBy(name => name, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => group.Key).ToArray();
    if (duplicates.Length > 0)
        throw new InvalidOperationException($"Duplicate functions found: {string.Join(", ", duplicates)}");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Trigger initializer count changed; expected all 499 entry points to remain.");
    if (Regex.Matches(script, @"(?m)^endfunction\s*$").Count != names.Count)
        throw new InvalidOperationException("Function/endfunction balance changed.");

    var joinedStatements = Regex.Matches(script, @"(?m)^\s*(?:call|set|if|elseif|exitwhen|return|local)\b.*\)[ \t]{2,}(?:call|set|if|elseif|else|endif|loop|endloop|exitwhen|return|local)\b")
        .Select(match => match.Value.Trim())
        .ToArray();
    if (joinedStatements.Length > 0)
        throw new InvalidOperationException($"Joined JASS statements remain: {string.Join(" | ", joinedStatements)}");
}

static string PatchFunction(string script, string name, Func<string, string> patch)
{
    var function = ExtractFunction(script, name);
    var replacement = patch(function.Text);
    if (replacement == function.Text)
        throw new InvalidOperationException($"Patch made no changes to {name}.");
    return ReplaceRange(script, function.Start, function.End, replacement);
}

static string ReplaceFunction(string script, string name, string replacement)
{
    var function = ExtractFunction(script, name);
    return ReplaceRange(script, function.Start, function.End, replacement);
}

static string RemoveFunction(string script, string name)
{
    var function = ExtractFunction(script, name);
    var end = function.End;
    while (end < script.Length && (script[end] == '\r' || script[end] == '\n')) end++;
    return script.Remove(function.Start, end - function.Start);
}

static string ReplaceRange(string text, int start, int end, string replacement) =>
    text.Remove(start, end - start).Insert(start, ToLf(replacement).TrimEnd('\n'));

static string ReplaceOnce(string text, string oldValue, string newValue)
{
    var count = Regex.Matches(text, Regex.Escape(oldValue)).Count;
    if (count != 1)
        throw new InvalidOperationException($"Expected one exact replacement, found {count}: {oldValue.Trim()}");
    return text.Replace(oldValue, newValue, StringComparison.Ordinal);
}

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\n.*?^endfunction[ \t]*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value);
}

static string ToLf(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
static string ToCrlf(string text) => ToLf(text).Replace("\n", "\r\n", StringComparison.Ordinal);

static string BuildReport(string inputPath, string outputPath, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Startup Repairs Pass 11");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Corrected the malformed joined JASS statement in Single Draft.");
    b.AppendLine("- Reused and released Single Draft category/source groups without stale double-destruction.");
    b.AppendLine("- Removed three leftover Single Draft debug-count messages.");
    b.AppendLine("- Released startup camera, AP/AR relocation, and one-shot hero-pool locations/groups.");
    b.AppendLine("- Displayed all nine AP tavern labels and bounded stock data to the 91-hero catalog.");
    b.AppendLine("- Made All Random draw without replacement and store the exact granted hero for repick.");
    b.AppendLine("- Removed one obsolete AR helper and the repick command's duplicate force destruction.");
    b.AppendLine("- Preserved all 499 trigger initializer entry points.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text)
{
    public int End => Start + Length;
}
