# AI-layer architecture

One page per repository that describes its AI layer: who owns it, what is in it, where each piece lives in each agent tool, and how it is verified. Introduced in [03.1](../lessons/module-03/lesson-01.md) and [03.2](../lessons/module-03/lesson-02.md); extended in later modules (skills, MCP, hooks, CI, governance).

## 1. Ownership and change process

- Owner (person or team):
- CODEOWNERS entries covering the AI layer (catch-all first; AI-layer lines after it; CODEOWNERS owns itself):
- Branch protection / ruleset requires code-owner review: yes / no
- Changelog file and entry format (what changed · why/incident · evidence · verified by · reviewer):
- Review cadence (e.g. after each major migration, quarterly):

## 2. Component inventory

| Component | File(s) | Loads (always / on demand / on event) | Advise or enforce | Scope (org / user / repo / path / task) | Owner | Last verified |
|---|---|---|---|---|---|---|
| Root rules | | | | | | |
| Path-scoped rules | | | | | | |
| Skills | | | | | | |
| Sub-agents | | | | | | |
| MCP servers | | | | | | |
| Hooks | | | | | | |
| Permissions / settings | | | | | | |
| Scripts | | | | | | |
| CI agents | | | | | | |
| Eval tests / probes | | | | | | |
| Governance docs | | | | | | |

Decision rule: fact everywhere → root rules · fact in one area → path-scoped rule · procedure used sometimes → skill · must always hold → test, hook or CI gate · external system → MCP · isolated investigation → sub-agent.

## 3. Portability matrix

Canonical source of repo-wide rules: `AGENTS.md` (imported by `CLAUDE.md` with `@AGENTS.md`) / other:

| Rule or component | Claude Code | Cursor | GitHub Copilot | Other agent | How drift is prevented |
|---|---|---|---|---|---|
| | | | | | |

## 4. Verification

- Rules lint command and result:
- Convention tests guarding rules:
- Probes (prompt · must mention · must not mention · runs · results):
- Evaluation suite (Module 7):

## 5. Known gaps and risks

- Stale sources the agent may still read:
- Rules that are advisory but should be enforced:
- Components with execution rights (MCP, hooks) and their review status (Module 9):
