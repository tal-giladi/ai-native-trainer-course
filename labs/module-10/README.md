# Module 10 labs — Multi-agent systems

Everything the Module 10 labs need. Lessons: [10.1](../../lessons/module-10/lesson-01.md) · [10.2](../../lessons/module-10/lesson-02.md) · [10.3](../../lessons/module-10/lesson-03.md) · [10.4](../../lessons/module-10/lesson-04.md).

The labs run single- and multi-agent topologies on **Contoso Billing** ([`labs/module-03/brownfield`](../module-03/README.md)) against the Module 7 task set ([`labs/module-07/tasks-v1`](../module-07/tasks-v1/README.md)), and grade and compare them with **EvalHarness** ([`labs/module-07`](../module-07/README.md)). The role prompts are derived from the Module 6 `researcher` and `reviewer` agents ([`labs/module-06/layer/.claude/agents`](../module-06/README.md)).

> [!WARNING]
> `AgentTeam run` and `parallel` with `--agent claude` start several headless Claude Code sessions per task, which send prompts and repository context to your model provider and cost money: a planner → worker → reviewer pipeline is at least three sessions per trial, up to seven with two review rounds. The 10.4 project (24 tasks × 5 trials × 3 configurations, plus the single-agent arm from EvalHarness) is on the order of 1,000 sessions. Always set `--budget-usd`, run one trial first and read the cost line, and run only on the fictional Contoso code or a repository your provider agreement covers. Code tasks edit copies under the output folder, never your repository. Everything else in this README runs offline.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400. No NuGet packages for the tool; the golden tests restore Contoso's pinned test packages once (xunit 2.9.2).
- For offline work: nothing else. `--agent fake:<scenario>` is a scripted agent that replays recorded steps and applies their edits; the real golden tests decide the outcome.
- For real runs: Claude Code with `claude` on your PATH, a pinned version and `DISABLE_AUTOUPDATER=1` (lesson 07.5). The tool calls `claude -p … --output-format json --permission-mode dontAsk|acceptEdits --setting-sources project,local --settings <isolation file> --append-system-prompt <role>`. `--setting-sources project,local` keeps your `~/.claude/settings.json` and personal skills out but not your personal `~/.claude/CLAUDE.md` (checked 2026-09-29); the isolation file sets `claudeMdExcludes` to that file and `~/.claude/rules/**` and turns auto memory off. Personal instructions change results and runs must be reproducible. Another agent CLI works if you adapt `ClaudeAgent` in `Program.cs` (about 30 lines).
- EvalHarness from `labs/module-07/tools/EvalHarness` for `grade`, `compare` and `gate`.

## Contents

| Path | What it is |
|---|---|
| `tools/AgentTeam/` | Dependency-free C# tool: `run` (topologies `single`, `pw`, `pwr`, `specialists`, `route`), `parallel` (merge policies `naive`, `detect`, `rerun`, `serialize`), `trace`, `estimate`, `efficiency`. Writes run folders in EvalHarness's format plus one `*.trace.jsonl` per trial and the handoff messages under `handoff/`. |
| `roles/` | Role prompts: `planner` (from Module 6 `researcher`), `worker`, `reviewer` (from Module 6 `reviewer`), `solo` (one agent that verifies its own work), `coder`, `tester`. |
| `scenarios/` | Scripted runs for the offline agent: `t18-pipeline`, `t18-deadlock`, `specialists-t19`, `clobber`, and `make_scenarios.py` that wrote them. |
| `pipelines/` | Inputs for `estimate`: a code task and a question task (illustrative numbers). |
| `samples/` | **Illustrative** results and traces for the comparison (simulated, not measurements). See `samples/README.md`. |
| `break/` | The four breaks, indexed in `break/README.md`. |

## Quick start (offline, no API key)

From this folder (`labs/module-10`). Bash; in PowerShell type the full `dotnet run --project … --` command instead of `$H`.

```bash
H="dotnet run --project tools/AgentTeam --"
E="dotnet run --project ../module-07/tools/EvalHarness --"
T=../module-07/tasks-v1/tasks.json
B=../module-03/brownfield

# 10.1: one agent vs planner -> worker -> reviewer on T18, and a trace of each
$H run $T --repo $B --out runs/t18-single --topology single --only T18 --trials 1 --agent fake:scenarios/t18-pipeline.json
$H run $T --repo $B --out runs/t18-pwr    --topology pwr    --only T18 --trials 1 --agent fake:scenarios/t18-pipeline.json
$H trace runs/t18-pwr --task T18 --trial 1

# 10.2: three parallel workers on one file, four merge policies
$H parallel $T --repo $B --out runs/par-naive --only T18,T19,T20 --merge naive --agent fake:scenarios/clobber.json

# 10.3: expected cost and success of a pipeline; traces of 120 illustrative pipeline runs
$H estimate pipelines/code-task.json
$H trace samples/multi-vs-single/traces-pwr --results samples/multi-vs-single/results.csv --config pwr

# 10.4: the comparison on illustrative data
$E compare samples/multi-vs-single/results.csv --a single --b pwr
$H efficiency samples/multi-vs-single/results.csv --a single --b pwr

# Grade AgentTeam runs with EvalHarness exactly as in Module 7
$E grade $T runs/t18-pwr --config t18-pwr --out results.csv
```

## With a real agent

```bash
cp -r ../module-03/brownfield ~/m10-work && cp -r ../module-04/solution/. ~/m10-work/   # the reduced layer from Module 7

# single-agent baseline: exactly the Module 7 harness
$E run $T --repo ~/m10-work --out runs/single --trials 1
# the same tasks through the pipeline, with a hard budget per task
$H run $T --repo ~/m10-work --out runs/pwr --topology pwr --trials 1 --max-rounds 2 --budget-usd 1.50
```

Both commands are resumable: rerun with `--trials 2`, `3` … in an interleaved loop (lesson 07.5). `AgentTeam run` writes `manifest.json` with the same fields as EvalHarness (tasks hash, layer hash, agent version, model) plus `topology` and `roles_sha`, so `compare` can check the arms are comparable. Judge T21 and T22 with `EvalHarness judge` as in Module 7; the final answer of a pipeline is the worker's last answer.

## Verified results

Run from `labs/module-10` with the offline agent and .NET SDK 10.0.400. Costs and times are the scenarios' scripted values.

| Command | Result |
|---|---|
| `run … --topology single --only T18` (t18-pipeline) | golden 4/4; 1 call, $0.14, 112 s |
| `run … --topology pwr --only T18` (t18-pipeline) | golden 4/4; approved in round 1; 3 calls, $0.21, 170 s |
| `run … --topology specialists --only T19` | `build failed: error CS0246: … 'CollectionSummary' could not be found`; 3 calls, $0.26, 126 s |
| `run … --topology pw --only T19` (same scenario) | golden 3/3; 2 calls, $0.22, 169 s |
| `parallel … --merge naive` | 1/3 golden tasks correct; merged branch's own suite GREEN 7/7; 142 s, $0.43 |
| `parallel … --merge detect` | T18 merged; T19, T20 reported as CONFLICT, not merged; 1/3 |
| `parallel … --merge rerun` | 3/3 correct; 5 calls, 381 s, $0.75 |
| `parallel … --merge serialize` | leases: 3 waves `[T18] -> [T19] -> [T20]`; 3/3 correct; 3 calls, 303 s, $0.43 |
| `run … --topology pwr --only T18 --max-rounds 0 --budget-usd 1.00 --on-stall continue` (t18-deadlock) | stop `budget` after 6 rounds, 19 calls, $1.01, 775 s; EvalHarness grades it a fail although golden tests pass |
| same with `--max-rounds 3` (stall detection on by default) | stop `stalled`, escalated after 2 rounds, 6 calls, $0.36; golden 4/4 |
| `estimate pipelines/code-task.json` | single 0.600 / $0.300 / 180 s; pipeline 0.819 / $0.559 / 346 s |
| `estimate pipelines/qa-task.json` | single 0.880 / $0.050; pipeline 0.856 / $0.118 |
| `EvalHarness compare samples/…/results.csv --a single --b pwr` | 78% vs 78%, paired 95% CI −11.9 to +11.9 pts |
| `efficiency samples/…/results.csv --a single --b pwr` | cost 2.18×, latency 2.23× |
| `EvalHarness grade` on the four T18 run folders | pass, pass, fail (`the run reported an error`), pass |

## Lab sequence

1. **10.1 — Topologies.** Classify six workflows; run T18 as one agent and as a pipeline, read both traces; fill in the [multi-agent design template](../../templates/multi-agent-design.md). *Break:* parallel specialists on T19 each decide a name the plan left open.
2. **10.2 — Coordination.** Parallel T18/T19/T20 under four merge policies; read handoff files; write a handoff contract and a precedence rule. *Break:* naive merge keeps 1 of 3 changes and CI stays green.
3. **10.3 — Cost, latency, failure propagation.** `estimate` with sensitivity to reviewer recall; `trace` on 120 pipeline traces; budget and stall stops. *Break:* planner and reviewer disagree until the budget runs out.
4. **10.4 — When multi-agent is worse (project).** single vs solo-verify vs pwr vs route on tasks-v1, interleaved, paired, with an A/A run; write `reports/multi-vs-single.md`. *Break:* the vendor slide.

## Artifacts to commit to `agent-evals`

- `designs/<workflow>.md` — one filled [multi-agent design template](../../templates/multi-agent-design.md)
- `harness/AgentTeam/` and `roles/` (versioned like skills: changelog, owner)
- `reports/multi-vs-single.md` ([benchmark template](../../templates/benchmark.md)) with `results.csv`, every `manifest.json`, and a sample of traces
- `NOTES.md`: the trace diagnosis from 10.3 and the recommendation from 10.4

See the portfolio scaffold: [agent-evals](../../projects/agent-evals/README.md).
