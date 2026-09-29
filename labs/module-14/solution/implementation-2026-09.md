# Ground-Bound-Build-Prove — implementation notes (as of 2026-09)

> **Reference (illustrative).** Everything tool-specific lives here, not in the method. When a tool changes, this file changes; the method does not. Review quarterly.

| Step | Claude Code (primary) | Cursor | GitHub Copilot | Any agent |
|---|---|---|---|---|
| Ground | Explore subagent or `/prime` skill writing `research/<ticket>.md` | Ask mode + a rules file listing the reuse search | Agent mode with a prompt file | read-only session, brief written to a file |
| Bound | plan mode, then `/plan-feature` writing `plans/<ticket>.md` | Plan mode | Plan agent | plan file with an out-of-scope list |
| Build | fresh session after `/clear`, reads both files | new chat | new session | fresh context, reads files |
| Prove | `LoopGate all`, PreToolUse hook blocking test edits, CI | same gates in CI | same gates in CI | gates in CI the agent cannot edit |

Tool versions this was last checked with: see `AI-LAYER-CHANGELOG.md` in the team repository.
