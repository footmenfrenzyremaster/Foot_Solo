const fs = require("fs");
const path = require("path");
const zlib = require("zlib");

const mapPath = process.argv[2];
const outDir = process.argv[3];

if (!mapPath || !outDir) {
  console.error("Usage: node mpq-extract.js <map.w3x> <out-dir>");
  process.exit(2);
}

const cryptTable = new Uint32Array(0x500);
let seed = 0x00100001;
for (let index1 = 0; index1 < 0x100; index1++) {
  for (let index2 = index1, i = 0; i < 5; i++, index2 += 0x100) {
    seed = (seed * 125 + 3) % 0x2aaaab;
    const temp1 = (seed & 0xffff) << 16;
    seed = (seed * 125 + 3) % 0x2aaaab;
    cryptTable[index2] = (temp1 | (seed & 0xffff)) >>> 0;
  }
}

function hashString(str, hashType) {
  let seed1 = 0x7fed7fed;
  let seed2 = 0xeeeeeeee;
  const upper = Buffer.from(str.replace(/\//g, "\\").toUpperCase(), "latin1");
  for (const ch of upper) {
    const value = cryptTable[(hashType << 8) + ch];
    seed1 = (value ^ ((seed1 + seed2) >>> 0)) >>> 0;
    seed2 = (ch + seed1 + seed2 + ((seed2 << 5) >>> 0) + 3) >>> 0;
  }
  return seed1 >>> 0;
}

function decryptTable(buffer, key) {
  const out = Buffer.from(buffer);
  let seed1 = key >>> 0;
  let seed2 = 0xeeeeeeee;
  for (let offset = 0; offset < out.length; offset += 4) {
    seed2 = (seed2 + cryptTable[0x400 + (seed1 & 0xff)]) >>> 0;
    const value = (out.readUInt32LE(offset) ^ ((seed1 + seed2) >>> 0)) >>> 0;
    out.writeUInt32LE(value, offset);
    seed1 = ((((~seed1 >>> 0) << 21) >>> 0) + 0x11111111 | (seed1 >>> 11)) >>> 0;
    seed2 = (value + seed2 + ((seed2 << 5) >>> 0) + 3) >>> 0;
  }
  return out;
}

function ensureDir(filePath) {
  fs.mkdirSync(path.dirname(filePath), { recursive: true });
}

function decompressMulti(data, expectedSize) {
  if (data.length === expectedSize) return data;
  const method = data[0];
  const payload = data.subarray(1);
  if (method & 0x02) return zlib.inflateSync(payload);
  if (method & 0x08) return zlib.inflateSync(payload);
  throw new Error(`Unsupported MPQ compression method 0x${method.toString(16)}`);
}

function parseHeader(buf) {
  if (buf.subarray(0, 4).toString("latin1") !== "MPQ\x1a") {
    throw new Error("Not an MPQ archive");
  }
  const headerSize = buf.readUInt32LE(4);
  const version = buf.readUInt16LE(12);
  return {
    headerSize,
    archiveSize: buf.readUInt32LE(8),
    version,
    sectorSize: 512 << buf.readUInt16LE(14),
    hashTableOffset: buf.readUInt32LE(16),
    blockTableOffset: buf.readUInt32LE(20),
    hashTableEntries: buf.readUInt32LE(28),
    blockTableEntries: buf.readUInt32LE(32),
  };
}

const archive = fs.readFileSync(mapPath);
const header = parseHeader(archive);

const hashRaw = archive.subarray(header.hashTableOffset, header.hashTableOffset + header.hashTableEntries * 16);
const blockRaw = archive.subarray(header.blockTableOffset, header.blockTableOffset + header.blockTableEntries * 16);
const hashTable = decryptTable(hashRaw, hashString("(hash table)", 3));
const blockTable = decryptTable(blockRaw, hashString("(block table)", 3));

const hashes = [];
for (let i = 0; i < header.hashTableEntries; i++) {
  const o = i * 16;
  hashes.push({
    name1: hashTable.readUInt32LE(o),
    name2: hashTable.readUInt32LE(o + 4),
    locale: hashTable.readUInt16LE(o + 8),
    platform: hashTable.readUInt16LE(o + 10),
    blockIndex: hashTable.readUInt32LE(o + 12),
  });
}

const blocks = [];
for (let i = 0; i < header.blockTableEntries; i++) {
  const o = i * 16;
  blocks.push({
    offset: blockTable.readUInt32LE(o),
    compressedSize: blockTable.readUInt32LE(o + 4),
    fileSize: blockTable.readUInt32LE(o + 8),
    flags: blockTable.readUInt32LE(o + 12),
  });
}

function findBlockIndex(name) {
  const h1 = hashString(name, 1);
  const h2 = hashString(name, 2);
  const start = hashString(name, 0) & (header.hashTableEntries - 1);
  for (let step = 0; step < header.hashTableEntries; step++) {
    const entry = hashes[(start + step) & (header.hashTableEntries - 1)];
    if (entry.blockIndex === 0xffffffff) return -1;
    if (entry.blockIndex !== 0xfffffffe && entry.name1 === h1 && entry.name2 === h2) {
      return entry.blockIndex;
    }
  }
  return -1;
}

function readFileByBlock(block, name) {
  if (!block || !(block.flags & 0x80000000)) throw new Error(`Missing file ${name}`);
  let data = archive.subarray(block.offset, block.offset + block.compressedSize);
  if (block.flags & 0x00010000) {
    const key = hashString(path.basename(name), 3);
    data = decryptTable(data, key);
  }
  if (block.flags & 0x01000000) {
    return data.subarray(0, block.fileSize);
  }
  if (block.flags & 0x00000200) {
    return decompressMulti(data, block.fileSize);
  }
  if (block.compressedSize === block.fileSize) return data;
  return decompressMulti(data, block.fileSize);
}

function readNamed(name) {
  const idx = findBlockIndex(name);
  if (idx < 0) return null;
  return readFileByBlock(blocks[idx], name);
}

fs.mkdirSync(outDir, { recursive: true });

const seedNames = [
  "(listfile)",
  "war3map.j",
  "scripts\\war3map.j",
  "war3map.w3e",
  "war3map.w3i",
  "war3map.wtg",
  "war3map.wct",
  "war3map.wts",
  "war3map.doo",
  "war3mapUnits.doo",
  "war3map.wpm",
  "war3map.shd",
  "war3mapMap.blp",
  "war3mapPreview.tga",
  "war3mapMisc.txt",
  "war3mapSkin.txt",
  "war3mapExtra.txt",
  "war3mapImported\\listfile.txt",
];

const names = new Set(seedNames);
const listfile = readNamed("(listfile)");
if (listfile) {
  for (const line of listfile.toString("latin1").split(/\r?\n/)) {
    const clean = line.trim();
    if (clean) names.add(clean);
  }
}

const extracted = [];
const failed = [];
for (const name of names) {
  try {
    const data = readNamed(name);
    if (!data) continue;
    const safeName = name.replace(/\\/g, "/").replace(/^\/+/, "");
    const dest = path.join(outDir, "files", safeName);
    ensureDir(dest);
    fs.writeFileSync(dest, data);
    extracted.push(name);
  } catch (err) {
    failed.push({ name, error: err.message });
  }
}

fs.writeFileSync(path.join(outDir, "mpq-header.json"), JSON.stringify(header, null, 2));
fs.writeFileSync(path.join(outDir, "block-table.json"), JSON.stringify(blocks, null, 2));
fs.writeFileSync(path.join(outDir, "extraction-report.json"), JSON.stringify({
  mapPath,
  extractedCount: extracted.length,
  extracted,
  failed,
  hasListfile: Boolean(listfile),
  blockCount: blocks.length,
}, null, 2));

console.log(`Extracted ${extracted.length} files to ${outDir}`);
if (failed.length) console.log(`Failed ${failed.length} files; see extraction-report.json`);
