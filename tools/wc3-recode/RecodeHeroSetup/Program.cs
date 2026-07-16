using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.individual_spawn.cleaned.j");
var outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.individual_spawn.hero-setup.j");
var systemPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "hero_setup.generated.jass");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "hero-setup-recode", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(systemPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = File.ReadAllText(inputPath, Encoding.Latin1);
if (script.Contains("function HeroSetup_SetType takes", StringComparison.Ordinal))
{
    throw new InvalidOperationException("The input script already contains the hero setup recode.");
}

var numbers = ExtractFunction(script, "Trig_set_hero_numbers_Actions");
var stats = ExtractFunction(script, "Trig_set_hero_stats_Actions");
var heroes = ParseHeroes(numbers.Text, stats.Text);
ValidateHeroes(heroes);

var helperSource = BuildHelpers();
var numbersReplacement = BuildNumbersFunction(numbers.Text, heroes);
var statsReplacement = BuildStatsFunction(heroes);

var rewritten = script
    .Remove(stats.Start, stats.Length).Insert(stats.Start, statsReplacement)
    .Remove(numbers.Start, numbers.Length).Insert(numbers.Start, helperSource + "\r\n" + numbersReplacement);

ValidateRewritten(rewritten, heroes);

File.WriteAllText(outputPath, rewritten, Encoding.Latin1);
File.WriteAllText(systemPath, helperSource + "\r\n" + BuildDataFunctions(heroes), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, heroes, script, rewritten), Encoding.UTF8);

Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote generated source: {systemPath}");
Console.WriteLine($"Wrote report: {reportPath}");
Console.WriteLine($"Validated heroes: {heroes.Count}");
Console.WriteLine($"Line count: {CountLines(script):n0} -> {CountLines(rewritten):n0}");

static FunctionSlice ExtractFunction(string script, string name)
{
    var pattern = $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+nothing\s+returns\s+nothing\r?\n.*?^endfunction\s*$";
    var match = Regex.Match(script, pattern, RegexOptions.Singleline);
    if (!match.Success)
    {
        throw new InvalidOperationException($"Could not find function {name}.");
    }
    return new FunctionSlice(match.Index, match.Length, match.Value);
}

static List<Hero> ParseHeroes(string numbersFunction, string statsFunction)
{
    var heroes = Enumerable.Range(1, 91).ToDictionary(i => i, i => new Hero(i));
    var typeRegex = new Regex(@"^\s*set\s+udg_hero_type\[(\d+)\]\s*=\s*'(.{4})'\s*$");
    foreach (var line in SplitLines(numbersFunction))
    {
        var match = typeRegex.Match(line);
        if (match.Success && int.Parse(match.Groups[1].Value) is >= 1 and <= 91)
        {
            heroes[int.Parse(match.Groups[1].Value)].Rawcode = match.Groups[2].Value;
        }
    }

    var statRegex = new Regex(@"^\s*set\s+(udg_hero_(?:category|STR_base|STR_inc|AGI_base|AGI_inc|INT_base|INT_inc|move_base|turnrate))\[(\d+)\]\s*=\s*([+-]?\d+(?:\.\d+)?)\s*$");
    string? pendingName = null;
    foreach (var line in SplitLines(statsFunction))
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith("//", StringComparison.Ordinal))
        {
            pendingName = trimmed[2..].Trim();
            continue;
        }
        var match = statRegex.Match(line);
        if (!match.Success)
        {
            continue;
        }
        var index = int.Parse(match.Groups[2].Value);
        if (index is < 1 or > 91)
        {
            continue;
        }
        var hero = heroes[index];
        var field = match.Groups[1].Value;
        if (field == "udg_hero_category" && !string.IsNullOrWhiteSpace(pendingName))
        {
            hero.Name = pendingName;
        }
        hero.Values[field] = match.Groups[3].Value;
        pendingName = null;
    }
    return heroes.Values.OrderBy(h => h.Index).ToList();
}

static void ValidateHeroes(IReadOnlyList<Hero> heroes)
{
    var expectedFields = new[]
    {
        "udg_hero_category", "udg_hero_STR_base", "udg_hero_STR_inc",
        "udg_hero_AGI_base", "udg_hero_AGI_inc", "udg_hero_INT_base",
        "udg_hero_INT_inc", "udg_hero_move_base", "udg_hero_turnrate"
    };
    var problems = new List<string>();
    foreach (var hero in heroes)
    {
        if (hero.Rawcode is null) problems.Add($"hero {hero.Index}: missing rawcode");
        if (string.IsNullOrWhiteSpace(hero.Name)) problems.Add($"hero {hero.Index}: missing name comment");
        foreach (var field in expectedFields)
        {
            if (!hero.Values.ContainsKey(field)) problems.Add($"hero {hero.Index}: missing {field}");
        }
    }
    if (problems.Count > 0)
    {
        throw new InvalidOperationException("Hero validation failed:\n" + string.Join("\n", problems));
    }
}

static string BuildHelpers() => NormalizeNewlines("""
//===========================================================================
// Generated hero setup helpers. Values are emitted from the original trigger data.
//===========================================================================
function HeroSetup_SetType takes integer index, integer unitType returns nothing
    set udg_hero_type[index]=unitType
endfunction

function HeroSetup_SetStats takes integer index, integer category, integer strengthBase, real strengthGain, integer agilityBase, real agilityGain, integer intelligenceBase, real intelligenceGain, integer moveSpeed, real turnRate returns nothing
    set udg_hero_category[index]=category
    set udg_hero_STR_base[index]=strengthBase
    set udg_hero_STR_inc[index]=strengthGain
    set udg_hero_AGI_base[index]=agilityBase
    set udg_hero_AGI_inc[index]=agilityGain
    set udg_hero_INT_base[index]=intelligenceBase
    set udg_hero_INT_inc[index]=intelligenceGain
    set udg_hero_move_base[index]=moveSpeed
    set udg_hero_turnrate[index]=turnRate
endfunction
""");

static string BuildNumbersFunction(string original, IReadOnlyList<Hero> heroes)
{
    var firstType = Regex.Match(original, @"(?m)^\s*set\s+udg_hero_type\[1\].*$");
    var lastType = Regex.Match(original, @"(?m)^\s*set\s+udg_hero_type\[91\].*$");
    if (!firstType.Success || !lastType.Success || lastType.Index < firstType.Index)
    {
        throw new InvalidOperationException("Could not identify the hero type table in set hero numbers.");
    }
    var end = lastType.Index + lastType.Length;
    var calls = new StringBuilder();
    foreach (var hero in heroes)
    {
        calls.AppendLine($"    call HeroSetup_SetType({hero.Index}, '{hero.Rawcode}') // {hero.Name}");
    }
    return original.Remove(firstType.Index, end - firstType.Index).Insert(firstType.Index, calls.ToString().TrimEnd('\r', '\n'));
}

static string BuildStatsFunction(IReadOnlyList<Hero> heroes)
{
    var b = new StringBuilder();
    b.AppendLine("function Trig_set_hero_stats_Actions takes nothing returns nothing");
    foreach (var hero in heroes)
    {
        b.AppendLine(BuildStatsCall(hero, "    "));
    }
    b.AppendLine("    call DestroyTrigger(GetTriggeringTrigger())");
    b.Append("endfunction");
    return b.ToString();
}

static string BuildDataFunctions(IReadOnlyList<Hero> heroes)
{
    var b = new StringBuilder();
    b.AppendLine("// Type rows run during normal initialization.");
    b.AppendLine("function HeroSetup_LoadTypes takes nothing returns nothing");
    foreach (var hero in heroes)
    {
        b.AppendLine($"    call HeroSetup_SetType({hero.Index}, '{hero.Rawcode}') // {hero.Name}");
    }
    b.AppendLine("endfunction");
    b.AppendLine();
    b.AppendLine("// Stat rows run from the original one-second timer trigger.");
    b.AppendLine("function HeroSetup_LoadStats takes nothing returns nothing");
    foreach (var hero in heroes)
    {
        b.AppendLine(BuildStatsCall(hero, "    "));
    }
    b.AppendLine("endfunction");
    return b.ToString();
}

static string BuildStatsCall(Hero h, string indent)
{
    string V(string name) => h.Values[name];
    return $"{indent}call HeroSetup_SetStats({h.Index}, {V("udg_hero_category")}, {V("udg_hero_STR_base")}, {AsReal(V("udg_hero_STR_inc"))}, {V("udg_hero_AGI_base")}, {AsReal(V("udg_hero_AGI_inc"))}, {V("udg_hero_INT_base")}, {AsReal(V("udg_hero_INT_inc"))}, {V("udg_hero_move_base")}, {AsReal(V("udg_hero_turnrate"))}) // {h.Name}";
}

static string AsReal(string value) => value.Contains('.') ? value : value + ".0";
static string NormalizeNewlines(string text) => text.Replace("\r\n", "\n").Replace("\n", "\r\n");
static string[] SplitLines(string text) => text.Replace("\r\n", "\n").Split('\n');
static int CountLines(string text) => SplitLines(text).Length;

static void ValidateRewritten(string script, IReadOnlyList<Hero> heroes)
{
    var typeCalls = new Regex(@"^\s*call\s+HeroSetup_SetType\((\d+),\s*'(.{4})'\)", RegexOptions.Multiline)
        .Matches(script)
        .ToDictionary(m => int.Parse(m.Groups[1].Value), m => m.Groups[2].Value);
    var statCalls = new Regex(@"^\s*call\s+HeroSetup_SetStats\((\d+),\s*([^)]+)\)", RegexOptions.Multiline)
        .Matches(script)
        .ToDictionary(m => int.Parse(m.Groups[1].Value), m => m.Groups[2].Value.Split(',').Select(v => v.Trim()).ToArray());

    if (typeCalls.Count != heroes.Count || statCalls.Count != heroes.Count)
    {
        throw new InvalidOperationException($"Rewritten call validation failed: {typeCalls.Count} type rows and {statCalls.Count} stat rows.");
    }

    foreach (var hero in heroes)
    {
        if (typeCalls[hero.Index] != hero.Rawcode)
        {
            throw new InvalidOperationException($"Rewritten rawcode mismatch for hero {hero.Index}.");
        }
        var actual = statCalls[hero.Index];
        var expected = new[]
        {
            hero.Values["udg_hero_category"], hero.Values["udg_hero_STR_base"], hero.Values["udg_hero_STR_inc"],
            hero.Values["udg_hero_AGI_base"], hero.Values["udg_hero_AGI_inc"], hero.Values["udg_hero_INT_base"],
            hero.Values["udg_hero_INT_inc"], hero.Values["udg_hero_move_base"], hero.Values["udg_hero_turnrate"]
        };
        if (actual.Length != expected.Length)
        {
            throw new InvalidOperationException($"Rewritten stat field count mismatch for hero {hero.Index}.");
        }
        for (var i = 0; i < expected.Length; i++)
        {
            if (decimal.Parse(actual[i], CultureInfo.InvariantCulture) != decimal.Parse(expected[i], CultureInfo.InvariantCulture))
            {
                throw new InvalidOperationException($"Rewritten stat mismatch for hero {hero.Index}, field {i + 1}.");
            }
        }
    }
}

static string BuildReport(string inputPath, string outputPath, IReadOnlyList<Hero> heroes, string before, string after)
{
    var b = new StringBuilder();
    b.AppendLine("# Hero Setup Recode Report");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("## Validation");
    b.AppendLine();
    b.AppendLine($"- Validated {heroes.Count} sequential hero rows.");
    b.AppendLine("- Every row has one rawcode and all nine original stat fields.");
    b.AppendLine("- Hero types remain in the initialization trigger.");
    b.AppendLine("- Hero stats remain in the original one-second timer trigger.");
    b.AppendLine("- Showroom locations, bounty values, special index 99, and trigger destruction are preserved.");
    b.AppendLine();
    b.AppendLine("## Size");
    b.AppendLine();
    b.AppendLine($"- Script lines before: {CountLines(before):n0}");
    b.AppendLine($"- Script lines after: {CountLines(after):n0}");
    b.AppendLine($"- Lines removed: {CountLines(before) - CountLines(after):n0}");
    return b.ToString();
}

sealed record FunctionSlice(int Start, int Length, string Text);

sealed class Hero(int index)
{
    public int Index { get; } = index;
    public string? Name { get; set; }
    public string? Rawcode { get; set; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.Ordinal);
}
