#!/usr/bin/env bash
# Claude Code Stop hook (lesson 05.3): keeps the agent working while the loop gates fail.
# Install: copy to .claude/hooks/stop-gate.sh in ai-layer-lab and register it in .claude/settings.json
# (see labs/module-05/hooks/settings.example.json). Runs from the repository root.
#
# Exit 0 = let the turn end. Exit 2 = block the stop; stderr is shown to the agent as the reason.
# Claude Code sets "stop_hook_active": true when a Stop hook already blocked once; we then let the
# turn end to avoid an endless loop. The hook is a nudge; CI is the wall.
input=$(cat)
if printf '%s' "$input" | grep -Eq '"stop_hook_active"[[:space:]]*:[[:space:]]*true'; then
  exit 0
fi

if ! out=$(dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules \
             --min-tests "${LOOPGATE_MIN_TESTS:-6}" 2>&1); then
  printf '%s\n' "$out" | grep -E "FAIL|tests:" >&2
  echo "Validation gates failed. Fix the cause. Do not skip, delete or weaken tests or rules to pass." >&2
  exit 2
fi
exit 0
