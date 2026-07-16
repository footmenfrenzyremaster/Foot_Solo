param(
    [string]$Message = "Update WC3 projects $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot
try {
    if (-not (Test-Path -LiteralPath '.git')) {
        throw "Git repository not found at $repoRoot"
    }

    git pull --rebase --autostash origin main
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not update from GitHub. Resolve the reported Git problem before syncing.'
    }

    git add --all -- maps shared tools README.md AGENTS.md .gitignore
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not stage the WC3 project changes.'
    }

    git diff --cached --quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host 'No WC3 changes need to be uploaded.'
        exit 0
    }

    git commit -m $Message
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not create the Git commit.'
    }

    git push origin main
    if ($LASTEXITCODE -ne 0) {
        throw 'The commit was saved locally, but GitHub upload failed.'
    }

    $localCommit = (git rev-parse HEAD).Trim()
    $remoteLine = git ls-remote origin refs/heads/main
    $remoteCommit = ($remoteLine -split '\s+')[0]
    if ($localCommit -ne $remoteCommit) {
        throw 'GitHub verification failed: local and remote commits do not match.'
    }

    Write-Host "GitHub sync complete: $localCommit"
}
finally {
    Pop-Location
}
