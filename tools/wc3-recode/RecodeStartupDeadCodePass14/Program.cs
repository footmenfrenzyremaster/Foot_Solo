using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-13.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "startup_dead_code_pass_14.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-14.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "startup-dead-code-pass-14", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
ValidateOriginal(script);

script = ReplaceFunction(script, "Trig_Start_game_Func036A", ExtractFunction(source, "Trig_Start_game_Func036A").Text);
script = RemoveFunction(script, "Trig_Start_game_Func036Func003C");
script = ReplaceOnce(script, "timer udg_timer_upgradedelay= null\n", "");
script = PatchFunction(script, "InitGlobals", function => ReplaceOnce(function, "    set udg_timer_upgradedelay=CreateTimer()\n", ""));
script = PatchFunction(script, "Trig_Start_game_Actions", function =>
{
    function = ReplaceOnce(function, "    call SetPlayerTechResearchedSwap('R00N', 0, ConvertedPlayer(udg_X))\n", "");
    function = ReplaceOnce(function, "    call StartTimerBJ(udg_timer_upgradedelay, false, 280.00)\n", "");
    return function;
});

ValidateReplacement(script);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    var callback = ExtractFunction(script, "Trig_Start_game_Func036A").Text;
    if (Regex.Matches(callback, @"SetPlayerUnitAvailableBJ\(").Count != 10 ||
        !callback.Contains("Trig_Start_game_Func036Func003C()", StringComparison.Ordinal))
        throw new InvalidOperationException("Start-game player callback no longer matches the reviewed ten-unit unreachable branch.");

    var condition = ExtractFunction(script, "Trig_Start_game_Func036Func003C").Text;
    if (!condition.Contains("IsUnitType(GetTriggerUnit(), UNIT_TYPE_STRUCTURE)", StringComparison.Ordinal))
        throw new InvalidOperationException("Start-game structure condition no longer matches the reviewed eventless trigger-unit check.");

    if (Regex.Matches(script, @"SetPlayerTechResearchedSwap\('R00N'").Count != 1)
        throw new InvalidOperationException("Expected exactly one R00N script write.");
    if (Regex.Matches(script, @"udg_timer_upgradedelay").Count != 3)
        throw new InvalidOperationException("Expected the dead upgrade-delay timer to have exactly three references.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializer entry points in pass 13.");
}

static void ValidateReplacement(string script)
{
    var callback = ExtractFunction(script, "Trig_Start_game_Func036A").Text;
    if (Regex.Matches(callback, @"SetPlayerStateBJ\(").Count != 1 ||
        callback.Contains("SetPlayerUnitAvailableBJ", StringComparison.Ordinal) ||
        callback.Contains("GetTriggerUnit()", StringComparison.Ordinal))
        throw new InvalidOperationException("Start-game player callback was not reduced to starting-gold assignment.");

    var forbidden = new[]
    {
        "function Trig_Start_game_Func036Func003C",
        "SetPlayerTechResearchedSwap('R00N'",
        "udg_timer_upgradedelay",
        "call SetPlayerUnitAvailableBJ('h00W'",
        "call SetPlayerUnitAvailableBJ('h00V'"
    };
    foreach (var marker in forbidden)
    {
        if (script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Proven startup no-op remains: {marker}");
    }

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
    b.AppendLine("# Startup Dead Code Pass 14");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Removed the eventless triggering-unit structure test and its unreachable ten-base availability branch.");
    b.AppendLine("- Preserved the callback's live behavior: assigning starting gold to each enumerated player.");
    b.AppendLine("- Removed the lone R00N level-zero write, which targeted player 13 after the preceding loop.");
    b.AppendLine("- Removed the unobserved 280-second upgrade-delay timer start, global, and initialization.");
    b.AppendLine("- Preserved the real R00D/R00G/R00E tier-delay triggers and all 499 trigger initializer entry points.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text)
{
    public int End => Start + Length;
}
