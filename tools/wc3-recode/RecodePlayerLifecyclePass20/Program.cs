using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-19.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "player_lifecycle_pass_20.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-20.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "player-lifecycle-pass-20", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
var originalFunctionCount = Regex.Matches(script, @"(?m)^function\s+").Count;
var globals = ExtractMarked(source, "// PASS20_GLOBALS_BEGIN", "// PASS20_GLOBALS_END");
var helpers = ExtractMarked(source, "// PASS20_HELPERS_BEGIN", "// PASS20_HELPERS_END");

var readerPatches = new[]
{
    "Trig_Start_AR_Func022Func001C",
    "Trig_Votekick_Initiate_Func035C",
    "Trig_Votekick_Initiate_Conditions",
    "Trig_Votekick_Initiate_Func017C",
    "Trig_Votekick_Initiate_Func018C",
    "Trig_swap_command_Conditions",
    "Trig_swap_command_Func001Func009C",
    "Trig_swap_command_Func001Func010C",
    "Trig_swap_command_Actions",
    "Trig_swap_execute_Func006C",
    "Trig_swap_execute_Func007C",
    "Trig_swap_execute_Actions",
    "Trig_autopool_command_Conditions",
    "Trig_autopool_command_Func004Func005C",
    "Trig_autopool_command_Func004Func006C",
    "Trig_autopool_execute_Conditions",
    "Trig_autopool_execute_Actions",
    "Trig_autopool_time_Func001Func001C",
    "Trig_KICK_execute_Actions"
};

var lifecycleSections = new[]
{
    new TriggerSection("BASE collect gold", "InitTrig_BASE_collect_gold"),
    new TriggerSection("BASE destroy", "InitTrig_BASE_destroy"),
    new TriggerSection("check if team active", "InitTrig_check_if_team_active"),
    new TriggerSection("Kill units of inactive player", "InitTrig_Kill_units_of_inactive_player"),
    new TriggerSection("player leaves", "InitTrig_player_leaves"),
    new TriggerSection("player destroyed", "InitTrig_player_destroyed"),
    new TriggerSection("Visibility", "InitTrig_Visibility"),
    new TriggerSection("END", "InitTrig_END")
};

var obsoleteKickHelpers = new[]
{
    "Trig_KICK_execute_Func011Func001Func009Func004C",
    "Trig_KICK_execute_Func011Func001Func009Func006C",
    "Trig_KICK_execute_Func011Func001Func009C"
};

ValidateOriginal(script);

script = InsertGlobals(script, globals);
script = InsertHelpers(script, helpers);

foreach (var functionName in readerPatches)
    script = ReplaceFunction(script, functionName, ExtractFunction(source, functionName).Text);

script = PatchStartGame(script);
script = PatchVotekickEnumeration(script);

foreach (var functionName in obsoleteKickHelpers)
    script = RemoveFunction(script, functionName);

foreach (var section in lifecycleSections)
    script = ReplaceTriggerSection(script, section, ExtractTriggerSection(source, section).Text);

ValidateReplacement(script, readerPatches, obsoleteKickHelpers);
ValidateScenarioModel();

File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script, originalFunctionCount), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    Require(ExtractFunction(script, "Trig_END_Func001C").Text, "udg_mode_6v6");
    Require(ExtractFunction(script, "Trig_player_leaves_Func009C").Text, "udg_mode_koth");
    Require(ExtractFunction(script, "Trig_player_destroyed_Actions").Text, "set udg_player_active[udg_temp_playernumber_ally_1]=false");
    Require(ExtractFunction(script, "Trig_KICK_execute_Actions").Text, "CreateNUnitsAtLoc(1, 'h02Q'");
    Require(ExtractFunction(script, "Trig_BASE_destroy_Actions").Text, "call RemoveLocation(udg_temp_point)");
    Require(ExtractFunction(script, "Trig_swap_command_Actions").Text, "set udg_temp_unitgroup2=GetUnitsOfPlayerMatching");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializers in pass 19.");
}

static void ValidateReplacement(string script, IReadOnlyList<string> readerPatches, IReadOnlyList<string> obsoleteKickHelpers)
{
    foreach (var name in readerPatches)
    {
        if (Regex.Matches(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes").Count != 1)
            throw new InvalidOperationException($"Expected exactly one patched function: {name}");
    }

    foreach (var name in obsoleteKickHelpers)
    {
        if (Regex.IsMatch(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes"))
            throw new InvalidOperationException($"Obsolete kick helper still exists: {name}");
    }

    Require(script, "boolean array PlayerLifecycle_DepartureHandled");
    Require(script, "function PlayerLifecycle_EvaluateAllTeams takes boolean checkVictory returns nothing");
    Require(script, "return PlayerLifecycle_IsConnected(playerNumber) and udg_player_active[playerNumber] and PlayerLifecycle_IsBaseAlive(playerNumber)");
    Require(script, "if not PlayerLifecycle_TeamHasLivingBase(teamNumber) or not PlayerLifecycle_TeamHasConnectedPlayer(teamNumber) then");
    Require(script, "set PlayerLifecycle_GameStarted=true");

    var baseDeath = ExtractFunction(script, "Trig_player_destroyed_Actions").Text;
    Require(baseDeath, "set udg_player_active[playerNumber]=false");
    Require(baseDeath, "call PlayerLifecycle_EvaluateAllTeams(true)");
    if (baseDeath.Contains("ally_1", StringComparison.Ordinal) || baseDeath.Contains("ally_2", StringComparison.Ordinal))
        throw new InvalidOperationException("Base death still directly defeats allied players.");

    var endGame = ExtractFunction(script, "Trig_END_Actions").Text;
    Require(endGame, "PlayerLifecycle_GetWinnerTeam()");
    if (endGame.Contains("udg_mode_6v6", StringComparison.Ordinal))
        throw new InvalidOperationException("The unreachable 6v6 endgame branch remains.");

    var leave = ExtractFunction(script, "Trig_player_leaves_Actions").Text;
    Require(leave, "PlayerLifecycle_HandleDeparture");
    if (leave.Contains("udg_mode_koth", StringComparison.Ordinal))
        throw new InvalidOperationException("The retired KOTH departure branch remains.");

    var kick = ExtractFunction(script, "Trig_KICK_execute_Actions").Text;
    Require(kick, "PlayerLifecycle_HandleDeparture(kickedPlayer)");
    if (kick.Contains("CreateNUnitsAtLoc(1, 'h02Q'", StringComparison.Ordinal))
        throw new InvalidOperationException("Admin kick still creates duplicate abandoned-base controls.");

    var cleanup = ExtractFunction(script, "Trig_Kill_units_of_inactive_player_Actions").Text;
    Require(cleanup, "call DisableTrigger(GetTriggeringTrigger())");
    var baseDestroy = ExtractFunction(script, "Trig_BASE_destroy_Actions").Text;
    if (baseDestroy.Contains("RemoveLocation(udg_temp_point)", StringComparison.Ordinal))
        throw new InvalidOperationException("BASE destroy still removes an unowned shared location.");

    var swap = ExtractFunction(script, "Trig_swap_command_Actions").Text;
    Require(swap, "call DestroyGroup(udg_temp_unitgroup)");
    Require(swap, "call DestroyGroup(udg_temp_unitgroup2)");
    var swapExecute = ExtractFunction(script, "Trig_swap_execute_Actions").Text;
    Require(swapExecute, "call DisplayTextToPlayer(udg_temp_player");
    if (swapExecute.Contains("DisplayTextToForce(GetForceOfPlayer", StringComparison.Ordinal))
        throw new InvalidOperationException("Swap execution still allocates anonymous single-player forces.");

    var pool = ExtractFunction(script, "Trig_autopool_time_Func001Func001C").Text;
    Require(pool, "PlayerLifecycle_IsParticipating(targetNumber)");

    var votekick = ExtractFunction(script, "Trig_Votekick_Initiate_Actions").Text;
    if (votekick.Contains("ForForce(GetPlayersAll()", StringComparison.Ordinal))
        throw new InvalidOperationException("Votekick still allocates an anonymous all-player force.");

    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Trigger initializer count changed.");

    var names = Regex.Matches(script, @"(?m)^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
        .Select(match => match.Groups[1].Value).ToList();
    var duplicate = names.GroupBy(name => name, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
    if (duplicate is not null)
        throw new InvalidOperationException($"Duplicate function name found: {duplicate.Key}");
    var endCount = Regex.Matches(script, @"(?m)^endfunction\s*$").Count;
    if (endCount != names.Count)
        throw new InvalidOperationException($"Function/endfunction balance changed: functions={names.Count}, endfunctions={endCount}.");
}

static void ValidateScenarioModel()
{
    static bool ShouldDefeat(bool[] livingBases, bool[] connectedPlayers) =>
        !livingBases.Any(value => value) || !connectedPlayers.Any(value => value);

    static int Winner(params bool[] survivingTeams)
    {
        var winner = 0;
        var count = 0;
        for (var i = 0; i < survivingTeams.Length; i++)
        {
            if (!survivingTeams[i]) continue;
            count++;
            winner = i + 1;
        }
        return count == 1 ? winner : 0;
    }

    if (ShouldDefeat([true, false, false], [false, true, false]))
        throw new InvalidOperationException("Scenario failed: a connected base-less ally must keep a surviving abandoned base in play.");
    if (!ShouldDefeat([false, false, false], [true, true, true]))
        throw new InvalidOperationException("Scenario failed: a team with no living bases must be defeated.");
    if (!ShouldDefeat([true, false, false], [false, false, false]))
        throw new InvalidOperationException("Scenario failed: an abandoned team with no connected players must be defeated.");
    if (ShouldDefeat([false, true, false], [false, true, false]))
        throw new InvalidOperationException("Scenario failed: a normal connected player with a living base must remain active.");
    if (Winner(false, false, true, false) != 3)
        throw new InvalidOperationException("Scenario failed: the only surviving team must win.");
    if (Winner(true, false, true, false) != 0 || Winner(false, false, false, false) != 0)
        throw new InvalidOperationException("Scenario failed: victory requires exactly one surviving team.");

    var totalGold = 101;
    var recipientCount = 2;
    var share = totalGold / recipientCount;
    var remainder = totalGold - share * recipientCount;
    if (share + (remainder > 0 ? 1 : 0) + share != totalGold)
        throw new InvalidOperationException("Scenario failed: gold remainder distribution must preserve the full source total.");
}

static string InsertGlobals(string script, string globals)
{
    const string marker = "\nendglobals\n";
    var index = script.IndexOf(marker, StringComparison.Ordinal);
    if (index < 0 || script.IndexOf(marker, index + 1, StringComparison.Ordinal) >= 0)
        throw new InvalidOperationException("Expected exactly one global block terminator.");
    return script.Insert(index + 1, globals.Trim() + "\n");
}

static string InsertHelpers(string script, string helpers)
{
    var firstTrigger = new TriggerSection("New Shield Spell", "InitTrig_New_Shield_Spell");
    var section = ExtractTriggerSection(script, firstTrigger);
    return script.Insert(section.Start, helpers.Trim() + "\n\n");
}

static string PatchStartGame(string script)
{
    const string name = "Trig_Start_game_Actions";
    var function = ExtractFunction(script, name);
    var replacement = ReplaceOnce(function.Text,
        "function Trig_Start_game_Actions takes nothing returns nothing\n",
        "function Trig_Start_game_Actions takes nothing returns nothing\n    set PlayerLifecycle_GameStarted=true\n");
    return script.Remove(function.Start, function.Length).Insert(function.Start, replacement);
}

static string PatchVotekickEnumeration(string script)
{
    const string name = "Trig_Votekick_Initiate_Actions";
    var function = ExtractFunction(script, name);
    var replacement = ReplaceOnce(function.Text,
        "call ForForce(GetPlayersAll(), function Trig_Votekick_Initiate_Func003A)",
        "call ForForce(udg_all_players, function Trig_Votekick_Initiate_Func003A)");
    return script.Remove(function.Start, function.Length).Insert(function.Start, replacement);
}

static string ReplaceTriggerSection(string script, TriggerSection section, string replacement)
{
    var oldSection = ExtractTriggerSection(script, section);
    return script.Remove(oldSection.Start, oldSection.Length).Insert(oldSection.Start, ToLf(replacement).TrimEnd('\n') + "\n\n");
}

static TextSlice ExtractTriggerSection(string text, TriggerSection section)
{
    var marker = $"// Trigger: {section.CommentName}";
    var markerIndex = text.IndexOf(marker, StringComparison.Ordinal);
    if (markerIndex < 0)
        throw new InvalidOperationException($"Could not find trigger marker: {marker}");
    if (text.IndexOf(marker, markerIndex + marker.Length, StringComparison.Ordinal) >= 0)
        throw new InvalidOperationException($"Trigger marker is not unique: {marker}");
    const string divider = "//===========================================================================\n";
    var start = text.LastIndexOf(divider, markerIndex, StringComparison.Ordinal);
    if (start < 0)
        throw new InvalidOperationException($"Could not find section start for {section.CommentName}.");
    var initFunction = ExtractFunction(text, section.InitFunction);
    var end = initFunction.Start + initFunction.Length;
    while (end < text.Length && text[end] == '\n') end++;
    return new TextSlice(start, end - start, text[start..end]);
}

static string ExtractMarked(string text, string startMarker, string endMarker)
{
    var start = text.IndexOf(startMarker, StringComparison.Ordinal);
    var end = text.IndexOf(endMarker, StringComparison.Ordinal);
    if (start < 0 || end < 0 || end <= start)
        throw new InvalidOperationException($"Invalid marked source block: {startMarker}");
    start += startMarker.Length;
    return text[start..end].Trim('\n', '\r');
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

static TextSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^[ \t]*function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\n.*?^[ \t]*endfunction[ \t]*$", RegexOptions.Singleline);
    if (!match.Success)
        throw new InvalidOperationException($"Could not find function {name}.");
    return new TextSlice(match.Index, match.Length, match.Value);
}

static void Require(string text, string marker)
{
    if (!text.Contains(marker, StringComparison.Ordinal))
        throw new InvalidOperationException($"Expected reviewed marker: {marker}");
}

static string BuildReport(string inputPath, string outputPath, string script, int originalFunctionCount)
{
    var functionCount = Regex.Matches(script, @"(?m)^function\s+").Count;
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    var hash = Convert.ToHexString(SHA256.HashData(Encoding.Latin1.GetBytes(ToCrlf(script)))).ToLowerInvariant();
    var b = new StringBuilder();
    b.AppendLine("# Player Lifecycle Pass 20");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Separated connected-player state, living-base state, and team-defeat state.");
    b.AppendLine("- A team now survives a connected player's base loss while any allied base remains alive.");
    b.AppendLine("- Preserved abandoned-base collect/destroy controls and kept them usable by connected base-less allies.");
    b.AppendLine("- Unified gold splitting, including deterministic distribution of integer remainders.");
    b.AppendLine("- Replaced repeated visibility and victory branches with one guarded four-team result path.");
    b.AppendLine("- Removed retired KOTH handling and the never-enabled 6v6 endgame branch from this subsystem.");
    b.AppendLine("- Made inactive-player cleanup one-shot and preserved protected `h04R` plus active `h02Q` controls.");
    b.AppendLine("- Rejected departed players as AR, swap, votekick, and autopool participants.");
    b.AppendLine("- Routed admin kicks through the same idempotent departure handler.");
    b.AppendLine("- Passed modeled survival, abandonment, exact-one-winner, and gold-remainder scenarios.");
    b.AppendLine($"- Functions: {originalFunctionCount:n0} -> {functionCount:n0}.");
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output script SHA256: `{hash}`.");
    return b.ToString();
}

static string ToLf(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
static string ToCrlf(string text) => ToLf(text).Replace("\n", "\r\n", StringComparison.Ordinal);

sealed record TriggerSection(string CommentName, string InitFunction);
sealed record TextSlice(int Start, int Length, string Text);
