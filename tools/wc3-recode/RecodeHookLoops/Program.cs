using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-6.j");
var outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-7.j");
var reportPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "hook-loop-pass-7", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var script = File.ReadAllText(inputPath, Encoding.Latin1);
script = RewriteVariant(script, "_Copy");
script = RewriteVariant(script, "");
script = script.Replace("\r\n", "\n").Replace("\n", "\r\n");
Validate(script);
File.WriteAllText(outputPath, script, Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, script), Encoding.UTF8);
Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static string RewriteVariant(string script, string suffix)
{
    var throwName = $"Trig_HERO_Hook_Throw{suffix}_Actions";
    var loopName = $"Trig_HERO_Hook_Loop{suffix}_Actions";
    var throwSlice = ExtractFunction(script, throwName);
    var throwText = throwSlice.Text;
    throwText = ReplaceOnce(throwText, $"udg_hook_degree{suffix}[udg_X])", $"udg_hook_degree{suffix}[udg_temp_playernumber])", $"{throwName} facing index");
    throwText = ReplaceOnce(throwText,
        $"    set udg_hook_location{suffix}[udg_temp_playernumber]=udg_temp_point_caster",
        $"    set udg_hook_location{suffix}[udg_temp_playernumber]=Location(GetLocationX(udg_temp_point_caster), GetLocationY(udg_temp_point_caster))",
        $"{throwName} independent location");
    throwText = ReplaceOnce(throwText, "    call RemoveLocation(udg_temp_point_caster)\r\n", "", $"{throwName} persistent caster location");
    script = script.Remove(throwSlice.Start, throwSlice.Length).Insert(throwSlice.Start, throwText);

    var loopSlice = ExtractFunction(script, loopName);
    var loopText = loopSlice.Text;
    loopText = ReplaceOnce(loopText,
        $"function {loopName} takes nothing returns nothing\r\n",
        $"function {loopName} takes nothing returns nothing\r\n    local location nextLocation\r\n    local group randomGroup\r\n    local boolean anyActive=false\r\n",
        $"{loopName} locals");
    loopText = ReplaceOnce(loopText,
        $"        if ( Trig_HERO_Hook_Loop{suffix}_Func001Func001C() ) then",
        $"        if ( Trig_HERO_Hook_Loop{suffix}_Func001Func001C() ) and udg_dummy_hook{suffix}[udg_X] != null then",
        $"{loopName} active guard");
    loopText = ReplaceOnce(loopText,
        $"            set udg_hook_start_location{suffix}[udg_X]=GetUnitLoc(udg_herO[udg_X])",
        $"            call RemoveLocation(udg_hook_start_location{suffix}[udg_X])\r\n            set udg_hook_start_location{suffix}[udg_X]=GetUnitLoc(udg_herO[udg_X])",
        $"{loopName} caster location cleanup");

    var locationPattern = $@"(?m)^(\s*)set udg_hook_location{Regex.Escape(suffix)}\[udg_X\]=(PolarProjectionBJ\([^\r\n]+\))\s*$";
    var locationMatches = Regex.Matches(loopText, locationPattern);
    if (locationMatches.Count != 3) throw new InvalidOperationException($"{loopName}: expected three location projections, found {locationMatches.Count}.");
    loopText = Regex.Replace(loopText, locationPattern, m =>
        $"{m.Groups[1].Value}set nextLocation={m.Groups[2].Value}\r\n" +
        $"{m.Groups[1].Value}call RemoveLocation(udg_hook_location{suffix}[udg_X])\r\n" +
        $"{m.Groups[1].Value}set udg_hook_location{suffix}[udg_X]=nextLocation\r\n" +
        $"{m.Groups[1].Value}set nextLocation=null");

    var randomLinePattern = $@"(?m)^(\s*)call ForGroupBJ\(GetRandomSubGroup\(1, udg_hook_target_group{Regex.Escape(suffix)}\[udg_X\]\), function ([A-Za-z0-9_]+)\)\s*$";
    var randomMatches = Regex.Matches(loopText, randomLinePattern);
    if (randomMatches.Count != 1) throw new InvalidOperationException($"{loopName}: expected one random subgroup call.");
    loopText = Regex.Replace(loopText, randomLinePattern, m =>
        $"{m.Groups[1].Value}set randomGroup=GetRandomSubGroup(1, udg_hook_target_group{suffix}[udg_X])\r\n" +
        $"{m.Groups[1].Value}call ForGroupBJ(randomGroup, function {m.Groups[2].Value})\r\n" +
        $"{m.Groups[1].Value}call DestroyGroup(randomGroup)\r\n" +
        $"{m.Groups[1].Value}set randomGroup=null");

    if (suffix == "_Copy")
    {
        loopText = ReplaceOnce(loopText,
            "                    call DestroyGroup(udg_mjolnir_target[udg_X])",
            "                    call DestroyGroup(udg_hook_target_group_Copy[udg_X])",
            "copy hook target group bug");
    }

    loopText = Regex.Replace(loopText,
        $@"(?m)^(\s*)call UnitApplyTimedLifeBJ\(0\.10, 'BTLF', udg_dummy_hook{Regex.Escape(suffix)}\[udg_X\]\)\s*$",
        m => $"{m.Groups[1].Value}call UnitApplyTimedLifeBJ(0.10, 'BTLF', udg_dummy_hook{suffix}[udg_X])\r\n{m.Groups[1].Value}set udg_dummy_hook{suffix}[udg_X]=null");
    loopText = Regex.Replace(loopText,
        $@"(?m)^(\s*)call RemoveLocation\(udg_hook_(location|start_location){Regex.Escape(suffix)}\[udg_X\]\)\s*$",
        m => $"{m.Groups[1].Value}call RemoveLocation(udg_hook_{m.Groups[2].Value}{suffix}[udg_X])\r\n{m.Groups[1].Value}set udg_hook_{m.Groups[2].Value}{suffix}[udg_X]=null");

    loopText = Regex.Replace(loopText, @"(?m)^\s*call DisableTrigger\(GetTriggeringTrigger\(\)\)\r?\n", "");
    if (suffix.Length == 0)
    {
        loopText = ReplaceOnce(loopText,
            "                if ( Trig_HERO_Hook_Loop_Func001Func001Func004Func008C() ) then",
            "                if udg_dummy_hook[udg_X] != null and ( Trig_HERO_Hook_Loop_Func001Func001Func004Func008C() ) then",
            "main hook duplicate finish guard");
        loopText = ReplaceOnce(loopText,
            "                if ( Trig_HERO_Hook_Loop_Func001Func001Func004Func009C() ) then",
            "                if udg_dummy_hook[udg_X] != null and ( Trig_HERO_Hook_Loop_Func001Func001Func004Func009C() ) then",
            "main hook timer-context guard");
    }

    loopText = ReplaceOnce(loopText,
        "        set udg_X=udg_X + 1\r\n",
        $"        if udg_dummy_hook{suffix}[udg_X] != null and ( udg_hook_send_active{suffix}[udg_X] or udg_hook_return_active{suffix}[udg_X] ) then\r\n            set anyActive=true\r\n        endif\r\n        set udg_X=udg_X + 1\r\n",
        $"{loopName} activity scan");
    loopText = ReplaceOnce(loopText,
        "    endloop\r\nendfunction",
        "    endloop\r\n    if not anyActive then\r\n        call DisableTrigger(GetTriggeringTrigger())\r\n    endif\r\n    set randomGroup=null\r\n    set nextLocation=null\r\nendfunction",
        $"{loopName} final disable");

    script = script.Remove(loopSlice.Start, loopSlice.Length).Insert(loopSlice.Start, loopText);
    return script;
}

static FunctionSlice ExtractFunction(string text, string name)
{
    var match = Regex.Match(text, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+.*?\s+returns\s+.*?\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value.Replace("\r\n", "\n").Replace("\n", "\r\n"));
}

static string ReplaceOnce(string text, string oldValue, string newValue, string label)
{
    var count = Regex.Matches(text, Regex.Escape(oldValue)).Count;
    if (count != 1) throw new InvalidOperationException($"{label}: expected one match, found {count}.");
    return text.Replace(oldValue, newValue, StringComparison.Ordinal);
}

static void Validate(string script)
{
    foreach (var suffix in new[] { "", "_Copy" })
    {
        var loop = ExtractFunction(script, $"Trig_HERO_Hook_Loop{suffix}_Actions").Text;
        if (Regex.Matches(loop, "DisableTrigger\\(GetTriggeringTrigger\\(\\)\\)").Count != 1)
            throw new InvalidOperationException($"Hook{suffix} should disable only once after scanning players.");
        if (loop.Contains($"set udg_hook_location{suffix}[udg_X]=PolarProjectionBJ", StringComparison.Ordinal))
            throw new InvalidOperationException($"Hook{suffix} still overwrites projected locations directly.");
        if (!loop.Contains("call DestroyGroup(randomGroup)", StringComparison.Ordinal))
            throw new InvalidOperationException($"Hook{suffix} random subgroup cleanup is missing.");
    }
    var copyLoop = ExtractFunction(script, "Trig_HERO_Hook_Loop_Copy_Actions").Text;
    if (copyLoop.Contains("call DestroyGroup(udg_mjolnir_target[udg_X])", StringComparison.Ordinal))
        throw new InvalidOperationException("Copied hook still destroys the Mjolnir group.");
    var mainThrow = ExtractFunction(script, "Trig_HERO_Hook_Throw_Actions").Text;
    var copyThrow = ExtractFunction(script, "Trig_HERO_Hook_Throw_Copy_Actions").Text;
    if (mainThrow.Contains("udg_hook_degree[udg_X])", StringComparison.Ordinal) || copyThrow.Contains("udg_hook_degree_Copy[udg_X])", StringComparison.Ordinal))
        throw new InvalidOperationException("Hook throw still uses the shared loop index for facing.");
}

static string BuildReport(string inputPath, string outputPath, string script)
{
    var b = new StringBuilder();
    b.AppendLine("# Hook Loop Pass 7");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("- Applied the same lifecycle cleanup to both hook variants.");
    b.AppendLine("- Corrected hook facing to use the casting player's index.");
    b.AppendLine("- Preserves and separately owns caster and hook locations.");
    b.AppendLine("- Removes old per-tick locations before storing replacements.");
    b.AppendLine("- Destroys temporary random subgroups.");
    b.AppendLine("- Copied hook now destroys its own target group, not the Mjolnir group.");
    b.AppendLine("- Shared loop triggers disable only after all players are inactive.");
    b.AppendLine($"- Output lines: {script.Replace("\r\n", "\n").Split('\n').Length:n0}.");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);
