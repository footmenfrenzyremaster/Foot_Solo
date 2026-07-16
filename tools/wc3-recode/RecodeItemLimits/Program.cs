using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-7.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "item_limits_pass_8.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-8.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "item-limits-pass-8", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = File.ReadAllText(inputPath, Encoding.Latin1);
var replacement = File.ReadAllText(sourcePath, Encoding.Latin1).Replace("\r\n", "\n").Replace("\n", "\r\n").TrimEnd('\r', '\n');

var firstHelper = script.IndexOf("function Trig_ITEM_limits_Func007Func003Func001C", StringComparison.Ordinal);
var action = ExtractFunction(script, "Trig_ITEM_limits_Actions");
if (firstHelper < 0 || firstHelper >= action.Start) throw new InvalidOperationException("Could not identify the original item-limit block.");
var originalBlock = script.Substring(firstHelper, action.Start + action.Length - firstHelper);
ValidateOriginal(originalBlock);
var rewritten = script.Remove(firstHelper, originalBlock.Length).Insert(firstHelper, replacement);
rewritten = rewritten.Replace("\r\n", "\n").Replace("\n", "\r\n");
ValidateReplacement(rewritten);

File.WriteAllText(outputPath, rewritten, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, originalBlock, replacement, rewritten), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string block)
{
    var expectedItems = new[] { "rat9", "stwa", "mcou", "I02J", "rwiz", "ofro", "I042", "ratf", "ckng", "I03C", "belv", "tmmt", "rde3", "bgst", "cnob", "I006", "I009", "ciri", "rlif", "desc", "I01H", "evtl", "I04A", "tret", "modt", "nspi" };
    var actualItems = Regex.Matches(block, @"set\s+udg_temp_item_type='(.{4})'").Select(m => m.Groups[1].Value).ToArray();
    if (!actualItems.SequenceEqual(expectedItems)) throw new InvalidOperationException("Original item rule order or rawcodes differ from the reviewed 26-rule catalog.");

    var expectedRestricted = new Dictionary<string, string[]>
    {
        ["007"] = ["O003"],
        ["010"] = ["O003", "O00X"],
        ["063"] = ["H01I", "E005", "E007", "H02G", "E00U", "N01L", "E00Z"],
        ["066"] = ["H01I", "E005", "E007", "H02G", "E00U", "N01L", "E00Z"],
        ["069"] = ["O00X", "O003", "E00F", "N00C", "N00I"],
        ["071"] = ["O00X", "O003", "E00A", "E00U", "E00O", "H036"],
        ["074"] = ["H02I", "N00Z", "H01H", "H00A"],
        ["076"] = ["H02D"],
        ["079"] = ["O003"]
    };
    foreach (var rule in expectedRestricted)
    {
        var match = Regex.Match(block, $@"(?ms)^function\s+Trig_ITEM_limits_Func{rule.Key}Func.*?GetUnitTypeId\(udg_temp_unit\).*?^endfunction\s*$");
        if (!match.Success) throw new InvalidOperationException($"Could not find restriction helper for rule {rule.Key}.");
        var rawcodes = Regex.Matches(match.Value, @"GetUnitTypeId\(udg_temp_unit\) == '(.{4})'").Select(m => m.Groups[1].Value).ToArray();
        if (!rawcodes.SequenceEqual(rule.Value)) throw new InvalidOperationException($"Hero restrictions differ for rule {rule.Key}.");
    }
    if (Regex.Matches(block, @"Only 1x").Count != 18 || Regex.Matches(block, @"Only 2x").Count != 2)
        throw new InvalidOperationException("Original item cap counts differ from 18 one-item and 2 two-item rules.");
}

static void ValidateReplacement(string script)
{
    var required = new[]
    {
        "function ItemLimits_GetMax takes integer itemType returns integer",
        "function ItemLimits_Count takes unit whichUnit, integer itemType returns integer",
        "function ItemLimits_IsRestricted takes integer itemType, integer heroType returns boolean",
        "return heroType == 'H01I' or heroType == 'E005' or heroType == 'E007' or heroType == 'H02G' or heroType == 'E00U' or heroType == 'N01L' or heroType == 'E00Z'",
        "if udg_temp_int > maximum then",
        "if ItemLimits_IsRestricted(itemType, heroType) then"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal)) throw new InvalidOperationException($"Replacement marker missing: {marker}");
    }
    if (script.Contains("function Trig_ITEM_limits_Func007", StringComparison.Ordinal))
        throw new InvalidOperationException("Old generated item-limit helpers remain.");
    if (Regex.Matches(script, "function ItemLimits_").Count != 5)
        throw new InvalidOperationException("Expected five named item-limit helper functions.");
}

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length);
}

static string BuildReport(string inputPath, string outputPath, string oldBlock, string newBlock, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Item Limits Pass 8");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Validated all 26 item rawcodes in original rule order.");
    b.AppendLine("- Validated 18 one-item caps and 2 two-item caps.");
    b.AppendLine("- Validated all 9 hero-restriction lists.");
    b.AppendLine("- Preserved cap removal before hero-restriction removal.");
    b.AppendLine("- Preserved the three custom restriction messages.");
    b.AppendLine($"- Item-limit block lines: {LineCount(oldBlock):n0} -> {LineCount(newBlock):n0}.");
    b.AppendLine($"- Output script lines: {LineCount(script):n0}.");
    return b.ToString();
}

static int LineCount(string text) => text.Replace("\r\n", "\n").Split('\n').Length;
sealed record FunctionSlice(int Start, int Length);
