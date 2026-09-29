#!/usr/bin/env bash
# check-setup.sh — run once, two days before the workshop, from the repository root.
# Same checks as check-setup.ps1. Never reads or prints credentials.
set -u
failed=()
check() { # name ok(0/1) id detail
  if [ "$2" -eq 0 ]; then echo "  ok    $1 $4"; else echo "  FAIL  $1 $4 -> see $3"; failed+=("$3"); fi
}

sdk=$(dotnet --version 2>/dev/null || echo "none")
major=${sdk%%.*}; [[ "$major" =~ ^[0-9]+$ ]] || major=0
[ "$major" -ge 8 ]; check "dotnet SDK >= 8" $? T-01 "($sdk)"

gv=$(git --version 2>/dev/null | awk '{print $3}'); gmaj=${gv%%.*}; gmin=$(echo "$gv" | cut -d. -f2)
{ [ "${gmaj:-0}" -gt 2 ] || { [ "${gmaj:-0}" -eq 2 ] && [ "${gmin:-0}" -ge 30 ]; }; }; check "git >= 2.30" $? T-01 "($gv)"

[ "$(git rev-parse HEAD 2>/dev/null)" = "$(git rev-parse 'ws-0-start^{commit}' 2>/dev/null)" ]; check "at tag ws-0-start" $? T-03 ""
[ -z "$(git status --porcelain 2>/dev/null)" ]; check "clean working tree" $? T-03 ""

dotnet restore Contoso.Billing.sln >/dev/null 2>&1 || dotnet restore Contoso.Billing.sln --source ./packages >/dev/null 2>&1
check "restore" $? T-02 ""
out=$(dotnet test Contoso.Billing.sln --no-restore 2>&1); rc=$?
passed=$(echo "$out" | sed -n 's/.*Passed: *\([0-9]*\).*/\1/p' | head -1)
[ $rc -eq 0 ] && [ "${passed:-0}" -ge 6 ]; check "dotnet test" $? T-04 "(${passed:-0} passed)"

[ -f .claude/hooks/bin/AgentHooks.dll ]; check "agent hooks built" $? T-06 ""

agent=${WORKSHOP_AGENT:-claude}
av=$("$agent" --version 2>/dev/null); [ -n "$av" ]; check "agent CLI ($agent)" $? T-07 "($av)"

if [ ${#failed[@]} -eq 0 ]; then echo "SETUP OK"; exit 0; fi
echo "SETUP FAILED ($(printf '%s\n' "${failed[@]}" | sort -u | paste -sd, -))"; exit 1
