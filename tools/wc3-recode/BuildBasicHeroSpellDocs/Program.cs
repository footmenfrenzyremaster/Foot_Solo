using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

var root = Directory.GetCurrentDirectory();
var files = Path.Combine(root, "outputs", "WC3-799", "extracted", "799W-tester", "files");
var unitPath = Path.Combine(files, "war3map.w3u");
var abilityPath = Path.Combine(files, "war3map.w3a");
var wtsPath = Path.Combine(files, "war3map.wts");
var docsPath = Path.Combine(root, "outputs", "WC3-799", "docs", "basic_hero_spell_breakdowns.md");
var htmlPath = Path.Combine(root, "outputs", "WC3-799", "docs", "basic_hero_spell_cards.html");
var jsonPath = Path.Combine(root, "outputs", "WC3-799", "src", "data", "basic_hero_spells.json");

var units = ParseObjectData(File.ReadAllBytes(unitPath), ObjectKind.Unit);
var abilities = ParseObjectData(File.ReadAllBytes(abilityPath), ObjectKind.Ability);
var abilityById = abilities
    .GroupBy(a => a.Id, StringComparer.OrdinalIgnoreCase)
    .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Table).First(), StringComparer.OrdinalIgnoreCase);
var wts = ParseWts(File.ReadAllText(wtsPath, Encoding.UTF8));

var heroes = BasicHeroes();
foreach (var hero in heroes)
{
    var unit = units.FirstOrDefault(u => u.Id.Equals(hero.Rawcode, StringComparison.OrdinalIgnoreCase));
    var overrideList = unit?.Mods.LastOrDefault(m => m.Id == "uhab")?.Value?.ToString();
    hero.AbilityIds = string.IsNullOrWhiteSpace(overrideList)
        ? hero.DefaultAbilityIds
        : overrideList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    hero.Abilities = hero.AbilityIds.Select(id => BuildAbilityBreakdown(id, abilityById, wts)).ToList();
}

Directory.CreateDirectory(Path.GetDirectoryName(docsPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
File.WriteAllText(docsPath, BuildMarkdown(heroes), Encoding.UTF8);
File.WriteAllText(htmlPath, BuildHtml(heroes), Encoding.UTF8);
File.WriteAllText(jsonPath, JsonSerializer.Serialize(heroes, new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
}), Encoding.UTF8);

Console.WriteLine($"Wrote {docsPath}");
Console.WriteLine($"Wrote {htmlPath}");
Console.WriteLine($"Wrote {jsonPath}");
Console.WriteLine($"Heroes: {heroes.Count}");
Console.WriteLine($"Abilities: {heroes.Sum(h => h.Abilities.Count)}");

static AbilityBreakdown BuildAbilityBreakdown(string id, IReadOnlyDictionary<string, Entry> abilityById, WtsIndex wts)
{
    abilityById.TryGetValue(id, out var entry);
    var name = wts.FindAbilityName(id) ?? AbilityNameFallback(id);
    var learn = wts.FindBestAbilityText(id, "Researchubertip") ?? wts.FindBestAbilityText(id, "ubertip") ?? "";
    var normal = wts.FindBestAbilityText(id, "ubertip") ?? "";
    var cleanedLearn = CleanTooltip(ResolveTooltip(learn, abilityById));
    var cleanedNormal = CleanTooltip(ResolveTooltip(normal, abilityById));
    var description = cleanedLearn.Split("\n\n", 2, StringSplitOptions.None)[0].Trim();
    if (string.IsNullOrWhiteSpace(description))
    {
        description = cleanedNormal.Split('\n').FirstOrDefault()?.Trim() ?? "";
    }

    var placeholders = ExtractPlaceholderFields(learn + "\n" + normal);
    var levelCount = GetInt(entry, "alev", 0) ?? placeholders.Select(p => p.Level).DefaultIfEmpty(3).Max();
    if (levelCount <= 0)
    {
        levelCount = Math.Max(3, entry?.Mods.Where(m => m.Level > 0).Select(m => m.Level).DefaultIfEmpty(0).Max() ?? 3);
    }
    levelCount = Math.Clamp(levelCount, 1, 6);

    var fields = SelectFields(placeholders, entry).ToList();
    var levels = new List<Dictionary<string, string>>();
    for (var level = 1; level <= levelCount; level++)
    {
        var row = new Dictionary<string, string>
        {
            ["Level"] = level.ToString(),
        };
        foreach (var field in fields)
        {
            var lookupId = field.SourceAbilityId ?? id;
        row[field.Label] = FormatValue(ResolveField(abilityById, lookupId, field.Field, level), field.Field, field.IsPercent);
        }
        levels.Add(row);
    }

    return new AbilityBreakdown
    {
        Id = id,
        Name = name,
        BaseId = entry?.OldId ?? "",
        Description = description,
        LearnTooltip = cleanedLearn,
        NormalTooltip = cleanedNormal,
        Fields = fields.Select(f => f.Label).ToList(),
        Levels = levels,
        DataSource = entry is null ? "No object-data entry found; name/text only." : $"war3map.w3a entry {entry.Id} based on {entry.OldId}.",
    };
}

static IEnumerable<FieldSpec> SelectFields(IEnumerable<Placeholder> placeholders, Entry? entry)
{
    var chosen = placeholders
        .Where(p => p.Level > 0)
        .GroupBy(p => $"{p.AbilityId}:{p.Field}", StringComparer.OrdinalIgnoreCase)
        .Select(g => g.First())
        .Select(p => new FieldSpec(string.IsNullOrWhiteSpace(p.Label) ? PrettyField(p.Field) : p.Label, p.Field, p.AbilityId, p.IsPercent))
        .ToList();

    if (chosen.Count > 0)
    {
        return DeduplicateLabels(chosen);
    }

    if (entry is null)
    {
        return [];
    }

    var fallback = new List<FieldSpec>();
    if (entry.Mods.Any(m => m.DataPointer == 1 && m.Level > 0)) fallback.Add(new FieldSpec("Data A", "DataA", entry.Id, false));
    if (entry.Mods.Any(m => m.DataPointer == 2 && m.Level > 0)) fallback.Add(new FieldSpec("Data B", "DataB", entry.Id, false));
    if (entry.Mods.Any(m => m.Id == "aare" && m.Level > 0)) fallback.Add(new FieldSpec("AoE", "Area", entry.Id, false));
    if (entry.Mods.Any(m => m.Id == "aran" && m.Level > 0)) fallback.Add(new FieldSpec("Range", "Rng", entry.Id, false));
    if (entry.Mods.Any(m => m.Id == "acdn" && m.Level > 0)) fallback.Add(new FieldSpec("Cooldown", "Cool", entry.Id, false));
    if (entry.Mods.Any(m => m.Id == "amcs" && m.Level > 0)) fallback.Add(new FieldSpec("Mana Cost", "Cost", entry.Id, false));
    if (entry.Mods.Any(m => m.Id is "adur" or "ahdu" && m.Level > 0)) fallback.Add(new FieldSpec("Duration", "Dur", entry.Id, false));
    return fallback;
}

static string ResolveTooltip(string text, IReadOnlyDictionary<string, Entry> abilityById)
{
    return Regex.Replace(text, @"<([A-Za-z0-9]{4}),([A-Za-z]+)(\d)(?:,([^>]*))?>", match =>
    {
        var id = match.Groups[1].Value;
        var field = match.Groups[2].Value;
        var level = int.Parse(match.Groups[3].Value);
        var isPercent = match.Groups[4].Value.Contains('%');
        var value = ResolveField(abilityById, id, field, level);
        return value is null ? match.Value : FormatValue(value, field, isPercent);
    });
}

static IEnumerable<Placeholder> ExtractPlaceholderFields(string text)
{
    return Regex.Matches(text, @"<([A-Za-z0-9]{4}),([A-Za-z]+)(\d)(?:,([^>]*))?>")
        .Select(m => new Placeholder(
            m.Groups[1].Value,
            m.Groups[2].Value,
            int.Parse(m.Groups[3].Value),
            m.Groups[4].Value.Contains('%'),
            ExtractLabelBefore(text, m.Index)))
        .Where(p => !p.Field.Equals("Buff", StringComparison.OrdinalIgnoreCase));
}

static object? ResolveField(IReadOnlyDictionary<string, Entry> abilityById, string id, string field, int level)
{
    if (!abilityById.TryGetValue(id, out var entry))
    {
        return null;
    }
    return ResolveFieldFromEntry(abilityById, entry, field, level, allowBaseFallback: true);
}

static object? ResolveFieldFromEntry(IReadOnlyDictionary<string, Entry> abilityById, Entry entry, string field, int level, bool allowBaseFallback)
{
    object? value = null;
    var generic = field.ToLowerInvariant() switch
    {
        "cool" => "acdn",
        "cost" => "amcs",
        "dur" => "adur",
        "herodur" => "ahdu",
        "area" => "aare",
        "rng" or "range" => "aran",
        _ => null,
    };
    if (generic is not null)
    {
        value = entry.Mods.LastOrDefault(m => m.Id.Equals(generic, StringComparison.OrdinalIgnoreCase) && m.Level == level)?.Value;
    }
    else if (field.StartsWith("Data", StringComparison.OrdinalIgnoreCase) && field.Length >= 5)
    {
        var pointer = char.ToUpperInvariant(field[4]) - 'A' + 1;
        value = entry.Mods.LastOrDefault(m => m.DataPointer == pointer && m.Level == level)?.Value;
    }

    if (value is not null)
    {
        return value;
    }
    if (allowBaseFallback &&
        !string.IsNullOrWhiteSpace(entry.OldId) &&
        !entry.OldId.Equals(entry.Id, StringComparison.OrdinalIgnoreCase) &&
        abilityById.TryGetValue(entry.OldId, out var baseEntry))
    {
        return ResolveFieldFromEntry(abilityById, baseEntry, field, level, allowBaseFallback: false);
    }
    return null;
}

static int? GetInt(Entry? entry, string id, int level)
{
    var value = entry?.Mods.LastOrDefault(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && m.Level == level)?.Value;
    return value switch
    {
        int i => i,
        float f => (int)f,
        double d => (int)d,
        _ => null,
    };
}

static string FormatValue(object? value, string field, bool isPercent = false)
{
    if (value is null) return "default";
    if (value is float f)
    {
        var v = Math.Abs(f) < 0.00001 ? 0 : f;
        if (isPercent)
        {
            v *= 100;
        }
        if (field.StartsWith("Data", StringComparison.OrdinalIgnoreCase) || field is "Area" or "Rng" or "Range")
        {
            return v % 1 == 0 ? ((int)v).ToString() : v.ToString("0.##");
        }
        return v % 1 == 0 ? ((int)v).ToString() : v.ToString("0.##");
    }
    return value.ToString() ?? "";
}

static string ExtractLabelBefore(string text, int matchIndex)
{
    var start = Math.Max(0, matchIndex - 120);
    var before = text[start..matchIndex]
        .Replace("|n", "\n", StringComparison.OrdinalIgnoreCase)
        .Replace("|r", "", StringComparison.OrdinalIgnoreCase);
    before = Regex.Replace(before, @"\|c[0-9A-Fa-f]{8}", "");
    var segment = before.Split('\n', '|').LastOrDefault()?.Trim() ?? "";
    segment = Regex.Replace(segment, @"^Level\s+\d+\s*-?\s*", "", RegexOptions.IgnoreCase).Trim();
    segment = Regex.Replace(segment, @"\([^)]*$", "").Trim();
    segment = Regex.Replace(segment, @"<[^>]+>.*$", "").Trim();
    segment = segment.Trim(':', '-', ' ', '\t');
    if (segment.Length > 28 || string.IsNullOrWhiteSpace(segment))
    {
        return "";
    }
    return segment;
}

static IEnumerable<FieldSpec> DeduplicateLabels(IReadOnlyList<FieldSpec> fields)
{
    var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    foreach (var field in fields)
    {
        counts[field.Label] = counts.GetValueOrDefault(field.Label) + 1;
    }
    var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    foreach (var field in fields)
    {
        if (counts[field.Label] == 1)
        {
            yield return field;
            continue;
        }
        seen[field.Label] = seen.GetValueOrDefault(field.Label) + 1;
        yield return field with { Label = $"{field.Label} {seen[field.Label]}" };
    }
}

static string PrettyField(string field)
{
    if (field.StartsWith("Data", StringComparison.OrdinalIgnoreCase))
    {
        return field switch
        {
            "DataA" => "Data A",
            "DataB" => "Data B",
            "DataC" => "Data C",
            "DataD" => "Data D",
            "DataE" => "Data E",
            "DataF" => "Data F",
            _ => field,
        };
    }

    return field.ToLowerInvariant() switch
    {
        "cool" => "Cooldown",
        "cost" => "Mana Cost",
        "dur" => "Duration",
        "herodur" => "Hero Duration",
        "area" => "AoE",
        "rng" or "range" => "Range",
        _ => field,
    };
}

static string CleanTooltip(string text)
{
    if (string.IsNullOrWhiteSpace(text)) return "";
    var cleaned = text
        .Replace("|n", "\n", StringComparison.OrdinalIgnoreCase)
        .Replace("|r", "", StringComparison.OrdinalIgnoreCase);
    cleaned = Regex.Replace(cleaned, @"\|c[0-9A-Fa-f]{8}", "");
    cleaned = Regex.Replace(cleaned, @"[ \t]+\n", "\n");
    cleaned = Regex.Replace(cleaned, @"\n{3,}", "\n\n");
    return cleaned.Trim();
}

static string BuildMarkdown(IReadOnlyList<HeroBreakdown> heroes)
{
    var b = new StringBuilder();
    b.AppendLine("# Basic Hero Spell Breakdowns");
    b.AppendLine();
    b.AppendLine("First extracted draft for the 16 basic heroes. Layout follows the requested breakdown style: hero name, spell name, short behavior summary, per-level stats, and notes.");
    b.AppendLine();
    b.AppendLine("Data sources: `war3map.w3u`, `war3map.w3a`, and `war3map.wts`. Trigger-only behavior still needs a manual pass before this should be treated as final player-facing copy.");
    b.AppendLine();

    foreach (var hero in heroes)
    {
        b.AppendLine($"## {hero.Name}");
        b.AppendLine();
        b.AppendLine($"Rawcode `{hero.Rawcode}`. Ability list: {string.Join(", ", hero.AbilityIds.Select(a => $"`{a}`"))}.");
        b.AppendLine();

        foreach (var ability in hero.Abilities)
        {
            b.AppendLine($"### {ability.Name}");
            b.AppendLine();
            b.AppendLine($"Ability `{ability.Id}`" + (string.IsNullOrEmpty(ability.BaseId) ? "." : $" based on `{ability.BaseId}`."));
            b.AppendLine();
            if (!string.IsNullOrWhiteSpace(ability.Description))
            {
                b.AppendLine(ability.Description);
                b.AppendLine();
            }

            if (ability.Fields.Count > 0 && ability.Levels.Count > 0)
            {
                var headers = new[] { "Level" }.Concat(ability.Fields).ToList();
                b.AppendLine("| " + string.Join(" | ", headers) + " |");
                b.AppendLine("|" + string.Join("|", headers.Select(h => h == "Level" ? "---:" : "---:")) + "|");
                foreach (var row in ability.Levels)
                {
                    b.AppendLine("| " + string.Join(" | ", headers.Select(h => EscapeMd(row.GetValueOrDefault(h, "")))) + " |");
                }
                b.AppendLine();
            }

            b.AppendLine("Notes:");
            b.AppendLine($"- {ability.DataSource}");
            if (ability.LearnTooltip.Contains('\n'))
            {
                b.AppendLine("- Learn tooltip was used for the primary table shape.");
            }
            else
            {
                b.AppendLine("- Table fields were inferred from object-data modifications.");
            }
            b.AppendLine();
        }
    }
    return b.ToString();
}

static string BuildHtml(IReadOnlyList<HeroBreakdown> heroes)
{
    var json = JsonSerializer.Serialize(heroes, new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    });
    return $$"""
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Basic Hero Spell Breakdowns</title>
  <style>
    :root {
      color-scheme: dark;
      --bg: #031013;
      --panel: #06191d;
      --line: rgba(201, 177, 145, 0.36);
      --text: #f4efe6;
      --muted: #bcb5a9;
      --gold: #ffc400;
      --orange: #ff9700;
      --red: #ff3a1a;
      --violet: #e759f3;
      --blue: #98bfff;
    }
    * { box-sizing: border-box; }
    body {
      margin: 0;
      background:
        radial-gradient(circle at 18% 0%, rgba(213, 112, 30, 0.18), transparent 24rem),
        linear-gradient(135deg, #020b0d 0%, #05181c 52%, #02080a 100%);
      color: var(--text);
      font-family: "Segoe UI", Arial, sans-serif;
    }
    main { width: min(1560px, calc(100% - 32px)); margin: 0 auto; padding: 28px 0 64px; }
    .topbar {
      display: flex;
      gap: 12px;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 24px;
    }
    h1 {
      margin: 0;
      font-family: Georgia, "Times New Roman", serif;
      font-size: clamp(32px, 5vw, 64px);
      letter-spacing: 0;
      text-transform: uppercase;
      text-shadow: 0 3px 0 #000;
    }
    select {
      min-width: 220px;
      background: #071114;
      color: var(--text);
      border: 1px solid var(--line);
      padding: 10px 12px;
      font-size: 15px;
    }
    .hero { margin: 32px 0 56px; }
    .hero-title {
      color: var(--gold);
      font-family: Georgia, "Times New Roman", serif;
      font-size: 28px;
      margin: 0 0 18px;
    }
    .cards { display: grid; gap: 28px; }
    .spell {
      border: 1px solid var(--line);
      background: rgba(2, 11, 13, 0.72);
      box-shadow: 0 18px 48px rgba(0, 0, 0, 0.34);
      padding: 18px;
    }
    .spell-head {
      display: grid;
      grid-template-columns: 132px 1fr;
      gap: 22px;
      align-items: start;
      margin-bottom: 20px;
    }
    .icon {
      aspect-ratio: 1;
      display: grid;
      place-items: center;
      border: 5px solid #9e7618;
      outline: 2px solid #2a1d0a;
      background:
        linear-gradient(135deg, rgba(255, 190, 43, 0.92), rgba(139, 33, 16, 0.96)),
        radial-gradient(circle at 42% 30%, rgba(255,255,255,.3), transparent 28%);
      color: #1a0802;
      font-family: Georgia, "Times New Roman", serif;
      font-size: 42px;
      font-weight: 800;
      text-shadow: 0 1px rgba(255,255,255,.35);
    }
    .spell h2 {
      margin: 0;
      font-family: Georgia, "Times New Roman", serif;
      font-size: clamp(34px, 5vw, 64px);
      letter-spacing: 0;
      text-transform: uppercase;
      line-height: 0.95;
      text-shadow: 0 3px 0 #000;
    }
    .hero-name {
      margin-top: 8px;
      color: var(--gold);
      font-family: Georgia, "Times New Roman", serif;
      font-size: 24px;
      font-weight: 700;
    }
    .description {
      max-width: 780px;
      color: var(--text);
      font-size: 20px;
      line-height: 1.42;
      margin: 14px 0 0;
    }
    table {
      width: 100%;
      border-collapse: collapse;
      overflow: hidden;
      border: 1px solid var(--line);
      background: rgba(0, 0, 0, 0.22);
    }
    th, td {
      border: 1px solid var(--line);
      text-align: center;
      padding: 14px 12px;
      font-size: 18px;
      white-space: nowrap;
    }
    th {
      color: var(--gold);
      font-family: Georgia, "Times New Roman", serif;
      font-size: 17px;
      text-transform: uppercase;
    }
    th:first-child { color: var(--text); }
    td:first-child { color: var(--text); }
    td:nth-child(2), th:nth-child(2) { color: var(--gold); }
    td:nth-child(3), th:nth-child(3) { color: var(--red); }
    td:nth-child(4), th:nth-child(4) { color: var(--orange); }
    td:nth-child(5), th:nth-child(5) { color: var(--violet); }
    td:nth-child(6), th:nth-child(6) { color: var(--blue); }
    .notes {
      margin-top: 14px;
      color: var(--muted);
      font-style: italic;
      font-size: 16px;
      line-height: 1.45;
    }
    .source {
      color: #8f9da3;
      font-size: 13px;
      margin-top: 8px;
      font-style: normal;
    }
    @media (max-width: 720px) {
      main { width: min(100% - 20px, 1560px); padding-top: 16px; }
      .topbar { align-items: stretch; flex-direction: column; }
      .spell { padding: 12px; overflow-x: auto; }
      .spell-head { grid-template-columns: 84px 1fr; gap: 14px; }
      .description { font-size: 16px; }
      th, td { padding: 10px 8px; font-size: 14px; }
      .icon { font-size: 28px; }
    }
  </style>
</head>
<body>
  <main>
    <div class="topbar">
      <h1>Basic Hero Spells</h1>
      <select id="heroFilter" aria-label="Hero filter"></select>
    </div>
    <div id="app"></div>
  </main>
  <script>
    const HEROES = {{json}};
    const app = document.getElementById("app");
    const filter = document.getElementById("heroFilter");

    filter.innerHTML = `<option value="">All basic heroes</option>` + HEROES
      .map(hero => `<option value="${escapeHtml(hero.Name)}">${escapeHtml(hero.Name)}</option>`)
      .join("");
    filter.addEventListener("change", render);
    render();

    function render() {
      const selected = filter.value;
      const heroes = selected ? HEROES.filter(hero => hero.Name === selected) : HEROES;
      app.innerHTML = heroes.map(hero => `
        <section class="hero">
          <h2 class="hero-title">${escapeHtml(hero.Name)} <span style="color:#8f9da3;font-size:16px"> ${escapeHtml(hero.Rawcode)}</span></h2>
          <div class="cards">
            ${hero.Abilities.map(ability => spellCard(hero, ability)).join("")}
          </div>
        </section>
      `).join("");
    }

    function spellCard(hero, ability) {
      const headers = ["Level", ...ability.Fields];
      const rows = ability.Levels.map(level => `
        <tr>${headers.map(header => `<td>${escapeHtml(level[header] || "")}</td>`).join("")}</tr>
      `).join("");
      return `
        <article class="spell">
          <div class="spell-head">
            <div class="icon">${initials(ability.Name)}</div>
            <div>
              <h2>${escapeHtml(ability.Name)}</h2>
              <div class="hero-name">${escapeHtml(hero.Name)}</div>
              <p class="description">${escapeHtml(ability.Description)}</p>
            </div>
          </div>
          <table>
            <thead><tr>${headers.map(header => `<th>${escapeHtml(header)}</th>`).join("")}</tr></thead>
            <tbody>${rows}</tbody>
          </table>
          <div class="notes">
            First extracted pass. Values marked "default" are inherited by the game object and need a stock-data pass before final art export.
            <div class="source">${escapeHtml(ability.Id)} based on ${escapeHtml(ability.BaseId || "unknown")}</div>
          </div>
        </article>
      `;
    }

    function initials(name) {
      return name.split(/\s+/).filter(Boolean).slice(0, 2).map(part => part[0]).join("").toUpperCase();
    }
    function escapeHtml(value) {
      return String(value).replace(/[&<>"']/g, ch => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[ch]));
    }
  </script>
</body>
</html>
""";
}

static string EscapeMd(string value)
{
    return value.Replace("|", "\\|", StringComparison.Ordinal);
}

static string AbilityNameFallback(string id) => id switch
{
    "AHhb" => "Holy Light",
    "AHds" => "Divine Shield",
    "AHad" => "Devotion Aura",
    "AHre" => "Resurrection",
    "AHbz" => "Blizzard",
    "AHab" => "Brilliance Aura",
    "AHwe" => "Summon Water Elemental",
    "AHmt" => "Mass Teleport",
    "AHtb" => "Storm Bolt",
    "AHtc" => "Thunder Clap",
    "AHbh" => "Bash",
    "AHav" => "Avatar",
    "AHfs" => "Flame Strike",
    "AHbn" => "Banish",
    "AHdr" => "Siphon Mana",
    "AHpx" => "Phoenix",
    "AOwk" => "Wind Walk",
    "AOmi" => "Mirror Image",
    "AOcr" => "Critical Strike",
    "AOww" => "Bladestorm",
    "AOcl" => "Chain Lightning",
    "AOfs" => "Feral Spirit",
    "AOsf" => "Feral Spirit",
    "AOeq" => "Earthquake",
    "AOsh" => "Shockwave",
    "AOws" => "War Stomp",
    "AOae" => "Endurance Aura",
    "AOre" => "Reincarnation",
    "AOhw" => "Healing Wave",
    "AOhx" => "Hex",
    "AOsw" => "Serpent Ward",
    "AOvd" => "Big Bad Voodoo",
    "AUdc" => "Death Coil",
    "AUdp" => "Death Pact",
    "AUau" => "Unholy Aura",
    "AUan" => "Animate Dead",
    "AUfn" => "Frost Nova",
    "AUfu" => "Frost Armor",
    "AUdr" => "Dark Ritual",
    "AUdd" => "Death and Decay",
    "AUsl" => "Sleep",
    "AUcs" => "Carrion Swarm",
    "AUav" => "Vampiric Aura",
    "AUin" => "Inferno",
    "AUim" => "Impale",
    "AUts" => "Spiked Carapace",
    "AUcb" => "Carrion Beetles",
    "AUls" => "Locust Swarm",
    "AEer" => "Entangling Roots",
    "AEfn" => "Force of Nature",
    "AEah" => "Thorns Aura",
    "AEtq" => "Tranquility",
    "AEst" => "Scout",
    "AHfa" => "Searing Arrows",
    "AEar" => "Trueshot Aura",
    "AEsf" => "Starfall",
    "AEmb" => "Mana Burn",
    "AEim" => "Immolation",
    "AEev" => "Evasion",
    "AEme" => "Metamorphosis",
    "AEfk" => "Fan of Knives",
    "AEbl" => "Blink",
    "AEsh" => "Shadow Strike",
    "AEsv" => "Vengeance",
    _ => id,
};

static List<HeroBreakdown> BasicHeroes() =>
[
    new("Paladin", "H005", ["AHhb", "AHds", "AHad", "AHre"]),
    new("Archmage", "H008", ["AHbz", "AHab", "AHwe", "AHmt"]),
    new("Mountain King", "H00A", ["AHtc", "AHtb", "AHbh", "AHav"]),
    new("Blood Mage", "H009", ["AHfs", "AHbn", "AHdr", "AHpx"]),
    new("Blademaster", "O003", ["AOwk", "AOmi", "AOcr", "AOww"]),
    new("Far Seer", "O004", ["AOcl", "AOsf", "AOfs", "AOeq"]),
    new("Tauren Chieftain", "O005", ["AOsh", "AOws", "AOae", "AOre"]),
    new("Shadow Hunter", "O006", ["AOhw", "AOhx", "AOsw", "AOvd"]),
    new("Death Knight", "U002", ["AUdc", "AUdp", "AUau", "AUan"]),
    new("Lich King", "U003", ["AUfn", "AUdr", "AUfu", "AUdd"]),
    new("Dread Lord", "U004", ["AUav", "AUsl", "AUcs", "AUin"]),
    new("Crypt Lord", "U005", ["AUim", "AUts", "AUcb", "AUls"]),
    new("Keeper of the Groove", "E002", ["AEer", "AEah", "AEfn", "AEtq"]),
    new("Priestess of the Moon", "E003", ["AEst", "AHfa", "AEar", "AEsf"]),
    new("Demon Hunter", "E00F", ["AEmb", "AEim", "AEev", "AEme"]),
    new("Warden", "E005", ["AEfk", "AEbl", "AEsh", "AEsv"]),
];

static WtsIndex ParseWts(string text)
{
    var strings = new List<WtsString>();
    var lines = text.Replace("\r\n", "\n").Split('\n');
    for (var i = 0; i < lines.Length; i++)
    {
        var match = Regex.Match(lines[i], @"^STRING\s+(\d+)");
        if (!match.Success) continue;

        var id = int.Parse(match.Groups[1].Value);
        string? comment = null;
        var cursor = i + 1;
        while (cursor < lines.Length && lines[cursor].StartsWith("//", StringComparison.Ordinal))
        {
            comment = lines[cursor];
            cursor++;
        }
        if (cursor >= lines.Length || lines[cursor].Trim() != "{") continue;
        cursor++;
        var value = new StringBuilder();
        while (cursor < lines.Length && lines[cursor].Trim() != "}")
        {
            if (value.Length > 0) value.Append('\n');
            value.Append(lines[cursor]);
            cursor++;
        }
        strings.Add(new WtsString(id, comment ?? "", value.ToString()));
        i = cursor;
    }
    return new WtsIndex(strings);
}

static List<Entry> ParseObjectData(byte[] data, ObjectKind kind)
{
    var r = new Reader(data);
    _ = r.Int32();
    var entries = new List<Entry>();
    for (var table = 0; table < 2; table++)
    {
        var count = r.Int32();
        for (var i = 0; i < count; i++)
        {
            var oldId = r.Id();
            var newId = r.Id();
            var entryFlags = r.Int32();
            var entryExtra = r.Int32();
            var mods = r.Int32();
            var entry = new Entry(table, CleanId(oldId), CleanId(newId), entryFlags, entryExtra);
            for (var m = 0; m < mods; m++)
            {
                var modId = CleanId(r.Id());
                var type = r.Int32();
                var level = 0;
                var dataPointer = 0;
                if (kind == ObjectKind.Ability)
                {
                    level = r.Int32();
                    dataPointer = r.Int32();
                }
                object value = type switch
                {
                    0 => r.Int32(),
                    1 => r.Single(),
                    2 => r.Single(),
                    3 => r.String(),
                    _ => throw new InvalidDataException($"Unknown object value type {type}.")
                };
                var end = CleanId(r.Id());
                entry.Mods.Add(new Mod(modId, type, level, dataPointer, value, end));
            }
            entries.Add(entry);
        }
    }
    return entries;
}

static string CleanId(string id) => id.Replace("\0", "", StringComparison.Ordinal);

public sealed class WtsIndex(IReadOnlyList<WtsString> strings)
{
    public string? FindAbilityName(string abilityId)
    {
        var match = strings.FirstOrDefault(s =>
            s.Comment.Contains($"Abilities: {abilityId} ", StringComparison.OrdinalIgnoreCase));
        if (match is null) return null;
        var name = Regex.Match(match.Comment, @"Abilities:\s+\w+\s+\((.*?)\)");
        return name.Success ? name.Groups[1].Value : null;
    }

    public string? FindBestAbilityText(string abilityId, string field)
    {
        return strings
            .Where(s => s.Comment.Contains($"Abilities: {abilityId} ", StringComparison.OrdinalIgnoreCase) &&
                        s.Comment.Contains(field, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(s => s.Text.Contains("Level 6", StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(s => s.Text.Length)
            .Select(s => s.Text)
            .FirstOrDefault();
    }
}

public sealed record WtsString(int Id, string Comment, string Text);
public sealed record Placeholder(string AbilityId, string Field, int Level, bool IsPercent, string Label);
public sealed record FieldSpec(string Label, string Field, string? SourceAbilityId, bool IsPercent);
public enum ObjectKind { Unit, Ability }

public sealed record Entry(int Table, string OldId, string NewId, int EntryFlags, int EntryExtra)
{
    public string Id => string.IsNullOrEmpty(NewId) ? OldId : NewId;
    public List<Mod> Mods { get; } = [];
}

public sealed record Mod(string Id, int Type, int Level, int DataPointer, object Value, string EndToken);

public sealed class HeroBreakdown(string name, string rawcode, List<string> defaultAbilityIds)
{
    public string Name { get; set; } = name;
    public string Rawcode { get; set; } = rawcode;
    public List<string> DefaultAbilityIds { get; set; } = defaultAbilityIds;
    public List<string> AbilityIds { get; set; } = [];
    public List<AbilityBreakdown> Abilities { get; set; } = [];
}

public sealed class AbilityBreakdown
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string BaseId { get; set; } = "";
    public string Description { get; set; } = "";
    public string LearnTooltip { get; set; } = "";
    public string NormalTooltip { get; set; } = "";
    public List<string> Fields { get; set; } = [];
    public List<Dictionary<string, string>> Levels { get; set; } = [];
    public string DataSource { get; set; } = "";
}

sealed class Reader(byte[] data)
{
    private int _offset;
    public int Int32()
    {
        var v = BitConverter.ToInt32(data, _offset);
        _offset += 4;
        return v;
    }
    public float Single()
    {
        var v = BitConverter.ToSingle(data, _offset);
        _offset += 4;
        return v;
    }
    public string Id()
    {
        var s = Encoding.Latin1.GetString(data, _offset, 4);
        _offset += 4;
        return s;
    }
    public string String()
    {
        var start = _offset;
        while (_offset < data.Length && data[_offset] != 0) _offset++;
        var s = Encoding.UTF8.GetString(data, start, _offset - start);
        if (_offset < data.Length) _offset++;
        return s;
    }
}
