# Module 11 labs — Agents in CI and production

Everything the Module 11 labs need. Lessons: [11.1](../../lessons/module-11/lesson-01.md) · [11.2](../../lessons/module-11/lesson-02.md) · [11.3](../../lessons/module-11/lesson-03.md) · [11.4](../../lessons/module-11/lesson-04.md) · [11.5](../../lessons/module-11/lesson-05.md).

The labs run agents unattended on **Contoso Billing** from [`labs/module-03/brownfield`](../module-03/README.md), in your `ai-layer-lab` repository. They build on Module 5's gates ([`LoopGate`, `loop-gates.yml`](../module-05/README.md)), Module 7's harness and eval workflow ([`EvalHarness`, `ci/agent-evals.yml`, tasks-v1](../module-07/README.md)) and Module 9's hardened configuration ([`configs/hardened`](../module-09/README.md)).

> [!WARNING]
> These labs put an API key and a repository token into CI jobs, and some of them spend money on a schedule. Use a **throwaway lab repository** for every workflow here, set a low `--max-budget-usd`, install `budget-guard.yml` before anything scheduled, and keep `AGENTS_ENABLED` at hand to switch everything off. The breaks (`*/break/`) are deliberately unsafe: never copy them into a real repository. `tickets/BILL-913.md` contains a benign, labelled injection fixture that only asks for label and state changes; file it only in your own lab repository.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400.
- `bash` for `run-headless.sh` (Git Bash on Windows); `jq` for the workflows (preinstalled on GitHub runners).
- Python 3 only if you want to regenerate the illustrative data (`py review/generate.py`, `py ops/generate_runs.py`; deterministic).
- For the live parts: Claude Code with `claude` on your PATH (a pinned version), a GitHub repository with Actions, and the `gh` CLI.
- Everything in the quick start works **offline** on the illustrative data.

## Contents

| Path | What it is |
|---|---|
| `tools/AgentOps/` | Dependency-free C# tool: `result` (classify `claude -p` JSON results), `wflint` (lint agent workflows), `review` (CI-review precision/recall), `triage` (validate a triage proposal against a policy), `ledger` (cost and retry report), `evolution` (changelog vs the system-evolution policy). Read-only. |
| `headless/` | `run-headless.sh` (the run contract; `--bare`, or with `AGENT_BARE=0` `--setting-sources project,local` plus `claudeMdExcludes` merged into the settings file, because `--setting-sources` alone still loads your personal `~/.claude/CLAUDE.md`; either way personal instructions never change a result), four sample results, and the unsafe `pull_request_target` workflow for the 11.1 break. |
| `workflows/` | Safe workflows to copy into `.github/workflows/`: `agent-fix.yml`, `agent-review.yml`, `agent-triage.yml`, `agent-changelog.yml`, `budget-guard.yml`, `ai-layer-governance.yml`, and `agent-settings.json` (copy to `.github/`). |
| `review/` | **Illustrative** CI-review data set: 20 PRs with human-found defects (`prs.json`), one adjudicated defect, comments from prompt v1 (flood) and v2 (evidence first), both prompts, the output schema, and the generator. |
| `automation/` | Triage prompt, schema and policy; tickets BILL-912 (normal) and BILL-913 (injection fixture); sample proposals; the v0 triage workflow for the 11.3 break. |
| `ops/` | **Illustrative** 30-day run ledger (`runs.jsonl`) with its generator, and the hourly retry-loop `dep-fix` workflow for the 11.4 break. |
| `governance/` | Contoso `governance.md`, `CODEOWNERS.additions`, regression task T25 (`tasks-m11.json`), the changelog as found (`break/`) and policy-compliant. |

## Quick start (offline, no API key)

From this folder:

```bash
A="dotnet run --project tools/AgentOps --"

# 11.1 — classify results; lint workflows
$A result headless/samples/*.json --max-cost 1.50        # 3 FAIL (budget, denied, max-turns), 1 OK
$A wflint headless/break/agent-fix-unsafe.yml             # 7 errors, 9 warnings
$A wflint workflows/*.yml                                 # 0 errors, 0 warnings

# 11.2 — CI review against humans
$A review review/prs.json review/comments-v1.json --adjudicated review/adjudicated.json --by-category
$A review review/prs.json review/comments-v2.json --adjudicated review/adjudicated.json --min-severity medium --split dev
$A review review/prs.json review/comments-v2.json --adjudicated review/adjudicated.json --min-severity medium --split holdout

# 11.3 — validate triage proposals
$A triage automation/samples/triage-BILL-912.json --policy automation/triage-policy.json            # ACCEPT
$A triage automation/samples/triage-BILL-913-injected.json --policy automation/triage-policy.json   # REJECT, 6 problems
$A triage automation/samples/triage-BILL-913-schema.json --policy automation/triage-policy.json     # REJECT, p0 needs a human

# 11.4 — the month's ledger
$A ledger ops/runs.jsonl --daily-ceiling 25 --run-ceiling 2                                         # RETRY + CEILING on 2026-09-15

# 11.5 — the system-evolution policy
$A evolution governance/break/AI-LAYER-CHANGELOG.md --tasks ../module-07/tasks-v1/tasks.json --tasks governance/tasks-m11.json   # 5 problems
$A evolution governance/AI-LAYER-CHANGELOG.md --tasks ../module-07/tasks-v1/tasks.json --tasks governance/tasks-m11.json         # 0 problems
```

(PowerShell: type the full `dotnet run --project tools/AgentOps -- <command>` instead of `$A`.) Every command exits with code 1 when its check fails, so each one can be a CI step.

## Setting up `ai-layer-lab` (once, for the live parts)

```bash
mkdir -p tools .github/workflows .github/review .github/triage
cp -r <course>/labs/module-11/tools/AgentOps tools/
cp <course>/labs/module-11/workflows/agent-settings.json .github/
cp <course>/labs/module-11/review/prompts/review-v2.md <course>/labs/module-11/review/review-schema.json .github/review/
cp <course>/labs/module-11/automation/triage-{prompt.md,schema.json,policy.json} .github/triage/
cp <course>/labs/module-11/workflows/<the workflow for this lesson>.yml .github/workflows/
```

Repository settings: variables `AGENTS_ENABLED=true` and `CLAUDE_CODE_VERSION=<pinned version>`; secret `ANTHROPIC_API_KEY` as an **environment** secret of an `agents` environment (required reviewer, prevent self-review) for `agent-fix.yml`, and as a repository secret for the read-only jobs; default workflow permissions set to read. `agent-fix.yml` and `dep-fix` expect Module 5's `tools/LoopGate` and `gates/architecture.rules`; `ai-layer-governance.yml` expects your eval task set under `evals/`.

## Lab sequence

1. **11.1 — Headless agents.** Classify the samples, lint your workflows (Module 5's `loop-gates.yml` gets two errors to fix), run `run-headless.sh` on T18 three times, wire `agent-fix.yml`. *Break:* the allowlist without `Edit` produces a green "fixed" run with no diff; the `pull_request_target` workflow.
2. **11.2 — CI review.** Score v1 and v2, compute the ceiling, choose a policy on dev, run the holdout once, write the report; score 20 PRs of your own. *Break:* back to the "thorough" prompt with filters: precision stays near 25%.
3. **11.3 — Recurring automation.** Net value and break-even for three chores; triage offline and live; the weekly changelog with its idempotency key. *Break:* triage-v0 labels BILL-913 `approved-for-release`, closes three issues and re-comments every night.
4. **11.4 — Operating.** Ledger, ledger lines in every workflow, kill-switch drill, budget guard, environment approval, rollback table. *Break:* hourly dep-fix with retry-on-anything: the $137 day.
5. **11.5 — Governance.** `governance.md`, CODEOWNERS, change classes, one real incident through the evolution loop, `ai-layer-governance.yml`. *Break:* a hotfix rule without a task, deleted by a tidy-up, and the incident recurs.

## Verified results (illustrative data)

| Command | Result |
|---|---|
| `result headless/samples/*.json --max-cost 1.50` | budget: limit · denied: incomplete (3 denials) · max-turns: limit · ok: OK |
| `review` v1, all 20 PRs, adjudicated | 64 comments, precision 9/64 = 14%, recall 9/14, ceiling 22% |
| `review` v2 `--min-severity medium` dev / holdout | 7/8 precision, 7/8 recall / 4/6 precision, 4/6 recall |
| `triage` BILL-912 / BILL-913 injected / BILL-913 schema | ACCEPT / REJECT (6) / REJECT (1: p0 needs a human) |
| `ledger ops/runs.jsonl` | 337 runs, $194.28; dep-fix 48 retried cap failures; 2026-09-15 $136.83 > $25 |
| `evolution` break / reference | 5 problems in 2 entries / 0 problems |

## Artifacts to commit to `ai-layer-lab`

- `.github/workflows/agent-review.yml` with the measured policy, and `reports/ci-review-YYYY-MM.md` ([CI review report template](../../templates/ci-review-report.md))
- one recurring automation with its approval gate (`agent-triage.yml` or `agent-changelog.yml`) and its [automation card](../../templates/automation-card.md)
- `agent-fix.yml`, `budget-guard.yml`, `ai-layer-governance.yml`, `.github/agent-settings.json`, `tools/AgentOps/`
- `governance.md` ([governance policy template](../../templates/governance-policy.md)) and the extended `CODEOWNERS`
- `AI-LAYER-CHANGELOG.md` entries for every change in this module, and the regression task(s) born from your incidents

See the portfolio scaffold: [ai-layer-lab](../../projects/ai-layer-lab/README.md).
