# Coding agents in CI: zero-privilege issue/PR text reaching runner secrets

- **Topic ID:** coding-agent-ci-untrusted-issue-secrets
- **Status:** REJECT
- **Next review:** on new evidence
- **Course change:** none as a topic (the Claude Code advisory GHSA-fg94-h982-f3mm was used as evidence in the 2026-W41 FIX to lessons/module-09/lesson-05.md)
- **Candidates:** C-20261008-01

## Summary

A headless coding agent in a CI job that reads issue or PR text (untrusted), holds CI secrets (private data) and can reach the network (external comms) completes the lethal trifecta taught in 09.1. Two 2026 CVEs, presented at Black Hat USA 2026 ("Trusted Enough to Run", Novee Security), show attackers with no repository privileges reaching runner secrets: a pre-approved WebFetch domain in Claude Code, and workspace `.env` loading before the sandbox in Gemini CLI's headless mode.

## Evidence

- https://github.com/anthropics/claude-code/security/advisories/GHSA-fg94-h982-f3mm — fetched directly 2026-10-11: CVE-2026-54316, CVSS v4 6.0, fixed 2.1.163, bare-hostname huggingface.co pre-approval as covert exfiltration channel (vendor advisory, primary).
- Gemini CLI GHSA-wpqr-6v78-jr5g / CVE-2026-12537 — the GitHub advisory URL returned 404 to this session and the API is session-restricted; known only via secondary trackers (cybersecuritynews.com, vpncentral.com, feedly) that disagree on CVSS (7.8 vs 10.0).
- Black Hat talk — secondary only (CSA research note, dailysecurityreview.com); slides not reachable.
- https://github.com/anthropics/claude-code-action/releases/tag/v1.0.273 (2026-11-12; fetched 2026-11-15) — the Claude Code action now denies code-running commands on PRs whose author lacks write access unless `restrict_untrusted_prs: false`, and restores `GITHUB_ENV`/`GITHUB_PATH`/git config at session end: the vendor moving the untrusted-PR boundary into the tool. Cited in 11.3 by the W46 FIX; relevant to the 2026-11-22 joint review.
- 2026-11-22: GHSA-wpqr-6v78-jr5g still returns 404 to the weekly review; no primary Gemini CLI text or Black Hat slides reachable after six weeks. Meanwhile claude-code-action v1.0.273–v1.0.285 moved the untrusted-PR boundary into the tool (code-running commands denied, config taken from the base/default branch for authors without write access); 11.1 now teaches loading agent config from the base branch (W47 FIX).

## What would change the decision

The Gemini CLI advisory read from its primary page (to settle severity and mechanism), or the Black Hat slides read directly; then a short case-study callout in 09.1 or 09.6 (not a new lesson) is a reasonable FIX. 07.6 and 09.6 already teach "do not run agents on untrusted PRs with secrets", so the bar is a verified, two-vendor example, not new teaching.

## History

- 2026-10-11 — WAIT — Claude Code half primary-verified and used in the 09.5 FIX; Gemini CLI half and talk framing secondary-only with conflicting severity — weekly/2026-W41.md — candidates C-20261008-01
- 2026-11-22 — REJECT — the teachable part is already in the course (09.5 advisory, 11.1 rules 1–3 and the W47 base-branch paragraph, 07.6/09.6); the remaining Gemini CLI half adds only vendor diversity and its primary text has stayed unreachable for six weeks. Reopen if a primary advisory or talk shows a mechanism the course does not teach — weekly/2026-W47.md — candidates C-20261008-01
