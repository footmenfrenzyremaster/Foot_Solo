using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-12.j");
var sourcePath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "runtime_location_cleanup_pass_13.jass");
var outputPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-13.j");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "runtime-cleanup-pass-13", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = ToLf(File.ReadAllText(inputPath, Encoding.Latin1));
var source = ToLf(File.ReadAllText(sourcePath, Encoding.Latin1));
ValidateOriginal(script);

script = ReplaceFunction(script, "Trig_keep_footy_in_base_Actions",
    ExtractRange(source, "Runtime_MoveEnteringUnit", "Trig_keep_footy_in_base_Actions"));

foreach (var name in new[]
{
    "Trig_Mass_START_Func002Func014Func004A",
    "Trig_Mass_START_Func002Func030Func003A",
    "Trig_block_from_shop_t1_Actions",
    "Trig_block_from_shop_t2_Actions",
    "Trig_block_from_shop_t3_Actions",
    "Trig_block_from_shop_t4_Actions",
    "Trig_kill_blink_shop_t1_Actions",
    "Trig_kill_blink_shop_t2_Actions",
    "Trig_kill_blink_shop_t3_Actions",
    "Trig_kill_blink_shop_t4_Actions",
    "Trig_phoenix_block_Actions"
})
{
    script = ReplaceFunction(script, name, ExtractFunction(source, name).Text);
}

script = PatchFunction(script, "Trig_Mass_START_Actions", function =>
{
    function = ReplaceOnce(function,
        "function Trig_Mass_START_Actions takes nothing returns nothing\n",
        "function Trig_Mass_START_Actions takes nothing returns nothing\n    local group footmen=null\n");
    function = ReplaceOnce(function,
        "            call ForGroupBJ(GetUnitsOfPlayerAndTypeId(ConvertedPlayer(udg_X), 'hfoo'), function Trig_Mass_START_Func002Func030Func003A)\n",
        "            set footmen=GetUnitsOfPlayerAndTypeId(ConvertedPlayer(udg_X), 'hfoo')\n            call ForGroupBJ(footmen, function Trig_Mass_START_Func002Func030Func003A)\n            call DestroyGroup(footmen)\n");
    function = ReplaceOnce(function,
        "            call ForGroupBJ(GetUnitsOfPlayerAndTypeId(ConvertedPlayer(udg_X), 'hfoo'), function Trig_Mass_START_Func002Func014Func004A)\n",
        "            set footmen=GetUnitsOfPlayerAndTypeId(ConvertedPlayer(udg_X), 'hfoo')\n            call ForGroupBJ(footmen, function Trig_Mass_START_Func002Func014Func004A)\n            call DestroyGroup(footmen)\n");
    function = ReplaceExactCount(function, "        call DestroyGroup(udg_temp_unitgroup)\n", "", 2);
    function = ReplaceOnce(function, "    endif\nendfunction", "    endif\n    set footmen=null\nendfunction");
    return function;
});

ValidateReplacement(script);
File.WriteAllText(outputPath, ToCrlf(script), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static void ValidateOriginal(string script)
{
    var keep = ExtractFunction(script, "Trig_keep_footy_in_base_Actions").Text;
    if (Regex.Matches(keep, @"GetUnitLoc\(").Count != 2 || Regex.Matches(keep, @"RemoveLocation\(").Count != 1)
        throw new InvalidOperationException("keep_footy_in_base no longer matches the reviewed two-point leak shape.");

    foreach (var name in new[] { "t1", "t2", "t3", "t4" })
    {
        var block = ExtractFunction(script, "Trig_block_from_shop_" + name + "_Actions").Text;
        if (Regex.Matches(block, @"OffsetLocation\(GetUnitLoc\(").Count != 1 || Regex.Matches(block, @"RemoveLocation\(").Count != 1)
            throw new InvalidOperationException($"block_from_shop_{name} no longer matches the reviewed nested-point leak shape.");

        var kill = ExtractFunction(script, "Trig_kill_blink_shop_" + name + "_Actions").Text;
        if (Regex.Matches(kill, @"GetUnitLoc\(").Count != 1 || Regex.Matches(kill, @"GetRectCenter\(").Count != 1 || Regex.Matches(kill, @"RemoveLocation\(").Count != 1)
            throw new InvalidOperationException($"kill_blink_shop_{name} no longer matches the reviewed source/center leak shape.");
    }

    var phoenix = ExtractFunction(script, "Trig_phoenix_block_Actions").Text;
    if (Regex.Matches(phoenix, @"OffsetLocation\(GetUnitLoc\(").Count != 1 || Regex.Matches(phoenix, @"RemoveLocation\(").Count != 1)
        throw new InvalidOperationException("phoenix_block no longer matches the reviewed nested-point leak shape.");

    foreach (var name in new[] { "Trig_Mass_START_Func002Func014Func004A", "Trig_Mass_START_Func002Func030Func003A" })
    {
        if (Regex.Matches(ExtractFunction(script, name).Text, @"GetRectCenter\(GetPlayableMapRect\(\)\)").Count != 1)
            throw new InvalidOperationException($"{name} no longer matches the reviewed mass-order point leak.");
    }

    var mass = ExtractFunction(script, "Trig_Mass_START_Actions").Text;
    if (Regex.Matches(mass, @"ForGroupBJ\(GetUnitsOfPlayerAndTypeId").Count != 2 || Regex.Matches(mass, @"DestroyGroup\(udg_temp_unitgroup\)").Count != 2)
        throw new InvalidOperationException("Mass START no longer matches the reviewed two leaked-enumeration-group shape.");
    if (Regex.Matches(script, @"(?m)^function\s+InitTrig_").Count != 499)
        throw new InvalidOperationException("Expected all 499 trigger initializer entry points in pass 12.");
}

static void ValidateReplacement(string script)
{
    var required = new[]
    {
        "function Runtime_MoveEnteringUnit takes real offsetX, real offsetY returns nothing",
        "function Runtime_KillBlinkShopUnit takes string warning returns nothing",
        "call Runtime_MoveEnteringUnit(50.00, -50.00)",
        "call Runtime_MoveEnteringUnit(-50.00, -50.00)",
        "call Runtime_MoveEnteringUnit(-50.00, 50.00)",
        "call Runtime_MoveEnteringUnit(50.00, 50.00)",
        "call Runtime_MoveEnteringUnit(30.00, 0.00)",
        "call Runtime_KillBlinkShopUnit(\"TRIGSTR_1109\")",
        "call Runtime_KillBlinkShopUnit(\"TRIGSTR_2332\")",
        "call Runtime_KillBlinkShopUnit(\"TRIGSTR_4019\")",
        "call Runtime_KillBlinkShopUnit(\"TRIGSTR_4495\")",
        "local group footmen=null",
        "call IssuePointOrder(GetEnumUnit(), \"attack\"",
        "call DestroyGroup(footmen)"
    };
    foreach (var marker in required)
    {
        if (!script.Contains(marker, StringComparison.Ordinal))
            throw new InvalidOperationException($"Pass-13 replacement marker missing: {marker}");
    }

    var reviewed = new[]
    {
        "Trig_keep_footy_in_base_Actions",
        "Trig_Mass_START_Func002Func014Func004A",
        "Trig_Mass_START_Func002Func030Func003A",
        "Trig_block_from_shop_t1_Actions",
        "Trig_block_from_shop_t2_Actions",
        "Trig_block_from_shop_t3_Actions",
        "Trig_block_from_shop_t4_Actions",
        "Trig_kill_blink_shop_t1_Actions",
        "Trig_kill_blink_shop_t2_Actions",
        "Trig_kill_blink_shop_t3_Actions",
        "Trig_kill_blink_shop_t4_Actions",
        "Trig_phoenix_block_Actions"
    };
    foreach (var name in reviewed)
    {
        var function = ExtractFunction(script, name).Text;
        if (function.Contains("GetUnitLoc(", StringComparison.Ordinal) ||
            function.Contains("OffsetLocation(", StringComparison.Ordinal) ||
            function.Contains("GetRectCenter(", StringComparison.Ordinal))
            throw new InvalidOperationException($"Reviewed location allocation remains in {name}.");
    }

    var mass = ExtractFunction(script, "Trig_Mass_START_Actions").Text;
    if (Regex.Matches(mass, @"set footmen=GetUnitsOfPlayerAndTypeId").Count != 2 ||
        Regex.Matches(mass, @"DestroyGroup\(footmen\)").Count != 2 ||
        mass.Contains("ForGroupBJ(GetUnitsOfPlayerAndTypeId", StringComparison.Ordinal) ||
        mass.Contains("DestroyGroup(udg_temp_unitgroup)", StringComparison.Ordinal))
        throw new InvalidOperationException("Mass START temporary group ownership was not repaired exactly twice.");

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

static string ExtractRange(string text, string firstFunction, string lastFunction)
{
    var start = ExtractFunction(text, firstFunction).Start;
    var end = ExtractFunction(text, lastFunction).End;
    return text[start..end];
}

static string ReplaceRange(string text, int start, int end, string replacement) =>
    text.Remove(start, end - start).Insert(start, ToLf(replacement).TrimEnd('\n'));

static string ReplaceOnce(string text, string oldValue, string newValue) => ReplaceExactCount(text, oldValue, newValue, 1);

static string ReplaceExactCount(string text, string oldValue, string newValue, int expected)
{
    var count = Regex.Matches(text, Regex.Escape(oldValue)).Count;
    if (count != expected)
        throw new InvalidOperationException($"Expected {expected} exact replacements, found {count}: {oldValue.Trim()}");
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
    b.AppendLine("# Runtime Location Cleanup Pass 13");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Replaced boundary and Phoenix nested point allocations with direct coordinate movement.");
    b.AppendLine("- Replaced four shop blink source/center/destination point chains with one coordinate helper.");
    b.AppendLine("- Preserved all four movement directions, kill order, Ankh removal, and warning strings.");
    b.AppendLine("- Replaced two mass autosend center-point allocations with coordinate orders.");
    b.AppendLine("- Gave both Mass START footman enumerations explicit local-group ownership and cleanup.");
    b.AppendLine("- Preserved all trigger events, conditions, and 499 trigger initializer entry points.");
    var lineCount = script.Split('\n').Length - (script.EndsWith('\n') ? 1 : 0);
    b.AppendLine($"- Output script lines: {lineCount:n0}.");
    b.AppendLine($"- Output functions: {Regex.Matches(script, @"(?m)^function\s+").Count:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text)
{
    public int End => Start + Length;
}
