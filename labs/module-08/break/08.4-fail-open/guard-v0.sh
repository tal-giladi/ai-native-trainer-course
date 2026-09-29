#!/usr/bin/env bash
# guard-v0.sh - the first version of the architecture guard (lesson 08.4 break). DO NOT INSTALL outside the break branch.
# PreToolUse on Edit|Write. Looks for DbContext in the edit and blocks with exit 2.
set -euo pipefail
input=$(cat)
file=$(printf '%s' "$input" | grep -o '"file_path":"[^"]*"' | cut -d'"' -f4)
new=$(printf '%s' "$input" | grep -o '"new_string":"[^"]*"' | cut -d'"' -f4)
case "$file" in
  *src/*.cs)
    if printf '%s' "$new" | grep -q 'DbContext'; then
      echo "Blocked: ADR 0007 - data access is Dapper repositories, not EF Core." >&2
      exit 2
    fi ;;
esac
exit 0
