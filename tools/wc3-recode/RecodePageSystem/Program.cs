using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-3.j");
var systemPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "page_system.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-4.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "page-system-recode", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = File.ReadAllText(inputPath, Encoding.Latin1);
var replacement = NormalizeNewlines(File.ReadAllText(systemPath, Encoding.Latin1)).TrimEnd('\r', '\n');
var startMarker = "function Trig_page_system_Func004Func005002003 takes nothing returns boolean";
var endFunction = ExtractFunction(script, "Trig_page_system_Actions");
var start = script.IndexOf(startMarker, StringComparison.Ordinal);
if (start < 0) throw new InvalidOperationException("Could not find the first generated page-system helper.");
if (endFunction.Start <= start) throw new InvalidOperationException("Invalid page-system function range.");

var oldBlock = script.Substring(start, endFunction.Start + endFunction.Length - start);
ValidateOriginalMappings(oldBlock);
var rewritten = script.Remove(start, oldBlock.Length).Insert(start, replacement);
ValidateReplacement(rewritten);

File.WriteAllText(outputPath, rewritten, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script, rewritten), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");
Console.WriteLine("Validated page mappings: 10");
Console.WriteLine($"Line count: {CountLines(script):n0} -> {CountLines(rewritten):n0}");

static FunctionSlice ExtractFunction(string script, string name)
{
    var match = Regex.Match(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+nothing\s+returns\s+nothing\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length);
}

static void ValidateOriginalMappings(string block)
{
    var soldTypes = Regex.Matches(block, @"GetUnitTypeId\(udg_temp_unit\) == '(.{4})'").Select(m => m.Groups[1].Value).ToArray();
    var targetTypes = Regex.Matches(block, @"GetUnitTypeId\(GetFilterUnit\(\)\) == '(.{4})'").Select(m => m.Groups[1].Value).ToArray();
    var expectedSold = new[] { "h02U", "h02V", "h03P", "h03O", "h02X", "h03S", "h02Y", "h03T", "h037", "h038" };
    var expectedTarget = new[] { "n02U", "n00R", "n03L", "n02U", "n02V", "n03Z", "n02N", "n02V", "n02Z", "n01Y" };
    if (!soldTypes.SequenceEqual(expectedSold) || !targetTypes.SequenceEqual(expectedTarget))
        throw new InvalidOperationException("Original page mappings differ from the reviewed ten-row mapping.");
}

static void ValidateReplacement(string script)
{
    if (Regex.Matches(script, @"soldType == '(.{4})'").Count != 10)
        throw new InvalidOperationException("Replacement does not contain ten sold-unit mappings.");
    if (!script.Contains("call DestroyBoolExpr(matchingType)", StringComparison.Ordinal) ||
        !script.Contains("call DestroyGroup(nearby)", StringComparison.Ordinal) ||
        !script.Contains("if udg_temp_int == 0 then", StringComparison.Ordinal))
        throw new InvalidOperationException("Replacement cleanup or non-page guard is missing.");
    if (script.Contains("function Trig_page_system_Func004C", StringComparison.Ordinal))
        throw new InvalidOperationException("Old generated page-system helpers remain in the script.");
}

static string BuildReport(string inputPath, string outputPath, string before, string after)
{
    var b = new StringBuilder();
    b.AppendLine("# Page System Recode Report");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Preserved all ten sold-unit to showroom-unit mappings.");
    b.AppendLine("- Added an early return for unrelated sold units.");
    b.AppendLine("- Uses a fresh local group for each valid page selection.");
    b.AppendLine("- Destroys the temporary filter and group after enumeration.");
    b.AppendLine("- Removed the temporary location allocation by enumerating around unit coordinates.");
    b.AppendLine($"- Script lines: {CountLines(before):n0} -> {CountLines(after):n0}.");
    return b.ToString();
}

static string NormalizeNewlines(string text) => text.Replace("\r\n", "\n").Replace("\n", "\r\n");
static int CountLines(string text) => text.Replace("\r\n", "\n").Split('\n').Length;
sealed record FunctionSlice(int Start, int Length);
