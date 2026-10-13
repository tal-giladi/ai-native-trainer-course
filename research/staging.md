# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261012-01 · Claude Code plugin marketplace + "Claude Mods" extensibility layer

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-10-12
- **Date published:** 2026-10-01 through 2026-10-09 (v2.1.287–v2.1.296)
- **Source:** https://github.com/anthropics/claude-code/releases (primary — official repo release notes, dated), cross-checked against https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md
- **Organization/researchers:** Anthropic (Claude Code team)
- **Category:** agent-config/context / skills/subagents/hooks
- **What changed:** Across v2.1.287–v2.1.296 (Oct 1–9 2026) Claude Code shipped: (a) `claude plugin install --marketplace`, installing plugins from a marketplace rather than only local/git paths; (b) "Claude Mods," described by the release notes as letting plugins "modify deeper behavior" beyond skills/subagents/hooks, plus a built-in "You should know" mod and a `$.ui.notify` / `$.ui.selection()` mod API; (c) smaller related items: an `effort` parameter on the Agent (subagent) tool, `autoCompactWindow` for subagents, `CLAUDE_CODE_WORKFLOW_SUBAGENT_MODEL` to pin all workflow agents to one model.
- **Technical summary:** This is a packaging/distribution layer on top of the skills + subagents + hooks + MCP primitives the course already teaches in module-03/module-06: a plugin now bundles those plus "mod" hooks into one installable unit discoverable from a marketplace, and mods get a small JS-like API (`$.ui.*`, `$.model.complete` with caching) to alter the host UI/behavior, not just register a tool or inject context.
- **Why it might matter:** If it holds up, it is the next rung above "write your own SKILL.md/subagent" — a distribution and governance question (who publishes to a marketplace, what a mod can see/do) that module-06 (skills/subagents authoring) and module-09 (supply chain / tool poisoning) would both want to address, parallel to how 09.3 already treats MCP server trust.
- **Evidence of adoption:** none yet findable independent of Anthropic's own release notes — (vendor claim / own changelog only); no independent security review or practitioner write-up of "Claude Mods" located this session.
- **Major organizations using it:** none documented yet.
- **Open-source implementation:** the feature ships inside https://github.com/anthropics/claude-code itself; no third-party marketplace or example mod repo located.
- **Paper:** none.
- **Code:** https://github.com/anthropics/claude-code (CHANGELOG.md entries for 2.1.287, 2.1.288, 2.1.292).
- **Relationship to existing course material:** extends module-03 (CLAUDE.md/skills/subagents/hooks/MCP) and module-06 (authoring skills/subagents) and touches module-09's supply-chain treatment of third-party MCP servers (09.3) — no existing lesson mentions plugins or mods by name.
- **Potential course lesson:** too fresh for a lesson yet; worth a short "what's new" watch note for module-06/module-09 once docs.claude.com-class documentation and any third-party security commentary on mod capabilities/trust exist. `code.claude.com` (where the authoritative Plugins/Mods docs would live) was DNS-unreachable again this session — same block noted on 2026-10-07 through 2026-10-11 — so this record is built from GitHub release notes only, not the full spec.
- **Confidence:** low (single primary source, days old, no independent scrutiny yet, authoritative docs page unreachable)
- **Recommendation:** WAIT — monitor for 2–3 more weeks until (a) `code.claude.com/docs/en/plugins` or equivalent becomes reachable/stable, or (b) independent practitioner/security coverage of what a "mod" can access appears. Re-check before adding to module-06 or module-09.

### C-20261013-01 · Anthropic's "Claude Frontier Academy" / Frontier Deployed Engineer (FDE) credential

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-10-13
- **Date published:** 2026-10-02
- **Source:** https://www.anthropic.com/news/claude-frontier-academy (primary, Anthropic's own site, fetched and resolves this session — bare `anthropic.com` is DNS-unreachable from this session, same pattern as `code.claude.com`/`platform.claude.com` on prior days, but `www.anthropic.com` resolved)
- **Organization/researchers:** Anthropic
- **Category:** adoption/change-management (also touches consulting/business)
- **What changed:** Anthropic announced a $100M program, "Claude Frontier Academy," whose first offering is the "Frontier Deployed Engineer (FDE) Residency": a multi-day in-person course (use-case selection → security review → handover, ending in a graded practical) that awards a "Claude Resident Engineer" badge, followed by a 12-week residency in which the engineer leads a real Claude deployment at their own employer, ending in a "Claude Frontier Deployed Engineer" badge. Goal: 10,000 FDEs trained by end of 2027; cohorts run in San Francisco, New York and London; first credentials expected early 2027. Participants are nominated by their employer from Anthropic's Claude Partner Network. Named first-cohort organizations: Accenture, Bain, Capgemini, Commonwealth Bank of Australia, Deloitte, McKinsey, Morgan Stanley, Novo Nordisk, "and others."
- **Technical summary:** Not an API/technical change — Anthropic itself is defining and credentialing a job role ("the highest level of AI fluency who can take Claude from an idea to a system in production") with a named badge and a structured curriculum (classroom + simulated enterprise deployment + graded practical + supervised real residency + final assessment), explicitly modelled on a medical-residency structure.
- **Why it might matter:** This is the frontier lab itself naming and credentialing close to the exact competency this course trains toward — an engineer who can design, secure, deploy and operate AI-native software inside a real enterprise, with security review named as part of the pipeline. It is a concrete, dated market signal (not an opinion piece) that large professional-services and enterprise employers (Accenture, Deloitte, McKinsey, Bain, Morgan Stanley, Novo Nordisk, Commonwealth Bank of Australia) are already nominating staff into a formal program for this role.
- **Evidence of adoption:** (vendor claim) — the $100M figure, 10,000-engineer target, and the partner-company list all come from Anthropic's own announcement; no independent confirmation found from any of the named partner companies' own channels this session.
- **Major organizations using it:** as listed above (first cohorts), per Anthropic's announcement only.
- **Open-source implementation:** none (closed, employer-nominated program).
- **Paper:** none.
- **Code:** none.
- **Relationship to existing course material:** no lesson or registry topic currently mentions this program, "Frontier Deployed Engineer," or "Claude Frontier Academy" (confirmed via `research.py lookup`). Touches module-01 (course framing / why this matters), module-19 (enablement/rollout at employer scale), and module-22 (productization, consulting market for this skill) without contradicting anything any of them currently states.
- **Potential course lesson:** not a lesson on its own. Worth a possible one-line market-validation citation in module-01/lesson-01 or module-22 once the program has actually issued its first credentials (expected early 2027) and, ideally, published a public competency framework/syllabus rather than only a press release — at that point it would be concrete enough to cite as evidence of industry demand for the role this course targets, and possibly as a contrast point ("this course trains the same competency without requiring employer nomination into a partner network").
- **Confidence:** medium (primary source, official page, but the dollar figure, headcount target and partner list are entirely self-reported and the program has not yet produced any graduates or public syllabus to check claims against)
- **Recommendation:** WAIT — monitor until first FDE credentials are issued (expected early 2027) or a public syllabus/competency framework appears; re-check before adding any citation to module-01 or module-22. No course edit justified yet (nothing existing is contradicted; there is no fact to FIX).
