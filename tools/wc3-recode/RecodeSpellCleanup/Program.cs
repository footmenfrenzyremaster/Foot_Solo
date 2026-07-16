using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-4.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "spell_cleanup_pass_5.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-5.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "spell-cleanup-pass-5", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = File.ReadAllText(inputPath, Encoding.Latin1);
var source = File.ReadAllText(sourcePath, Encoding.Latin1);
var names = new[]
{
    "Trig_HERO_Myrmidon_Sinkhole_suck_Func001Func001Func007A",
    "Trig_HERO_Myrmidon_Sinkhole_suck_Actions",
    "Trig_HERO_Myrmidon_Sinkhole_end_Actions",
    "Trig_HERO_Prison_Func029A",
    "Trig_HERO_Prison_Wall_countdown_Actions"
};

foreach (var name in names)
{
    var replacement = ExtractFunction(source, name);
    var original = ExtractFunction(script, name);
    script = script.Remove(original.Start, original.Length).Insert(original.Start, replacement.Text);
}

var sinkholeActions = ExtractFunction(script, "Trig_HERO_Myrmidon_Sinkhole_suck_Actions");
var helper = ExtractFunction(source, "Sinkhole_EndForPlayer");
script = script.Insert(sinkholeActions.Start, helper.Text + "\r\n\r\n");

Validate(script);
File.WriteAllText(outputPath, script, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);

Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");
Console.WriteLine("Replaced spell functions: 5 plus 1 new cleanup helper");

static FunctionSlice ExtractFunction(string text, string name)
{
    var pattern = $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\r?\n.*?^endfunction\s*$";
    var match = Regex.Match(text, pattern, RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value.Replace("\r\n", "\n").Replace("\n", "\r\n"));
}

static void Validate(string script)
{
    var required = new[]
    {
        "function Sinkhole_EndForPlayer takes integer playerNumber returns nothing",
        "call Sinkhole_EndForPlayer(udg_X)",
        "call Sinkhole_EndForPlayer(GetConvertedPlayerId(GetTriggerPlayer()))",
        "call GroupClear(udg_waterfall_units[udg_X])",
        "call DestroyBoolExpr(unitFilter)",
        "GetRectCenterX(gg_rct_EXILE)",
        "call RemoveLocation(udg_prison_block_point_4[udg_temp_playernumber])"
    };
    foreach (var value in required)
    {
        if (!script.Contains(value, StringComparison.Ordinal)) throw new InvalidOperationException($"Validation marker missing: {value}");
    }
    if (script.Contains("call ConditionalTriggerExecute(gg_trg_HERO_Myrmidon_Sinkhole_end)", StringComparison.Ordinal))
        throw new InvalidOperationException("The invalid timer-context sinkhole cleanup call remains.");
    if (script.Contains("call SetUnitPositionLoc(GetEnumUnit(), GetRectCenter(gg_rct_EXILE))", StringComparison.Ordinal))
        throw new InvalidOperationException("The prison exile location leak remains.");
}

static string BuildReport(string inputPath, string outputPath, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Spell Cleanup Pass 5");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Sinkhole timer completion now calls cleanup directly with the correct player index.");
    b.AppendLine("- Sinkhole dummy groups are recreated after cleanup, allowing later casts.");
    b.AppendLine("- Sinkhole temporary groups and filters are destroyed each tick.");
    b.AppendLine("- Sinkhole pull movement uses coordinates instead of three temporary locations per unit.");
    b.AppendLine("- Sinkhole completion sound uses the live center instead of a removed offset location.");
    b.AppendLine("- Prison exile movement uses rectangle coordinates without allocating a location.");
    b.AppendLine("- Prison wall and block locations are removed once and nulled after expiration.");
    b.AppendLine($"- Output lines: {script.Replace("\r\n", "\n").Split('\n').Length:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
