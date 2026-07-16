using System.Buffers.Binary;
using System.Text;

var root = Directory.GetCurrentDirectory();
var inputPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "extracted", "799W-tester", "files", "war3map.w3u");
var outputPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "build", "war3map.tavern-capacity-pass-10.w3u");
var reportPath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "build", "tavern-capacity-pass-10", "report.md");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
var data = File.ReadAllBytes(inputPath);
var reader = new Reader(data);
var version = reader.Int32();
var targets = new Dictionary<string, int>(StringComparer.Ordinal)
{
    ["n01A"] = 8,
    ["n02I"] = 8
};
var referenceTwelve = new HashSet<string>(StringComparer.Ordinal) { "n02E", "n02J" };
var patched = new List<string>();
var references = new List<string>();

for (var table = 0; table < 2; table++)
{
    var count = reader.Int32();
    for (var i = 0; i < count; i++)
    {
        var oldId = reader.Id();
        var newId = reader.Id();
        _ = reader.Int32();
        _ = reader.Int32();
        var modCount = reader.Int32();
        var id = string.IsNullOrEmpty(Clean(newId)) ? Clean(oldId) : Clean(newId);
        for (var mod = 0; mod < modCount; mod++)
        {
            var modId = Clean(reader.Id());
            var type = reader.Int32();
            var valueOffset = reader.Offset;
            object value = type switch
            {
                0 => reader.Int32(),
                1 => reader.Single(),
                2 => reader.Single(),
                3 => reader.String(),
                _ => throw new InvalidDataException($"Unknown modification type {type} for {id}:{modId}")
            };
            _ = reader.Id();

            if (modId == "utco" && targets.TryGetValue(id, out var expected))
            {
                if (type != 0 || value is not int current || current != expected)
                    throw new InvalidOperationException($"Expected {id}:utco to be integer {expected}, found {value}.");
                BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(valueOffset, 4), 12);
                patched.Add(id);
            }
            if (modId == "utco" && referenceTwelve.Contains(id))
            {
                if (type != 0 || value is not int current || current != 12)
                    throw new InvalidOperationException($"Expected reference tavern {id}:utco to be 12, found {value}.");
                references.Add(id);
            }
        }
    }
}

if (!patched.Order().SequenceEqual(targets.Keys.Order()))
    throw new InvalidOperationException($"Patched taverns differ from expected set: {string.Join(", ", patched)}");
if (!references.Order().SequenceEqual(referenceTwelve.Order()))
    throw new InvalidOperationException($"Reference taverns differ from expected set: {string.Join(", ", references)}");

File.WriteAllBytes(outputPath, data);
var report = $"""
# Tavern Capacity Object Patch - Pass 10

Input: `{inputPath}`
Output: `{outputPath}`

- Object data version: {version}.
- Changed AP tavern `n01A` field `utco` from 8 to 12.
- Changed SD tavern `n02I` field `utco` from 8 to 12.
- Validated existing 12-hero reference taverns `n02E` and `n02J` both use `utco = 12`.
- No other object-data fields were changed.
""";
File.WriteAllText(reportPath, report, Encoding.UTF8);
Console.WriteLine($"Wrote patched object data: {outputPath}");
Console.WriteLine($"Wrote report: {reportPath}");

static string Clean(string value) => value.Replace("\0", "", StringComparison.Ordinal);

sealed class Reader(byte[] data)
{
    private int offset;
    public int Offset => offset;

    public int Int32()
    {
        var value = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset, 4));
        offset += 4;
        return value;
    }

    public float Single()
    {
        var value = BitConverter.ToSingle(data, offset);
        offset += 4;
        return value;
    }

    public string Id()
    {
        var value = Encoding.Latin1.GetString(data, offset, 4);
        offset += 4;
        return value;
    }

    public string String()
    {
        var start = offset;
        while (offset < data.Length && data[offset] != 0) offset++;
        var value = Encoding.UTF8.GetString(data, start, offset - start);
        if (offset < data.Length) offset++;
        return value;
    }
}
