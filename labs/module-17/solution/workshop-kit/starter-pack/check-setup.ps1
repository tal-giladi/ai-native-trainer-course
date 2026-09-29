# check-setup.ps1 — run once, two days before the workshop, from the repository root.
# Read-only except for `dotnet build`/`dotnet test` output folders. Never reads or prints credentials.
# Prints one line per check and ends with SETUP OK or SETUP FAILED (<troubleshooting IDs>).
$ErrorActionPreference = 'Continue'
$failed = @()

function Check([string]$name, [bool]$ok, [string]$id, [string]$detail) {
    if ($ok) { Write-Host ("  ok    {0} {1}" -f $name, $detail) }
    else { Write-Host ("  FAIL  {0} {1} -> see {2}" -f $name, $detail, $id); $script:failed += $id }
}

# .NET SDK 8 or newer
$sdk = (& dotnet --version 2>$null)
$major = if ($sdk -match '^(\d+)\.') { [int]$Matches[1] } else { 0 }
Check 'dotnet SDK >= 8' ($major -ge 8) 'T-01' "($sdk)"

# Git 2.30 or newer
$gv = (& git --version 2>$null)
$gitOk = $false
if ($gv -match '(\d+)\.(\d+)') { $gitOk = ([int]$Matches[1] -gt 2) -or ([int]$Matches[1] -eq 2 -and [int]$Matches[2] -ge 30) }
Check 'git >= 2.30' $gitOk 'T-01' "($gv)"

# Repository at ws-0-start, clean
$head = (& git rev-parse HEAD 2>$null)
$tag = (& git rev-parse 'ws-0-start^{commit}' 2>$null)
Check 'at tag ws-0-start' ($head -and $head -eq $tag) 'T-03' ''
$dirty = (& git status --porcelain 2>$null)
Check 'clean working tree' ([string]::IsNullOrWhiteSpace($dirty)) 'T-03' ''

# Build and test (restore from the local feed if the network restore fails)
& dotnet restore Contoso.Billing.sln *> $null
if ($LASTEXITCODE -ne 0) { & dotnet restore Contoso.Billing.sln --source ./packages *> $null }
Check 'restore' ($LASTEXITCODE -eq 0) 'T-02' ''
$test = (& dotnet test Contoso.Billing.sln --no-restore 2>&1 | Out-String)
$passed = if ($test -match 'Passed:\s+(\d+)') { [int]$Matches[1] } else { 0 }
Check 'dotnet test' ($LASTEXITCODE -eq 0 -and $passed -ge 6) 'T-04' "($passed passed)"

# Agent hooks built
Check 'agent hooks built' (Test-Path '.claude/hooks/bin/AgentHooks.dll') 'T-06' ''

# Agent command-line tool present (the name is set by the organizer; claude by default)
$agent = if ($env:WORKSHOP_AGENT) { $env:WORKSHOP_AGENT } else { 'claude' }
$av = (& $agent --version 2>$null)
Check "agent CLI ($agent)" ($LASTEXITCODE -eq 0 -and $av) 'T-07' "($av)"

if ($failed.Count -eq 0) { Write-Host 'SETUP OK'; exit 0 }
Write-Host ("SETUP FAILED ({0})" -f (($failed | Select-Object -Unique) -join ', ')); exit 1
