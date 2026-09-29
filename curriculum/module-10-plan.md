# Module 10 plan — Multi-Agent Systems

4 lessons · ~60 min instruction · ~5 h practice · depends on Module 2 (agent loop, $p^k$, instruction hierarchy), Module 6 (sub-agents, the four questions, `researcher`/`reviewer`), Module 7 (EvalHarness, tasks-v1, paired comparisons). Module 9 (written in parallel) is linked by manifest path for agent-to-agent trust (09.1, 09.5).

The module's thesis: most coding work is sequential and coupled, so multi-agent designs usually cost more for the same result. Prove it (or the exception) on your own task set.

Shared lab: `labs/module-10/` on top of `labs/module-03/brownfield` (Contoso Billing) and `labs/module-07` (tasks-v1 + EvalHarness). New: `tools/AgentTeam` (dependency-free C#: `run` with topologies `single`, `pw`, `pwr`, `specialists`, `route`; `parallel` with `--merge naive|detect|rerun`; `trace`; `estimate`; `efficiency`), role prompts in `roles/` derived from the Module 6 `researcher` and `reviewer` agents, a scripted offline agent (`--agent fake:<scenario>`) with four scenarios that run the real .NET golden tests, illustrative results and traces for the comparison, and one break per lesson. AgentTeam writes run folders in EvalHarness's format, so `EvalHarness grade/compare/gate` work unchanged.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 10.1 Multi-agent topologies | Name six topologies and what each buys (isolation, parallelism, independent check, specialization) and costs; pick a topology from task properties (decomposable? coupled? verifiable?); map to Claude Code subagents, agent teams, headless pipelines | 06.3, 02.4 | Classify six workflows; run `single` and `pwr` offline on T18 and read both traces; fill the design template | Parallel specialists on coupled T19: tester guesses the type name, build fails | `agent-evals/designs/<topology>.md` ([template](../templates/multi-agent-design.md)) |
| 10.2 Coordination | Design handoff contracts (what is passed, in what shape, validated how); choose shared-state model (artifact, message log, blackboard) with ownership; resolve conflicting outputs by precedence, not by vote or last writer | 10.1, 05.2, 02.3 | Parallel T18/T19/T20 with three merge policies; handoff files; ownership leases | Two workers edit `InvoiceService.cs`; naive merge silently loses two of three changes | merge policy + handoff contract in the design doc |
| 10.3 Cost, latency and failure propagation | Compute expected cost $\sum$ calls × tokens × price, loop rounds, critical-path latency; end-to-end success $\prod p_i$ and a reviewer's value from recall and false alarms; trace a run and locate the originating span | 10.2, 07.4, 02.2 | `estimate` sensitivity; `trace` on illustrative pwr traces; budget and stall stops | Planner and reviewer disagree for seven rounds; budget stop fails a correct fix | trace notes, pipeline budget policy |
| 10.4 When multi-agent is worse | Run a fair comparison (same tasks, pinned, paired, A/A, compute-matched baseline); report pass rate, cost, latency with intervals; recommend on evidence | 10.3, 07.5 | single vs solo-verify vs pwr vs route on tasks-v1 × 5 | "Pipeline wins on code tasks" vs a single agent without verification; compute-matched baseline erases it | `agent-evals/reports/multi-vs-single.md` |

Math (§10): expected cost per task, expected rounds of a capped review loop $\frac{1-(1-a)^R}{a}$, critical-path latency (10.3); $\prod p_i$ compounding and the reviewer equation $P = p(1-f) + \dots$ (10.3); paired intervals on pass rate, cost and latency (10.4, reusing 07.5).

Simulation: `simulations/multi-agent/` (built later) linked from 10.1 (`preset=topologies`), 10.3 (`preset=compounding`) and 10.4 (`preset=review-loop`).

Templates created: `templates/multi-agent-design.md`. Reused: `templates/sub-agent-design.md` (06.3), `templates/benchmark.md` (07.5).
