---
id: "11.4"
module: 11
minutes: 16
practice_minutes: 60
prerequisites: ["11.2", "11.3", "07.6"]
objectives:
  - Choose an approval mechanism for each agent job from its reversibility and blast radius (none, draft PR with code owners, environment reviewers), and never let an agent merge to the default branch.
  - Define rollback for every versioned part of an agent job (agent version, model, layer, prompt, policy) and operate a kill switch and a shadow run.
  - Instrument agent jobs with a per-run ledger and OpenTelemetry, and compute cost per useful outcome.
  - Bound spend at three levels (run, job, day), and handle failures by class, including the expected cost of retries.
volatility: implementation
sources:
  - title: "GitHub Docs — Managing environments for deployment (required reviewers, environment secrets)"
    url: https://docs.github.com/en/actions/managing-workflow-runs-and-deployments/managing-deployments/managing-environments-for-deployment
  - title: "Claude Code docs — Monitoring (OpenTelemetry metrics and events)"
    url: https://code.claude.com/docs/en/monitoring-usage
  - title: "Claude Code docs — How the agent loop works (turns and budget)"
    url: https://code.claude.com/docs/en/agent-sdk/agent-loop
  - title: "Google SRE Workbook — Canarying releases"
    url: https://sre.google/workbook/canarying-releases/
  - title: "NIST AI RMF 1.0 — Core functions (GOVERN, MAP, MEASURE, MANAGE)"
    url: https://airc.nist.gov/airmf-resources/airmf/5-sec-core/
last_verified: "2026-09-28"
---

# 11.4 · Operating agents: approval, rollback, observability and cost

## Why it matters

By now Contoso runs five agent jobs: fix-on-request, PR review, triage, the weekly changelog, and a nightly dependency fixer. Together they are a small production service — with users (the team), inputs that change without notice, a vendor that ships new versions, and a meter running on every call. Nobody would run a service without approval for risky changes, a way to roll back, dashboards, and a budget. Agent jobs get skipped because each one looks like "just a workflow".

Then one Monday someone makes the dependency fixer hourly, "until the build is green", with a retry loop. Tuesday costs $137 instead of the usual $2, and nobody notices until the invoice. This lesson is the operations manual that would have caught it at 10 a.m.

> [!NOTE]
> Content tags. **Concept** (stable): approval by reversibility and blast radius, versioned rollback and kill switches, shadow runs, cost per useful outcome, ceilings at three levels, failure classes and retry cost. **Implementation** (as of 2026-09): GitHub environments, `gh workflow disable`, Claude Code OpenTelemetry, `AgentOps ledger`, `budget-guard.yml`.

## How it works

### Approval: by what can go wrong

| Output of the job | Reversible? | Blast radius | Approval |
|---|---|---|---|
| Review comments, triage labels | yes, trivially | a thread | none per run; the *policy* was approved (11.2, 11.3) |
| Draft PR (fix, changelog, docs, dependency) | yes, close it | nothing until merged | CODEOWNERS review of the PR, as for any human PR |
| Job that pushes, deploys, spends big or touches production data | partly | environment-wide | GitHub **environment** with required reviewers |
| Merge to `main` | only by revert, after the fact | everyone | **never** by an agent |

Environments do two things at once: the job waits until a required reviewer approves, and the environment's secrets are released only "after any configured rules (for example, required reviewers) pass" ([GitHub environments](https://docs.github.com/en/actions/managing-workflow-runs-and-deployments/managing-deployments/managing-environments-for-deployment)). Turn on "prevent self-review" so whoever triggered the run cannot approve it. `agent-fix.yml` from 11.1 uses an `agents` environment for exactly this.

Approval fatigue is real: if every run needs a click, clicks become reflexes. Approve policies and gates once, with evidence; approve individual runs only where the output is irreversible.

### Rollback: what is the "version" of an agent job?

| Part | Where it lives | Roll back by |
|---|---|---|
| Agent version | repository variable `CLAUDE_CODE_VERSION` | re-pin previous value (and its baseline, 07.6) |
| Model | `--model` full name in the workflow | revert the workflow commit |
| AI layer | `AGENTS.md`, `.claude/`, tagged `layer-v1.5.1` | revert or check out the tag |
| Prompt, schema, policy | `.github/review/`, `.github/triage/` | revert the commit |
| The job itself | the workflow file | `gh workflow disable <file>` |
| All agents at once | repository variable `AGENTS_ENABLED` | set it to `false` |

If a part is not in the table — a prompt pasted into a UI, a model alias that moves, an unpinned install — you cannot roll it back. The kill switch is the "deactivate" mechanism NIST's AI RMF asks for: mechanisms and assigned responsibility "to supersede, disengage, or deactivate AI systems" that perform inconsistently with intended use ([MANAGE 2.4](https://airc.nist.gov/airmf-resources/airmf/5-sec-core/)). Every agent job in the lab starts with `if: vars.AGENTS_ENABLED == 'true'`; test the switch before you need it.

For changes you cannot fully evaluate offline — a new review prompt, a new model — use a **shadow run**: the candidate runs on the same PRs as production, its output saved as an artifact and never posted. That is a canary with zero user exposure; the SRE Workbook defines canarying as "a partial and time-limited deployment of a change in a service and its evaluation", comparing the canary with a control ([SRE Workbook](https://sre.google/workbook/canarying-releases/)). After two weeks, score shadow and production against the same acted-on outcomes (11.2) and switch only on evidence.

### Observability: one line per run, one number per job

Every agent job appends a **ledger line** built from its JSON result: `ts, job, run_id, subtype, num_turns, cost_usd, duration_ms, outcome, attempt`. `outcome` is what happened to the output: `applied`, `merged`, `rejected`, `no-op`, `failed`. From the ledger, the one number per job that matters:

$$\text{cost per useful outcome} = \frac{\text{total cost}}{\#\{\text{applied or merged}\}}$$

It folds cost, success rate and acceptance into one figure you can compare with the human minutes the job saves (11.3).

For fleet-wide telemetry, Claude Code exports OpenTelemetry when `CLAUDE_CODE_ENABLE_TELEMETRY=1`: metrics such as `claude_code.cost.usage` and `claude_code.token.usage`, and events such as `tool_decision` and `tool_result` ([monitoring docs](https://code.claude.com/docs/en/monitoring-usage)). Prompt text is not logged unless you set `OTEL_LOG_USER_PROMPTS=1`; in CI, leave it off — prompts carry diffs and ticket text, and your telemetry backend is not where code should live (Module 12 returns to this).

Alert on three things: failures, ceiling breaches, and **absence** — a nightly job with no ledger line since yesterday.

### Cost: ceilings at three levels

| Level | Bounds | Mechanism |
|---|---|---|
| Run | one agent session | `--max-turns`, `--max-budget-usd` (result subtype `error_max_budget_usd`) |
| Job run | one workflow run | `timeout-minutes`, `concurrency`, retry policy |
| Day | the fleet | `budget-guard.yml`: sum today's ledger, disable offending workflows, open an issue |

Above those, a spend limit on the provider workspace that holds the CI key bounds the month (Module 12).

### Failure handling, and what retries cost

*Intuition.* A retry is only useful if the next attempt is likely to succeed for a different reason than the last one failed.

*Equation.* With up to $k$ attempts and a probability $q$ that an attempt fails, the expected number of attempts is

$$E[\text{attempts}] = 1 + q + q^2 + \dots + q^{k-1} = \frac{1-q^k}{1-q}$$

*Tiny example.* A transient API interruption, $q = 0.05$, $k = 3$: $E = 1.05$ attempts, 5% overhead. A task too large for its turn cap fails the same way every time, $q \approx 1$: $E \to k = 3$, every attempt wasted. Now multiply by the schedule: hourly × 3 attempts × about $1.87 = 72 runs and roughly $135 in a day.

*Interpretation.* Retry by failure **class**, never by exit code:

| Class (from `AgentOps result`) | Action |
|---|---|
| `error_during_execution` (transient) | retry once with backoff; then alert |
| `error_max_turns`, `error_max_budget_usd` (cap) | fail, alert, no retry; narrow the task or raise the cap in a reviewed change |
| success with denials (incomplete) | fail; it is a configuration bug |
| validation reject | route to a human; never retry into shape |
| unreadable result | fail closed |

The CLI already retries API-level errors such as rate limits and overload itself (it emits `api_retry` events in stream output), so your job-level retry should not stack on top of them.

## Show me

Thirty days of illustrative Contoso telemetry (`labs/module-11/ops/runs.jsonl`, simulated):

```text
$ dotnet run --project tools/AgentOps -- ledger ops/runs.jsonl --daily-ceiling 25 --run-ceiling 2
337 runs over 30 days, total $194.28

job               runs  ok-runs  useful  cost      $/run  $/useful  p95 min  limit-fails  retried-limit
agent-changelog      5        5       2  $   0.72   0.14    0.36      1.2            0              0
agent-review       130      127      71  $  45.76   0.35    0.64      3.9            3              0
agent-triage       101      101      77  $   3.52   0.03    0.05      0.4            0              0
dep-fix            101       25       5  $ 144.28   1.43   28.86     11.5           76             48

RETRY   dep-fix: 48 retries of runs that had hit a turn or budget cap; a cap is not a transient error
CEILING 2026-09-15: $136.83 > daily $25.00; dep-fix spent $134.58 in 72 runs
2 ceiling/retry finding(s)
```

A normal Contoso day costs about $2. Before the change, dep-fix cost $9.48 over 22 nightly runs for 5 merged fixes — $1.90 per useful outcome. For the month it shows $28.86, and 76 limit failures of which 48 were retries. Note what did **not** fire: no single run exceeded the $2 per-run ceiling. Per-run caps cannot see a loop of runs; only the day level can.

## Try it

Budget: 60 minutes.

1. **Read the ledger.** Run `ledger` on `ops/runs.jsonl`. For each job, compare $/useful with your 11.3 estimate of minutes saved per accepted output. Which job would you cut first?
2. **Ledger lines everywhere.** Copy the "Ledger line" step and artifact upload from `agent-fix.yml` into every agent workflow you wired in 11.1–11.3.
3. **Kill switch drill.** Set `AGENTS_ENABLED=false`, dispatch two agent workflows, confirm both skip. Set it back. Write the drill date on each automation card.
4. **Breaker.** Copy `workflows/budget-guard.yml`, set `AGENT_DAILY_CEILING_USD` to something you would notice (e.g. three times your normal day), and dispatch it once. Confirm it reads today's ledger lines.
5. **Approval.** Configure the `agents` environment with a required reviewer and "prevent self-review". Dispatch `agent-fix.yml` and have someone else approve it.
6. **Rollback table.** Fill the rollback rows of the [automation card](../../templates/automation-card.md) for your review job: where each versioned part lives and the exact command to roll it back. If a row has no answer, fix that first.
7. **Telemetry (optional).** Run one local headless session with `CLAUDE_CODE_ENABLE_TELEMETRY=1 OTEL_METRICS_EXPORTER=console OTEL_LOGS_EXPORTER=console` and find `claude_code.cost.usage` in the output.

<details>
<summary>Hint: budget-guard finds no ledger lines</summary>

It only sees artifacts that contain a file named exactly `ledger.jsonl`, from workflows listed in `AGENT_WORKFLOWS`, created today in UTC. Check the artifact contents of one agent run, and remember the ledger step runs with `if: always()` only if the job reached it; a job cancelled by `timeout-minutes` writes nothing, which is why absence is also an alert.
</details>

## Break it

> [!CAUTION]
> Lab repository only, with a low `--max-budget-usd` and the budget guard installed first. This break is about spending money in a loop; keep the loop short.

Install `ops/break/dep-fix-hourly.yml` as `dep-fix.yml` on a branch of the lab repository with a deliberately unfixable build (add a call to a method that does not exist in any package version). Dispatch it once and watch the attempts in the log. Then run `wflint` on the file. Offline: explain the $136.83 day in `ops/runs.jsonl` from the workflow alone, before reading the Fix.

## Fix it

**Diagnose.**

1. *Symptom:* one day at 60× the normal spend; 48 retries of capped runs; `wflint` shows only a warning.
2. *Mechanism:* the task exceeded 40 turns every time ($q \approx 1$), the loop retried on any non-zero exit ($k = 3$), and the schedule multiplied by 24. No per-run budget, no daily ceiling, and nobody watching the ledger.
3. *Root cause:* retry and schedule policy chosen for "make it green" instead of from failure classes, and cost bounded only per run.

**Modify.**

- Schedule back to nightly; one attempt; retry only when `AgentOps result` classifies the failure as transient.
- Add `--max-budget-usd 1.50` and a step that runs `AgentOps result --max-cost 1.50` so a capped run fails the job with class `limit` and opens an issue for a human.
- Install `budget-guard.yml` with a daily ceiling. Its RETRY finding alone would have disabled `dep-fix.yml` at its 01:17 UTC run, the first check after a capped run had been retried, with about $7.50 spent instead of $137.
- Add the job to the automation card with its ceilings and retry policy, and the change to the AI-layer changelog as a class C change (11.5).

**Rerun.** On the unfixable branch: one run, subtype `error_max_turns` or `error_max_budget_usd`, job fails, one issue opened, total under $2. `ledger` on the new lines: no RETRY, no CEILING.

<details>
<summary>Solution notes</summary>

The per-run cap did its job and the incident happened anyway, because incidents in agent operations are usually about *how many* runs, not *one* run. The retry formula makes the policy obvious: retries buy reliability only against independent failures. A cap, a denial or a validation reject is not independent across attempts. Put the class-based retry in one shared script so no workflow reinvents "retry 3 times".
</details>

## How do I know it works?

- [ ] Each agent job has an approval level matched to its reversibility, and no agent can merge to `main`.
- [ ] Every versioned part of each job can be rolled back with a known command, and the kill switch drill passed this month.
- [ ] Every run writes a ledger line; you can state cost per useful outcome per job.
- [ ] Spend is bounded per run, per job run and per day, and the day-level breaker has been tested.
- [ ] Retries happen only for transient failures; capped, denied and rejected runs alert a human.
- [ ] You get an alert when a scheduled job silently stops running.

## Use / don't use

**Use** the ledger from the first day an agent job exists; three weeks of history is what lets you see the odd day. **Use** shadow runs for prompt, model and agent-version changes to jobs that face people.

**Don't** gate every run on a human click; gate the policy and the irreversible outputs. **Don't** retry on exit codes. **Don't** log prompts to your observability stack by default.

**Limitations.**

- Ledger outcomes like `merged` arrive days later; a weekly job must update them or $/useful will look worse than it is.
- `total_cost_usd` is a client-side estimate and can differ from the bill; reconcile monthly with the provider's usage data.
- The breaker reacts within the hour it is scheduled on; a truly fast loop needs the provider-side spend limit as well.

## Reflect

1. Which of your agent jobs could you not roll back today, and which part of it is unversioned?
2. What is the cost per useful outcome of your most expensive job, and would you pay a person that much for the same output?
3. When did you last test that your kill switch actually stops every agent?

## Sources

- [GitHub Docs — Managing environments for deployment](https://docs.github.com/en/actions/managing-workflow-runs-and-deployments/managing-deployments/managing-environments-for-deployment) — required reviewers (up to six, one approval), preventing self-review, environment secrets available only after protection rules pass.
- [Claude Code docs — Monitoring](https://code.claude.com/docs/en/monitoring-usage) — `CLAUDE_CODE_ENABLE_TELEMETRY`, OTLP/console exporters, `claude_code.cost.usage`, `claude_code.token.usage`, `tool_decision` and `tool_result` events; prompt content not logged unless `OTEL_LOG_USER_PROMPTS=1` (as of 2026-09).
- [Claude Code docs — How the agent loop works](https://code.claude.com/docs/en/agent-sdk/agent-loop) — `max_turns` and `max_budget_usd` end the run with `error_max_turns` / `error_max_budget_usd`; subagent spend counts toward the budget.
- [Google SRE Workbook — Canarying releases](https://sre.google/workbook/canarying-releases/) — canarying as a partial, time-limited deployment compared against a control population.
- [NIST AI RMF — Core (GOVERN, MANAGE)](https://airc.nist.gov/airmf-resources/airmf/5-sec-core/) — MANAGE 2.4: mechanisms and responsibilities to supersede, disengage or deactivate AI systems.
