# Benchmark and comparison report template

Use for every A-versus-B claim about an agent system: model A vs B, rules v1 vs v2, skill v1 vs v2, context before vs after. One report per comparison, committed next to the results it describes.
Goal: a reader can see what changed, what stayed fixed, how many trials ran, the interval, and what the result does **not** show. Introduced in [07.5 · Comparisons and paired designs](../lessons/module-07/lesson-05.md); statistics from [07.4](../lessons/module-07/lesson-04.md). Business-impact comparisons use the experiment design in Module 13 instead.

## 1. Question and decision

- Question (one sentence, e.g. "Does the reduced layer hold or improve pass rate at lower cost?"):
- Decision it informs, and the threshold that decides it (written before the runs):
- Date · author:

## 2. What changed and what was held fixed

| | Arm A | Arm B |
|---|---|---|
| Treatment (the one thing that differs) | | |
| AI layer hash | | |
| Agent and version (pinned, auto-update off) | | |
| Model id | | |
| Task set version and hash | | |
| Trials per task · order (interleaved?) · seed | | |
| Date and time window of the runs | | |

A/A check (same configuration run twice): interval ___ (should include 0).

## 3. Results

| Metric | A | B | Difference (B − A) | 95% interval | Method |
|---|---|---|---|---|---|
| Pass rate, all tasks | | | | | paired by task |
| Pass rate, holdout | | | | | paired by task |
| Golden tasks (each) | | | | — | floor check |
| pass^k (k = ) | | | | — | |
| Mean cost per trial | | | | | |
| Cost per passing trial | | | | | |
| Mean latency | | | | | |

Tasks better / worse / equal: __ / __ / __. Largest per-task moves (with a one-line reason from reading transcripts):

## 4. Verdict

- In one sentence, with the interval (e.g. "B is better by 22 points, 95% CI +10 to +33, on 24 tasks × 5 trials"):
- Or: "No detectable difference at this sample size; an effect smaller than ___ points could not be seen."

## 5. Threats and limits

- [ ] Grader error (calibration report link; which tasks use a judge)
- [ ] Contamination / leak check result
- [ ] Anything that changed besides the treatment
- [ ] What the task set does not cover
- [ ] Results valid for this model and agent version only; rerun on upgrade

## 6. Artifacts

- `results.csv`, run folders with `manifest.json`, calibration report, commit links.
