using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: MpqExtract <map.w3x> <out-dir>");
    return 2;
}

var mapPath = args[0];
var outDir = args[1];
var archive = File.ReadAllBytes(mapPath);
Directory.CreateDirectory(outDir);

var cryptTable = BuildCryptTable();
uint HashString(string value, int hashType)
{
    uint seed1 = 0x7fed7fed;
    uint seed2 = 0xeeeeeeee;
    foreach (var b in Encoding.Latin1.GetBytes(value.Replace('/', '\\').ToUpperInvariant()))
    {
        seed1 = cryptTable[(hashType << 8) + b] ^ (seed1 + seed2);
        seed2 = b + seed1 + seed2 + (seed2 << 5) + 3;
    }
    return seed1;
}

byte[] DecryptTable(ReadOnlySpan<byte> source, uint key)
{
    var output = source.ToArray();
    uint seed1 = key;
    uint seed2 = 0xeeeeeeee;
    for (var offset = 0; offset + 4 <= output.Length; offset += 4)
    {
        seed2 += cryptTable[0x400 + (seed1 & 0xff)];
        var value = BinaryPrimitives.ReadUInt32LittleEndian(output.AsSpan(offset, 4)) ^ (seed1 + seed2);
        BinaryPrimitives.WriteUInt32LittleEndian(output.AsSpan(offset, 4), value);
        seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
        seed2 = value + seed2 + (seed2 << 5) + 3;
    }
    return output;
}

var header = ParseHeader(archive);
var hashBytes = DecryptTable(archive.AsSpan((int)header.HashOffset, checked((int)header.HashEntries * 16)), HashString("(hash table)", 3));
var blockBytes = DecryptTable(archive.AsSpan((int)header.BlockOffset, checked((int)header.BlockEntries * 16)), HashString("(block table)", 3));

var hashes = new HashEntry[header.HashEntries];
for (var i = 0; i < hashes.Length; i++)
{
    var s = hashBytes.AsSpan(i * 16, 16);
    hashes[i] = new HashEntry(
        BinaryPrimitives.ReadUInt32LittleEndian(s[0..4]),
        BinaryPrimitives.ReadUInt32LittleEndian(s[4..8]),
        BinaryPrimitives.ReadUInt16LittleEndian(s[8..10]),
        BinaryPrimitives.ReadUInt16LittleEndian(s[10..12]),
        BinaryPrimitives.ReadUInt32LittleEndian(s[12..16]));
}

var blocks = new BlockEntry[header.BlockEntries];
for (var i = 0; i < blocks.Length; i++)
{
    var s = blockBytes.AsSpan(i * 16, 16);
    blocks[i] = new BlockEntry(
        BinaryPrimitives.ReadUInt32LittleEndian(s[0..4]),
        BinaryPrimitives.ReadUInt32LittleEndian(s[4..8]),
        BinaryPrimitives.ReadUInt32LittleEndian(s[8..12]),
        BinaryPrimitives.ReadUInt32LittleEndian(s[12..16]));
}

int FindBlockIndex(string name)
{
    var h1 = HashString(name, 1);
    var h2 = HashString(name, 2);
    var mask = header.HashEntries - 1;
    var start = HashString(name, 0) & mask;
    for (uint step = 0; step < header.HashEntries; step++)
    {
        var entry = hashes[(start + step) & mask];
        if (entry.BlockIndex == 0xffffffff) return -1;
        if (entry.BlockIndex != 0xfffffffe && entry.Name1 == h1 && entry.Name2 == h2)
        {
            return checked((int)entry.BlockIndex);
        }
    }
    return -1;
}

byte[]? ReadNamed(string name)
{
    var index = FindBlockIndex(name);
    if (index < 0 || index >= blocks.Length) return null;
    return ReadBlock(blocks[index], name);
}

byte[] ReadBlock(BlockEntry block, string name)
{
    const uint Compressed = 0x00000200;
    const uint Encrypted = 0x00010000;
    const uint FixKey = 0x00020000;
    const uint SingleUnit = 0x01000000;
    const uint Exists = 0x80000000;

    if ((block.Flags & Exists) == 0)
    {
        throw new InvalidOperationException($"{name} is not marked as an existing file");
    }

    var data = archive.AsSpan((int)block.Offset, (int)block.CompressedSize).ToArray();
    if ((block.Flags & Encrypted) != 0)
    {
        var key = HashString(Path.GetFileName(name.Replace('\\', Path.DirectorySeparatorChar)), 3);
        if ((block.Flags & FixKey) != 0)
        {
            key = (key + block.Offset) ^ block.FileSize;
        }
        data = DecryptTable(data, key);
    }

    if ((block.Flags & SingleUnit) != 0)
    {
        if ((block.Flags & Compressed) != 0 && block.CompressedSize < block.FileSize)
        {
            return Decompress(data, (int)block.FileSize);
        }
        return data.Take((int)block.FileSize).ToArray();
    }

    var sectorCount = checked((int)((block.FileSize + header.SectorSize - 1) / header.SectorSize));
    var sectorTableSize = (sectorCount + 1) * 4;
    var offsets = new uint[sectorCount + 1];
    for (var i = 0; i < offsets.Length; i++)
    {
        offsets[i] = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(i * 4, 4));
    }

    using var output = new MemoryStream((int)block.FileSize);
    for (var i = 0; i < sectorCount; i++)
    {
        var start = checked((int)offsets[i]);
        var end = checked((int)offsets[i + 1]);
        if (start < sectorTableSize || end > data.Length || end < start)
        {
            throw new InvalidOperationException($"{name} has an invalid sector table");
        }
        var sector = data.AsSpan(start, end - start).ToArray();
        var expected = checked((int)Math.Min(header.SectorSize, block.FileSize - (uint)i * header.SectorSize));
        var decoded = ((block.Flags & Compressed) != 0 && sector.Length < expected) ? Decompress(sector, expected) : sector;
        output.Write(decoded, 0, decoded.Length);
    }
    return output.ToArray();
}

byte[] Decompress(byte[] data, int expectedSize)
{
    if (data.Length == expectedSize) return data;
    if (data.Length == 0) return data;
    var method = data[0];
    var payload = data.AsSpan(1).ToArray();
    if ((method & 0x02) != 0 || (method & 0x08) != 0)
    {
        using var input = new MemoryStream(payload);
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream(expectedSize);
        zlib.CopyTo(output);
        return output.ToArray();
    }
    throw new NotSupportedException($"Unsupported MPQ compression method 0x{method:x2}");
}

var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "(listfile)",
    "war3map.j",
    "scripts\\war3map.j",
    "war3map.w3e",
    "war3map.w3i",
    "war3map.wtg",
    "war3map.wct",
    "war3map.wts",
    "war3map.imp",
    "war3map.mmp",
    "war3map.w3a",
    "war3map.w3b",
    "war3map.w3c",
    "war3map.w3d",
    "war3map.doo",
    "war3map.w3h",
    "war3map.w3q",
    "war3map.w3r",
    "war3map.w3s",
    "war3map.w3t",
    "war3map.w3u",
    "war3mapUnits.doo",
    "war3map.wpm",
    "war3map.shd",
    "war3mapMap.blp",
    "war3mapPreview.tga",
    "war3mapMisc.txt",
    "war3mapSkin.txt",
    "war3mapExtra.txt",
};

byte[]? listfile = null;
object? listfileError = null;
try
{
    listfile = ReadNamed("(listfile)");
}
catch (Exception ex)
{
    listfileError = new { name = "(listfile)", error = ex.Message };
}
if (listfile is not null)
{
    foreach (var line in Encoding.Latin1.GetString(listfile).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
    {
        var clean = line.Trim();
        if (clean.Length > 0) names.Add(clean);
    }
}

var importList = ReadNamed("war3map.imp");
if (importList is not null)
{
    foreach (var importedName in ParseWar3Imports(importList))
    {
        names.Add(importedName);
    }
}

var extracted = new List<string>();
var failed = new List<object>();
foreach (var name in names)
{
    try
    {
        var content = ReadNamed(name);
        if (content is null) continue;
        var safe = name.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var destination = Path.Combine(outDir, "files", safe);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.WriteAllBytes(destination, content);
        extracted.Add(name);
    }
    catch (Exception ex)
    {
        failed.Add(new { name, error = ex.Message });
    }
}

File.WriteAllText(Path.Combine(outDir, "mpq-header.json"), JsonSerializer.Serialize(header, new JsonSerializerOptions { WriteIndented = true }));
File.WriteAllText(Path.Combine(outDir, "block-table.json"), JsonSerializer.Serialize(blocks, new JsonSerializerOptions { WriteIndented = true }));
File.WriteAllText(Path.Combine(outDir, "extraction-report.json"), JsonSerializer.Serialize(new
{
    mapPath,
    extractedCount = extracted.Count,
    extracted,
    failed,
    listfileError,
    hasListfile = listfile is not null,
    blockCount = blocks.Length
}, new JsonSerializerOptions { WriteIndented = true }));

Console.WriteLine($"Extracted {extracted.Count} files to {outDir}");
if (failed.Count > 0) Console.WriteLine($"Failed {failed.Count} files; see extraction-report.json");
return failed.Count == 0 ? 0 : 1;

static uint[] BuildCryptTable()
{
    var table = new uint[0x500];
    uint seed = 0x00100001;
    for (var index1 = 0; index1 < 0x100; index1++)
    {
        for (int index2 = index1, i = 0; i < 5; i++, index2 += 0x100)
        {
            seed = (seed * 125 + 3) % 0x2aaaab;
            var temp1 = (seed & 0xffff) << 16;
            seed = (seed * 125 + 3) % 0x2aaaab;
            table[index2] = temp1 | (seed & 0xffff);
        }
    }
    return table;
}

static MpqHeader ParseHeader(byte[] data)
{
    if (Encoding.Latin1.GetString(data, 0, 4) != "MPQ\x1a")
    {
        throw new InvalidOperationException("Not an MPQ archive");
    }
    return new MpqHeader(
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(4, 4)),
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(8, 4)),
        BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(12, 2)),
        512u << BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(14, 2)),
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(16, 4)),
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(20, 4)),
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(24, 4)),
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(28, 4)));
}

static IEnumerable<string> ParseWar3Imports(byte[] data)
{
    if (data.Length < 8) yield break;
    var count = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(4, 4));
    var offset = 8;
    for (var i = 0; i < count && offset < data.Length; i++)
    {
        offset++;
        var start = offset;
        while (offset < data.Length && data[offset] != 0) offset++;
        if (offset > start)
        {
            var name = Encoding.UTF8.GetString(data, start, offset - start);
            yield return name;
            if (!name.StartsWith("war3mapImported\\", StringComparison.OrdinalIgnoreCase) &&
                !name.StartsWith("war3mapImported/", StringComparison.OrdinalIgnoreCase))
            {
                yield return "war3mapImported\\" + name;
            }
        }
        offset++;
    }
}

public sealed record MpqHeader(uint HeaderSize, uint ArchiveSize, ushort Version, uint SectorSize, uint HashOffset, uint BlockOffset, uint HashEntries, uint BlockEntries);
public sealed record HashEntry(uint Name1, uint Name2, ushort Locale, ushort Platform, uint BlockIndex);
public sealed record BlockEntry(uint Offset, uint CompressedSize, uint FileSize, uint Flags);
