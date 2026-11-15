# Claude Code idle-triggered automatic compaction

- **Topic ID:** claude-code-idle-compaction
- **Status:** ADD
- **Next review:** n/a
- **Course change:** lessons/module-04/lesson-04.md (one paragraph under "Compact, clear or continue?", "idle compaction" in the Five moves table, one source; instructor-notes sentence; glossary "Compaction" entry extended; no new lesson, quiz unchanged)
- **Candidates:** C-20261014-01

## Summary

Since v2.1.299 (2026-10-14) Claude Code compacts a 200K+ token terminal conversation left idle for about 54 minutes; v2.1.302 added an on/off toggle in `/config`. A third compaction trigger beside manual `/compact` and API-side compaction taught in 04.4.

## Evidence

- https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.299, v2.1.302) — vendor changelog.
- https://code.claude.com/docs/en/settings-reference#idlecompaction — first-party docs, fetched 2026-11-15: key `idleCompaction`, default `true`, any settings file; on for interactive terminal/VS Code/desktop sessions with auto-compact on, the one-hour cache TTL, context at or above 200,000 tokens (`CLAUDE_CODE_IDLE_COMPACT_MIN_TOKENS`, floor 100,000) and about an hour idle; notice "Compacted while idle, before the prompt cache expired"; `/config` row (v2.1.302+); `DISABLE_AUTO_COMPACT` also disables it.
- Independent coverage (secondary, consistent with the docs): https://www.dsebastien.net/2026-10-16-claude-code-idle-compaction-cheaper-to-resume-but-from-a-summary/, https://www.developersdigest.tech/blog/claude-code-idle-compaction-turn-off-cache-cost-2026

## What would change the decision

Met on 2026-11-15. If the default, threshold or idle time changes, update the 04.4 paragraph. Original condition: the first-party context/compaction docs describing the default and the setting name, so a one-line note can go into 04.4 and the module-04 lab README (learners testing compaction survival may hit it after a long break). Nothing 04.4 says is currently wrong.

## History

- 2026-10-18 — WAIT — single product feature, changelog-only, no 04.4 claim contradicted; revisit when docs are reachable — weekly/2026-W42.md — candidates C-20261014-01
- 2026-11-15 — ADD — first-party settings reference now documents the default, threshold and setting name, the condition set at W42; a third compaction trigger the learner will meet, added as one paragraph in 04.4, not a lesson — weekly/2026-W46.md — candidates C-20261014-01
