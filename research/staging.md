# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261026-01 · KnotScryer: detecting insecure cross-skill compositions (clean skills, unsafe combinations)

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-10-26
- **Date published:** 2026-10-23
- **Source:** https://arxiv.org/abs/2610.32155 (fetched and verified this session)
- **Organization/researchers:** Yihao Li, Chuanpu Fu, Yizhou Zhao, Miao Chen, Qi Li, Ke Xu (academic preprint; affiliations not shown on the abstract page)
- **Category:** skills/subagents/hooks (cross-cutting with agent-security)
- **What changed:** New empirical result: agent "skills" that are each individually benign can still combine into an insecure capability chain when one skill's output feeds another skill's input during agent orchestration — a risk existing malicious-skill scanners miss because they audit one skill at a time.
- **Technical summary:** Introduces KnotScryer: abstracts each skill into a capability summary, builds a "Skill Capability-Flow Graph" whose edges are cross-skill information flow, then searches for paths along which protected/untrusted data reaches an insecure sink. A two-layer check (reachability replay + task-anchored LLM audit) confirms each candidate chain is real and semantically plausible. Scanned 13,598 "clean" skills pulled from a skill marketplace (ClawHub); found 612 insecure compositions exploitable across 3 agent frameworks and 3 LLMs. Under GLM-5.1, raised the exploitation rate from 16.5–18.2% (tag-match / graph-only baselines) to 85.6%, while cutting the search space 85.3%. 13.7% of the flagged chains consisted entirely of skills a single-skill scanner had labeled safe.
- **Why it might matter:** Module 06 teaches skill authoring/review (incl. "review `allowed-tools` of repo skills") and module 09 teaches excessive agency / supply-chain risk, but neither currently names cross-skill *composition* as a distinct failure mode — i.e., that per-skill review is insufficient even when every skill passes it individually. `lookup "skill composition"` returns only module-06's generic skill-mapping line, no coverage of this specific risk.
- **Evidence of adoption:** none — single non-peer-reviewed arXiv preprint, authors' own pipeline and a marketplace scrape; no vendor or production tool implements this detector yet.
- **Major organizations using it:** none named.
- **Open-source implementation:** not confirmed from the fetched abstract page (not stated as public there); needs a repo link check before use.
- **Paper:** https://arxiv.org/abs/2610.32155
- **Code:** unconfirmed.
- **Relationship to existing course material:** lessons/module-06/lesson-01.md–lesson-04.md (skill fields, `allowed-tools`, evaluation); lessons/module-09/lesson-03.md (supply chain), lesson-04.md (excessive agency). New angle, not currently taught.
- **Potential course lesson:** not a new lesson; candidate for a short addition to module-06's skill-review checklist ("review compositions, not just individual skills") and/or module-09's supply-chain or excessive-agency lesson.
- **Confidence:** medium — concrete numbers (612/13,598) come only from the authors' own pipeline; no independent replication yet.
- **Recommendation:** monitor; revisit for a small addition to module 06 or 09 if replicated or echoed by a vendor advisory.

### C-20261026-02 · Steganographic prompt injection via MCP tool outputs + decode-time provenance binding defense

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-10-26
- **Date published:** 2026-10-22
- **Source:** https://arxiv.org/abs/2610.31974 (fetched and verified this session)
- **Organization/researchers:** Eric Yocam, Varghese Vaidyan, Paris Kalathas, Patrick Tague, Bruce DeBruhl, Yong Wang (academic preprint; affiliations not shown on the abstract page)
- **Category:** mcp/tool-protocols (cross-cutting with agent-security)
- **What changed:** Demonstrates that indirect-prompt-injection instructions can be hidden inside images (LSB, DCT, or metadata steganography) so they pass text-based content scanners, then surface as plain instructions only after an MCP tool decodes the image mid-pipeline — bypassing defenses that only inspect text. Proposes "decode-time provenance binding" as a countermeasure.
- **Technical summary:** Decoded output is marked untrusted via HMAC-authenticated trust labels plus a provenance ledger the MCP layer maintains; context-taint tracking propagates the untrusted status through downstream reasoning; sink enforcement blocks critical tool calls influenced by tainted content even if a later, compromised agent step presents a forged "trusted" label (enforcement keys on lineage, not a single label). In a real MCP testbed, 3 steganographic carriers x 24 adaptive payloads: undefended pipeline 100% attack success; keyword/regex filtering still let 62.5% through; the proposed mechanism blocked all 24 while an audited "trusted-promotion" step raised legitimate-task completion from 67% to 100%.
- **Why it might matter:** Module 08 (MCP) and module 09 (security) teach "tool results are untrusted content" and keyword/regex-style mitigations as one layer, but neither names non-text (image) carriers as a way to bypass that layer. `lookup "steganograph"` returns no matches — new to the course and registry. The reported 62.5% bypass rate against keyword/regex filtering is a concrete number against a mitigation the course already recommends.
- **Evidence of adoption:** none — single preprint, authors' own testbed and attack set; no production MCP client/server implements this defense.
- **Major organizations using it:** none named.
- **Open-source implementation:** the search summary of the abstract states "code and data are public," but the exact repo URL was not independently confirmed from the page fetched this session — verify before citing.
- **Paper:** https://arxiv.org/abs/2610.31974
- **Code:** unconfirmed (claimed public; link not verified this session).
- **Relationship to existing course material:** lessons/module-08/lesson-01.md, lesson-02.md (MCP threat model, tool permission layers); lessons/module-09/lesson-02.md (prompt injection), lesson-05.md (defense-in-depth stack). New attack-carrier angle, not currently taught.
- **Potential course lesson:** not a new lesson; candidate one-paragraph addition to 09.2 or 08.1 noting text-based scanners can be bypassed by non-text injection carriers decoded mid-pipeline.
- **Confidence:** low-medium — single preprint, 24-payload attack set, no independent replication, code link unverified.
- **Recommendation:** monitor; verify the code/data link and look for independent replication or a vendor response (e.g. an MCP SDK advisory) before considering a course addition.

### C-20261028-01 · Third real-world recurrence of the write-time symlink TOCTOU pattern in Claude Code (Edit tool, macOS sandbox escape)

- **Class:** B
- **Kind:** correction (new evidence for an existing lesson, not a wrong fact)
- **Date discovered:** 2026-10-28
- **Date published:** 2026-10-26
- **Source:** https://github.com/anthropics/claude-code/security/advisories/GHSA-5xjx-9qx6-jcrv (fetched and verified this session)
- **Organization/researchers:** Anthropic (advisory published by maintainer `ddworken`); reported by Sunder Singh (hackerone.com/mahoraga007)
- **Category:** agent-security (cross-cutting with coding-agents)
- **What changed:** A second, distinct CVE (CVE-2026-88968, CVSS v4.0 7.7) in the same vulnerability *class* that lessons/module-09/lesson-05.md already teaches by name via GHSA-5j29-h97v-84ch (CVE-2026-103435, fixed 2.1.129, published 2026-10-05): time-of-check/time-of-use (TOCTOU) symlink following around a file write. This new one is scoped to the Edit tool specifically and to a sandbox-escape attack path on macOS, not the general path-validation bug the first advisory fixed.
- **Technical summary:** Claude Code validated the Edit tool's target path (resolving symlinks) at check time but did not bind that check to the underlying directory inode/file descriptor. Inside Claude Code's Bash sandbox, a process from a malicious repo or dependency script can atomically swap an in-workspace parent directory for a symlink after validation but before the write; the unsandboxed Edit implementation then follows the symlink and overwrites an attacker-chosen, user-writable file outside the workspace. CWE-59 (link following) + CWE-367 (TOCTOU race). Affected < 2.1.251; fixed in 2.1.251.
- **Why it might matter:** 09.5's further-reading list already cites two CVEs as its running example of "a layer that looked sufficient had a gap a different layer should have covered" (GHSA-5j29 symlink TOCTOU; GHSA-fg94 bare-hostname pre-approval). A *third*, independent instance of the identical symlink-TOCTOU root cause — nine-plus months after the first was fixed, in a different tool path (Edit) and a different attack surface (sandbox escape, not general path validation) — is good evidence for the lesson's own thesis that this is a structural, recurring risk category in agentic file-editing tools, not a one-off bug. `lookup "TOCTOU"` / `"symlink"` confirm only the first CVE is currently cited.
- **Evidence of adoption:** N/A — this is a disclosed-and-patched vulnerability in Claude Code itself, not a third-party tool or technique with an adoption question.
- **Major organizations using it:** N/A.
- **Open-source implementation:** N/A (vulnerability advisory, not a tool).
- **Paper:** none.
- **Code:** fix shipped in `@anthropic-ai/claude-code` 2.1.251 (npm); advisory has no public PoC.
- **Relationship to existing course material:** lessons/module-09/lesson-05.md lines ~20-23 and ~176-180 (sandboxing/defense-in-depth further reading, cites GHSA-5j29-h97v-84ch and GHSA-fg94-h982-f3mm as the two running examples); `last_verified: 2026-10-11` predates this advisory.
- **Potential course lesson:** not a new lesson; candidate one-line addition to 09.5's further-reading list as a third bullet (same format as the existing two), and bump `last_verified`. Strengthens, does not contradict, the existing "use every layer together" argument.
- **Confidence:** high — primary GitHub Security Advisory, directly fetched and verified this session, with CVE, CVSS, affected/fixed versions and a dated attack-scenario writeup.
- **Recommendation:** review for a small FIX/addition to 09.5 (third bullet + `last_verified` bump); not a new topic and not urgent enough to be A on its own.

### C-20261029-01 · Claude Code plugin commit-SHA pin bypass via same-named branch (CVE-2026-86063)

- **Class:** A
- **Kind:** new-topic (also strengthens the deferred "Claude Code plugin marketplace and Mods" topic and qualifies the "pin and verify" principle in 09.3)
- **Date discovered:** 2026-10-29
- **Date published:** 2026-10-09
- **Source:** https://github.com/anthropics/claude-code/security/advisories/GHSA-pq7j-f95f-qfcv (fetched and verified this session)
- **Organization/researchers:** Anthropic (advisory published by maintainer `ddworken`); reported by Or Nevo Michrowski of Air Security
- **Category:** skills/subagents/hooks (cross-cutting with agent-security supply-chain)
- **What changed:** A confirmed, patched vulnerability in Claude Code's plugin install flow shows that "pin to a commit hash" — the exact mitigation 09.3 teaches generically (pin and detect drift) — is not self-verifying: the install code checked out the pinned 40-char SHA with `git checkout <SHA>` but never confirmed the resulting HEAD actually equalled that SHA.
- **Technical summary:** An attacker who controls a plugin's upstream git repo can create a branch whose name is the identical 40-character string as the SHA a user pinned to. `git checkout` resolves a SHA-shaped ref ambiguously; here it silently resolved to the attacker's branch tip instead of the immutable commit, so a user who diligently pinned to a reviewed hash still got attacker-controlled code with no warning. CWE-494 (Download of Code Without Integrity Check) + CWE-829 (Inclusion of Functionality from Untrusted Control Sphere). CVSS v4 7.7 (High). Affected `@anthropic-ai/claude-code` npm < 2.1.179; fixed 2.1.179.
- **Why it might matter:** Lessons/module-09/lesson-03.md teaches "pin and detect" (pinned snapshots of MCP tool descriptions, DRIFT checks) as the general supply-chain defense pattern, and `last_verified: 2026-09-28` predates this advisory. This CVE is a different instance of the same *class* of gap one level up the stack: pinning a plugin to a commit hash is also presented in the ecosystem as sufficient, and here it demonstrably was not, because the verification step (confirm HEAD == pin) was missing, not the pin itself. It also directly extends `research/deferred/claude-code-plugin-marketplace-mods.md`, which is WAITing specifically for "first-party docs that define... how marketplaces are trusted" — this advisory is exactly that kind of first-party evidence about plugin-install trust. `lookup "plugin install"` / `"pinned commit hash plugin"` confirm neither this mechanism nor any correction is yet recorded.
- **Evidence of adoption:** N/A — disclosed-and-patched vulnerability in Claude Code itself, not a third-party tool with an adoption question. Standard auto-update users already had the fix before public disclosure; manual-update users needed to upgrade to >= 2.1.179.
- **Major organizations using it:** N/A.
- **Open-source implementation:** N/A (vulnerability advisory, not a tool).
- **Paper:** none.
- **Code:** fix shipped in `@anthropic-ai/claude-code` 2.1.179 (npm); advisory has no public PoC.
- **Relationship to existing course material:** lessons/module-09/lesson-03.md ("pin and detect" principle, `last_verified: 2026-09-28`); `research/deferred/claude-code-plugin-marketplace-mods.md` (WAIT, next review 2026-11-29, explicitly wants first-party trust-model evidence like this).
- **Potential course lesson:** not a new lesson; candidate for (a) a short addition to 09.3's "pin and detect" section making explicit that a pin must be *verified after the fact*, not just specified at checkout time, citing this CVE as the concrete case, and (b) feeding it into the plugin-marketplace deferred topic's evidence list ahead of its 2026-11-29 review.
- **Confidence:** high — primary GitHub Security Advisory, directly fetched and verified this session, with CVE, CVSS, precise mechanism, affected/fixed versions and credited researcher.
- **Recommendation:** review for a small FIX/addition to 09.3 (pin-verification nuance + `last_verified` bump) and attach as new evidence to the plugin-marketplace deferred topic; not large enough alone to justify a new lesson.

### C-20261030-01 · Claude Code v2.1.314 adds `permissions.blockWritesOutsideWorkingDirectories` — a built-in control for exactly the write-outside-workspace failure class 09.5 already teaches

- **Class:** A
- **Kind:** correction (new first-party mitigation for an existing lesson's threat model, not a wrong fact)
- **Date discovered:** 2026-10-30
- **Date published:** 2026-10-30 (version has no date in the changelog; this is the first session to find it — immediately follows v2.1.312, last logged 2026-10-28)
- **Source:** https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md, "## 2.1.314" section (fetched and verified this session, primary vendor changelog)
- **Organization/researchers:** Anthropic (Claude Code team)
- **Category:** agent-security (cross-cutting with coding-agents config)
- **What changed:** Claude Code shipped a new settings key, `permissions.blockWritesOutsideWorkingDirectories`, that makes the Write, Edit and NotebookEdit tools refuse any path outside the configured working directories, in every permission mode (not just `ask`/`deny` rules that can be bypassed by an unanticipated path or a race).
- **Technical summary:** Verbatim changelog line (confirmed twice from the raw file this session): "Added `permissions.blockWritesOutsideWorkingDirectories`: Write, Edit and NotebookEdit refuse paths outside the working directories in every permission mode." This is a containment check enforced by the tool implementation itself, independent of the pattern-matched allow/deny/ask rules 09.5 already teaches, and independent of the sandbox's network/filesystem isolation. It is the same category of control that two real CVEs already cited in 09.5 show a gap in: GHSA-5j29-h97v-84ch (symlink TOCTOU on write, fixed 2.1.129) and GHSA-5xjx-9qx6-jcrv (a second, Edit-tool-specific symlink TOCTOU, fixed 2.1.251, already staged as `C-20261028-01`) both resulted in a write landing outside the project directory despite path validation at approval time. A setting that re-checks containment at the moment of write, "in every permission mode," is presented by the vendor as closing that general gap rather than one specific symlink bug — though nothing in the changelog line states *how* it binds the check to avoid the same TOCTOU class (inode/fd binding vs. re-resolving the path), so whether it actually closes the race or only narrows it is not yet verifiable from this one line.
- **Why it might matter:** lessons/module-09/lesson-05.md teaches exactly this stack (permission rules, sandbox, hooks) and uses the *absence* of a workspace-containment backstop as implicit context for why the two symlink CVEs mattered; it does not yet mention this setting, and `last_verified: 2026-10-11` predates this release. `lookup "blockWritesOutsideWorkingDirectories"` and `lookup "working directories permission"` confirm it is new to the course and registry. This is also relevant to module-08/lesson-02.md, which already documents `permissions.allow/deny/ask` syntax (`lookup` match) and would be the natural place for the setting name itself, with 09.5 cross-referencing it as the containment layer in the defense-in-depth stack.
- **Evidence of adoption:** N/A — first-party Claude Code configuration option, not a third-party tool; ships to auto-update users as soon as they're on >= 2.1.314.
- **Major organizations using it:** N/A.
- **Open-source implementation:** N/A (built into `@anthropic-ai/claude-code`, not a separate tool).
- **Paper:** none.
- **Code:** shipped in `@anthropic-ai/claude-code` 2.1.314 (npm, per vendor changelog).
- **Relationship to existing course material:** lessons/module-09/lesson-05.md (layered defenses; the two symlink-TOCTOU CVEs already cited as further reading, `last_verified: 2026-10-11`); lessons/module-08/lesson-02.md (`permissions.allow/deny/ask` settings syntax — natural home for the setting name itself).
- **Potential course lesson:** not a new lesson; candidate for (a) a short addition to 09.5 naming the setting as an explicit containment backstop alongside the existing permission/sandbox/hook layers, directly tied to the symlink-TOCTOU CVEs it would have mitigated, and `last_verified` bump, and (b) a one-line mention in 08.2's permissions-settings reference.
- **Confidence:** high for the feature's existence and exact wording (primary vendor changelog, fetched twice this session, identical text both times); low-medium for *how completely* it closes the TOCTOU race specifically, since the changelog does not describe the enforcement mechanism — note this caveat if it is written into the lesson.
- **Recommendation:** review for a small FIX/addition to 09.5 (new containment-setting bullet + `last_verified` bump) and a one-line mention in 08.2; verify the enforcement mechanism (does it bind to an fd/inode, or re-resolve the path?) against first-party docs before stating it closes the TOCTOU race rather than just narrowing it, if `code.claude.com` becomes reachable to a future session (it was not reachable from this one).
