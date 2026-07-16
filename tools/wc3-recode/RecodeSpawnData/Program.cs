using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.individual_spawn.hero-setup.j");
var outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.recode-pass-3.j");
var systemPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "spawn_unit_data.generated.jass");
var reportPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "build", "spawn-data-recode", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(systemPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var script = File.ReadAllText(inputPath, Encoding.Latin1);
if (script.Contains("function SpawnData_Set takes", StringComparison.Ordinal))
{
    throw new InvalidOperationException("The input script already contains the spawn data recode.");
}

var function = ExtractFunction(script, "Trig_set_spawn_variables_Actions");
var rows = ParseRows(function.Text);
ValidateRows(rows);
var helpers = BuildHelpers();
var replacement = BuildTrigger(rows);
var rewritten = script.Remove(function.Start, function.Length).Insert(function.Start, helpers + "\r\n" + replacement);
ValidateRewritten(rewritten, rows);

File.WriteAllText(outputPath, rewritten, Encoding.Latin1);
File.WriteAllText(systemPath, helpers + "\r\n" + BuildLoadFunction(rows), Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, rows, script, rewritten), Encoding.UTF8);

Console.WriteLine($"Wrote recoded script: {outputPath}");
Console.WriteLine($"Wrote generated source: {systemPath}");
Console.WriteLine($"Wrote report: {reportPath}");
Console.WriteLine($"Validated spawn rows: {rows.Count}");
Console.WriteLine($"Line count: {CountLines(script):n0} -> {CountLines(rewritten):n0}");

static FunctionSlice ExtractFunction(string script, string name)
{
    var match = Regex.Match(script, $@"(?m)^function\s+{Regex.Escape(name)}\s+takes\s+nothing\s+returns\s+nothing\r?\n.*?^endfunction\s*$", RegexOptions.Singleline);
    if (!match.Success) throw new InvalidOperationException($"Could not find function {name}.");
    return new FunctionSlice(match.Index, match.Length, match.Value);
}

static List<SpawnRow> ParseRows(string text)
{
    var rows = Enumerable.Range(0, 51).ToDictionary(i => i, i => new SpawnRow(i));
    var assignment = new Regex(@"^\s*set\s+(udg_(?:spawn_unit|unit_(?:HP|armor_type|armor|att_base|att_num_dice|att_dice_sides|att_CD|bounty_base|bounty_num_dice|bounty_dice_sides|range|level)))\[(\d+)\]\s*=\s*(?:'(.{4})'|([+-]?\d+(?:\.\d+)?))\s*$");
    string? pendingName = null;
    foreach (var line in SplitLines(text))
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith("//", StringComparison.Ordinal))
        {
            pendingName = trimmed[2..].Trim();
            continue;
        }
        var match = assignment.Match(line);
        if (!match.Success) continue;
        var index = int.Parse(match.Groups[2].Value);
        if (index is < 0 or > 50) continue;
        var row = rows[index];
        var field = match.Groups[1].Value;
        if (field == "udg_spawn_unit" && !string.IsNullOrWhiteSpace(pendingName)) row.Name = pendingName;
        row.Values[field] = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value;
        pendingName = null;
    }
    return rows.Values.OrderBy(r => r.Index).ToList();
}

static string[] SpawnFields() =>
[
    "udg_spawn_unit", "udg_unit_HP", "udg_unit_armor_type", "udg_unit_armor",
    "udg_unit_att_base", "udg_unit_att_num_dice", "udg_unit_att_dice_sides", "udg_unit_att_CD",
    "udg_unit_bounty_base", "udg_unit_bounty_num_dice", "udg_unit_bounty_dice_sides",
    "udg_unit_range", "udg_unit_level"
];

static void ValidateRows(IReadOnlyList<SpawnRow> rows)
{
    var problems = new List<string>();
    foreach (var row in rows)
    {
        if (string.IsNullOrWhiteSpace(row.Name)) problems.Add($"row {row.Index}: missing name");
        foreach (var field in SpawnFields())
        {
            if (!row.Values.ContainsKey(field)) problems.Add($"row {row.Index}: missing {field}");
        }
    }
    if (problems.Count > 0) throw new InvalidOperationException("Spawn data validation failed:\n" + string.Join("\n", problems));
}

static string BuildHelpers() => NormalizeNewlines("""
//===========================================================================
// Generated spawn unit data helper. Values are emitted from the original trigger data.
//===========================================================================
function SpawnData_Set takes integer index, integer unitType, integer hitPoints, integer armorType, real armor, integer attackBase, integer attackDice, integer attackSides, real attackCooldown, integer bountyBase, integer bountyDice, integer bountySides, real attackRange, integer unitLevel returns nothing
    set udg_spawn_unit[index]=unitType
    set udg_unit_HP[index]=hitPoints
    set udg_unit_armor_type[index]=armorType
    set udg_unit_armor[index]=armor
    set udg_unit_att_base[index]=attackBase
    set udg_unit_att_num_dice[index]=attackDice
    set udg_unit_att_dice_sides[index]=attackSides
    set udg_unit_att_CD[index]=attackCooldown
    set udg_unit_bounty_base[index]=bountyBase
    set udg_unit_bounty_num_dice[index]=bountyDice
    set udg_unit_bounty_dice_sides[index]=bountySides
    set udg_unit_range[index]=attackRange
    set udg_unit_level[index]=unitLevel
endfunction
""");

static string BuildTrigger(IReadOnlyList<SpawnRow> rows)
{
    var b = new StringBuilder();
    b.AppendLine("function Trig_set_spawn_variables_Actions takes nothing returns nothing");
    foreach (var row in rows) b.AppendLine(BuildCall(row));
    b.AppendLine("    call DestroyTrigger(GetTriggeringTrigger())");
    b.Append("endfunction");
    return b.ToString();
}

static string BuildLoadFunction(IReadOnlyList<SpawnRow> rows)
{
    var b = new StringBuilder();
    b.AppendLine("function SpawnData_Load takes nothing returns nothing");
    foreach (var row in rows) b.AppendLine(BuildCall(row));
    b.AppendLine("endfunction");
    return b.ToString();
}

static string BuildCall(SpawnRow row)
{
    string V(string field) => row.Values[field];
    return $"    call SpawnData_Set({row.Index}, '{V("udg_spawn_unit")}', {V("udg_unit_HP")}, {V("udg_unit_armor_type")}, {AsReal(V("udg_unit_armor"))}, {V("udg_unit_att_base")}, {V("udg_unit_att_num_dice")}, {V("udg_unit_att_dice_sides")}, {AsReal(V("udg_unit_att_CD"))}, {V("udg_unit_bounty_base")}, {V("udg_unit_bounty_num_dice")}, {V("udg_unit_bounty_dice_sides")}, {AsReal(V("udg_unit_range"))}, {V("udg_unit_level")}) // {row.Name}";
}

static void ValidateRewritten(string script, IReadOnlyList<SpawnRow> rows)
{
    var matches = new Regex(@"^\s*call\s+SpawnData_Set\((\d+),\s*'(.{4})',\s*([^)]+)\)", RegexOptions.Multiline).Matches(script);
    if (matches.Count != rows.Count) throw new InvalidOperationException($"Rewritten row validation found {matches.Count} calls, expected {rows.Count}.");
    foreach (Match match in matches)
    {
        var row = rows[int.Parse(match.Groups[1].Value)];
        if (match.Groups[2].Value != row.Values["udg_spawn_unit"]) throw new InvalidOperationException($"Rawcode mismatch at spawn row {row.Index}.");
        var actual = match.Groups[3].Value.Split(',').Select(v => v.Trim()).ToArray();
        var numericFields = SpawnFields().Skip(1).ToArray();
        if (actual.Length != numericFields.Length) throw new InvalidOperationException($"Field count mismatch at spawn row {row.Index}.");
        for (var i = 0; i < actual.Length; i++)
        {
            if (decimal.Parse(actual[i], CultureInfo.InvariantCulture) != decimal.Parse(row.Values[numericFields[i]], CultureInfo.InvariantCulture))
                throw new InvalidOperationException($"Value mismatch at spawn row {row.Index}, field {numericFields[i]}.");
        }
    }
}

static string BuildReport(string inputPath, string outputPath, IReadOnlyList<SpawnRow> rows, string before, string after)
{
    var b = new StringBuilder();
    b.AppendLine("# Spawn Unit Data Recode Report");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("## Validation");
    b.AppendLine();
    b.AppendLine($"- Validated {rows.Count} sequential spawn-unit rows (indices 0-50).");
    b.AppendLine("- Every row has one rawcode and all twelve numeric stat fields.");
    b.AppendLine("- The original initialization trigger and trigger-destruction timing are preserved.");
    b.AppendLine("- Spawn rates are not part of this table and remain individually configurable.");
    b.AppendLine();
    b.AppendLine("## Size");
    b.AppendLine();
    b.AppendLine($"- Script lines before: {CountLines(before):n0}");
    b.AppendLine($"- Script lines after: {CountLines(after):n0}");
    b.AppendLine($"- Lines removed: {CountLines(before) - CountLines(after):n0}");
    return b.ToString();
}

static string AsReal(string value) => value.Contains('.') ? value : value + ".0";
static string NormalizeNewlines(string text) => text.Replace("\r\n", "\n").Replace("\n", "\r\n");
static string[] SplitLines(string text) => text.Replace("\r\n", "\n").Split('\n');
static int CountLines(string text) => SplitLines(text).Length;

sealed record FunctionSlice(int Start, int Length, string Text);

sealed class SpawnRow(int index)
{
    public int Index { get; } = index;
    public string? Name { get; set; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.Ordinal);
}
