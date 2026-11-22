# Claude Code plugin marketplace and "Claude Mods"

- **Topic ID:** claude-code-plugin-marketplace-mods
- **Status:** WAIT
- **Next review:** 2026-11-29
- **Course change:** none
- **Candidates:** C-20261012-01

## Summary

Claude Code v2.1.287–v2.1.302 (Oct 2026) added marketplace plugin installs and "mods": plugin code with a `$.ui.*` / `$.model.*` / hook API that can change the client's UI and behaviour, packaged on top of the skills, subagents, hooks and MCP the course already teaches (modules 03, 06, 09).

## Evidence

- https://github.com/anthropics/claude-code/releases and https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md — vendor release notes only (vendor claim).
- https://github.com/anthropics/claude-code/security/advisories/GHSA-pq7j-f95f-qfcv — first-party advisory on plugin-install trust (SHA pin not verified after checkout, CVE-2026-86063, fixed 2.1.179); recorded as its own topic `verify-pins-after-fetch`, to be decided with this one.
- No independent practitioner or security write-up; no third-party marketplace or example mod found. The docs site (code.claude.com) was DNS-unreachable from every run, including the 2026-10-18 review.
- https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.321, 2026-11-09; via daily/2026-11-10.md) — mods no longer turn on without asking in auto and bypass modes; `.claude/skills` inside `~/.claude/plugins` no longer load; managed `extraKnownMarketplaces` blocks marketplace names declared elsewhere. Vendor changelog; still no first-party page defining what a mod can access.
- 2026-11-22 (found during the W47 review, not staged): the headless docs (https://code.claude.com/docs/en/headless) now say that under `--permission-mode dontAsk` "a mod you installed can still approve some of those calls", linking a permissions section on precedence among rules, hooks and mods. Relevant to 09.5/11.1 if mods become teachable: `--bare` (which the course requires in CI) skips installed plugins.

## What would change the decision

First-party docs that define what a mod can access and how marketplaces are trusted, plus independent security commentary or real third-party marketplaces. Then likely a paragraph in 06.x (distribution) and 09.3 (supply-chain trust), not a new lesson.

## History

- 2026-10-18 — WAIT — release notes only, docs unreachable, no independent coverage or third-party marketplace yet — weekly/2026-W42.md — candidates C-20261012-01
- 2026-11-01 — WAIT — new first-party evidence (GHSA-pq7j-f95f-qfcv) on plugin-install trust attached; still no docs or independent coverage; review date unchanged — weekly/2026-W44.md — candidates C-20261029-01
- 2026-11-15 — WAIT — reappeared in daily/2026-11-10.md with v2.1.321 consent/marketplace changes (changelog only); condition unchanged, keep the 2026-11-29 review — weekly/2026-W46.md — candidates none new
