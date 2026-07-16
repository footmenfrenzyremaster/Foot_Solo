using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-17.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "guardian_force_field_pass_18.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-18.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "guardian-force-field-pass-18", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));

var replacementFunctions = new[]
{
    "Trig_Guardian_Force_Field_Cast_Actions",
    "Trig_Guardian_Force_Field_Loop_Actions",
    "Trig_Guardian_Force_Field_Damage_Actions"
};
var helperFunctions = new[]
{
    "GuardianFF_GetWallCount",
    "GuardianFF_CreateSidePoint",
    "GuardianFF_AddWall",
    "GuardianFF_RemoveWallPathing",
    "GuardianFF_DestroyInstanceEffects",
    "GuardianFF_MoveInstanceEffects"
};
var obsoleteFunctions = new[]
{
    "Trig_Guardian_Force_Field_Cast_Func005C",
    "Trig_Guardian_Force_Field_Cast_Func028C",
    "Trig_Guardian_Force_Field_Cast_Func031C",
    "Trig_Guardian_Force_Field_Cast_Func033C",
    "Trig_Guardian_Force_Field_Loop_Func001Func003Func003C",
    "Trig_Guardian_Force_Field_Loop_Func001Func003Func013Func001C",
    "Trig_Guardian_Force_Field_Loop_Func001Func003Func013C",
    "Trig_Guardian_Force_Field_Loop_Func001Func003Func030C",
    "Trig_Guardian_Force_Field_Loop_Func001Func003C",
    "Trig_Guardian_Force_Field_Damage_Func013Func008C",
    "Trig_Guardian_Force_Field_Damage_Func013C"
};

ValidateOriginal(script, helperFunctions);
foreach (var name in replacementFunctions)
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);
foreach (var name in obsoleteFunctions)
    script = RemoveFunction(script, name);

var helperBlock = string.Join("\n\n", helperFunctions.Select(name => ExtractFunction(source, name).Text)) + "\n\n";
var castAction = ExtractFunction(script, "Trig_Guardian_Force_Field_Cast_Actions");
script = script.Insert(castAction.Start, helperBlock);

ValidateReplacement(script, obsoleteFunctions, helperFunctions);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script, IEnumerable<string> helperFunctions)
{
    foreach (var name in helperFunctions)
        if (Regex.IsMatch(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes"))
            throw new InvalidOperationException($"Pass 18 helper already exists: {name}");

    var cast = ExtractFunction(script, "Trig_Guardian_Force_Field_Cast_Actions").Text;
    Require(cast, "exitwhen udg_Guardian_Integer[0] > 6");
    Require(cast, "CPP_Add_Pathing(udg_Guardian_Point[udg_Guardian_Integer[0]]");

    var loopLevel3 = ExtractFunction(script, "Trig_Guardian_Force_Field_Loop_Func001Func003Func013C").Text;
    Require(loopLevel3, "udg_Guardian_FF_Level[udg_Guardian_FF_Index]");
    var loopLevel5 = ExtractFunction(script, "Trig_Guardian_Force_Field_Loop_Func001Func003Func013Func001C").Text;
    Require(loopLevel5, "> 6");
    var damage = ExtractFunction(script, "Trig_Guardian_Force_Field_Damage_Actions").Text;
    Require(damage, "set udg_Guardian_Integer[10]=1");
    var damageBranch = ExtractFunction(script, "Trig_Guardian_Force_Field_Damage_Func013C").Text;
    Require(damageBranch, "<= 6");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializers in pass 17.");
}

static void ValidateReplacement(string script, IEnumerable<string> obsoleteFunctions, IEnumerable<string> helperFunctions)
{
    foreach (var name in obsoleteFunctions)
        if (Regex.IsMatch(script, $@"\b{Regex.Escape(name)}\b"))
            throw new InvalidOperationException($"Obsolete Guardian helper remains referenced: {name}");
    foreach (var name in helperFunctions)
        if (Regex.Matches(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes").Count != 1)
            throw new InvalidOperationException($"Expected exactly one Pass 18 helper: {name}");

    var cast = ExtractFunction(script, "Trig_Guardian_Force_Field_Cast_Actions").Text;
    Require(cast, "set wallCount=GuardianFF_GetWallCount(level)");
    Require(cast, "call GuardianFF_AddWall(wallPoint");
    if (cast.Contains("udg_Guardian_Integer", StringComparison.Ordinal))
        throw new InvalidOperationException("Guardian cast still depends on shared scratch-loop integers.");

    var loop = ExtractFunction(script, "Trig_Guardian_Force_Field_Loop_Actions").Text;
    Require(loop, "set level=udg_Guardian_FF_Level[udg_Guardian_FF_Loop]");
    Require(loop, "call GuardianFF_MoveInstanceEffects(lastIndex, udg_Guardian_FF_Loop)");
    Require(loop, "set udg_Guardian_FF_Point[lastIndex]=null");
    if (loop.Contains("udg_Guardian_FF_Level[udg_Guardian_FF_Index]", StringComparison.Ordinal))
        throw new InvalidOperationException("Guardian expiry still reads the newest instance level.");

    var damage = ExtractFunction(script, "Trig_Guardian_Force_Field_Damage_Actions").Text;
    Require(damage, "local integer level=udg_Guardian_FF_Level[udg_Guardian_FF_Loop]");
    Require(damage, "exitwhen sideSlot >= wallCount");

    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Trigger initializer count changed.");
    var names = Regex.Matches(script, @"(?m)^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes")
        .Select(match => match.Groups[1].Value).ToList();
    if (names.GroupBy(name => name, StringComparer.Ordinal).Any(group => group.Count() > 1))
        throw new InvalidOperationException("Duplicate function names found.");
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

static string RemoveFunction(string script, string name)
{
    var function = ExtractFunction(script, name);
    var end = function.Start + function.Length;
    while (end < script.Length && script[end] == '\n') end++;
    return script.Remove(function.Start, end - function.Start);
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
    b.AppendLine("# Guardian Force Field Pass 18");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Rebuilt AFOR around its object-data maximum of six levels.");
    b.AppendLine("- Implements the documented 1/1/3/3/5/5 wall pattern at cast and expiry.");
    b.AppendLine("- Uses the currently processed loop instance for level, caster, duration, damage, pathing, and effects.");
    b.AppendLine("- Compacts active instances without retaining duplicate hashtable effect handles.");
    b.AppendLine("- Removed eleven obsolete generated branch helpers while preserving all 499 trigger initializers.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
