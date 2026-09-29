#!/usr/bin/env sh
# Trigger test: does the model load <skill> on its own for each query? K fresh headless sessions per query.
# usage: sh run-triggers.sh <skill-name> <queries.tsv> <repo-dir> <out.tsv> [K=3]
#   queries.tsv: header, then  id<TAB>expected(1|0)<TAB>query
#   out.tsv:     id<TAB>expected<TAB>run<TAB>fired, ready for `SkillCheck triggers`
# Read-only: plan mode, at most 5 turns (the model often reads the repo for a turn or two before it loads a skill). Costs about (queries x K) short sessions.
# Detection (as of 2026-09): a model-invoked skill appears in stream-json output as a tool_use of the
# Skill tool with input {"skill": "<name>"}, or as a Read of .claude/skills/<name>/SKILL.md (the model
# sometimes opens the file directly instead). If your version logs it differently, change FIRED below.
set -eu
[ $# -ge 4 ] || { echo "usage: sh run-triggers.sh <skill> <queries.tsv> <repo-dir> <out.tsv> [K]"; exit 2; }
SKILL="$1"
QUERIES="$(cd "$(dirname "$2")" && pwd)/$(basename "$2")"
REPO="$3"
mkdir -p "$(dirname "$4")"
OUTDIR="$(cd "$(dirname "$4")" && pwd)"
OUT="$OUTDIR/$(basename "$4")"
K="${5:-3}"
RAW="$OUTDIR/raw-$SKILL"
mkdir -p "$RAW"
TAB="$(printf '\t')"
printf 'id\texpected\trun\tfired\n' > "$OUT"
# Isolation (checked 2026-09-29): --setting-sources project,local keeps ~/.claude/settings.json and personal skills out,
# but NOT ~/.claude/CLAUDE.md. claudeMdExcludes in a --settings file keeps that out; auto memory is switched off too.
# Personal instructions change results, and runs must be reproducible on anyone's machine.
HOME_FWD="$(cd ~ && (pwd -W 2>/dev/null || pwd))"
ISO="$(mktemp)"; trap 'rm -f "$ISO"' EXIT
printf '{"claudeMdExcludes":["%s/.claude/CLAUDE.md","%s/.claude/rules/**"],"autoMemoryEnabled":false}\n' "$HOME_FWD" "$HOME_FWD" > "$ISO"
cd "$REPO"
tail -n +2 "$QUERIES" | while IFS="$TAB" read -r id expected query; do
  [ -n "$query" ] || continue
  i=1
  while [ "$i" -le "$K" ]; do
    log="$RAW/$id.r$i.jsonl"
    claude -p "$query" --output-format stream-json --verbose --permission-mode plan --max-turns 5 --setting-sources project,local --settings "$ISO" \
      < /dev/null > "$log" 2>/dev/null || true
    fired=0
    if grep -q '"name"[[:space:]]*:[[:space:]]*"Skill"' "$log" &&
       grep -q "\"skill\"[[:space:]]*:[[:space:]]*\"$SKILL\"" "$log"; then fired=1; fi
    if grep -Eq "\"file_path\"[[:space:]]*:[[:space:]]*\"[^\"]*skills[\\/]+$SKILL[\\/]+SKILL\.md\"" "$log"; then fired=1; fi
    printf '%s\t%s\t%s\t%s\n' "$id" "$expected" "$i" "$fired" >> "$OUT"
    echo "$id run $i/$K fired=$fired"
    i=$((i + 1))
  done
done
echo "done: $OUT (raw logs in $RAW)"
