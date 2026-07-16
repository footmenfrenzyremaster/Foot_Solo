using System.Text;
using System.Text.Json;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: W3ObjectDump <object-data-file> [rawcode[,rawcode...]] [--json]");
    return 2;
}

var path = args[0];
var filter = args.Length > 1 && !args[1].StartsWith("--", StringComparison.Ordinal)
    ? args[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet(StringComparer.OrdinalIgnoreCase)
    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var json = args.Any(arg => arg.Equals("--json", StringComparison.OrdinalIgnoreCase));
var kind = Path.GetExtension(path).Equals(".w3a", StringComparison.OrdinalIgnoreCase) ? ObjectKind.Ability : ObjectKind.Unit;
var data = File.ReadAllBytes(path);
var entries = ParseObjectData(data, kind);

var selected = filter.Count == 0
    ? entries
    : entries.Where(e => filter.Contains(e.Id) || filter.Contains(e.OldId) || filter.Contains(e.NewId)).ToList();

if (json)
{
    Console.WriteLine(JsonSerializer.Serialize(selected, new JsonSerializerOptions { WriteIndented = true }));
    return 0;
}

var reader = new Reader(data);
Console.WriteLine($"version={reader.Int32()}");
Console.WriteLine($"kind={kind}");
Console.WriteLine($"entries={entries.Count}");
foreach (var e in selected)
{
    Console.WriteLine();
    Console.WriteLine($"ENTRY table={e.Table} id={e.Id} old={e.OldId} new={e.NewId} flags={e.EntryFlags} extra={e.EntryExtra} mods={e.Mods.Count}");
    foreach (var m in e.Mods)
    {
        var levelText = kind == ObjectKind.Ability ? $" level={m.Level} data={m.DataPointer}" : "";
        Console.WriteLine($"  {m.Id}{levelText} type={m.Type} value={m.Value} end={m.EndToken}");
    }
}

return 0;

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
            var entryOffset = r.Offset;
            var oldId = r.Id();
            var newId = r.Id();
            var entryFlags = r.Int32();
            var entryExtra = r.Int32();
            var mods = r.Int32();
            if (mods < 0 || mods > 50000)
            {
                throw new InvalidDataException($"Unreasonable mod count {mods} at table={table} index={i} offset={entryOffset} old={oldId} new={newId}");
            }

            var entry = new Entry(table, CleanId(oldId), CleanId(newId), entryFlags, entryExtra);
            for (var m = 0; m < mods; m++)
            {
                var modOffset = r.Offset;
                var modId = r.Id();
                var type = r.Int32();
                if (type < 0 || type > 3)
                {
                    throw new InvalidDataException($"Unknown type {type} at table={table} index={i} mod={m} offset={modOffset} old={oldId} new={newId} modId={modId}");
                }

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
                    _ => $"<unknown type {type}>"
                };
                var end = r.Id();
                entry.Mods.Add(new Mod(CleanId(modId), type, level, dataPointer, value, CleanId(end)));
            }
            entries.Add(entry);
        }
    }
    return entries;
}

static string CleanId(string id)
{
    return id.Replace("\0", "", StringComparison.Ordinal);
}

public enum ObjectKind
{
    Unit,
    Ability,
}

public sealed record Entry(int Table, string OldId, string NewId, int EntryFlags, int EntryExtra)
{
    public string Id => string.IsNullOrEmpty(NewId) ? OldId : NewId;
    public List<Mod> Mods { get; } = [];
}

public sealed record Mod(string Id, int Type, int Level, int DataPointer, object Value, string EndToken);

sealed class Reader(byte[] data)
{
    private int _offset;
    public int Offset => _offset;

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
