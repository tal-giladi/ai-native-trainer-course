# Claude Code idle-triggered automatic compaction

- **Topic ID:** claude-code-idle-compaction
- **Status:** WAIT
- **Next review:** 2026-11-15
- **Course change:** none
- **Candidates:** C-20261014-01

## Summary

Since v2.1.299 (2026-10-14) Claude Code compacts a 200K+ token terminal conversation left idle for about 54 minutes; v2.1.302 added an on/off toggle in `/config`. A third compaction trigger beside manual `/compact` and API-side compaction taught in 04.4.

## Evidence

- https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.299, v2.1.302) — vendor changelog only. Docs page (code.claude.com) unreachable from the review session.

## What would change the decision

The first-party context/compaction docs describing the default and the setting name, so a one-line note can go into 04.4 and the module-04 lab README (learners testing compaction survival may hit it after a long break). Nothing 04.4 says is currently wrong.

## History

- 2026-10-18 — WAIT — single product feature, changelog-only, no 04.4 claim contradicted; revisit when docs are reachable — weekly/2026-W42.md — candidates C-20261014-01
