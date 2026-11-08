# Claude Code mode-independent permission settings (blockWritesOutsideWorkingDirectories, denyInsteadOfPrompting)

- **Topic ID:** claude-code-mode-independent-permission-settings
- **Status:** WAIT
- **Next review:** 2026-12-06
- **Course change:** none
- **Candidates:** C-20261103-01, C-20261107-01

## Summary

Two opt-in `permissions` settings shipped a week apart. `permissions.blockWritesOutsideWorkingDirectories` (v2.1.314, 2026-10-30) makes Write, Edit and NotebookEdit refuse paths outside the working directories in every permission mode — a direct control for the file-tool gap 09.5 Limitations describes. `permissions.denyInsteadOfPrompting` (v2.1.319, 2026-11-06) denies any tool call that would wait on a permission prompt and continues the turn, in terminal sessions, subagents and teammates — the interactive/multi-agent counterpart of the headless `dontAsk` behaviour 11.1 teaches.

## Evidence

- https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.314, v2.1.319) — re-fetched 2026-11-08, both bullets present (vendor changelog). Release tags cross-checked by the daily runs.
- code.claude.com/docs/en/permissions — DNS-unreachable from the 2026-11-08 review session; exact syntax, defaults and the pre-setting behaviour for subagents/teammates not confirmed.

## What would change the decision

The first-party permissions reference documenting both settings (name, default, scope, interaction with permission modes and managed settings), and what a subagent/teammate does on a would-block prompt without `denyInsteadOfPrompting`. Then: one sentence in 09.5 Limitations (and possibly 08.4) for the write boundary, and one in 10.1/10.2 cross-referenced to 11.1 for prompt denial. Nothing either lesson says is currently wrong.

## History

- 2026-11-08 — WAIT — two single product settings, changelog-only, one and two weeks old; docs page unreachable; no lesson claim contradicted — weekly/2026-W45.md — candidates C-20261103-01, C-20261107-01
