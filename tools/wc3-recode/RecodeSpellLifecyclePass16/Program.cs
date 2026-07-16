using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-15.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "spell_lifecycle_cleanup_pass_16.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-16.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "spell-lifecycle-pass-16", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));

var oldStaff = ExtractFunction(script, "Trig_Staff_of_the_River_Actions").Text;
var oldAkama = ExtractFunction(script, "Trig_Akama_Blink_Start_Actions").Text;
Require(oldStaff, "PolarProjectionBJ(GetUnitLoc(GetTriggerUnit()), 150.00");
Require(oldAkama, "call PolledWait(5.00)");
if (oldAkama.Contains("RemoveLocation(udg_Akama_Blink_Point", StringComparison.Ordinal))
    throw new InvalidOperationException("Akama start already owns timeout cleanup; review before applying pass 16.");

script = ReplaceFunction(script, "Trig_Staff_of_the_River_Actions", ExtractFunction(source, "Trig_Staff_of_the_River_Actions").Text);
script = ReplaceFunction(script, "Trig_Akama_Blink_Start_Actions", ExtractFunction(source, "Trig_Akama_Blink_Start_Actions").Text);

var newStaff = ExtractFunction(script, "Trig_Staff_of_the_River_Actions").Text;
var newAkama = ExtractFunction(script, "Trig_Akama_Blink_Start_Actions").Text;
Require(newStaff, "PolarProjectionBJ(udg_Jungle_Point[0], 150.00");
if (newStaff.Contains("PolarProjectionBJ(GetUnitLoc", StringComparison.Ordinal))
    throw new InvalidOperationException("River Staff still creates an unowned inline launch location.");
Require(newAkama, "call RemoveLocation(udg_Akama_Blink_Point[udg_Akama_CV])");
Require(newAkama, "set udg_Akama_Blink_Point[udg_Akama_CV]=null");

var names = Regex.Matches(script, @"(?m)^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
    .Select(match => match.Groups[1].Value).ToList();
if (names.GroupBy(name => name, StringComparer.Ordinal).Any(group => group.Count() > 1))
    throw new InvalidOperationException("Duplicate function names found.");
if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
    throw new InvalidOperationException("Trigger initializer count changed.");
if (Regex.Matches(script, @"(?m)^endfunction\s*$").Count != names.Count)
    throw new InvalidOperationException("Function/endfunction balance changed.");

File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

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
    b.AppendLine("# Spell Lifecycle Cleanup Pass 16");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Reused River Staff's existing caster point when creating the missile start point.");
    b.AppendLine("- Released Akama's stored Blink return point when the five-second return window expires unused.");
    b.AppendLine("- Preserved Akama's existing return-trigger cleanup path and all 499 trigger initializers.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
