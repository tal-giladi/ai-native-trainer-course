# Deterministic gate for lesson 02.3: Orders must not use EF Core.
# Usage: pwsh gate/check.ps1 fixture-repo      (exit 0 = pass, 1 = violation)
param([string]$Root = "fixture-repo")
$hits = Get-ChildItem -Path (Join-Path $Root "src/Contoso.Orders") -Recurse -File |
    Select-String -Pattern 'DbContext|Microsoft\.EntityFrameworkCore|\.Include\(|\.ToListAsync\('
if ($hits) {
    Write-Output "FAIL: EF Core usage in src/Contoso.Orders"
    $hits | ForEach-Object { Write-Output ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim()) }
    exit 1
}
Write-Output "PASS: src/Contoso.Orders uses no EF Core"
