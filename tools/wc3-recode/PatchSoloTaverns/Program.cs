using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: PatchSoloTaverns <input-war3map.w3u> <output-war3map.w3u> [report.md]");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
var reportPath = args.Length > 2 ? Path.GetFullPath(args[2]) : null;
var source = File.ReadAllBytes(inputPath);
var reader = new Reader(source);
using var output = new MemoryStream(source.Length);
using var writer = new BinaryWriter(output, Encoding.UTF8, leaveOpen: true);
var version = CopyInt32(reader, writer);
var capacityChanges = 0;
var abilityChanges = 0;

for (var table = 0; table < 2; table++)
{
    var count = CopyInt32(reader, writer);
    for (var entryIndex = 0; entryIndex < count; entryIndex++)
    {
        var oldIdBytes = CopyBytes(reader, writer, 4);
        var newIdBytes = CopyBytes(reader, writer, 4);
        _ = CopyInt32(reader, writer);
        _ = CopyInt32(reader, writer);
        var modificationCount = CopyInt32(reader, writer);
        var oldId = CleanId(oldIdBytes);
        var newId = CleanId(newIdBytes);
        var objectId = string.IsNullOrEmpty(newId) ? oldId : newId;

        for (var modificationIndex = 0; modificationIndex < modificationCount; modificationIndex++)
        {
            var fieldBytes = CopyBytes(reader, writer, 4);
            var fieldId = CleanId(fieldBytes);
            var valueType = CopyInt32(reader, writer);
            if (valueType is 0 or 1 or 2)
            {
                var valueBytes = reader.Bytes(4);
                if (objectId == "n02I" && fieldId == "utco")
                {
                    var current = BinaryPrimitives.ReadInt32LittleEndian(valueBytes);
                    if (valueType != 0 || current != 8)
                    {
                        throw new InvalidDataException($"Expected n02I:utco integer 8, found type={valueType} value={current}.");
                    }
                    BinaryPrimitives.WriteInt32LittleEndian(valueBytes, 12);
                    capacityChanges++;
                }
                writer.Write(valueBytes);
            }
            else if (valueType == 3)
            {
                var valueBytes = reader.CStringBytes();
                if (objectId == "n02I" && fieldId == "uabi")
                {
                    var current = Encoding.UTF8.GetString(valueBytes, 0, valueBytes.Length - 1);
                    if (current != "Asud,A05T,Avul")
                    {
                        throw new InvalidDataException($"Expected n02I:uabi Asud,A05T,Avul, found {current}.");
                    }
                    valueBytes = Encoding.UTF8.GetBytes("Asud,Avul\0");
                    abilityChanges++;
                }
                writer.Write(valueBytes);
            }
            else
            {
                throw new InvalidDataException($"Unknown value type {valueType} for {objectId}:{fieldId}.");
            }
            _ = CopyBytes(reader, writer, 4);
        }
    }
}

if (reader.Remaining != 0)
{
    throw new InvalidDataException($"Unexpected {reader.Remaining} trailing bytes in unit object data.");
}
if (capacityChanges != 1 || abilityChanges != 1)
{
    throw new InvalidDataException($"Expected one capacity and one ability change; found capacity={capacityChanges}, abilities={abilityChanges}.");
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
var patched = output.ToArray();
File.WriteAllBytes(outputPath, patched);

var sourceHash = Convert.ToHexString(SHA256.HashData(source)).ToLowerInvariant();
var outputHash = Convert.ToHexString(SHA256.HashData(patched)).ToLowerInvariant();
if (reportPath is not null)
{
    Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
    File.WriteAllText(reportPath, $"""
# Solo Foots Two-Tavern Object Patch

- Input: `{inputPath}`
- Output: `{outputPath}`
- Object-data version: {version}
- Changed SD tavern `n02I` stock capacity (`utco`) from 8 to 12.
- Removed neutral-shop selector `A05T` from `n02I`; abilities are now `Asud,Avul`.
- No other object fields were changed.
- Source SHA256: `{sourceHash}`
- Output SHA256: `{outputHash}`
""", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
}

Console.WriteLine($"Wrote: {outputPath}");
Console.WriteLine($"Capacity changes: {capacityChanges}");
Console.WriteLine($"Ability changes: {abilityChanges}");
Console.WriteLine($"SHA256: {outputHash}");
return 0;

static int CopyInt32(Reader reader, BinaryWriter writer)
{
    var bytes = reader.Bytes(4);
    writer.Write(bytes);
    return BinaryPrimitives.ReadInt32LittleEndian(bytes);
}

static byte[] CopyBytes(Reader reader, BinaryWriter writer, int count)
{
    var bytes = reader.Bytes(count);
    writer.Write(bytes);
    return bytes;
}

static string CleanId(byte[] bytes)
    => Encoding.Latin1.GetString(bytes).Replace("\0", "", StringComparison.Ordinal);

sealed class Reader(byte[] data)
{
    private int offset;
    public int Remaining => data.Length - offset;

    public byte[] Bytes(int count)
    {
        if (count < 0 || offset + count > data.Length)
        {
            throw new EndOfStreamException($"Cannot read {count} bytes at offset {offset}.");
        }
        var value = data.AsSpan(offset, count).ToArray();
        offset += count;
        return value;
    }

    public byte[] CStringBytes()
    {
        var start = offset;
        while (offset < data.Length && data[offset] != 0)
        {
            offset++;
        }
        if (offset >= data.Length)
        {
            throw new EndOfStreamException($"Unterminated string at offset {start}.");
        }
        offset++;
        return data.AsSpan(start, offset - start).ToArray();
    }
}
