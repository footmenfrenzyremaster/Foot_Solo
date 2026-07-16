using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-5.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "glaive_loop_pass_6.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-6.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "glaive-loop-pass-6", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = File.ReadAllText(inputPath, Encoding.Latin1);
var source = File.ReadAllText(sourcePath, Encoding.Latin1);
var names = new[]
{
    "Trig_HERO_Throw_Glaive_Loop_Func001Func001Func016Func001Func002C",
    "Trig_HERO_Throw_Glaive_Loop_Func001Func001Func016Func001C",
    "Trig_HERO_Throw_Glaive_Loop_Actions"
};
foreach (var name in names)
{
    var original = ExtractFunction(script, name);
    var replacement = ExtractFunction(source, name);
    script = script.Remove(original.Start, original.Length).Insert(original.Start, replacement.Text);
}
Validate(script);
File.WriteAllText(outputPath, script, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value.Replace("\r\n", "\n").Replace("\n", "\r\n"));
}

static void Validate(string script)
{
    var action = ExtractFunction(script, "Trig_HERO_Throw_Glaive_Loop_Actions").Text;
    if (action.Contains("AngleBetweenPoints(GetUnitLoc", StringComparison.Ordinal)) throw new InvalidOperationException("Glaive angle location leak remains.");
    if (!action.Contains("local boolean anyActive=false", StringComparison.Ordinal) || !action.Contains("set udg_dummy_glaive[udg_X]=null", StringComparison.Ordinal))
        throw new InvalidOperationException("Multiplayer-safe glaive lifetime markers are missing.");
    if (Regex.Matches(action, "DisableTrigger\\(GetTriggeringTrigger\\(\\)\\)").Count != 1)
        throw new InvalidOperationException("Glaive loop should disable only once after scanning all players.");
    if (Regex.Matches(action, "RemoveLocation\\(udg_glaive_location\\[udg_X\\]\\)").Count != 2)
        throw new InvalidOperationException("Glaive location replacement/final cleanup is incomplete.");
}

static string BuildReport(string inputPath, string outputPath, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Glaive Loop Pass 6");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Replaced per-tick angle locations with unit-coordinate math.");
    b.AppendLine("- Removes the previous movement location before storing its replacement.");
    b.AppendLine("- Distance checks use coordinates without allocating hero locations.");
    b.AppendLine("- Finished glaives clear their unit and location handles.");
    b.AppendLine("- The shared periodic trigger disables only after all players have finished.");
    b.AppendLine($"- Output lines: {script.Replace("\r\n", "\n").Split('\n').Length:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
