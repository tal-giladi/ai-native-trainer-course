#!/usr/bin/env sh
# Deterministic gate for lesson 02.3: Orders must not use EF Core.
# Usage: sh gate/check.sh fixture-repo      (exit 0 = pass, 1 = violation)
root="${1:-fixture-repo}"
hits=$(grep -rnE "DbContext|Microsoft\.EntityFrameworkCore|\.Include\(|\.ToListAsync\(" "$root/src/Contoso.Orders" 2>/dev/null)
if [ -n "$hits" ]; then
  echo "FAIL: EF Core usage in src/Contoso.Orders"
  echo "$hits"
  exit 1
fi
echo "PASS: src/Contoso.Orders uses no EF Core"
