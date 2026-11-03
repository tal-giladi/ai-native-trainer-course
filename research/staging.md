# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261103-01 · Claude Code adds `permissions.blockWritesOutsideWorkingDirectories`

- **Class:** A
- **Kind:** new-topic
- **Date discovered:** 2026-11-03
- **Date published:** 2026-10-30 (shipped in v2.1.314)
- **Source:** https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.314 entry); version/date cross-checked against https://github.com/anthropics/claude-code/releases/tag/v2.1.314 (published 30 Oct 2026)
- **Organization/researchers:** Anthropic (Claude Code team)
- **Category:** agent-config/context (cross-cutting with agent-security)
- **What changed:** A new permission setting, `permissions.blockWritesOutsideWorkingDirectories`, makes the Write, Edit and NotebookEdit tools refuse any path outside the session's working directories, in **every** permission mode (including `acceptEdits`/`bypassPermissions`-style modes where file writes were previously not path-checked against the workspace boundary).
- **Technical summary:** Opt-in boolean in `.claude/settings.json` (or managed settings) under `permissions`. When set, the three file-writing tools do a workspace-boundary check unconditionally, independent of which permission mode is active, closing the class of bug where a permissive mode (or a mode that doesn't prompt) let a write tool touch a path outside the project.
- **Why it might matter:** Module 9 lesson 5 teaches exactly this gap today: "The sandbox only confines the commands it runs. The agent's own file tools are a separate path." It then documents two real incidents in this same gap — a write-time symlink TOCTOU (GHSA-5j29-h97v-84ch, fixed 2.1.129) and an Edit-tool symlink TOCTOU from inside the sandbox (GHSA-5xjx-9qx6-jcrv, fixed 2.1.251) — as things to defend against with layered controls, without a direct "block all writes outside the workspace, in every mode" switch. This setting is close to exactly that control and would strengthen the "Limitations" paragraph's defense-in-depth stack and the Module 8 permission-configuration material (08.2, 08.4) with one more concrete, official layer.
- **Evidence of adoption:** Official changelog entry, not a vendor marketing claim; shipped (not just announced).
- **Major organizations using it:** n/a (CLI feature, opt-in setting)
- **Open-source implementation:** n/a (closed-source CLI; setting documented in changelog only — code.claude.com/docs/en/permissions could not be reached from this session's network allowlist to cross-check the permissions reference page itself)
- **Paper:** none
- **Code:** none
- **Relationship to existing course material:** lessons/module-09/lesson-05.md ("Limitations" paragraph, the three cited CVE rows); lessons/module-08/lesson-02.md and lesson-04.md (permission configuration). `lookup` for "block writes outside working", "write outside the project", "sandbox" and "working directory" found no existing mention of this setting — not a duplicate.
- **Potential course lesson:** not a new lesson. Candidate FIX/addition for weekly review: one sentence in 09.5's "Limitations" paragraph (and/or a settings-table row in 08.2/08.4) naming `permissions.blockWritesOutsideWorkingDirectories` as the official mode-independent mitigation for the write-outside-workspace gap, with a note that it is opt-in (off by default) so still needs to be turned on.
- **Confidence:** medium — the changelog is primary and the version/date is confirmed via GitHub Releases, but the permissions reference page itself (code.claude.com) was unreachable this session, so exact config syntax/defaults are from the changelog bullet only, not the full doc page.
- **Recommendation:** review for FIX/addition to lessons/module-09/lesson-05.md and lessons/module-08/lesson-02.md or lesson-04.md; re-verify config syntax against https://code.claude.com/docs/en/permissions once reachable, before writing the lesson sentence.
