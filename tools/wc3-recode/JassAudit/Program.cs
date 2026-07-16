using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var scriptPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "extracted", "799W-tester", "files", "war3map.j");
var outDir = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "jass-audit");
Directory.CreateDirectory(outDir);

var lines = File.ReadAllLines(scriptPath, Encoding.Latin1);
var functions = ParseFunctions(lines);
var functionByName = functions.ToDictionary(f => f.Name, StringComparer.Ordinal);
var globals = ParseGlobals(lines);
var triggers = functions
    .Where(f => f.Name.StartsWith("InitTrig_", StringComparison.Ordinal))
    .Select(f => AnalyzeTrigger(f, lines))
    .OrderBy(t => t.Name, StringComparer.Ordinal)
    .ToList();

foreach (var function in functions)
{
    AnalyzeFunctionReferences(function, functionByName);
}

var roots = new HashSet<string>(StringComparer.Ordinal)
{
    "main",
    "config",
    "InitGlobals",
    "InitSounds",
    "CreateAllUnits",
    "CreateRegions",
    "CreateCameras",
    "InitCustomTriggers",
    "RunInitializationTriggers",
    "InitUpgrades",
    "InitAllyPriorities",
    "InitCustomTeams",
    "InitCustomPlayerSlots",
};

foreach (var f in functions)
{
    if (f.ExternalFunctionRefs.Count > 0)
    {
        roots.Add(f.Name);
    }
}

var reachable = ComputeReachable(roots, functionByName);
foreach (var f in functions)
{
    f.Reachable = reachable.Contains(f.Name);
}

ApplyTriggerUsage(functions, triggers);

var leakFindings = AnalyzeLeaks(functions, lines);
var duplicateInitBodies = triggers
    .GroupBy(t => NormalizeBody(t.Body))
    .Where(g => g.Count() > 1 && g.Key.Length > 20)
    .Select(g => new DuplicateTriggerGroup(g.Select(t => t.Name).ToList(), g.Count()))
    .OrderByDescending(g => g.Count)
    .ToList();

var report = new AuditReport(
    ScriptPath: scriptPath,
    LineCount: lines.Length,
    GlobalCount: globals.Count,
    FunctionCount: functions.Count,
    ReachableFunctionCount: functions.Count(f => f.Reachable),
    UnreachableFunctionCount: functions.Count(f => !f.Reachable),
    InitTriggerCount: triggers.Count,
    InitiallyDisabledTriggerCount: triggers.Count(t => t.InitiallyDisabled),
    TriggerWithoutEventCount: triggers.Count(t => t.RegisteredEvents.Count == 0),
    DormantButLinkedTriggerCount: triggers.Count(t => t.IsDormantCandidate && t.IsLinked),
    ReviewTriggerCandidateCount: triggers.Count(t => t.IsDormantCandidate && !t.IsLinked),
    TriggerWithPeriodicTimerCount: triggers.Count(t => t.RegisteredEvents.Any(e => e.Contains("Periodic", StringComparison.Ordinal) || e.Contains("TimerStart", StringComparison.Ordinal))),
    CreateGroupCount: CountPattern(lines, @"\bCreateGroup\s*\("),
    DestroyGroupCount: CountPattern(lines, @"\bDestroyGroup\s*\("),
    GetUnitsGroupCount: CountPattern(lines, @"\bGetUnits"),
    CreateLocationCount: CountPattern(lines, @"\b(Location|GetUnitLoc|GetRectCenter|PolarProjectionBJ|OffsetLocation)\s*\("),
    RemoveLocationCount: CountPattern(lines, @"\bRemoveLocation\s*\("),
    CreateTimerCount: CountPattern(lines, @"\bCreateTimer\s*\("),
    DestroyTimerCount: CountPattern(lines, @"\bDestroyTimer\s*\("),
    DoNothingCount: CountPattern(lines, @"\bDoNothing\s*\("),
    BjDebugMsgCount: CountPattern(lines, @"\bBJDebugMsg\s*\("),
    DisplayTextCount: CountPattern(lines, @"\bDisplay(?:Timed)?TextTo"),
    Triggers: triggers,
    UnreachableFunctions: functions.Where(f => !f.Reachable).Select(f => new FunctionSummary(f.Name, f.StartLine, f.EndLine, f.Kind)).ToList(),
    LargestFunctions: functions.OrderByDescending(f => f.LineCount).Take(40).Select(f => new FunctionSummary(f.Name, f.StartLine, f.EndLine, f.Kind)).ToList(),
    LeakFindings: leakFindings,
    DuplicateTriggerGroups: duplicateInitBodies);

var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
File.WriteAllText(Path.Combine(outDir, "jass_audit.json"), JsonSerializer.Serialize(report, jsonOptions), Encoding.UTF8);
File.WriteAllText(Path.Combine(outDir, "jass_audit.md"), BuildMarkdown(report), Encoding.UTF8);

Console.WriteLine($"Wrote {Path.Combine(outDir, "jass_audit.json")}");
Console.WriteLine($"Wrote {Path.Combine(outDir, "jass_audit.md")}");
Console.WriteLine($"Functions: {report.FunctionCount:n0} ({report.ReachableFunctionCount:n0} reachable, {report.UnreachableFunctionCount:n0} unreachable)");
Console.WriteLine($"Init triggers: {report.InitTriggerCount:n0} ({report.InitiallyDisabledTriggerCount:n0} initially disabled, {report.TriggerWithoutEventCount:n0} with no registered event)");
Console.WriteLine($"Leak suspects: {report.LeakFindings.Count:n0}");

static List<FunctionInfo> ParseFunctions(string[] lines)
{
    var functions = new List<FunctionInfo>();
    FunctionInfo? current = null;
    var functionRegex = new Regex(@"^function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes\s+.*\s+returns\s+.*", RegexOptions.Compiled);
    for (var i = 0; i < lines.Length; i++)
    {
        var match = functionRegex.Match(lines[i]);
        if (match.Success)
        {
            current = new FunctionInfo(match.Groups[1].Value, i + 1);
            continue;
        }
        if (current is not null && lines[i].Trim() == "endfunction")
        {
            current.EndLine = i + 1;
            current.Body = string.Join("\n", lines.Skip(current.StartLine).Take(current.EndLine - current.StartLine - 1));
            current.Kind = ClassifyFunction(current.Name);
            functions.Add(current);
            current = null;
        }
    }
    return functions;
}

static string ClassifyFunction(string name)
{
    if (name.StartsWith("InitTrig_", StringComparison.Ordinal)) return "init-trigger";
    if (name.StartsWith("Trig_", StringComparison.Ordinal)) return "gui-trigger";
    if (name.StartsWith("Init", StringComparison.Ordinal)) return "map-init";
    if (name.StartsWith("s__", StringComparison.Ordinal) || name.StartsWith("sc__", StringComparison.Ordinal) || name.StartsWith("sa__", StringComparison.Ordinal)) return "generated-library";
    if (name.StartsWith("jasshelper__", StringComparison.Ordinal)) return "generated-library";
    return "function";
}

static List<string> ParseGlobals(string[] lines)
{
    var result = new List<string>();
    var inGlobals = false;
    foreach (var raw in lines)
    {
        var line = raw.Trim();
        if (line == "globals")
        {
            inGlobals = true;
            continue;
        }
        if (line == "endglobals") break;
        if (inGlobals && line.Length > 0 && !line.StartsWith("//", StringComparison.Ordinal))
        {
            result.Add(line);
        }
    }
    return result;
}

static TriggerInfo AnalyzeTrigger(FunctionInfo f, string[] lines)
{
    var bodyLines = lines.Skip(f.StartLine).Take(f.EndLine - f.StartLine - 1).ToList();
    var body = string.Join("\n", bodyLines);
    var triggerName = f.Name["InitTrig_".Length..];
    var registeredEvents = bodyLines
        .Select(line => line.Trim())
        .Where(line => line.StartsWith("call TriggerRegister", StringComparison.Ordinal) || line.Contains("TriggerRegister", StringComparison.Ordinal) || line.Contains("TimerStart(", StringComparison.Ordinal))
        .ToList();
    var actions = Regex.Matches(body, @"TriggerAddAction\s*\([^,]+,\s*function\s+([A-Za-z_][A-Za-z0-9_]*)\s*\)")
        .Select(m => m.Groups[1].Value)
        .Distinct()
        .ToList();
    var conditions = Regex.Matches(body, @"(?:Condition|Filter)\s*\(\s*function\s+([A-Za-z_][A-Za-z0-9_]*)\s*\)")
        .Select(m => m.Groups[1].Value)
        .Distinct()
        .ToList();
    return new TriggerInfo
    {
        Name = triggerName,
        InitFunction = f.Name,
        StartLine = f.StartLine,
        EndLine = f.EndLine,
        InitiallyDisabled = body.Contains("DisableTrigger(", StringComparison.Ordinal),
        RegisteredEvents = registeredEvents,
        Actions = actions,
        Conditions = conditions,
        Body = body
    };
}

static void AnalyzeFunctionReferences(FunctionInfo f, IReadOnlyDictionary<string, FunctionInfo> functionByName)
{
    foreach (Match match in Regex.Matches(f.Body, @"\bcall\s+([A-Za-z_][A-Za-z0-9_]*)\s*\("))
    {
        var name = match.Groups[1].Value;
        if (functionByName.ContainsKey(name))
        {
            f.Calls.Add(name);
        }
    }
    foreach (Match match in Regex.Matches(f.Body, @"\bfunction\s+([A-Za-z_][A-Za-z0-9_]*)"))
    {
        var name = match.Groups[1].Value;
        if (functionByName.ContainsKey(name))
        {
            f.Calls.Add(name);
            f.ExternalFunctionRefs.Add(name);
        }
    }
    foreach (Match match in Regex.Matches(f.Body, @"ExecuteFunc\s*\(\s*""([A-Za-z_][A-Za-z0-9_]*)""\s*\)"))
    {
        var name = match.Groups[1].Value;
        if (functionByName.ContainsKey(name))
        {
            f.Calls.Add(name);
        }
    }
}

static HashSet<string> ComputeReachable(HashSet<string> roots, IReadOnlyDictionary<string, FunctionInfo> functionByName)
{
    var reachable = new HashSet<string>(StringComparer.Ordinal);
    var stack = new Stack<string>(roots.Where(functionByName.ContainsKey));
    while (stack.Count > 0)
    {
        var name = stack.Pop();
        if (!reachable.Add(name)) continue;
        foreach (var called in functionByName[name].Calls)
        {
            if (!reachable.Contains(called))
            {
                stack.Push(called);
            }
        }
    }
    return reachable;
}

static void ApplyTriggerUsage(IReadOnlyList<FunctionInfo> functions, IReadOnlyList<TriggerInfo> triggers)
{
    var triggerByName = triggers.ToDictionary(t => t.Name, StringComparer.Ordinal);
    var regex = new Regex(@"\b(EnableTrigger|DisableTrigger|TriggerExecute|ConditionalTriggerExecute)\s*\(\s*(gg_trg_[A-Za-z0-9_]+)\s*\)", RegexOptions.Compiled);
    foreach (var f in functions)
    {
        var bodyLines = f.Body.Split('\n');
        for (var i = 0; i < bodyLines.Length; i++)
        {
            foreach (Match match in regex.Matches(bodyLines[i]))
            {
                var operation = match.Groups[1].Value;
                var triggerName = match.Groups[2].Value["gg_trg_".Length..];
                if (!triggerByName.TryGetValue(triggerName, out var trigger))
                {
                    continue;
                }

                var line = f.StartLine + i + 1;
                var usage = new TriggerUsageRef(f.Name, line);
                if (operation == "EnableTrigger")
                {
                    trigger.EnabledBy.Add(usage);
                }
                else if (operation == "DisableTrigger")
                {
                    if (f.Name != trigger.InitFunction)
                    {
                        trigger.DisabledBy.Add(usage);
                    }
                }
                else
                {
                    trigger.ExecutedBy.Add(usage);
                }
            }
        }
    }
}

static List<LeakFinding> AnalyzeLeaks(IReadOnlyList<FunctionInfo> functions, string[] lines)
{
    var findings = new List<LeakFinding>();
    foreach (var f in functions)
    {
        var body = f.Body;
        AddIfMoreCreatedThanDestroyed(findings, f, "location", Count(body, @"\b(Location|GetUnitLoc|GetRectCenter|PolarProjectionBJ|OffsetLocation)\s*\("), Count(body, @"\bRemoveLocation\s*\("));
        AddIfMoreCreatedThanDestroyed(findings, f, "unit group", Count(body, @"\b(GetUnits|CreateGroup)\b"), Count(body, @"\bDestroyGroup\s*\("));
        AddIfMoreCreatedThanDestroyed(findings, f, "force", Count(body, @"\b(GetPlayers|CreateForce)\b"), Count(body, @"\bDestroyForce\s*\("));
        AddIfMoreCreatedThanDestroyed(findings, f, "effect", Count(body, @"\b(AddSpecialEffect|AddSpecialEffectLoc|AddSpecialEffectTarget)\b"), Count(body, @"\bDestroyEffect"));
        AddIfMoreCreatedThanDestroyed(findings, f, "timer", Count(body, @"\bCreateTimer\s*\("), Count(body, @"\bDestroyTimer\s*\("));
    }
    return findings
        .OrderByDescending(f => f.Excess)
        .ThenBy(f => f.StartLine)
        .Take(500)
        .ToList();
}

static void AddIfMoreCreatedThanDestroyed(List<LeakFinding> findings, FunctionInfo function, string resource, int created, int destroyed)
{
    if (created > destroyed)
    {
        findings.Add(new LeakFinding(function.Name, function.StartLine, function.EndLine, resource, created, destroyed, created - destroyed));
    }
}

static int CountPattern(string[] lines, string pattern) => lines.Sum(line => Count(line, pattern));
static int Count(string text, string pattern) => Regex.Matches(text, pattern).Count;
static string NormalizeBody(string body) => Regex.Replace(body, @"\s+", " ").Trim();

static string BuildMarkdown(AuditReport report)
{
    var b = new StringBuilder();
    b.AppendLine("# Whole Map JASS Audit");
    b.AppendLine();
    b.AppendLine($"Source: `{report.ScriptPath}`");
    b.AppendLine();
    b.AppendLine("## Summary");
    b.AppendLine();
    b.AppendLine($"- Lines: {report.LineCount:n0}");
    b.AppendLine($"- Globals: {report.GlobalCount:n0}");
    b.AppendLine($"- Functions: {report.FunctionCount:n0}");
    b.AppendLine($"- Reachable functions: {report.ReachableFunctionCount:n0}");
    b.AppendLine($"- Unreachable function candidates: {report.UnreachableFunctionCount:n0}");
    b.AppendLine($"- Init triggers: {report.InitTriggerCount:n0}");
    b.AppendLine($"- Initially disabled triggers: {report.InitiallyDisabledTriggerCount:n0}");
    b.AppendLine($"- Init triggers with no registered event: {report.TriggerWithoutEventCount:n0}");
    b.AppendLine($"- Dormant triggers with wake-up/execute links: {report.DormantButLinkedTriggerCount:n0}");
    b.AppendLine($"- Dormant triggers needing manual review: {report.ReviewTriggerCandidateCount:n0}");
    b.AppendLine();
    b.AppendLine("## Resource Smell Counters");
    b.AppendLine();
    b.AppendLine($"- `CreateGroup`: {report.CreateGroupCount:n0}; `DestroyGroup`: {report.DestroyGroupCount:n0}");
    b.AppendLine($"- `GetUnits*`: {report.GetUnitsGroupCount:n0}");
    b.AppendLine($"- Location creates/lookups: {report.CreateLocationCount:n0}; `RemoveLocation`: {report.RemoveLocationCount:n0}");
    b.AppendLine($"- `CreateTimer`: {report.CreateTimerCount:n0}; `DestroyTimer`: {report.DestroyTimerCount:n0}");
    b.AppendLine($"- `DoNothing`: {report.DoNothingCount:n0}");
    b.AppendLine($"- Text/debug output calls: {report.DisplayTextCount + report.BjDebugMsgCount:n0}");
    b.AppendLine();
    b.AppendLine("## Highest-Risk Leak Candidates");
    b.AppendLine();
    foreach (var f in report.LeakFindings.Take(60))
    {
        b.AppendLine($"- `{f.FunctionName}` line {f.StartLine}: {f.ResourceType} created {f.Created}, cleaned {f.Destroyed}, excess {f.Excess}");
    }
    b.AppendLine();
    b.AppendLine("## Largest Functions");
    b.AppendLine();
    foreach (var f in report.LargestFunctions.Take(40))
    {
        b.AppendLine($"- `{f.Name}` lines {f.StartLine}-{f.EndLine} ({f.EndLine - f.StartLine + 1:n0} lines, {f.Kind})");
    }
    b.AppendLine();
    b.AppendLine("## Linked Dormant Triggers");
    b.AppendLine();
    foreach (var t in report.Triggers.Where(t => t.IsDormantCandidate && t.IsLinked).Take(140))
    {
        var wakeups = string.Join(", ", t.EnabledBy.Take(3).Select(u => $"{u.FunctionName}:{u.Line}"));
        var executions = string.Join(", ", t.ExecutedBy.Take(3).Select(u => $"{u.FunctionName}:{u.Line}"));
        var reason = t.InitiallyDisabled ? "disabled" : "no registered event";
        var links = string.Join("; ", new[] { wakeups.Length > 0 ? $"enabled by {wakeups}" : "", executions.Length > 0 ? $"executed by {executions}" : "" }.Where(s => s.Length > 0));
        b.AppendLine($"- `{t.Name}` line {t.StartLine}: {reason}; {links}");
    }
    b.AppendLine();
    b.AppendLine("## Dormant Trigger Review Candidates");
    b.AppendLine();
    foreach (var t in report.Triggers.Where(t => t.IsDormantCandidate && !t.IsLinked).Take(160))
    {
        var status = t.InitiallyDisabled ? "disabled" : "no registered event";
        b.AppendLine($"- `{t.Name}` line {t.StartLine}: {status}");
    }
    b.AppendLine();
    b.AppendLine("## Unreachable Function Candidates");
    b.AppendLine();
    foreach (var f in report.UnreachableFunctions.Take(160))
    {
        b.AppendLine($"- `{f.Name}` line {f.StartLine} ({f.Kind})");
    }
    return b.ToString();
}

public sealed class FunctionInfo(string name, int startLine)
{
    public string Name { get; set; } = name;
    public int StartLine { get; set; } = startLine;
    public int EndLine { get; set; }
    public int LineCount => EndLine - StartLine + 1;
    public string Kind { get; set; } = "";
    public string Body { get; set; } = "";
    public HashSet<string> Calls { get; } = new(StringComparer.Ordinal);
    public HashSet<string> ExternalFunctionRefs { get; } = new(StringComparer.Ordinal);
    public bool Reachable { get; set; }
}

public sealed record FunctionSummary(string Name, int StartLine, int EndLine, string Kind);
public sealed class TriggerInfo
{
    public string Name { get; set; } = "";
    public string InitFunction { get; set; } = "";
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public bool InitiallyDisabled { get; set; }
    public List<string> RegisteredEvents { get; set; } = [];
    public List<string> Actions { get; set; } = [];
    public List<string> Conditions { get; set; } = [];
    public List<TriggerUsageRef> EnabledBy { get; set; } = [];
    public List<TriggerUsageRef> DisabledBy { get; set; } = [];
    public List<TriggerUsageRef> ExecutedBy { get; set; } = [];
    public string Body { get; set; } = "";
    public bool IsDormantCandidate => InitiallyDisabled || RegisteredEvents.Count == 0;
    public bool IsLinked => EnabledBy.Count > 0 || ExecutedBy.Count > 0;
}
public sealed record TriggerUsageRef(string FunctionName, int Line);
public sealed record LeakFinding(string FunctionName, int StartLine, int EndLine, string ResourceType, int Created, int Destroyed, int Excess);
public sealed record DuplicateTriggerGroup(List<string> TriggerNames, int Count);
public sealed record AuditReport(
    string ScriptPath,
    int LineCount,
    int GlobalCount,
    int FunctionCount,
    int ReachableFunctionCount,
    int UnreachableFunctionCount,
    int InitTriggerCount,
    int InitiallyDisabledTriggerCount,
    int TriggerWithoutEventCount,
    int DormantButLinkedTriggerCount,
    int ReviewTriggerCandidateCount,
    int TriggerWithPeriodicTimerCount,
    int CreateGroupCount,
    int DestroyGroupCount,
    int GetUnitsGroupCount,
    int CreateLocationCount,
    int RemoveLocationCount,
    int CreateTimerCount,
    int DestroyTimerCount,
    int DoNothingCount,
    int BjDebugMsgCount,
    int DisplayTextCount,
    List<TriggerInfo> Triggers,
    List<FunctionSummary> UnreachableFunctions,
    List<FunctionSummary> LargestFunctions,
    List<LeakFinding> LeakFindings,
    List<DuplicateTriggerGroup> DuplicateTriggerGroups);
