#!/usr/bin/env bash
# Builds brownfield-demo: Contoso Billing (Module 3) with a dated, multi-author git history,
# the company around it (tickets, PRDs, handbook), an empty-AI-layer "before" tag, the AI layer
# from Modules 3-11 as its own commits up to the "after" tag, and a pre-baked solution branch.
#
#   bash scripts/assemble-demo.sh --out ../../../brownfield-demo            # the demo repository
#   bash scripts/assemble-demo.sh --out /tmp/demo-151 --break 15.1         # lesson 15.1 break
#   bash scripts/assemble-demo.sh --out /tmp/demo-152 --break 15.2         # lesson 15.2 break
#
# Run from labs/module-15 (or anywhere: paths are resolved from this script). Needs git and bash.
# The trainer's commits use DEMO_AUTHOR_NAME / DEMO_AUTHOR_EMAIL (default "Demo Trainer",
# trainer@contoso.example). Use a public address or your GitHub noreply address, never your work e-mail.
set -euo pipefail

HERE=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
LABS=$(cd "$HERE/.." && pwd)
ROOT=$(cd "$LABS/.." && pwd)
OUT=""
BREAK=""
while [ $# -gt 0 ]; do
  case "$1" in
    --out) OUT=$2; shift 2 ;;
    --break) BREAK=$2; shift 2 ;;
    *) echo "usage: assemble-demo.sh --out <new dir> [--break 15.1|15.2]" >&2; exit 2 ;;
  esac
done
[ -n "$OUT" ] || { echo "usage: assemble-demo.sh --out <new dir> [--break 15.1|15.2]" >&2; exit 2; }
[ -e "$OUT" ] && { echo "$OUT already exists; pick a new directory" >&2; exit 2; }
case "$BREAK" in
  "") BREAKDIR="" ;;
  15.1) BREAKDIR="$HERE/break/15.1-leaky-demo" ;;
  15.2) BREAKDIR="$HERE/break/15.2-spoiled-before" ;;
  *) echo "unknown --break $BREAK (15.1 or 15.2)" >&2; exit 2 ;;
esac

TRAINER_NAME=${DEMO_AUTHOR_NAME:-Demo Trainer}
TRAINER_EMAIL=${DEMO_AUTHOR_EMAIL:-trainer@contoso.example}

mkdir -p "$OUT"
OUT=$(cd "$OUT" && pwd)
git -C "$OUT" init -q
git -C "$OUT" symbolic-ref HEAD refs/heads/main
git -C "$OUT" config core.autocrlf false

who() {
  case "$1" in
    moshe) echo "Moshe Levi|moshe.levi@contoso.example" ;;
    avi) echo "Avi Ben-David|avi.bendavid@contoso.example" ;;
    dana) echo "Dana Katz|dana.katz@contoso.example" ;;
    yossi) echo "Yossi Mizrahi|yossi.mizrahi@contoso.example" ;;
    noa) echo "Noa Shapiro|noa.shapiro@contoso.example" ;;
    trainer) echo "$TRAINER_NAME|$TRAINER_EMAIL" ;;
    trainer-work) echo "$TRAINER_NAME|demo.trainer@northwindpayments.test" ;;  # 15.1 break only
    *) echo "unknown author $1" >&2; exit 2 ;;
  esac
}

resolve() {
  case "$1" in
    B:*) echo "$LABS/module-03/brownfield/${1#B:}" ;;
    H:*) echo "$HERE/history/${1#H:}" ;;
    K:*) echo "$HERE/${1#K:}" ;;
    L:*) echo "$LABS/${1#L:}" ;;
    R:*) echo "$ROOT/${1#R:}" ;;
    X:*) echo "$BREAKDIR/${1#X:}" ;;
    *) echo "bad source '$1'" >&2; exit 2 ;;
  esac
}

copy_dir() {  # copy a folder without build output
  local from=$1 to=$2
  (cd "$from" && find . -type f ! -path '*/bin/*' ! -path '*/obj/*' ! -path '*/out/*') | while IFS= read -r f; do
    mkdir -p "$to/$(dirname "$f")"
    cp "$from/$f" "$to/$f"
  done
}

TAGS=()
apply() {
  local op
  op=$(echo "$1" | sed -E 's/^ +//; s/ +$//')
  [ -z "$op" ] && return
  case "$op" in
    "mv "*) local a=${op#mv }; git -C "$OUT" mv -- "${a%%>*}" "${a#*>}" ;;
    "rm "*) git -C "$OUT" rm -q -r --ignore-unmatch -- "${op#rm }" ;;
    "touch "*) mkdir -p "$OUT/$(dirname "${op#touch }")"; : > "$OUT/${op#touch }" ;;
    "tag "*) TAGS+=("${op#tag }") ;;
    +*)
      local s=${op#+} src dst from
      if [[ $s == *'>'* ]]; then src=${s%%>*}; dst=${s#*>}; else src=$s; dst=${s#B:}; fi
      from=$(resolve "$src")
      if [ -d "$from" ]; then
        mkdir -p "$OUT/$dst"; copy_dir "$from" "$OUT/$dst"
      elif [ -f "$from" ]; then
        mkdir -p "$OUT/$(dirname "$dst")"; cp "$from" "$OUT/$dst"
      else
        echo "missing source: $from" >&2; exit 1
      fi ;;
    *) echo "bad operation '$op'" >&2; exit 2 ;;
  esac
}

commit_line() {
  local date=$1 author=$2 msg=$3 ops=$4 name email
  IFS='|' read -r name email <<< "$(who "$author")"
  TAGS=()
  local IFS_OLD=$IFS; IFS=';'
  for op in $ops; do IFS=$IFS_OLD; apply "$op"; IFS=';'; done
  IFS=$IFS_OLD
  git -C "$OUT" add -A
  GIT_AUTHOR_NAME="$name" GIT_AUTHOR_EMAIL="$email" GIT_COMMITTER_NAME="$name" GIT_COMMITTER_EMAIL="$email" \
  GIT_AUTHOR_DATE="$date 10:00:00 +0200" GIT_COMMITTER_DATE="$date 10:00:00 +0200" \
    git -C "$OUT" commit -q --allow-empty -m "$msg"
  for t in "${TAGS[@]+"${TAGS[@]}"}"; do
    GIT_COMMITTER_NAME="$name" GIT_COMMITTER_EMAIL="$email" GIT_COMMITTER_DATE="$date 10:00:00 +0200" \
      git -C "$OUT" tag -a "$t" -m "$t"
  done
}

replay() {  # replay one manifest; @BREAK pulls in the break's manifest of the same name
  local file=$1
  while IFS= read -r line || [ -n "$line" ]; do
    line=${line%$'\r'}
    case "$line" in ''|'#'*) continue ;; esac
    if [ "$line" = "@BREAK" ]; then
      if [ -n "$BREAKDIR" ] && [ -f "$BREAKDIR/$(basename "$file")" ]; then replay "$BREAKDIR/$(basename "$file")"; fi
      continue
    fi
    IFS=$'\t' read -r date author msg ops <<< "$line"
    commit_line "$date" "$author" "$msg" "$ops"
  done < "$file"
}

replay "$HERE/history.tsv"
replay "$HERE/layer.tsv"

# Pre-baked fallback for lesson 15.4: the demo ticket done, on its own branch. main stays at the after tag.
git -C "$OUT" checkout -q -b demo/BILL-97-done
commit_line "2026-09-28" trainer "BILL-97 pre-baked fallback: revenue report on a Dapper repository" \
  "rm src/Contoso.Billing/Legacy/MonthlyRevenueReport.cs ; +K:solution/BILL-97/src/>src/ ; +K:solution/BILL-97/tests/>tests/ ; +K:solution/BILL-97/research/>research/ ; +K:solution/BILL-97/plans/>plans/"
git -C "$OUT" checkout -q main

echo "brownfield-demo assembled in $OUT"
git -C "$OUT" log --oneline --decorate -n 3 main
echo "$(git -C "$OUT" rev-list --count main) commits on main; tags: $(git -C "$OUT" tag | tr '\n' ' ')"
