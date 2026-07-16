$ErrorActionPreference = "Stop"

param(
    [string]$MapPath = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot "..")) "basemap\MyMap_DEV.w3x"),
    [string]$OutDir = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot "..")) "extracted-reference")
)

$sourceTool = "C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\work\MpqExtract\MpqExtract.csproj"

if (-not (Test-Path -LiteralPath $sourceTool)) {
    throw "MpqExtract tool was not found at $sourceTool"
}

dotnet run --project $sourceTool -- $MapPath $OutDir
if ($LASTEXITCODE -ne 0) {
    throw "Map extraction failed with exit code $LASTEXITCODE"
}

Write-Host "Extracted $MapPath to $OutDir"
