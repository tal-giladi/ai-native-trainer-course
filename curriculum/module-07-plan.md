# Module 07 plan — Agent Evaluation

6 lessons · ~90 min instruction · ~6 h practice · depends on Module 2 (sampling, $p^k$, failure taxonomy, model selection), Module 4 (tasks-v0, the context audit), Module 5 (gates, NOTES.md) and Module 6 (skills and their test tickets).

Shared lab material: `labs/module-07/` on top of `labs/module-03/brownfield` (Contoso Billing). New: `tasks-v1/` (24 tasks, dev 16 / holdout 8: T01–T10 carried over from `labs/module-04/tasks-v0` with repaired graders, 7 new question tasks, 3 code tasks graded by hidden golden tests + scope, 2 rubric tasks graded by an LLM judge; dev/holdout split; golden/regression tags), `tools/EvalHarness` (validate, run, judge, grade, stats, compare, calibrate, gate, leak, runs), a 24-answer human-labelled calibration set with two judge prompts, illustrative result sets for the statistics breaks, a CI workflow, and one break per lesson.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 07.1 Why evaluate agents | Explain the improve-without-measuring trap; name the parts of an eval (task, trial, grader, transcript, outcome, harness); separate qualitative transcript reading from quantitative scoring; write an eval charter | 02.2, 04.5 | Set up `agent-evals/`, run the harness end-to-end on replayed M4 answers, write `CHARTER.md` | A one-run demo "proves" skill v2 (14/20 vs 12/20); the five-trial data shows no difference | `agent-evals/CHARTER.md` |
| 07.2 Task datasets | Build representative, golden, regression tasks with pass criteria written first; validate every task (reference passes, counterexample fails); split dev/holdout; detect contamination | 07.1, 04.5, 06.4 | Turn tasks-v0 into tasks-v1 (≥20 tasks), `validate --repo` | Task answers pasted into `AGENTS.md` → dev jumps, holdout does not; `leak` finds it | `agent-evals/tasks/tasks-v1.json`, dataset card |
| 07.3 Graders | Choose deterministic check vs rubric vs human vs LLM judge; compute grader precision/recall from a confusion matrix; correct an observed pass rate for grader error; calibrate a judge | 07.2, 02.3 | Label 24 answers, run judge v1 and v2, `calibrate` | Judge that rewards verbosity: precision 0.54, kappa 0.08, length bias | `graders/`, calibration report |
| 07.4 Statistics for stochastic systems | Use repeated trials; distinguish pass@k and pass^k; compute Wilson intervals; estimate runs needed; account for clustering by task | 07.3, 02.2 | `stats` on your runs; `runs` calculator; interval-width table | "92% on 100 trials" that are 4 tasks × 25 → task-clustered interval 2.5× wider | stats section of the report |
| 07.5 Comparisons and paired designs | Hold everything but the treatment fixed; pin and record versions; pair by task, interleave, A/A test; compute a paired CI; re-score the M4 context reduction | 07.4, 04.5 | Compare bloated vs reduced layer (M4), 24 tasks × 5 trials | Arm B run after an agent auto-update, fewer tasks; manifest check flags it | `reports/context-reduction.md` (first comparison report) |
| 07.6 Evals in the loop | Gate AI-layer changes (paired lower bound, per-golden-task floors, cost ceiling); wire smoke on PR and full suite nightly; turn incidents into regression tasks | 07.5, 05.3, 03.1 | `gate` locally, CI workflow on paths filter, changelog with eval evidence | Aggregate-only gate passes while golden T06 collapses | `.github/workflows/agent-evals.yml`, gate policy |

Math (§10): grader precision/recall and the observed-rate correction (07.3); binomial and Wilson intervals, runs needed, pass@k vs pass^k, clustered standard error (07.4); paired differences and why pairing narrows the interval (07.5). Bootstrap and permutation tests are left to Module 13.

Simulation: `simulations/evaluation/` (built later) linked from 07.3 (`preset=biased-judge`), 07.4 (`preset=one-run`) and 07.5 (`preset=paired`).

Templates created: `templates/evaluation-dataset.md`, `templates/evaluation-rubric.md`, `templates/benchmark.md` (comparison report).

Links to Module 6 (written in parallel) use manifest paths `lessons/module-06/lesson-0N.md`.
