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
