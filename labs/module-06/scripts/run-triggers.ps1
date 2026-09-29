# Trigger test: does the model load <skill> on its own for each query? K fresh headless sessions per query.
# usage: pwsh run-triggers.ps1 <skill-name> <queries.tsv> <repo-dir> <out.tsv> [K=3]
#   queries.tsv: header, then  id<TAB>expected(1|0)<TAB>query
#   out.tsv:     id<TAB>expected<TAB>run<TAB>fired, ready for `SkillCheck triggers`
# Read-only: plan mode, at most 5 turns (the model often reads the repo for a turn or two before it loads a skill). Detection as of 2026-09: a tool_use of the Skill tool with
# input {"skill": "<name>"} in stream-json output, or a Read of .claude/skills/<name>/SKILL.md (the model
# sometimes opens the file directly instead). Adjust the patterns if your version differs.
param(
    [Parameter(Mandatory)] [string] $Skill,
    [Parameter(Mandatory)] [string] $Queries,
    [Parameter(Mandatory)] [string] $Repo,
    [Parameter(Mandatory)] [string] $Out,
    [int] $K = 3
)
$ErrorActionPreference = 'Stop'
$Queries = (Resolve-Path $Queries).Path
$outDir = Split-Path -Parent ([IO.Path]::GetFullPath($Out))
$raw = Join-Path $outDir "raw-$Skill"
New-Item -ItemType Directory -Force $raw | Out-Null
$rows = @("id`texpected`trun`tfired")
# Isolation (checked 2026-09-29): --setting-sources project,local keeps ~/.claude/settings.json and personal skills out,
# but NOT ~/.claude/CLAUDE.md. claudeMdExcludes in a --settings file keeps that out; auto memory is switched off too.
# Personal instructions change results, and runs must be reproducible on anyone's machine.
$homeFwd = [Environment]::GetFolderPath('UserProfile').Replace('\', '/')
$iso = [IO.Path]::GetTempFileName()
[IO.File]::WriteAllText($iso, (@{ claudeMdExcludes = @("$homeFwd/.claude/CLAUDE.md", "$homeFwd/.claude/rules/**"); autoMemoryEnabled = $false } |
    ConvertTo-Json -Compress))   # no BOM
Push-Location $Repo
try {
    foreach ($line in (Get-Content $Queries | Select-Object -Skip 1)) {
        $id, $expected, $query = $line -split "`t", 3
        if (-not $query) { continue }
        for ($i = 1; $i -le $K; $i++) {
            $log = Join-Path $raw "$id.r$i.jsonl"
            claude -p $query --output-format stream-json --verbose --permission-mode plan --max-turns 5 --setting-sources project,local --settings $iso 2>$null |
                Set-Content -Encoding utf8 $log
            $text = Get-Content -Raw $log
            $fired = if ($text -match '"name"\s*:\s*"Skill"' -and $text -match ('"skill"\s*:\s*"' + [regex]::Escape($Skill) + '"') -or $text -match ('"file_path"\s*:\s*"[^"]*skills[\\/]+' + [regex]::Escape($Skill) + '[\\/]+SKILL\.md"')) { 1 } else { 0 }
            $rows += "$id`t$expected`t$i`t$fired"
            Write-Host "$id run $i/$K fired=$fired"
        }
    }
}
finally { Pop-Location; Remove-Item -Force $iso -ErrorAction SilentlyContinue }
$rows | Set-Content -Encoding utf8 $Out
Write-Host "done: $Out (raw logs in $raw)"
