using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-16.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "claude_review_fixes_pass_17.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-17.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "claude-review-pass-17", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));

var functions = new[]
{
    "Runtime_KillBlinkShopUnit",
    "Trig_Mass_START_Func002Func014Func004A",
    "Trig_Mass_START_Func002Func030Func003A",
    "Trig_ITEM_Scroll_of_Beast_start_Actions",
    "Trig_Staff_of_the_River_Actions",
    "Trig_Staff_of_the_River_OnHit_Actions",
    "Trig_Akama_Blink_Return_Actions"
};

ValidateOriginal(script);
foreach (var name in functions)
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);

script = ReplaceOnce(script,
    "// Give red 30s to choose pub/priv and if that choice isn't made it defaults to PUB -> SD",
    "// Give Red 10s to choose Pub Default or Pub Custom; timeout defaults to Pub Default -> SD");
script = ReplaceOnce(script,
    "function InitTrig_Start_Vote takes nothing returns nothing",
    "// Trigger: retired Start_Vote\nfunction InitTrig_Start_Vote takes nothing returns nothing");
script = ReplaceOnce(script,
    "function InitTrig_Vote_Timer_Expires takes nothing returns nothing",
    "// Trigger: retired Vote_Timer_Expires\nfunction InitTrig_Vote_Timer_Expires takes nothing returns nothing");

ValidateReplacement(script);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    Require(ExtractFunction(script, "Runtime_KillBlinkShopUnit").Text, "GetRectMinX(gg_rct_ENTIRE_MAP)");
    Require(ExtractFunction(script, "Trig_Mass_START_Func002Func014Func004A").Text, "GetRectMinX(GetPlayableMapRect())");
    Require(ExtractFunction(script, "Trig_Mass_START_Func002Func030Func003A").Text, "GetRectMinX(GetPlayableMapRect())");
    Require(ExtractFunction(script, "Trig_ITEM_Scroll_of_Beast_start_Actions").Text, "GetUnitsInRectMatching(gg_rct_ENTIRE_MAP");
    Require(ExtractFunction(script, "Trig_Staff_of_the_River_OnHit_Actions").Text, "SetUnitPositionLocFacingBJ(udg_MonkeyKing");
    var akama = ExtractFunction(script, "Trig_Akama_Blink_Return_Actions").Text;
    if (akama.Contains("set udg_Akama_Blink_Point[udg_Akama_CV]=null", StringComparison.Ordinal))
        throw new InvalidOperationException("Akama return slot is already cleared; review pass 17 before applying.");
    if (Regex.Matches(script, @"(?m)^// Trigger: retired ").Count != 37)
        throw new InvalidOperationException("Expected 37 labeled retired triggers before adding the two unlabeled vote stubs.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializers in pass 16.");
}

static void ValidateReplacement(string script)
{
    var shop = ExtractFunction(script, "Runtime_KillBlinkShopUnit").Text;
    Require(shop, "local real centerX=GetLocationX(udg_centermap)");
    if (shop.Contains("gg_rct_ENTIRE_MAP", StringComparison.Ordinal))
        throw new InvalidOperationException("Shop-kill helper still uses the corner sliver rect.");

    foreach (var name in new[] { "Trig_Mass_START_Func002Func014Func004A", "Trig_Mass_START_Func002Func030Func003A" })
    {
        var callback = ExtractFunction(script, name).Text;
        Require(callback, "GetLocationX(udg_centermap), GetLocationY(udg_centermap)");
        if (callback.Contains("GetPlayableMapRect", StringComparison.Ordinal))
            throw new InvalidOperationException($"Mass callback still diverges from hero center: {name}");
    }

    var scroll = ExtractFunction(script, "Trig_ITEM_Scroll_of_Beast_start_Actions").Text;
    Require(scroll, "GetUnitsInRectMatching(GetPlayableMapRect()");
    if (scroll.Contains("gg_rct_ENTIRE_MAP", StringComparison.Ordinal))
        throw new InvalidOperationException("Scroll of Beast still enumerates the corner sliver.");

    var river = ExtractFunction(script, "Trig_Staff_of_the_River_OnHit_Actions").Text;
    Require(river, "SetUnitPositionLocFacingBJ(udg_MissileSource");
    Require(river, "AngleBetweenPoints(udg_MissileStart, udg_MissileFinish)");
    if (river.Contains("udg_MonkeyKing", StringComparison.Ordinal) || river.Contains("udg_Jungle_Angle", StringComparison.Ordinal))
        throw new InvalidOperationException("River Staff impact still reads cast-global source or angle state.");

    var cast = ExtractFunction(script, "Trig_Staff_of_the_River_Actions").Text;
    if (cast.Contains("set udg_MonkeyKing=", StringComparison.Ordinal))
        throw new InvalidOperationException("River Staff still writes its obsolete shared caster global.");

    var akama = ExtractFunction(script, "Trig_Akama_Blink_Return_Actions").Text;
    Require(akama, "set udg_Akama_Blink_Point[udg_Akama_CV]=null");
    if (Regex.Matches(script, @"(?m)^// Trigger: retired ").Count != 39)
        throw new InvalidOperationException("Expected all 39 compatibility stubs to have retired labels.");
    if (!script.Contains("// Give Red 10s to choose Pub Default or Pub Custom", StringComparison.Ordinal))
        throw new InvalidOperationException("AFK setup comment was not corrected to ten seconds.");

    var names = Regex.Matches(script, @"(?m)^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
        .Select(match => match.Groups[1].Value).ToList();
    if (names.GroupBy(name => name, StringComparer.Ordinal).Any(group => group.Count() > 1))
        throw new InvalidOperationException("Duplicate function names found.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Trigger initializer count changed.");
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

static string ReplaceOnce(string text, string oldValue, string newValue)
{
    if (Regex.Matches(text, Regex.Escape(oldValue)).Count != 1)
        throw new InvalidOperationException($"Expected one exact replacement: {oldValue}");
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
    b.AppendLine("# Claude Review Fixes Pass 17");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Corrected shop-kill inward movement to use the initialized map-center location.");
    b.AppendLine("- Corrected Scroll of Beast to enumerate the playable map.");
    b.AppendLine("- Matched Mass footman attack orders to the existing hero center target.");
    b.AppendLine("- Made River Staff impact read its caster and trajectory from the active missile instance.");
    b.AppendLine("- Cleared Akama's returned Blink point slot and labeled all 39 compatibility stubs.");
    b.AppendLine("- Corrected the AFK setup comment from 30 seconds to 10 seconds.");
    b.AppendLine("- Preserved all 499 trigger initializer entry points.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
