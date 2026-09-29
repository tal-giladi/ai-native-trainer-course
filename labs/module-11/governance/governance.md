# AI-layer governance — Contoso Billing (reference example, lesson 11.5)

Version 1.0 · 2026-09-28 · Owner: @contoso/billing-leads · Changes to this document are class C.
Built from [templates/governance-policy.md](../../../templates/governance-policy.md). Numbers below come from the Module 7, 9 and 11 labs.

## 1. Scope — what "the AI layer" is here

`AGENTS.md`, `CLAUDE.md`, `.claude/**` (rules, skills, agents, hooks, settings), `.mcp.json`, `.github/workflows/agent-*.yml`, `dep-fix.yml`, `budget-guard.yml`, `.github/agent-settings.json`, `.github/review/**`, `.github/triage/**`, `evals/**` (tasks, graders, baseline, gate thresholds), `AI-LAYER-CHANGELOG.md`, this file. Repository variables `CLAUDE_CODE_VERSION`, `AGENTS_ENABLED`, `AGENT_DAILY_CEILING_USD`.

## 2. Owners

| Area | Owner | Second |
|---|---|---|
| Rules, skills, agents, prompts | @contoso/billing-leads | — |
| Permissions, MCP, hooks, agent workflows, settings, secrets | @contoso/billing-leads | @contoso/security |
| Eval task set, graders, gate thresholds | @contoso/billing-leads (eval maintainer: A. Levi) | — |
| Each automation | named on its automation card | — |
| Kill switch and budget breaker | @contoso/billing-leads | on-call engineer |

Enforced by CODEOWNERS (catch-all first, AI-layer lines after it, 03.1) and branch protection requiring code-owner review plus the `agent-evals`, `security-evals` and `ai-layer-governance` checks.

## 3. Change classes

| Class | Examples | Review | Evidence required in the PR |
|---|---|---|---|
| A — wording | typo, reordering, comments, docs about the layer | 1 owner | smoke gate passes |
| B — behavior | a rule, a skill step, a review or triage prompt, a policy threshold | 1 owner | smoke gate passes; changelog cites numbers; if from an incident, the regression task failed before and passes after |
| C — capability and trust | permissions, allowed tools, MCP servers, hooks, workflows, secrets, triage/review policy files | owner + security | threat model updated (09.1); attack suite gate at 1.0 (09.6); `wflint` clean |
| D — platform | agent version, model | owner | full suite A/B, 24 × 5, paired interval; baseline refreshed in the same PR (07.5, 07.6) |

When in doubt, the higher class applies.

## 4. System evolution — what happens when an agent gets something wrong

1. **Record** (within 1 business day): an incident note `INC-YYYY-NNN` in `NOTES.md` — what the agent did, where it was caught, the cost.
2. **Reproduce**: a regression task in `evals/` from the incident's own wording; run it on the current layer and see it **fail**. Tag it `golden` if it must never recur.
3. **Fix**: the smallest layer change, in the file that owns the concern (`AGENTS.md` for conventions, a hook for enforcement, a policy file for automation limits). Prefer a deterministic check over a sentence when one exists.
4. **Gate**: the task passes, the smoke gate passes.
5. **Log**: a changelog entry with Class, Changed, Why, Incident, Regression task, Verified (numbers), Reviewed by. `AgentOps evolution` enforces it.
6. **Review**: the incident is on the next monthly review's agenda.

**Emergency path.** On-call may merge a class A or B fix without owner review to stop an ongoing incident. Within 24 hours the regression task and the owner review are added and the entry records the override. No emergency path exists for class C or D.

## 5. Cadence

| When | What | Who |
|---|---|---|
| Weekly (15 min) | ledger by job, failures, breaker trips, `needs-human` queue | automation owners |
| Monthly (45 min) | CI review precision and acted-on rate; automation acceptance vs break-even; cost per useful outcome; incidents and overrides; stale or contradictory rules (context audit, 04.5) | owners + one developer from outside the team |
| Quarterly | agent-version upgrade (class D); threat-model refresh; retire automations below break-even for two months; review this document | owners + security |

## 6. Thresholds (current)

| Metric | Threshold | Action when breached |
|---|---|---|
| CI review precision, holdout / acted-on rate | ≥ 67% / ≥ 50% | two months below: back to shadow mode |
| Automation acceptance | above its break-even $a^*$ | two months below: disable and retire |
| Daily agent spend | $25 | breaker disables the job, opens an incident |
| Golden tasks | ≥ 80% of trials, ≤ 1 pass lost vs baseline | gate fails |
| Attack suite | 1.0 on every attack | gate fails |

Changing a threshold is a class B change (class C for security thresholds) and needs the measurement that justifies it.

## 7. Overrides and deactivation

- A gate override needs an owner's approval and an `Override:` line in the changelog entry with the reason; overrides are reviewed monthly.
- `AGENTS_ENABLED=false` stops all agents. Any owner or the on-call engineer may flip it; only an owner re-enables, after an incident note exists.
- Deactivate a single automation with `gh workflow disable`; record why on its automation card.

## 8. What this policy does not cover

Model-provider contracts, data residency, retention and enterprise identity (Module 12); adoption and enablement (Module 19).
