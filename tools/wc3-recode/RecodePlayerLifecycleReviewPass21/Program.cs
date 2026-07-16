using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-20.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "player_lifecycle_review_pass_21.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-21.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "player-lifecycle-review-pass-21", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
var originalFunctionCount = Regex.Matches(script, @"(?m)^function\s+").Count;

var replacements = new[]
{
    "PlayerLifecycle_RemoveInactiveUnits",
    "PlayerLifecycle_DefeatTeam",
    "PlayerLifecycle_HandleDeparture",
    "Trig_Kill_units_of_inactive_player_Actions",
    "Trig_player_destroyed_Actions",
    "InitTrig_Visibility",
    "Trig_Votekick_Select_Ally_To_Kick_Actions",
    "Trig_Votekick_Show_Yes_and_No_Conditions",
    "Trig_Votekick_Vote_Yes_or_No_Conditions",
    "Trig_Votekick_Vote_Yes_or_No_Func002Func005Func003C",
    "Trig_Votekick_Vote_Yes_or_No_Actions",
    "Trig_Votekick_RESET_Actions",
    "Trig_autopool_execute_Conditions",
    "Trig_autopool_th_execute_Actions"
};

ValidateOriginal(script);

script = ReplaceOnce(script, "boolean PlayerLifecycle_RemoveBaseControls=false\n", string.Empty);
var helpers = ExtractMarked(source, "// PASS21_HELPERS_BEGIN", "// PASS21_HELPERS_END");
var cleanupStart = ExtractFunction(script, "PlayerLifecycle_RemoveInactiveUnits").Start;
script = script.Insert(cleanupStart, helpers.Trim() + "\n\n");

foreach (var functionName in replacements)
    script = ReplaceFunction(script, functionName, ExtractFunction(source, functionName).Text);

script = RemoveFunction(script, "Trig_Visibility_Actions");

ValidateReplacement(script, replacements);
ValidateScenarioModel();

File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script, originalFunctionCount), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    if (Regex.Matches(script, @"(?m)^boolean PlayerLifecycle_RemoveBaseControls=false$").Count != 1)
        throw new InvalidOperationException("Expected the single Pass 20 global cleanup switch.");

    Require(ExtractFunction(script, "PlayerLifecycle_RemoveInactiveUnits").Text, "PlayerLifecycle_RemoveBaseControls or unitType != 'h02Q'");
    Require(ExtractFunction(script, "PlayerLifecycle_DefeatTeam").Text, "set PlayerLifecycle_RemoveBaseControls=true");
    Require(ExtractFunction(script, "Trig_player_destroyed_Actions").Text, "set PlayerLifecycle_RemoveBaseControls=false");
    Require(ExtractFunction(script, "Trig_Votekick_Select_Ally_To_Kick_Actions").Text, "DisplayTimedTextToForce(GetForceOfPlayer");
    Require(ExtractFunction(script, "Trig_Votekick_Vote_Yes_or_No_Func002Func005Func003C").Text, "GetPlayerSlotState(udg_Votekick_Target_Player)");
    Require(ExtractFunction(script, "Trig_Votekick_RESET_Actions").Text, "ForForce(GetPlayersAll()");
    Require(ExtractFunction(script, "Trig_autopool_execute_Conditions").Text, "PlayerLifecycle_IsConnected");
    Require(ExtractFunction(script, "Trig_autopool_th_execute_Actions").Text, "DestroyForce(udg_temp_playergroup)");
    Require(ExtractFunction(script, "InitTrig_Visibility").Text, "function Trig_Visibility_Actions");

    if (Regex.Matches(script, @"\bTrig_Visibility_Actions\b").Count != 2)
        throw new InvalidOperationException("Visibility has an unexpected caller; do not retire it automatically.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializers in Pass 20.");
}

static void ValidateReplacement(string script, IReadOnlyList<string> replacements)
{
    foreach (var name in replacements)
    {
        if (Regex.Matches(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes").Count != 1)
            throw new InvalidOperationException($"Expected exactly one patched function: {name}");
    }

    if (script.Contains("PlayerLifecycle_RemoveBaseControls", StringComparison.Ordinal))
        throw new InvalidOperationException("The cross-team global cleanup switch still exists.");
    if (Regex.IsMatch(script, @"(?m)^function\s+Trig_Visibility_Actions\s+takes"))
        throw new InvalidOperationException("The dead Visibility action still exists.");

    Require(script, "function PlayerLifecycle_ShouldRemoveInactiveUnit takes integer controllerNumber, unit whichUnit returns boolean");
    Require(script, "function PlayerLifecycle_RemoveDepartingTransientUnits takes integer playerNumber returns nothing");
    Require(ExtractFunction(script, "PlayerLifecycle_RemoveInactiveUnits").Text, "PlayerLifecycle_ShouldRemoveInactiveUnit(playerNumber, ownedUnit)");
    Require(ExtractFunction(script, "PlayerLifecycle_ShouldRemoveInactiveUnit").Text, "if unitType == 'n02G' then");
    Require(ExtractFunction(script, "PlayerLifecycle_ShouldRemoveInactiveUnit").Text, "udg_force_defeat[sourceTeam] or not PlayerLifecycle_IsBaseAlive(sourcePlayerNumber)");
    Require(ExtractFunction(script, "PlayerLifecycle_HandleDeparture").Text, "PlayerLifecycle_RemoveDepartingTransientUnits(playerNumber)");

    var visibility = ExtractFunction(script, "InitTrig_Visibility").Text;
    Require(visibility, "call DisableTrigger(gg_trg_Visibility)");
    if (visibility.Contains("TriggerAddAction", StringComparison.Ordinal))
        throw new InvalidOperationException("Visibility still registers its retired action.");

    var select = ExtractFunction(script, "Trig_Votekick_Select_Ally_To_Kick_Actions").Text;
    Require(select, "PlayerLifecycle_IsParticipating(targetNumber)");
    Require(select, "DisplayTimedTextToPlayer(notifyPlayer");
    if (select.Contains("GetForceOfPlayer", StringComparison.Ordinal))
        throw new InvalidOperationException("Votekick selection still allocates anonymous forces.");

    Require(ExtractFunction(script, "Trig_Votekick_Show_Yes_and_No_Conditions").Text, "PlayerLifecycle_IsParticipating(playerNumber)");
    Require(ExtractFunction(script, "Trig_Votekick_Vote_Yes_or_No_Conditions").Text, "PlayerLifecycle_IsParticipating");
    Require(ExtractFunction(script, "Trig_Votekick_Vote_Yes_or_No_Func002Func005Func003C").Text, "PlayerLifecycle_IsParticipating");

    var vote = ExtractFunction(script, "Trig_Votekick_Vote_Yes_or_No_Actions").Text;
    Require(vote, "DisplayTextToForce(udg_all_players");
    if (vote.Contains("GetPlayersAll()", StringComparison.Ordinal))
        throw new InvalidOperationException("Votekick completion still allocates an all-player force.");

    var reset = ExtractFunction(script, "Trig_Votekick_RESET_Actions").Text;
    Require(reset, "ForForce(udg_all_players");
    if (reset.Contains("GetPlayersAll()", StringComparison.Ordinal))
        throw new InvalidOperationException("Votekick reset still allocates an all-player force.");

    Require(ExtractFunction(script, "Trig_autopool_execute_Conditions").Text, "PlayerLifecycle_IsParticipating");
    var threshold = ExtractFunction(script, "Trig_autopool_th_execute_Actions").Text;
    Require(threshold, "if not PlayerLifecycle_IsParticipating(playerNumber) then");
    Require(threshold, "DisplayTextToPlayer(ConvertedPlayer(playerNumber)");
    if (threshold.Contains("GetForceOfPlayer", StringComparison.Ordinal) || threshold.Contains("DestroyForce", StringComparison.Ordinal))
        throw new InvalidOperationException("Autopool threshold still mutates temporary force state.");

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

    var allDeclaredNames = Regex.Matches(script, @"(?m)^[ \t]*function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
        .Select(match => match.Groups[1].Value).ToHashSet(StringComparer.Ordinal);
    foreach (Match match in Regex.Matches(script, @"ExecuteFunc\(\""([^\""\r\n]+)\""\)"))
    {
        var target = match.Groups[1].Value;
        if (!allDeclaredNames.Contains(target))
            throw new InvalidOperationException($"ExecuteFunc target is undefined: {target}");
    }
}

static void ValidateScenarioModel()
{
    static bool ShouldRemove(UnitKind kind, bool controllerConnected, int controllerTeam = 1, bool sourceValid = true,
        int sourceTeam = 1, bool sourceTeamDefeated = false, bool sourceBaseAlive = true) => kind switch
    {
        UnitKind.Protected => false,
        UnitKind.Vision => !controllerConnected,
        UnitKind.Normal => true,
        UnitKind.BaseControl => !controllerConnected || !sourceValid || sourceTeam is < 1 or > 4 ||
                                controllerTeam != sourceTeam || sourceTeamDefeated || !sourceBaseAlive,
        _ => true
    };

    if (ShouldRemove(UnitKind.BaseControl, true, controllerTeam: 3, sourceTeam: 3))
        throw new InvalidOperationException("Scenario failed: unrelated team defeat must not remove a valid surviving-team control.");
    if (!ShouldRemove(UnitKind.BaseControl, true, sourceTeamDefeated: true))
        throw new InvalidOperationException("Scenario failed: a defeated team's control must be removed.");
    if (!ShouldRemove(UnitKind.BaseControl, true, sourceBaseAlive: false))
        throw new InvalidOperationException("Scenario failed: a dead base's control must be removed.");
    if (!ShouldRemove(UnitKind.BaseControl, false))
        throw new InvalidOperationException("Scenario failed: a departed controller's control must be removed.");
    if (!ShouldRemove(UnitKind.BaseControl, true, controllerTeam: 2, sourceTeam: 3))
        throw new InvalidOperationException("Scenario failed: a wrong-team control must be removed.");
    if (ShouldRemove(UnitKind.Vision, true))
        throw new InvalidOperationException("Scenario failed: a connected defeated spectator must retain center vision.");
    if (!ShouldRemove(UnitKind.Vision, false))
        throw new InvalidOperationException("Scenario failed: a departed spectator's center vision must be removed.");
    if (ShouldRemove(UnitKind.Protected, false))
        throw new InvalidOperationException("Scenario failed: h04R remains protected.");
    if (!ShouldRemove(UnitKind.Normal, true))
        throw new InvalidOperationException("Scenario failed: ordinary inactive-player units must be removed.");

    static bool CanUseDialog(bool connected, bool active, bool baseAlive) => connected && active && baseAlive;
    if (CanUseDialog(true, false, true) || CanUseDialog(true, true, false) || CanUseDialog(false, true, true))
        throw new InvalidOperationException("Scenario failed: stale votekick/autopool dialogs must reject non-participants.");
    if (!CanUseDialog(true, true, true))
        throw new InvalidOperationException("Scenario failed: active participants must retain dialog access.");
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
    b.AppendLine("# Player Lifecycle Review Pass 21");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Replaced the global inactive-unit cleanup switch with per-control source-team and base-state checks.");
    b.AppendLine("- Preserved valid abandoned-base controls on surviving teams during an unrelated team's defeat.");
    b.AppendLine("- Preserved defeated connected players' center-vision units across later cleanup passes.");
    b.AppendLine("- Removed transient base controls and center vision when their owning player actually departs.");
    b.AppendLine("- Revalidated votekick initiators, voters, and targets when stale dialogs are clicked.");
    b.AppendLine("- Revalidated autopool dialogs and removed the threshold action's anonymous-force leak and stale-force destruction.");
    b.AppendLine("- Retired the uncalled Visibility action while retaining its disabled GUI trigger initializer.");
    b.AppendLine("- Passed modeled cross-team control, vision persistence, departure cleanup, and stale-dialog scenarios.");
    b.AppendLine($"- Functions: {originalFunctionCount:n0} -> {functionCount:n0}.");
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output script SHA256: `{hash}`.");
    return b.ToString();
}

static string ToLf(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
static string ToCrlf(string text) => ToLf(text).Replace("\n", "\r\n", StringComparison.Ordinal);

enum UnitKind { Protected, Vision, BaseControl, Normal }
sealed record TextSlice(int Start, int Length, string Text);
