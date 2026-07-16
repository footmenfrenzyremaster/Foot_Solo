using System.Buffers.Binary;
using System.Text;

var root = Directory.GetCurrentDirectory();
var mapPath = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(root, "outputs", "WC3-799", "map", "799W-tester.w3x");
var baseScriptPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(root, "outputs", "WC3-799", "extracted", "799W-tester", "files", "war3map.j");
var systemSourcePath = args.Length > 2 ? Path.GetFullPath(args[2]) : Path.Combine(root, "outputs", "WC3-799", "src", "systems", "individual_spawn_rates.jass");
var outputMapPath = args.Length > 3 ? Path.GetFullPath(args[3]) : Path.Combine(root, "outputs", "WC3-799", "releases", "799W-tester-individual-spawn.w3x");
var patchedScriptPath = Path.Combine(root, "outputs", "WC3-799", "build", "war3map.individual_spawn.j");

Directory.CreateDirectory(Path.GetDirectoryName(outputMapPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(patchedScriptPath)!);

var baseScript = File.ReadAllText(baseScriptPath, Encoding.Latin1);
var systemSource = File.ReadAllText(systemSourcePath, Encoding.Latin1);
var converted = ConvertIndividualSpawnSystem(systemSource);
var patchedScript = InjectSystem(baseScript, converted);

File.WriteAllText(patchedScriptPath, patchedScript, Encoding.Latin1);
File.Copy(mapPath, outputMapPath, overwrite: true);
PatchMpqFile(outputMapPath, "war3map.j", Encoding.Latin1.GetBytes(patchedScript));

Console.WriteLine($"Wrote patched script: {patchedScriptPath}");
Console.WriteLine($"Wrote patched map:    {outputMapPath}");
Console.WriteLine($"Script bytes:         {Encoding.Latin1.GetByteCount(patchedScript):n0}");

static ConvertedSystem ConvertIndividualSpawnSystem(string source)
{
    var lines = SplitLines(source);
    var globals = new List<string>
    {
        "    // Individual Spawn Rates",
    };
    var functions = new List<string>
    {
        "",
        "// Individual Spawn Rates",
        "// Injected plain-JASS version of src/systems/individual_spawn_rates.jass.",
    };

    var inGlobals = false;
    var inFunctions = false;
    foreach (var raw in lines)
    {
        var line = raw.TrimEnd('\r');
        var trimmed = line.Trim();

        if (trimmed.StartsWith("library ", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }
        if (trimmed.Equals("endlibrary", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }
        if (trimmed.Equals("globals", StringComparison.OrdinalIgnoreCase))
        {
            inGlobals = true;
            inFunctions = false;
            continue;
        }
        if (trimmed.Equals("endglobals", StringComparison.OrdinalIgnoreCase))
        {
            inGlobals = false;
            inFunctions = true;
            continue;
        }

        if (inGlobals)
        {
            var converted = RemovePrivate(line);
            if (converted.Trim().Equals("timer ISR_Timer = CreateTimer()", StringComparison.OrdinalIgnoreCase))
            {
                converted = converted[..converted.IndexOf("timer", StringComparison.Ordinal)] + "timer ISR_Timer = null";
            }
            globals.Add(converted);
        }
        else if (inFunctions)
        {
            functions.Add(RemovePrivate(line));
        }
    }

    AddLegacyDisableFunction(functions);
    EnsureTimerCreatedInStart(functions);

    if (!functions.Any(line => line.Contains("function ISR_Start takes nothing returns nothing", StringComparison.Ordinal)))
    {
        throw new InvalidOperationException("Converted spawn system is missing ISR_Start.");
    }

    return new ConvertedSystem(globals, functions);
}

static void AddLegacyDisableFunction(List<string> functions)
{
    var insertAt = functions.FindIndex(line => line.Contains("function ISR_Start takes nothing returns nothing", StringComparison.Ordinal));
    if (insertAt < 0)
    {
        insertAt = functions.Count;
    }

    var disableFunction = new[]
    {
        "",
        "function ISR_DisableLegacySpawnTriggers takes nothing returns nothing",
        "    call DisableTrigger(gg_trg_eight_start_timer)",
        "    call DisableTrigger(gg_trg_eight)",
        "    call DisableTrigger(gg_trg_five)",
        "    call DisableTrigger(gg_trg_ten)",
        "    call DisableTrigger(gg_trg_eight_new)",
        "    call DisableTrigger(gg_trg_ten_new)",
        "    call DisableTrigger(gg_trg_twelve_new)",
        "    call PauseTimer(udg_spawnTimer)",
        "endfunction",
    };

    functions.InsertRange(insertAt, disableFunction);
}

static void EnsureTimerCreatedInStart(List<string> functions)
{
    var start = functions.FindIndex(line => line.Contains("function ISR_Start takes nothing returns nothing", StringComparison.Ordinal));
    if (start < 0)
    {
        return;
    }

    for (var i = start + 1; i < functions.Count; i++)
    {
        if (functions[i].Contains("call ISR_SetDefaultRates()", StringComparison.Ordinal))
        {
            functions.InsertRange(i, new[]
            {
                "    if ISR_Timer == null then",
                "        set ISR_Timer = CreateTimer()",
                "    endif",
            });
            return;
        }
        if (functions[i].Trim().Equals("endfunction", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
    }
}

static string InjectSystem(string baseScript, ConvertedSystem system)
{
    if (baseScript.Contains("Individual Spawn Rates", StringComparison.Ordinal))
    {
        throw new InvalidOperationException("Base script already appears to contain the individual spawn system.");
    }
    if (baseScript.Contains("ISR_", StringComparison.Ordinal))
    {
        throw new InvalidOperationException("Base script already contains ISR_ symbols.");
    }

    var lines = SplitLines(baseScript).ToList();
    var endGlobals = lines.FindIndex(line => line.Trim().Equals("endglobals", StringComparison.Ordinal));
    if (endGlobals < 0)
    {
        throw new InvalidOperationException("Could not find the main endglobals marker.");
    }

    lines.InsertRange(endGlobals, system.Globals);

    var functionsInsert = endGlobals + system.Globals.Count + 1;
    lines.InsertRange(functionsInsert, system.Functions);

    ReplaceTechDelayUnlock(lines);
    AddStartupCalls(lines);

    var patched = string.Join("\r\n", lines);
    ValidatePatchedScript(patched);
    return patched;
}

static void ReplaceTechDelayUnlock(List<string> lines)
{
    for (var i = 0; i < lines.Count - 1; i++)
    {
        if (lines[i].Trim().Equals("call EnableTrigger(gg_trg_ten)", StringComparison.Ordinal) &&
            lines[i + 1].Trim().Equals("call EnableTrigger(gg_trg_five)", StringComparison.Ordinal))
        {
            lines[i] = "        call ISR_UnlockTech1SpawnGroups()";
            lines.RemoveAt(i + 1);
            return;
        }
    }

    throw new InvalidOperationException("Could not find the Tech Delay T1 legacy trigger-enable block.");
}

static void AddStartupCalls(List<string> lines)
{
    for (var i = 0; i < lines.Count; i++)
    {
        if (lines[i].Trim().Equals("call ConditionalTriggerExecute(gg_trg_set_base)", StringComparison.Ordinal))
        {
            lines.InsertRange(i + 1, new[]
            {
                "    call ISR_DisableLegacySpawnTriggers()",
                "    call ISR_Start()",
            });
            return;
        }
    }

    throw new InvalidOperationException("Could not find the spawn setup initialization block.");
}

static void ValidatePatchedScript(string patched)
{
    var checks = new Dictionary<string, int>
    {
        ["function ISR_Start takes nothing returns nothing"] = 1,
        ["function ISR_DisableLegacySpawnTriggers takes nothing returns nothing"] = 1,
        ["call ISR_DisableLegacySpawnTriggers()"] = 1,
        ["call ISR_Start()"] = 1,
        ["call ISR_UnlockTech1SpawnGroups()"] = 1,
    };

    foreach (var (needle, expected) in checks)
    {
        var actual = CountOccurrences(patched, needle);
        if (actual != expected)
        {
            throw new InvalidOperationException($"Expected {expected} occurrence(s) of '{needle}', found {actual}.");
        }
    }

    if (patched.Contains("library IndividualSpawnRates", StringComparison.Ordinal) ||
        patched.Contains("endlibrary", StringComparison.Ordinal) ||
        patched.Contains("private function", StringComparison.Ordinal) ||
        patched.Contains("private constant", StringComparison.Ordinal) ||
        patched.Contains("private timer", StringComparison.Ordinal) ||
        patched.Contains("private boolean", StringComparison.Ordinal))
    {
        throw new InvalidOperationException("Patched script still contains vJASS-only syntax.");
    }
}

static int CountOccurrences(string haystack, string needle)
{
    var count = 0;
    var index = 0;
    while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
    {
        count++;
        index += needle.Length;
    }
    return count;
}

static string RemovePrivate(string line)
{
    var leading = line.Length - line.TrimStart().Length;
    var trimmed = line[leading..];
    if (trimmed.StartsWith("private ", StringComparison.Ordinal))
    {
        return line[..leading] + trimmed["private ".Length..];
    }
    return line;
}

static IReadOnlyList<string> SplitLines(string text)
{
    return text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
}

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

public sealed record ConvertedSystem(IReadOnlyList<string> Globals, IReadOnlyList<string> Functions);
public sealed record MpqHeader(uint HeaderSize, uint ArchiveSize, ushort Version, uint SectorSize, uint HashOffset, uint BlockOffset, uint HashEntries, uint BlockEntries);
public sealed record HashEntry(uint Name1, uint Name2, ushort Locale, ushort Platform, uint BlockIndex);
public sealed record BlockEntry(uint Offset, uint CompressedSize, uint FileSize, uint Flags);
