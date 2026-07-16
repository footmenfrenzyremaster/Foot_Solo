$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$tool = Join-Path $root "tools\ReplaceWar3MapScript"
$baseMap = Join-Path $root "basemap\MyMap_DEV.w3x"
$script = Join-Path $root "src\war3map.j"
$output = Join-Path $root "builds\MyMap_test.w3x"

dotnet run --project $tool -- $baseMap $script $output
if ($LASTEXITCODE -ne 0) {
    throw "Map build failed with exit code $LASTEXITCODE"
}

Write-Host "Built $output"
