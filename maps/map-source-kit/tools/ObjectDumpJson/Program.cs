using System.Text;
using System.Text.Json;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: ObjectDumpJson <unit|ability> <input.w3u/w3a> <output.json>");
    Environment.Exit(2);
}

var kind = args[0].ToLowerInvariant();
var inputPath = Path.GetFullPath(args[1]);
var outputPath = Path.GetFullPath(args[2]);

var data = File.ReadAllBytes(inputPath);
var reader = new Reader(data);
var version = reader.Int32();
var tables = new List<TableDump>();

for (var tableIndex = 0; tableIndex < 2; tableIndex++)
{
    var count = reader.Int32();
    var entries = new List<EntryDump>();

    for (var i = 0; i < count; i++)
    {
        entries.Add(kind switch
        {
            "unit" => ReadUnitEntry(reader, tableIndex),
            "ability" => ReadAbilityEntry(reader, tableIndex),
            _ => throw new InvalidOperationException("Kind must be unit or ability.")
        });
    }

    tables.Add(new TableDump(tableIndex, entries));
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, JsonSerializer.Serialize(new ObjectDump(version, kind, tables), new JsonSerializerOptions
{
    WriteIndented = true
}));

static EntryDump ReadUnitEntry(Reader reader, int tableIndex)
{
    var oldId = reader.Id();
    var newId = reader.Id();
    var entryFlags = reader.Int32();
    var entryExtra = reader.Int32();
    var modCount = reader.Int32();
    var mods = new List<ModDump>();

    for (var i = 0; i < modCount; i++)
    {
        var modId = reader.Id();
        var type = reader.Int32();
        var value = reader.Value(type);
        var endToken = reader.Id();
        mods.Add(new ModDump(modId, type, null, null, value, endToken));
    }

    return new EntryDump(tableIndex, oldId, newId, entryFlags, entryExtra, mods);
}

static EntryDump ReadAbilityEntry(Reader reader, int tableIndex)
{
    var oldId = reader.Id();
    var newId = reader.Id();
    var entryFlags = reader.Int32();
    var entryExtra = reader.Int32();
    var modCount = reader.Int32();
    var mods = new List<ModDump>();

    for (var i = 0; i < modCount; i++)
    {
        var modId = reader.Id();
        var type = reader.Int32();
        var level = reader.Int32();
        var dataPointer = reader.Int32();
        var value = reader.Value(type);
        var endToken = reader.Id();
        mods.Add(new ModDump(modId, type, level, dataPointer, value, endToken));
    }

    return new EntryDump(tableIndex, oldId, newId, entryFlags, entryExtra, mods);
}

sealed class Reader(byte[] data)
{
    private int _offset;

    public int Int32()
    {
        var value = BitConverter.ToInt32(data, _offset);
        _offset += 4;
        return value;
    }

    public float Single()
    {
        var value = BitConverter.ToSingle(data, _offset);
        _offset += 4;
        return value;
    }

    public string Id()
    {
        var value = Encoding.Latin1.GetString(data, _offset, 4);
        _offset += 4;
        return value;
    }

    public object Value(int type)
    {
        return type switch
        {
            0 => Int32(),
            1 => Single(),
            2 => Single(),
            3 => String(),
            _ => throw new InvalidDataException($"Unknown value type {type} at offset {_offset}.")
        };
    }

    private string String()
    {
        var start = _offset;
        while (_offset < data.Length && data[_offset] != 0)
        {
            _offset++;
        }

        var value = Encoding.UTF8.GetString(data, start, _offset - start);
        if (_offset < data.Length)
        {
            _offset++;
        }
        return value;
    }
}

public sealed record ObjectDump(int Version, string Kind, IReadOnlyList<TableDump> Tables);
public sealed record TableDump(int Index, IReadOnlyList<EntryDump> Entries);
public sealed record EntryDump(int Table, string OldId, string NewId, int? EntryFlags, int? EntryExtra, IReadOnlyList<ModDump> Mods);
public sealed record ModDump(string Id, int Type, int? Level, int? DataPointer, object Value, string EndToken);
