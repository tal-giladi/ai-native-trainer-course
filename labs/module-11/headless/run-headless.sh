#!/usr/bin/env bash
# run-headless.sh — the run contract for one unattended agent job (lesson 11.1).
#
#   bash run-headless.sh <prompt-file> <out.json> [allowed-tools]
#
# Every flag below is a decision you would otherwise leave to defaults:
#   --bare                 load nothing implicitly (no hooks, .mcp.json, CLAUDE.md, ~/.claude); pass the layer explicitly
#   --append-system-prompt-file AGENTS.md   ...which is how the AI layer gets in
#   --settings             deny rules from the Module 9 hardened config (secrets, egress, the agent layer itself)
#   --permission-mode dontAsk + --allowedTools   a fixed tool surface; anything else is denied, never prompted
#   --max-turns / --max-budget-usd               hard caps; hitting one is a result subtype, not a hang
#   --output-format json   so the job can read subtype, cost and permission_denials
#   --no-session-persistence                     nothing written to disk for later resume
# --bare authenticates ONLY with ANTHROPIC_API_KEY (or an apiKeyHelper); it never reads a claude.ai
# login. In CI that is what you want. To try the script locally on a subscription login, run it with
# AGENT_BARE=0 — and know that the run then loads the checkout's CLAUDE.md, hooks and .mcp.json
# (but still not ~/.claude: --setting-sources project,local keeps ~/.claude/settings.json and personal skills out, and
# claudeMdExcludes, merged into the settings file below, keeps ~/.claude/CLAUDE.md out; --setting-sources alone does not,
# checked 2026-09-29).
# The agent's own summary is never the verdict. AgentOps classifies the result, then the job checks the outcome.
set -euo pipefail

prompt_file=${1:?prompt file}
out=${2:?output json}
allowed=${3:-"Read,Grep,Glob,Edit(./src/**),Edit(./tests/**),Bash(dotnet build *),Bash(dotnet test *)"}
: "${AGENT_MAX_TURNS:=15}"
: "${AGENT_MAX_BUDGET_USD:=1.00}"
: "${AGENTOPS:=dotnet run --project tools/AgentOps --}"
bare=--bare; settings=.github/agent-settings.json
if [ "${AGENT_BARE:-1}" = "0" ]; then
  bare="--setting-sources project,local"
  home_fwd="$(cd ~ && (pwd -W 2>/dev/null || pwd))"
  settings="$(mktemp)"; trap 'rm -f "$settings"' EXIT
  # merge without jq (Git Bash ships none): drop the object's closing brace, append two keys
  base="$(cat .github/agent-settings.json)"; base="${base%\}*}"
  printf '%s,\n  "claudeMdExcludes": ["%s/.claude/CLAUDE.md", "%s/.claude/rules/**"],\n  "autoMemoryEnabled": false\n}\n' \
    "$base" "$home_fwd" "$home_fwd" > "$settings"
fi

if [ "${AGENTS_ENABLED:-true}" != "true" ]; then
  echo "agents disabled by kill switch (AGENTS_ENABLED=${AGENTS_ENABLED:-})"; exit 0
fi

set +e
claude $bare -p "$(cat "$prompt_file")" \
  --append-system-prompt-file AGENTS.md \
  --settings "$settings" \
  --permission-mode dontAsk \
  --allowedTools "$allowed" \
  --max-turns "$AGENT_MAX_TURNS" \
  --max-budget-usd "$AGENT_MAX_BUDGET_USD" \
  --output-format json \
  --no-session-persistence > "$out"
agent_exit=$?
set -e
echo "agent exit code: $agent_exit"

# 1. What kind of ending was it? (success / limit / interrupted / success-with-denials)
$AGENTOPS result "$out" --max-cost "$AGENT_MAX_BUDGET_USD"

# 2. Did the task actually happen? Deterministic checks, not the agent's words.
if git diff --quiet; then
  echo "FAIL: no changes in the working tree; the agent's summary is not evidence"; exit 1
fi
dotnet test --nologo
