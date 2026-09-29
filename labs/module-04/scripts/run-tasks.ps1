# Runs every task in a task set K times, each in a fresh headless Claude Code session, read-only.
# usage: pwsh run-tasks.ps1 <tasks.json> <repo-dir> <out-dir> [K=3] [-Only T04,T05]
# Each run is saved as <out-dir>/<task>.r<k>.json (Claude Code --output-format json), ready for ContextLab grade.
param(
    [Parameter(Mandatory)] [string] $Tasks,
    [Parameter(Mandatory)] [string] $Repo,
    [Parameter(Mandatory)] [string] $Out,
    [int] $K = 3,
    [string[]] $Only = @()
)
$ErrorActionPreference = 'Stop'
$lab = Split-Path -Parent $PSScriptRoot
$tasksPath = (Resolve-Path $Tasks).Path
New-Item -ItemType Directory -Force $Out | Out-Null
$outDir = (Resolve-Path $Out).Path

$prompts = dotnet run --project (Join-Path $lab 'tools/ContextLab') -- prompts $tasksPath --only ($Only -join ",")
# Isolation (checked 2026-09-29): --setting-sources project,local keeps ~/.claude/settings.json and personal skills out,
# but NOT ~/.claude/CLAUDE.md. claudeMdExcludes in a --settings file keeps that out; auto memory is switched off too.
# Personal instructions change results, and runs must be reproducible on anyone's machine.
$homeFwd = [Environment]::GetFolderPath('UserProfile').Replace('\', '/')
$iso = [IO.Path]::GetTempFileName()
[IO.File]::WriteAllText($iso, (@{ claudeMdExcludes = @("$homeFwd/.claude/CLAUDE.md", "$homeFwd/.claude/rules/**"); autoMemoryEnabled = $false } |
    ConvertTo-Json -Compress))   # no BOM
Push-Location $Repo
try {
    foreach ($line in $prompts) {
        $id, $prompt = $line -split "`t", 2
        if (-not $prompt) { continue }   # skip any non-task line
        for ($i = 1; $i -le $K; $i++) {
            Write-Host "$id run $i/$K"
            # dontAsk: anything that would need approval (edits, shell) is denied; reads in the repo are allowed.
            # --setting-sources + --settings $iso: your ~/.claude settings, skills and CLAUDE.md stay out (see Isolation above).
            $json = $null | claude -p $prompt --output-format json --permission-mode dontAsk --setting-sources project,local --settings $iso
            Set-Content -Path (Join-Path $outDir "$id.r$i.json") -Value $json -Encoding utf8
        }
    }
}
finally { Pop-Location; Remove-Item -Force $iso -ErrorAction SilentlyContinue }
Write-Host "done: $outDir"
