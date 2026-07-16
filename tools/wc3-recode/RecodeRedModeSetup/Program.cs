using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-9.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "red_mode_setup_pass_10.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-10.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "red-mode-setup-pass-10", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = Normalize(File.ReadAllText(inputPath, Encoding.Latin1));
var source = Normalize(File.ReadAllText(sourcePath, Encoding.Latin1));

ValidateOriginal(script);

script = ReplaceRange(
    script,
    ExtractFunction(script, "Trig_AFK_Host_Setup_Actions").Start,
    ExtractFunction(script, "InitTrig_MODE_GUI_execute").End,
    ExtractRange(source, "ModeSetup_ShowColossalDialog", "InitTrig_MODE_GUI_execute"));

foreach (var name in new[]
{
    "Trig_MODE_set_pub_game_default_Actions",
    "Trig_MODE_set_pub_game_custom_Actions",
    "Trig_MODE_set_pro_mode_Actions",
    "Trig_MODE_colossal_GUI_execute_Actions",
    "Trig_MODE_colossal_amount_GUI_execute_Actions",
    "Trig_MODE_new_Conditions"
})
{
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);
}

var oldInhouseStart = ExtractFunction(script, "Trig_MODE_set_inhouse_Func010C").Start;
var oldInhouseEnd = ExtractFunction(script, "Trig_MODE_set_inhouse_Actions").End;
script = ReplaceRange(script, oldInhouseStart, oldInhouseEnd, ExtractFunction(source, "Trig_MODE_set_inhouse_Actions").Text);

script = ReplaceRange(
    script,
    ExtractFunction(script, "Trig_Start_Vote_Conditions").Start,
    ExtractFunction(script, "InitTrig_Vote_Timer_Expires").End,
    ExtractRange(source, "ModeSetup_StartHeroMode", "InitTrig_Vote_Timer_Expires"));

script = ReplaceFunction(script, "InitTrig_MODE_1v1_check", """
function InitTrig_MODE_1v1_check takes nothing returns nothing
    set gg_trg_MODE_1v1_check=CreateTrigger()
    call DisableTrigger(gg_trg_MODE_1v1_check)
endfunction
""");

script = AddSingleDraftWildcard(script);
script = Normalize(script);
ValidateReplacement(script);

File.WriteAllText(outputPath, script, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    var required = new[]
    {
        "call StartTimerBJ(udg_Host_AFK_Timer, false, 8.00)",
        "function Trig_MODE_GUI_execute_Func004C",
        "function Trig_Vote_Timer_Expires_Func002Func008Func001Func002C",
        "set udg_AP_votes=( udg_AP_votes + udg_AP[udg_X] )",
        "call ConditionalTriggerExecute(gg_trg_Start_Vote)",
        "exitwhen udg_X_category > udg_numberofSDcategories"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Reviewed pass-9 marker missing: {marker}");
    }

    var afk = ExtractFunction(script, "Trig_AFK_Host_Timer_Expires_Actions").Text;
    if (Regex.Matches(afk, @"ConditionalTriggerExecute\(gg_trg_Start_game\)").Count != 1)
        throw new InvalidOperationException("Expected the original AFK path to contain the reviewed duplicate Start_game call.");
    var sd = ExtractFunction(script, "Trig_Start_SD_Actions").Text;
    if (sd.Contains("Single Draft wildcard", StringComparison.Ordinal))
        throw new InvalidOperationException("Single Draft wildcard patch already exists.");
}

static string AddSingleDraftWildcard(string script)
{
    var function = ExtractFunction(script, "Trig_Start_SD_Actions");
    const string marker = "    set udg_temp_unitgroup=GetUnitsInRectOfPlayer(gg_rct_ALL_HERO, Player(PLAYER_NEUTRAL_PASSIVE))";
    var markerIndex = function.Text.IndexOf(marker, StringComparison.Ordinal);
    if (markerIndex < 0) throw new InvalidOperationException("Could not find the reviewed Single Draft cleanup insertion point.");
    var wildcard = """
    // Single Draft wildcard: add one unique twelfth choice to each team tavern.
    set udg_temp_unitgroup=CreateGroup()
    set udg_X_category=1
    loop
        exitwhen udg_X_category > udg_numberofSDcategories
        call GroupAddGroup(udg_hero_category_group[udg_X_category], udg_temp_unitgroup)
        set udg_X_category=udg_X_category + 1
    endloop
    set udg_deal_shop=1
    loop
        exitwhen udg_deal_shop > 4
        set udg_temp_unit=GroupPickRandomUnit(udg_temp_unitgroup)
        if udg_temp_unit != null then
            call AddUnitToStockBJ(GetUnitTypeId(udg_temp_unit), udg_real_shop[udg_deal_shop], 1, 1)
            call GroupRemoveUnitSimple(udg_temp_unit, udg_temp_unitgroup)
        endif
        set udg_deal_shop=udg_deal_shop + 1
    endloop
    call DestroyGroup(udg_temp_unitgroup)
""";
    var updated = function.Text.Insert(markerIndex, Normalize(wildcard));
    return script.Remove(function.Start, function.Length).Insert(function.Start, updated);
}

static void ValidateReplacement(string script)
{
    var required = new[]
    {
        "call StartTimerBJ(udg_Host_AFK_Timer, false, 10.00)",
        "if GetTriggerPlayer() != Player(0) then",
        "function ModeSetup_BeginHeroChoice takes nothing returns nothing",
        "function ModeSetup_StartHeroMode takes integer selectedMode returns nothing",
        "return udg_voting and GetTriggerPlayer() == Player(0) and GetSpellAbilityId() == 'A01T'",
        "call ModeSetup_StartHeroMode(3)",
        "Single Draft wildcard: add one unique twelfth choice",
        "call GroupAddGroup(udg_hero_category_group[udg_X_category], udg_temp_unitgroup)"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Replacement marker missing: {marker}");
    }

    var forbidden = new[]
    {
        "function Trig_MODE_GUI_execute_Func004C",
        "function Trig_Vote_Timer_Expires_Func002",
        "set udg_AP_votes=( udg_AP_votes + udg_AP[udg_X] )",
        "call ConditionalTriggerExecute(gg_trg_Start_Vote)",
        "call TriggerRegisterTimerExpireEventBJ(gg_trg_Vote_Timer_Expires"
    };
    foreach (var marker in forbidden)
    {
        if (script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Removed vote marker still present: {marker}");
    }

    var oneVsOneInit = ExtractFunction(script, "InitTrig_MODE_1v1_check").Text;
    if (oneVsOneInit.Contains("TriggerRegisterTimerEventSingle", StringComparison.Ordinal))
        throw new InvalidOperationException("Automatic 1v1 startup event remains enabled.");
    if (Regex.Matches(script, @"function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Trigger initializer count changed; expected all 499 entry points to remain.");
}

static string ReplaceFunction(string script, string name, string replacement)
{
    var function = ExtractFunction(script, name);
    return ReplaceRange(script, function.Start, function.End, replacement);
}

static string ExtractRange(string text, string firstFunction, string lastFunction)
{
    var start = ExtractFunction(text, firstFunction).Start;
    var end = ExtractFunction(text, lastFunction).End;
    return text[start..end];
}

static string ReplaceRange(string text, int start, int end, string replacement)
{
    return text.Remove(start, end - start).Insert(start, Normalize(replacement).TrimEnd('\r', '\n'));
}

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, Normalize(match.Value));
}

static string Normalize(string text) => text.Replace("\r\n", "\n").Replace("\n", "\r\n");

static string BuildReport(string inputPath, string outputPath, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Red-Only Mode Setup Pass 10");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Red is the only player allowed to choose the game setup.");
    b.AppendLine("- Replaced the vote tally with immediate Red-only SD/AP/AR selection.");
    b.AppendLine("- Pub Default starts Single Draft automatically.");
    b.AppendLine("- The AFK fallback is exactly 10 seconds and starts Pub Default once.");
    b.AppendLine("- Pub Custom preserves icon-based special-mode selection and the Colossal configuration dialog.");
    b.AppendLine("- Disabled automatic 1v1 startup so it cannot bypass Red's decision.");
    b.AppendLine("- Added one unique wildcard hero to each Single Draft team tavern.");
    b.AppendLine($"- Output script lines: {script.Replace("\r\n", "\n").Split('\n').Length:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text)
{
    public int End => Start + Length;
}
