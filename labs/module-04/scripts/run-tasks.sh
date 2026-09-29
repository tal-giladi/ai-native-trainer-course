#!/usr/bin/env sh
# Runs every task in a task set K times, each in a fresh headless Claude Code session, read-only.
# usage: sh run-tasks.sh <tasks.json> <repo-dir> <out-dir> [K=3] [T04,T05]   (optional list of task ids)
# Each run is saved as <out-dir>/<task>.r<k>.json (Claude Code --output-format json), ready for ContextLab grade.
set -eu
[ $# -ge 3 ] || { echo "usage: sh run-tasks.sh <tasks.json> <repo-dir> <out-dir> [K]"; exit 2; }
TASKS="$(cd "$(dirname "$1")" && pwd)/$(basename "$1")"
REPO="$2"
K="${4:-3}"
LAB="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$3"
OUT="$(cd "$3" && pwd)"

dotnet run --project "$LAB/tools/ContextLab" -- prompts "$TASKS" --only "${5:-}" > "$OUT/prompts.tsv"
# Isolation (checked 2026-09-29): --setting-sources project,local keeps ~/.claude/settings.json and personal skills out,
# but NOT ~/.claude/CLAUDE.md. claudeMdExcludes in a --settings file keeps that out; auto memory is switched off too.
# Personal instructions change results, and runs must be reproducible on anyone's machine.
HOME_FWD="$(cd ~ && (pwd -W 2>/dev/null || pwd))"
ISO="$(mktemp)"; trap 'rm -f "$ISO"' EXIT
printf '{"claudeMdExcludes":["%s/.claude/CLAUDE.md","%s/.claude/rules/**"],"autoMemoryEnabled":false}\n' "$HOME_FWD" "$HOME_FWD" > "$ISO"
cd "$REPO"
TAB="$(printf '\t')"
while IFS="$TAB" read -r id prompt; do
  [ -n "$prompt" ] || continue   # skip any non-task line
  i=1
  while [ "$i" -le "$K" ]; do
    echo "$id run $i/$K"
    # dontAsk: anything that would need approval (edits, shell) is denied; reads in the repo are allowed.
    # --setting-sources + --settings "$ISO": your ~/.claude settings, skills and CLAUDE.md stay out (see Isolation above).
    # </dev/null stops claude from consuming the task list on stdin.
    claude -p "$prompt" --output-format json --permission-mode dontAsk --setting-sources project,local --settings "$ISO" < /dev/null > "$OUT/$id.r$i.json" || true
    i=$((i + 1))
  done
done < "$OUT/prompts.tsv"
echo "done: $OUT"
