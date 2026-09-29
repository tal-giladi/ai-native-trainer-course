#!/usr/bin/env bash
# Creates one GitHub issue per Contoso ticket in YOUR ai-layer-lab repository, so the integration lab has a
# tracker to read without a Jira site (lesson 08.2). Uses the GitHub CLI with your own login, not the agent.
#   bash integration/import-tickets.sh <owner>/<repo> tickets/*.md
set -euo pipefail
repo=$1; shift
gh label create contoso-ticket --repo "$repo" --color 0e8a16 --description "Imported Contoso Billing ticket" 2>/dev/null || true
for f in "$@"; do
  title=$(head -n 1 "$f" | sed -E 's/^# //')
  if gh issue list --repo "$repo" --search "\"$title\" in:title" --state all --json number --jq length | grep -q '^0$'; then
    gh issue create --repo "$repo" --title "$title" --body-file "$f" --label contoso-ticket
  else
    echo "exists: $title"
  fi
done
