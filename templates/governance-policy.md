# AI-layer governance policy — template

Who owns the AI layer, how it changes, and what happens when an agent gets something wrong. One page is the goal; if it grows past two, something belongs in a runbook instead. Lesson: [11.5 · Governance of the AI layer](../lessons/module-11/lesson-05.md). Worked example: [Contoso governance.md](../labs/module-11/governance/governance.md).

Version __ · date __ · owner __ · changes to this document are class C (section 3).

## 1. Scope

List every file, folder and setting that steers an agent in this repository:

- Rules and instructions: e.g. `AGENTS.md`, `CLAUDE.md`, `.cursor/rules/`, `.github/copilot-instructions.md`
- Skills, subagents, hooks, settings: e.g. `.claude/**`
- Tool connections: e.g. `.mcp.json`
- Agent jobs: workflow files, their settings, prompts, schemas and policies
- Evaluation: task sets, graders, baselines, gate thresholds
- Repository or organization variables: agent version, kill switch, ceilings
- The changelog and this document

## 2. Owners

| Area | Owner (team) | Second owner | How enforced |
|---|---|---|---|
| Rules, skills, prompts | | | CODEOWNERS |
| Permissions, tools, MCP, hooks, workflows, secrets | | security | CODEOWNERS |
| Evals and gate thresholds | | | CODEOWNERS |
| Each automation | see its automation card | | |
| Kill switch, budget breaker | | on-call | |

## 3. Change classes

| Class | What | Review | Evidence in the PR |
|---|---|---|---|
| A — wording | | | |
| B — behavior | | | |
| C — capability and trust | | | |
| D — platform (agent, model) | | | |

Rule for ambiguous changes: the higher class applies.

## 4. System evolution (agent mistake → rules update → eval)

1. Record: incident id, what happened, where caught, cost — within __.
2. Reproduce: regression task from the incident's wording; it must **fail** on the current layer.
3. Fix: smallest change in the file that owns the concern; prefer a deterministic check.
4. Gate: task and smoke suite pass.
5. Log: changelog entry (Class, Changed, Why, Incident, Regression task, Verified with numbers, Reviewed by), checked in CI.
6. Review: on the next review's agenda.

Emergency path (who, which classes, what must follow within how long):

## 5. Cadence

| When | What is reviewed | Who attends |
|---|---|---|
| Weekly | | |
| Monthly | | |
| Quarterly | | |

## 6. Thresholds

| Metric | Threshold | Action when breached | Source of the number |
|---|---|---|---|
| CI review precision / acted-on rate | | | |
| Automation acceptance vs break-even | | | |
| Daily agent spend | | | |
| Golden task floor | | | |
| Attack suite floor | | | |

## 7. Overrides and deactivation

- Who may override a gate, how it is recorded, when it is reviewed:
- Kill switch: who may turn agents off, who may turn them back on, and what must exist first:
- Retiring an automation:

## 8. Out of scope

What this policy deliberately leaves to other documents (enterprise architecture, data and compliance, adoption).
