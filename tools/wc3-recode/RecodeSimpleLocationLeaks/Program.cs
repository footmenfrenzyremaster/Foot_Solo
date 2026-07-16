using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-8.j");
var outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-9.j");
var reportPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "simple-location-pass-9", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = File.ReadAllText(inputPath, Encoding.Latin1);
var names = new[]
{
    "Trig_ITEM_Forked_Fury_NEW_Actions",
    "Trig_ITEM_Wall_Actions",
    "Trig_HERO_Architect_tower_build_Actions"
};
foreach (var name in names)
{
    var function = ExtractFunction(script, name);
    if (Regex.Matches(function.Text, @"GetUnitLoc\(").Count != 1 || function.Text.Contains("RemoveLocation(udg_temp_point)", StringComparison.Ordinal))
        throw new InvalidOperationException($"{name} no longer matches the reviewed one-location leak shape.");
    var replacement = function.Text.Replace("endfunction", "    call RemoveLocation(udg_temp_point)\r\nendfunction", StringComparison.Ordinal);
    script = script.Remove(function.Start, function.Length).Insert(function.Start, replacement);
}
script = script.Replace("\r\n", "\n").Replace("\n", "\r\n");
foreach (var name in names)
{
    var function = ExtractFunction(script, name).Text;
    if (Regex.Matches(function, @"RemoveLocation\(udg_temp_point\)").Count != 1)
        throw new InvalidOperationException($"{name} cleanup validation failed.");
}
File.WriteAllText(outputPath, script, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, names, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value.Replace("\r\n", "\n").Replace("\n", "\r\n"));
}

static string BuildReport(string inputPath, string outputPath, IReadOnlyList<string> names, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Simple Location Cleanup Pass 9");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    foreach (var name in names) b.AppendLine($"- Added final `RemoveLocation(udg_temp_point)` to `{name}`.");
    b.AppendLine($"- Output script lines: {script.Replace("\r\n", "\n").Split('\n').Length:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
