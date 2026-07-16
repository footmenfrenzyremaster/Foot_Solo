using System.Buffers.Binary;
using System.Text;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: ReplaceWar3MapScript <base-map.w3x> <war3map.j> <output-map.w3x>");
    Environment.Exit(2);
}

var baseMapPath = Path.GetFullPath(args[0]);
var scriptPath = Path.GetFullPath(args[1]);
var outputMapPath = Path.GetFullPath(args[2]);

if (!File.Exists(baseMapPath))
{
    throw new FileNotFoundException("Base map was not found.", baseMapPath);
}
if (!File.Exists(scriptPath))
{
    throw new FileNotFoundException("Script was not found.", scriptPath);
}

Directory.CreateDirectory(Path.GetDirectoryName(outputMapPath)!);
File.Copy(baseMapPath, outputMapPath, overwrite: true);

var scriptBytes = File.ReadAllBytes(scriptPath);
PatchMpqFile(outputMapPath, "war3map.j", scriptBytes);

Console.WriteLine($"Base map:   {baseMapPath}");
Console.WriteLine($"Script:     {scriptPath}");
Console.WriteLine($"Output map: {outputMapPath}");
Console.WriteLine($"Script bytes: {scriptBytes.Length:n0}");

static void PatchMpqFile(string archivePath, string internalName, byte[] replacement)
{
    var archive = File.ReadAllBytes(archivePath);
    var cryptTable = BuildCryptTable();
    var header = ParseHeader(archive);
    var hashBytes = DecryptTable(archive.AsSpan((int)header.HashOffset, checked((int)header.HashEntries * 16)), HashString(cryptTable, "(hash table)", 3), cryptTable);
    var blockBytes = DecryptTable(archive.AsSpan((int)header.BlockOffset, checked((int)header.BlockEntries * 16)), HashString(cryptTable, "(block table)", 3), cryptTable);
    var hashes = ReadHashTable(hashBytes);
    var blocks = ReadBlockTable(blockBytes);
    var blockIndex = FindBlockIndex(internalName, header, hashes, cryptTable);

    if (blockIndex < 0)
    {
        throw new InvalidOperationException($"Could not find {internalName} in the MPQ hash table.");
    }
    if (blockIndex >= blocks.Length)
    {
        throw new InvalidOperationException($"{internalName} points to invalid block index {blockIndex}.");
    }

    var appendOffset = checked((uint)archive.Length);
    blocks[blockIndex] = new BlockEntry(appendOffset, checked((uint)replacement.Length), checked((uint)replacement.Length), 0x81000000);
    var encryptedBlockTable = EncryptTable(WriteBlockTable(blocks), HashString(cryptTable, "(block table)", 3), cryptTable);

    using var stream = new FileStream(archivePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    stream.Seek(0, SeekOrigin.End);
    stream.Write(replacement);
    stream.Seek(header.BlockOffset, SeekOrigin.Begin);
    stream.Write(encryptedBlockTable);
    stream.Seek(8, SeekOrigin.Begin);
    Span<byte> sizeBytes = stackalloc byte[4];
    BinaryPrimitives.WriteUInt32LittleEndian(sizeBytes, checked((uint)(archive.Length + replacement.Length)));
    stream.Write(sizeBytes);
}

static int FindBlockIndex(string name, MpqHeader header, IReadOnlyList<HashEntry> hashes, uint[] cryptTable)
{
    var h1 = HashString(cryptTable, name, 1);
    var h2 = HashString(cryptTable, name, 2);
    var mask = header.HashEntries - 1;
    var start = HashString(cryptTable, name, 0) & mask;
    for (uint step = 0; step < header.HashEntries; step++)
    {
        var entry = hashes[(int)((start + step) & mask)];
        if (entry.BlockIndex == 0xffffffff)
        {
            return -1;
        }
        if (entry.BlockIndex != 0xfffffffe && entry.Name1 == h1 && entry.Name2 == h2)
        {
            return checked((int)entry.BlockIndex);
        }
    }
    return -1;
}

static HashEntry[] ReadHashTable(byte[] hashBytes)
{
    var hashes = new HashEntry[hashBytes.Length / 16];
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
    return hashes;
}

static BlockEntry[] ReadBlockTable(byte[] blockBytes)
{
    var blocks = new BlockEntry[blockBytes.Length / 16];
    for (var i = 0; i < blocks.Length; i++)
    {
        var s = blockBytes.AsSpan(i * 16, 16);
        blocks[i] = new BlockEntry(
            BinaryPrimitives.ReadUInt32LittleEndian(s[0..4]),
            BinaryPrimitives.ReadUInt32LittleEndian(s[4..8]),
            BinaryPrimitives.ReadUInt32LittleEndian(s[8..12]),
            BinaryPrimitives.ReadUInt32LittleEndian(s[12..16]));
    }
    return blocks;
}

static byte[] WriteBlockTable(IReadOnlyList<BlockEntry> blocks)
{
    var bytes = new byte[blocks.Count * 16];
    for (var i = 0; i < blocks.Count; i++)
    {
        var s = bytes.AsSpan(i * 16, 16);
        BinaryPrimitives.WriteUInt32LittleEndian(s[0..4], blocks[i].Offset);
        BinaryPrimitives.WriteUInt32LittleEndian(s[4..8], blocks[i].CompressedSize);
        BinaryPrimitives.WriteUInt32LittleEndian(s[8..12], blocks[i].FileSize);
        BinaryPrimitives.WriteUInt32LittleEndian(s[12..16], blocks[i].Flags);
    }
    return bytes;
}

static byte[] DecryptTable(ReadOnlySpan<byte> source, uint key, uint[] cryptTable)
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

static byte[] EncryptTable(ReadOnlySpan<byte> source, uint key, uint[] cryptTable)
{
    var output = new byte[source.Length];
    uint seed1 = key;
    uint seed2 = 0xeeeeeeee;
    for (var offset = 0; offset + 4 <= source.Length; offset += 4)
    {
        seed2 += cryptTable[0x400 + (seed1 & 0xff)];
        var plain = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(offset, 4));
        var encrypted = plain ^ (seed1 + seed2);
        BinaryPrimitives.WriteUInt32LittleEndian(output.AsSpan(offset, 4), encrypted);
        seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
        seed2 = plain + seed2 + (seed2 << 5) + 3;
    }
    return output;
}

static uint HashString(uint[] cryptTable, string value, int hashType)
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
        throw new InvalidOperationException("Not an MPQ archive.");
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

public sealed record MpqHeader(uint HeaderSize, uint ArchiveSize, ushort Version, uint SectorSize, uint HashOffset, uint BlockOffset, uint HashEntries, uint BlockEntries);
public sealed record HashEntry(uint Name1, uint Name2, ushort Locale, ushort Platform, uint BlockIndex);
public sealed record BlockEntry(uint Offset, uint CompressedSize, uint FileSize, uint Flags);
