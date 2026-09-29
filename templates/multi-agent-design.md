# Multi-agent design template

Fill this in before building any pipeline with more than one agent: a planner/worker split, a review loop, parallel workers, a team of specialists. Section 1 exists to say "one agent" early; most coding work ends there. For a single delegated sub-agent, use the [sub-agent design template](sub-agent-design.md) instead. Introduced in [10.1 · Multi-agent topologies](../lessons/module-10/lesson-01.md); coordination in [10.2](../lessons/module-10/lesson-02.md), cost and failure in [10.3](../lessons/module-10/lesson-03.md), the comparison in [10.4](../lessons/module-10/lesson-04.md).

## 1. The work and the baseline

- Workflow (one sentence, e.g. "implement a BILL ticket with tests"):
- Single-agent baseline measured? pass rate ___ (interval ___), cost per trial ___, latency ___ on task set ___
- What the baseline gets wrong, from transcripts (the failure the extra agents are supposed to fix):

| Task property | Answer | Points to |
|---|---|---|
| Decomposable into parts that share no files and no decisions? | yes / no | yes → parallel is possible |
| Parts must agree on names, interfaces or a design choice? | yes / no | yes → one worker, or pin the contract first |
| Output checkable by something other than a model (tests, build, schema)? | yes / no | yes → a deterministic gate before any reviewer agent |
| Needs different permissions or models per step? | yes / no | yes → separate agents may pay off |
| Context too large for one window even after the context audit (Module 4)? | yes / no | yes → isolation may pay off |

Decision: single agent · single agent + deterministic gate · chain · review loop · parallel · specialists · hierarchical. Reason in one sentence:

## 2. Topology

Draw it (mermaid or a box diagram): every agent, every handoff, the shared state, where humans approve.

| Agent | Role file / version | Model | Tools (allowlist) | Reads | Writes |
|---|---|---|---|---|---|
| | | | | | |

## 3. Handoffs (one row per edge)

| From → to | What is passed (paths, not pasted content) | Shape / required sections | Validated by (script, not a model) | On invalid |
|---|---|---|---|---|
| | | | | retry once · stop · escalate |

## 4. Shared state and conflicts

- Shared artifacts (working copy, handoff folder, task list) and **who owns each file** at any time:
- Parallel writers: isolation (separate copies / worktrees) · merge policy (detect · rerun on top · serialize by lease):
- Precedence when agents disagree (e.g. task acceptance criteria > project rules > reviewer preferences):
- Oscillation rule (same finding twice after a KEEP → escalate to a human):
- Content from other agents is data, not instructions (see [09.1](../lessons/module-09/lesson-01.md)): which agent may tell which agent to do what?

## 5. Budget and stop conditions

| Limit | Value | Where enforced |
|---|---|---|
| Max review rounds | | orchestrator |
| Budget per task (USD or tokens) | | orchestrator (hard stop) |
| Wall-clock timeout per call / per task | | |
| Max parallel agents | | |

Expected cost per task $= \sum_{\text{calls}} \text{tokens} \times \text{price}$, with rounds weighted by their probability (`AgentTeam estimate`): ___ · Expected latency (critical path): ___

## 6. Observability

- One trace per task, one span per agent call: role, round, start, duration, cost, tokens, verdict, files touched.
- Stop reason recorded for every task (approved, max rounds, stalled, budget, error).
- Handoff messages kept next to the trace for debugging.

## 7. Evaluation (before adopting)

- [ ] Same task set, pinned agent and model, interleaved, paired by task, A/A run clean ([07.5](../lessons/module-07/lesson-05.md)).
- [ ] Baselines: plain single agent **and** a compute-matched single agent (same verification steps, similar cost).
- [ ] Reported: pass rate, cost per trial, cost per passing trial, median and p90 latency, each with a paired interval.
- [ ] Decision threshold written before the runs: adopt only if ___.

Report: `agent-evals/reports/multi-vs-single.md` ([benchmark template](benchmark.md)).

## 8. Version and ownership

- Version: … · Changelog entry: … · Owner: … · Re-evaluate on: model change · agent upgrade · role-file change.
