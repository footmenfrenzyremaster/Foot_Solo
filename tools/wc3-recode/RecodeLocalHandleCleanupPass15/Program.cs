using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-14.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "local_handle_cleanup_pass_15.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-15.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "local-handle-cleanup-pass-15", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
var functions = new[]
{
    "Trig_Staff_of_the_River_OnHit_Actions",
    "Trig_HERO_Alchemist_Chemical_Rage_Bounty_Actions",
    "Trig_HERO_Thor_Powerhit_Actions",
    "Trig_HERO_Jaina_Ulti_Actions",
    "Trig_debugmode_Actions",
    "Trig_debug_Actions",
    "Trig_debug_control_Actions"
};

ValidateOriginal(script);
foreach (var name in functions)
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);
ValidateReplacement(script, functions);

File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    Require(ExtractFunction(script, "Trig_Staff_of_the_River_OnHit_Actions").Text,
        "GetUnitsInRangeOfLocAll(425.00, GetUnitLoc(udg_MissileSource))");
    Require(ExtractFunction(script, "Trig_HERO_Alchemist_Chemical_Rage_Bounty_Actions").Text,
        "set udg_temp_degree=AngleBetweenPoints(udg_temp_point_caster, udg_temp_point)");
    Require(ExtractFunction(script, "Trig_HERO_Thor_Powerhit_Actions").Text,
        "set udg_temp_point=PolarProjectionBJ(udg_temp_point, 15.00, udg_temp_degree)");
    Require(ExtractFunction(script, "Trig_HERO_Jaina_Ulti_Actions").Text,
        "GroupPickRandomUnit(GetUnitsInRangeOfLocMatching");
    Require(ExtractFunction(script, "Trig_debugmode_Actions").Text,
        "set udg_temp_point=OffsetLocation(GetPlayerStartLocationLoc");
    Require(ExtractFunction(script, "Trig_debug_Actions").Text,
        "call ForGroupBJ(GetUnitsSelectedAll(udg_temp_player)");
    Require(ExtractFunction(script, "Trig_debug_control_Actions").Text,
        "GetRectCenter(gg_rct_T2_COMPLETE)");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializer entry points in pass 14.");
}

static void ValidateReplacement(string script, IEnumerable<string> functions)
{
    foreach (var name in functions)
    {
        var function = ExtractFunction(script, name).Text;
        if (!function.Contains("RemoveLocation", StringComparison.Ordinal))
            throw new InvalidOperationException($"Replacement has no location cleanup: {name}");
    }

    var staff = ExtractFunction(script, "Trig_Staff_of_the_River_OnHit_Actions").Text;
    if (staff.Contains("GetUnitsInRangeOfLocAll(425.00, GetUnitLoc", StringComparison.Ordinal))
        throw new InvalidOperationException("River Staff still creates an inline source location.");

    var thor = ExtractFunction(script, "Trig_HERO_Thor_Powerhit_Actions").Text;
    if (Regex.Matches(thor, @"set nextPoint=PolarProjectionBJ").Count != 2 ||
        Regex.Matches(thor, @"call RemoveLocation\(udg_temp_point\)").Count != 3)
        throw new InvalidOperationException("Thor point replacement does not own both loop projections and the final point.");

    var jaina = ExtractFunction(script, "Trig_HERO_Jaina_Ulti_Actions").Text;
    foreach (var marker in new[] { "local unit frostOrb", "local unit fireOrb", "local unit lightningOrb", "call DestroyGroup(lightningTargets)" })
        Require(jaina, marker);
    if (jaina.Contains("GroupPickRandomUnit(GetUnitsInRangeOfLocMatching", StringComparison.Ordinal))
        throw new InvalidOperationException("Jaina still leaks the inline target group.");

    var debug = ExtractFunction(script, "Trig_debug_Actions").Text;
    if (debug.Contains("ForGroupBJ(GetUnitsSelectedAll", StringComparison.Ordinal))
        throw new InvalidOperationException("Debug selection still uses an unowned inline group.");

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
}

static void Require(string text, string marker)
{
    if (!text.Contains(marker, StringComparison.Ordinal))
        throw new InvalidOperationException($"Expected reviewed marker: {marker}");
}

static string ReplaceFunction(string script, string name, string replacement)
{
    var function = ExtractFunction(script, name);
    return script.Remove(function.Start, function.Length).Insert(function.Start, ToLf(replacement).TrimEnd('\n'));
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
    b.AppendLine("# Local Handle Cleanup Pass 15");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Cleaned River Staff impact and Alchemist bounty temporary locations.");
    b.AppendLine("- Cleaned every replaced point in Thor Powerhit's movement loop.");
    b.AppendLine("- Made Jaina ultimate orb ownership local per cast and destroyed its temporary target group.");
    b.AppendLine("- Cleaned temporary points and selected-unit groups in three debug helpers.");
    b.AppendLine("- Preserved all 499 trigger initializer entry points.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
