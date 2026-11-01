# Verify a pin after fetching, not only at checkout (Claude Code plugin SHA-pin bypass)

- **Topic ID:** verify-pins-after-fetch
- **Status:** WAIT
- **Next review:** 2026-11-29
- **Course change:** none
- **Candidates:** C-20261029-01

## Summary

Claude Code's plugin install flow ran `git checkout <pinned SHA>` without confirming that the resulting HEAD equalled the pin; an upstream owner could create a branch named with the same 40-character string and have the user install attacker code despite the hash pin (CVE-2026-86063, fixed 2.1.179). The durable point for 09.3's "pin and detect" section: a pin is only a control if something verifies, after fetching, that what you got is what you pinned.

## Evidence

- https://github.com/anthropics/claude-code/security/advisories/GHSA-pq7j-f95f-qfcv — primary advisory, fetched 2026-11-01: CVSS v4 7.7, CWE-494/CWE-829, affected < 2.1.179, fixed 2.1.179, published 2026-10-09, reported by Or Nevo Michrowski (Air Security). Exploitation needed a git host that allows SHA-shaped branch names.

## What would change the decision

Decide together with `claude-code-plugin-marketplace-mods` at its 2026-11-29 review: if plugins enter the course, cite this there; either way a one-sentence addition to 09.3 ("verify the pin after fetch") is the likely outcome. Deferred this week only because the ADD budget went to 07.3 and the plugin topic is due in four weeks.

## History

- 2026-11-01 — WAIT — sound, primary-sourced example of an unverified pin; held for the 2026-11-29 plugin-marketplace review because this week's single ADD went to 07.3 — weekly/2026-W44.md — candidates C-20261029-01
