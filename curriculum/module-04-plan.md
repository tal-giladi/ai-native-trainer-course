# Module 04 plan — Context Engineering

5 lessons · ~80 min instruction · ~5 h practice · depends on Module 2 (02.1 tokens and effective context, 02.4 token growth in the loop) and Module 3 (03.2 load timing, 03.4 grounded rules).

Shared lab material: `labs/module-04/` — `bloated/` (905-line "before" layer on top of Contoso Billing from `labs/module-03/brownfield`), `tasks-v0/` (10 read-only tasks, regex-graded), `tools/ContextLab` (`budget`, `audit`, `prompts`, `grade`, `report`), `scripts/run-tasks.{sh,ps1}` (headless `claude -p`, `dontAsk`), `breaks/`, `solution/` (34-line layer, path rule, topology map, filled audit), `samples/` (illustrative runs, not measurements).

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 04.1 What context is and what it costs | Compute $B = W - S - R - T - H - O$; compute per-run and monthly cost of $R$ with and without caching; explain nominal vs effective context | 02.1, 02.4, 03.4 | `ContextLab budget` on bloated vs Module 3 rules; `/context` in a live session | verbose `dotnet test -v diag` log enters history → per-turn cost jumps | budget sheet in `context/budget.md` |
| 04.2 Layering context | Place a fact in always / on-demand / never / generated; choose the loading mechanism (root, path rule, nested file, skill, retrieval, topology map); keep a pointer for every on-demand fact | 04.1, 03.2 | Sort bloated blocks into layers; write `docs/ai/topology.md`; path-scoped migrations rule | U### fact moved into the path rule only → T04 fails when no migration file is read | layered layer + topology map |
| 04.3 Context pathologies | Recognise pollution, staleness, contradiction, redundancy, irrelevance from agent output; confirm audit leads; trace a symptom to a line | 04.2, 03.4 | `ContextLab audit`; label leads; symptom→pathology table | contradictory `Invoices/CLAUDE.md` → T02 flips by trajectory | pathology log |
| 04.4 Compression and compaction | Explain what survives `/compact` and `/clear`; write compact instructions and a handoff note; choose compress vs reset | 04.3, 02.4 | compaction survival test; compact instructions; `HANDOFF.md` | constraint stated only in chat lost after `/compact` → agent edits V004 | compact instructions + handoff template |
| 04.5 The context audit | Run the four-bucket audit; cut ≥ 70 % while holding pass rate on tasks-v0; report tokens, pass rate, cost | 04.1–04.4 | Module project: baseline, cut, rerun, report | cut deletes "never edit merged V###" → T06 fails; ablation catches it | `context/context-audit.md`, reduced layer, `agent-evals/tasks-v0/` |

Breaks follow §6: insufficient context (04.2, 04.5), contradictory context (04.3). Math per §10: token/context budget arithmetic and expected cost per task (04.1, reused in 04.5). Simulation (§11): Context budget, linked from 04.1 (`preset=bloated-rules`) and 04.3 (`preset=irrelevant-docs`) — to be built later.

Template created: `templates/context-audit.md`.
