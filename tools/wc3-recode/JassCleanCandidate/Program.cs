using System.Text;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "extracted", "799W-tester", "files", "war3map.j");
var outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.cleaned.candidate.j");
var reportPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "jass-clean", "clean_candidate_report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

var lines = File.ReadAllLines(inputPath, Encoding.Latin1);
var doNothingRegex = new Regex(@"^\s*call\s+DoNothing\s*\(\s*\)\s*$", RegexOptions.Compiled);
var kept = new List<string>(lines.Length);
var removed = new List<RemovedLine>();

for (var i = 0; i < lines.Length; i++)
{
    if (doNothingRegex.IsMatch(lines[i]))
    {
        removed.Add(new RemovedLine(i + 1, lines[i]));
        continue;
    }

    kept.Add(lines[i]);
}

File.WriteAllText(outputPath, string.Join("\r\n", kept) + "\r\n", Encoding.Latin1);
File.WriteAllText(reportPath, BuildReport(inputPath, outputPath, lines.Length, kept.Count, removed), Encoding.UTF8);

Console.WriteLine($"Wrote cleaned candidate: {outputPath}");
Console.WriteLine($"Wrote report:            {reportPath}");
Console.WriteLine($"Removed DoNothing calls: {removed.Count:n0}");
Console.WriteLine($"Line count:              {lines.Length:n0} -> {kept.Count:n0}");

static string BuildReport(string inputPath, string outputPath, int inputLineCount, int outputLineCount, IReadOnlyList<RemovedLine> removed)
{
    var b = new StringBuilder();
    b.AppendLine("# JASS Clean Candidate Report");
    b.AppendLine();
    b.AppendLine($"Input: `{inputPath}`");
    b.AppendLine($"Output: `{outputPath}`");
    b.AppendLine();
    b.AppendLine("## Safe Mechanical Cleanup");
    b.AppendLine();
    b.AppendLine("- Removed lines whose whole statement was `call DoNothing()`.");
    b.AppendLine("- Did not remove debug text, trigger bodies, globals, dormant triggers, comments, or object data.");
    b.AppendLine("- This is intended as the first behavior-preserving cleanup pass before larger recodes.");
    b.AppendLine();
    b.AppendLine("## Counts");
    b.AppendLine();
    b.AppendLine($"- Original lines: {inputLineCount:n0}");
    b.AppendLine($"- Cleaned lines: {outputLineCount:n0}");
    b.AppendLine($"- Removed no-op statements: {removed.Count:n0}");
    b.AppendLine();
    b.AppendLine("## Removed Line Sample");
    b.AppendLine();
    foreach (var line in removed.Take(80))
    {
        b.AppendLine($"- line {line.LineNumber}: `{line.Text.Trim()}`");
    }
    return b.ToString();
}

public sealed record RemovedLine(int LineNumber, string Text);
