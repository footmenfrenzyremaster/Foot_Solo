using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-11.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "mode_pruning_pass_12.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-12.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "mode-pruning-pass-12", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
ValidateOriginal(script);

script = ReplaceOnce(script,
    "    boolean ISR_Running = false\n",
    "    boolean ISR_Running = false\n    boolean MODE_NoTransmute = false\n");

script = ReplaceFunction(script, "ModeSetup_ShowColossalDialog",
    ExtractRange(source, "ModeSetup_IsTransmuteHero", "ModeSetup_ResetUnsupportedModes"));
foreach (var name in new[]
{
    "ModeSetup_BeginHeroChoice",
    "Trig_MODE_GUI_show_Actions",
    "Trig_MODE_GUI_execute_Actions",
    "Trig_MODE_set_pub_game_default_Actions",
    "Trig_MODE_set_pub_game_custom_Actions",
    "Trig_Start_SD_Func025Func001A"
})
{
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);
}

script = ReplaceTriggerSection(script, "InitTrig_MODE_new",
    BuildTriggerSection("MODE new", ExtractRange(source, "Trig_MODE_new_Conditions", "InitTrig_MODE_new")));

var retiredModeTriggers = new[]
{
    "MODE_set_pro_mode",
    "MODE_set_inhouse",
    "MODE_colossal_GUI_execute",
    "MODE_colossal_amount_GUI_execute",
    "MODE_set_standard_units",
    "MODE_set_bounties",
    "MODE_1v1_check",
    "MODE_1v1",
    "MODE_King_Of_The_Hill"
};
var retiredKothTriggers = new[]
{
    "KOTH_Setup",
    "KOTH_Income",
    "KOTH_Hill_Dies",
    "KOTH_Gold_Depleted",
    "KOTH_Set_Neutral_Hill",
    "KOTH_Reset_Hill",
    "KOTH_Create_Hill_Glow",
    "KOTH_Update_Text",
    "KOTH_Update_Board_Gold",
    "KOTH_Second_Hill",
    "KOTH_Third_Hill",
    "KOTH_Fourth_Hill",
    "KOTH_Invul_Hill",
    "Multiboard_Create_with_KOTH"
};
var retiredColossalTriggers = new[]
{
    "colossal_variables",
    "colossal_spawn_message",
    "colossal_stats",
    "colossal_spawn",
    "colossal_death",
    "colossal_command_ct",
    "colossal_command_next"
};
var retiredSpawnTriggers = new[]
{
    "eight_start_timer",
    "eight",
    "five",
    "ten",
    "eight_new",
    "ten_new",
    "twelve_new"
};

foreach (var name in retiredModeTriggers.Concat(retiredKothTriggers).Concat(retiredColossalTriggers).Concat(retiredSpawnTriggers))
{
    script = ReplaceTriggerSection(script, "InitTrig_" + name, BuildDisabledTriggerSection(name));
}

script = PatchFunction(script, "InitGlobals", function => ReplaceOnce(function,
    "    set udg_mode_koth=true\n",
    "    set udg_mode_koth=false\n"));
script = PatchFunction(script, "Trig_init_Actions", function => ReplaceOnce(function,
    "    set udg_colossal_on=true\n",
    "    set udg_colossal_on=false\n"));
script = PatchFunction(script, "ISR_ShouldSpawn", function => ReplaceOnce(function,
    "    if udg_balanced then\n        return false\n    endif\n",
    ""));

script = PatchFunction(script, "Trig_Start_AP_Actions", function =>
{
    const string oldCall = "        call AddUnitToStockBJ(udg_hero_type[udg_X], udg_real_shop[";
    if (Regex.Matches(function, Regex.Escape(oldCall)).Count != 9)
        throw new InvalidOperationException("Expected nine reviewed AP stock calls.");
    return Regex.Replace(function,
        @"        call AddUnitToStockBJ\(udg_hero_type\[udg_X\], udg_real_shop\[(\d+)\], 3, udg_ap_number_of_heros\)",
        "        call ModeSetup_AddHeroToStock(udg_X, udg_real_shop[$1])");
});

script = PatchFunction(script, "Trig_Start_AR_Actions", function => ReplaceOnce(function,
    "        call CreateNUnitsAtLoc(1, udg_hero_type[udg_X], Player(PLAYER_NEUTRAL_PASSIVE), udg_temp_point, bj_UNIT_FACING)\n",
    "        if not MODE_NoTransmute or not ModeSetup_IsTransmuteHero(udg_X) then\n            call CreateNUnitsAtLoc(1, udg_hero_type[udg_X], Player(PLAYER_NEUTRAL_PASSIVE), udg_temp_point, bj_UNIT_FACING)\n        endif\n"));

script = PatchFunction(script, "Trig_Start_SD_Actions", function =>
{
    var wildcard = """
    // Fill every tavern to twelve choices. No Transmute needs two wildcard rounds
    // because category 8 is intentionally absent.
    set udg_temp_unitgroup=CreateGroup()
    set udg_X_category=1
    loop
        exitwhen udg_X_category > udg_numberofSDcategories
        call GroupAddGroup(udg_hero_category_group[udg_X_category], udg_temp_unitgroup)
        set udg_X_category=udg_X_category + 1
    endloop
    set udg_deal_index=1
    loop
        exitwhen udg_deal_index > ModeSetup_GetSDWildcardRounds()
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
        set udg_deal_index=udg_deal_index + 1
    endloop
    call DestroyGroup(udg_temp_unitgroup)
""";
    return ReplaceBetween(function,
        "    // Single Draft wildcard: add one unique twelfth choice to each team tavern.\n",
        "    set udg_temp_unitgroup=GetUnitsInRectOfPlayer(gg_rct_ALL_HERO, Player(PLAYER_NEUTRAL_PASSIVE))\n",
        ToLf(wildcard) + "\n",
        keepEndMarker: true);
});

script = PatchFunction(script, "Trig_Start_game_Actions", function =>
{
    function = ReplaceBetween(function,
        "    if ( Trig_Start_game_Func001C() ) then\n",
        "    call StartTimerBJ(udg_Fortification_Aura_Timer, false, 330.00)\n",
        "    // Only the standard multiboard remains.\n    call TriggerExecute(gg_trg_Multiboard_Create)\n    //                                                                                                     \n",
        keepEndMarker: true);
    function = ReplaceBetween(function,
        "    // COLOSSALS: ON / OFF\n",
        "    // ADD AUTOSEND ABILITY TO MAIN\n",
        "",
        keepEndMarker: true);

    var pubSetup = """
    // MODE: PUB
    set udg_mode_pub=true
    set udg_mode_inhouse=false
    set udg_autopoool_allowed=false
    set udg_startgold=100
    call SetMapFlag(MAP_LOCK_RESOURCE_TRADING, true)
    set udg_X=1
    loop
        exitwhen udg_X > 12
        call SetPlayerTechResearchedSwap('Rorb', 1, ConvertedPlayer(udg_X))
        set udg_X=udg_X + 1
    endloop
    if udg_mode_promode then
        call DisableTrigger(gg_trg_autopool_time)
        call DisableTrigger(gg_trg_autopool_command)
        call DisableTrigger(gg_trg_autopool_execute)
        set udg_t_trading=0.00
        call DisplayTextToForce(udg_all_players, "|cff00ffffNo Pool:|r gold trading and autopool are disabled.")
    else
        call EnableTrigger(gg_trg_autopool_time)
        call EnableTrigger(gg_trg_autopool_command)
        call EnableTrigger(gg_trg_autopool_execute)
        set udg_t_trading=230.00
        call StartTimerBJ(udg_timer_trading, false, udg_t_trading)
        call CreateTimerDialogBJ(udg_timer_trading, "TRIGSTR_13497")
        set udg_timer_window_trading=GetLastCreatedTimerDialogBJ()
    endif
""";
    function = ReplaceBetween(function,
        "    // MODE: PRIV / PUB\n",
        "    call SetPlayerTechResearchedSwap('R00N', 0, ConvertedPlayer(udg_X))\n",
        ToLf(pubSetup) + "\n",
        keepEndMarker: true);
    function = ReplaceBetween(function,
        "    if ( Trig_Start_game_Func026C() ) then\n",
        "    // make towers invu premass\n",
        "",
        keepEndMarker: true);
    return function;
});

foreach (var name in new[]
{
    "Trig_Start_game_Func001C",
    "Trig_Start_game_Func006C",
    "Trig_Start_game_Func016Func010C",
    "Trig_Start_game_Func016Func025Func002C",
    "Trig_Start_game_Func016Func025C",
    "Trig_Start_game_Func016C",
    "Trig_Start_game_Func020C",
    "Trig_Start_game_Func022C",
    "Trig_Start_game_Func026C"
})
{
    script = RemoveFunction(script, name);
}

ValidateReplacement(script, retiredModeTriggers, retiredKothTriggers, retiredColossalTriggers, retiredSpawnTriggers);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script, retiredModeTriggers.Length + retiredKothTriggers.Length + retiredColossalTriggers.Length + retiredSpawnTriggers.Length), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    var required = new[]
    {
        "function ModeSetup_ShowColossalDialog takes nothing returns nothing",
        "call AddUnitToStockBJ('h054', gg_unit_h03Y_0147, 1, 1)",
        "function Trig_MODE_King_Of_The_Hill_Actions takes nothing returns nothing",
        "function Trig_colossal_spawn_Actions takes nothing returns nothing",
        "function Trig_eight_Actions takes nothing returns nothing",
        "if udg_balanced then\n        return false\n    endif",
        "set udg_mode_koth=true",
        "set udg_colossal_on=true"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Reviewed pass-11 marker missing: {marker}");
    }
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializer entry points in pass 11.");
}

static void ValidateReplacement(string script, string[] retiredMode, string[] retiredKoth, string[] retiredColossal, string[] retiredSpawn)
{
    var required = new[]
    {
        "boolean MODE_NoTransmute = false",
        "function ModeSetup_IsTransmuteHero takes integer heroIndex returns boolean",
        "function ModeSetup_GetSDWildcardRounds takes nothing returns integer",
        "return heroIndex == 18 or heroIndex == 21 or heroIndex == 52 or heroIndex == 55 or heroIndex == 78 or heroIndex == 88",
        "call AddUnitToStockBJ('h03Z', gg_unit_h03Y_0147, 1, 1)",
        "call AddUnitToStockBJ('h03V', gg_unit_h03Y_0147, 1, 1)",
        "call AddUnitToStockBJ('h04B', gg_unit_h03Y_0147, 1, 1)",
        "call ModeSetup_AddHeroToStock(udg_X, udg_real_shop[21])",
        "exitwhen udg_deal_index > ModeSetup_GetSDWildcardRounds()",
        "if not MODE_NoTransmute or not ModeSetup_IsTransmuteHero(udg_X) then",
        "gold trading and autopool are disabled",
        "call TriggerExecute(gg_trg_Multiboard_Create)",
        "call StartTimerBJ(udg_Host_AFK_Timer, false, 10.00)"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Pass-12 replacement marker missing: {marker}");
    }

    var forbidden = new[]
    {
        "function ModeSetup_ShowColossalDialog",
        "call AddUnitToStockBJ('h054'",
        "call AddUnitToStockBJ('h04P'",
        "function Trig_MODE_set_pro_mode_Actions",
        "function Trig_MODE_set_inhouse_Actions",
        "function Trig_MODE_King_Of_The_Hill_Actions",
        "function Trig_colossal_spawn_Actions",
        "function Trig_eight_Actions",
        "if udg_balanced then\n        return false\n    endif",
        "call TriggerExecute(gg_trg_Multiboard_Create_with_KOTH)",
        "Trig_Start_game_Func006C()",
        "set udg_hero_category[udg_X]=0",
        "set udg_startgold=140"
    };
    foreach (var marker in forbidden)
    {
        if (script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Retired marker still present: {marker}");
    }

    foreach (var name in retiredMode.Concat(retiredKoth).Concat(retiredColossal).Concat(retiredSpawn))
    {
        var init = ExtractFunction(script, "InitTrig_" + name).Text;
        if (!init.Contains("call DisableTrigger(gg_trg_" + name + ")", StringComparison.Ordinal) ||
            init.Contains("TriggerAddAction", StringComparison.Ordinal) ||
            init.Contains("TriggerRegister", StringComparison.Ordinal))
            throw new InvalidOperationException($"Retired trigger {name} is not a disabled compatibility stub.");
    }

    var custom = ExtractFunction(script, "Trig_MODE_set_pub_game_custom_Actions").Text;
    if (Regex.Matches(custom, @"AddUnitToStockBJ\('").Count != 3)
        throw new InvalidOperationException("Pub Custom must expose exactly three optional modes.");

    var startGame = ExtractFunction(script, "Trig_Start_game_Actions").Text;
    if (startGame.Contains("udg_mode_koth", StringComparison.Ordinal) ||
        startGame.Contains("udg_colossal_on", StringComparison.Ordinal) ||
        startGame.Contains("udg_mode_inhouse == true", StringComparison.Ordinal))
        throw new InvalidOperationException("Unsupported mode branches remain in Start_game.");

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

    var joinedStatements = Regex.Matches(script, @"(?m)^\s*(?:call|set|if|elseif|exitwhen|return|local|endif)\b.*\)[ \t]{2,}(?:call|set|if|elseif|else|endif|loop|endloop|exitwhen|return|local)\b")
        .Select(match => match.Value.Trim())
        .ToArray();
    if (joinedStatements.Length > 0)
        throw new InvalidOperationException($"Joined JASS statements remain: {string.Join(" | ", joinedStatements)}");
}

static string BuildDisabledTriggerSection(string name) => BuildTriggerSection("retired " + name, $"""
function InitTrig_{name} takes nothing returns nothing
    set gg_trg_{name}=CreateTrigger()
    call DisableTrigger(gg_trg_{name})
endfunction
""");

static string BuildTriggerSection(string title, string body) => $"""
//===========================================================================
// Trigger: {title}
//===========================================================================
{ToLf(body).Trim()}
""";

static string ReplaceTriggerSection(string script, string initName, string replacement)
{
    var init = ExtractFunction(script, initName);
    var comment = script.LastIndexOf("// Trigger:", init.Start, StringComparison.Ordinal);
    if (comment < 0) throw new InvalidOperationException($"Could not find trigger comment for {initName}.");
    var start = script.LastIndexOf("//===========================================================================", comment, StringComparison.Ordinal);
    if (start < 0) throw new InvalidOperationException($"Could not find trigger section start for {initName}.");
    var end = init.End;
    while (end < script.Length && script[end] == '\n') end++;
    return script.Remove(start, end - start).Insert(start, ToLf(replacement).TrimEnd('\n') + "\n\n");
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
    while (end < script.Length && script[end] == '\n') end++;
    return script.Remove(function.Start, end - function.Start);
}

static string ReplaceBetween(string text, string startMarker, string endMarker, string replacement, bool keepEndMarker)
{
    var start = text.IndexOf(startMarker, StringComparison.Ordinal);
    if (start < 0) throw new InvalidOperationException($"Start marker not found: {startMarker.Trim()}");
    var end = text.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
    if (end < 0) throw new InvalidOperationException($"End marker not found: {endMarker.Trim()}");
    if (!keepEndMarker) end += endMarker.Length;
    return text.Remove(start, end - start).Insert(start, ToLf(replacement));
}

static string ExtractRange(string text, string firstFunction, string lastFunction)
{
    var start = ExtractFunction(text, firstFunction).Start;
    var end = ExtractFunction(text, lastFunction).End;
    return text[start..end];
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

static string BuildReport(string inputPath, string outputPath, string script, int retiredTriggerCount)
{
    var b = new StringBuilder();
    b.AppendLine("# Mode Pruning Pass 12");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Reduced Red's setup dialog to Pub Default and Pub Custom.");
    b.AppendLine("- Pub Default remains the ten-second AFK fallback and starts Single Draft.");
    b.AppendLine("- Pub Custom exposes exactly 2K, No Transmute, and No Pool before SD/AP/AR selection.");
    b.AppendLine("- No Transmute now filters the six cataloged Transmute heroes in SD, AP, and AR.");
    b.AppendLine("- No Pool now keeps manual gold trading locked and disables the autopool triggers.");
    b.AppendLine($"- Replaced {retiredTriggerCount} unsupported mode, KOTH, Colossal, balanced, and grouped-spawn trigger implementations with disabled compatibility stubs.");
    b.AppendLine("- Preserved mass-bonus spawning and the individual per-unit spawn scheduler.");
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
