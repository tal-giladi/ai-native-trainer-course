# Evaluation rubric and grader calibration template

Use when an answer cannot be graded by a deterministic check (an explanation, a review comment, a plan) and you want a rubric that a human or an LLM judge can apply the same way twice.
Goal: binary criteria tied to ground truth, a judge prompt that does not reward length or style, and a calibration against human labels before the judge grades anything that matters. Introduced in [07.3 · Graders](../lessons/module-07/lesson-03.md).

## 1. Rubric

- Task id and question:
- Ground truth (facts from the repository, with paths — the judge must not rely on its own opinion):
  -
- Criteria (each answerable yes / no, independent of the others):

| Id | Criterion | Yes when | No when |
|---|---|---|---|
| C1 | | | |
| C2 | | | |
| C3 | No invented facts | nothing contradicts the ground truth | any wrong name, ticket, API or date stated as fact |

- Verdict rule: PASS only if (e.g. all criteria are yes).
- Explicitly **not** criteria: length, tone, formatting, confidence, detail beyond the criteria.

## 2. Judge prompt checklist

- [ ] Rubric and ground truth come before the answer; the answer is fenced (`<answer>…</answer>`).
- [ ] "Length is not a criterion" is stated, with an example of a short passing answer.
- [ ] One line per criterion, then a fixed final line (`VERDICT: PASS|FAIL`) the harness parses.
- [ ] The judge runs in an empty folder with no project rules loaded, and with a pinned model.
- [ ] For pairwise judging: both orders are judged and disagreements count as ties (position bias).

## 3. Calibration set

| Field | Value |
|---|---|
| Number of answers (aim for 20–50, about half right) | |
| Where they came from (real agent runs; hand-written edge cases marked as such) | |
| Labellers (at least one domain expert; two for a subset to measure human agreement) | |
| Date · judge model and version | |

## 4. Calibration result (`EvalHarness calibrate`)

|  | Human pass | Human fail |
|---|---|---|
| Judge pass | TP | FP |
| Judge fail | FN | TN |

| Metric | Value | Target |
|---|---|---|
| Precision TP/(TP+FP) | | ≥ 0.90 when a pass gates a merge |
| Recall TP/(TP+FN) | | ≥ 0.80 |
| False-positive rate FP/(FP+TN) | | |
| Cohen's kappa | | ≥ 0.6 |
| Length check (long vs short pass rate at equal human label) | | gap < 20 points |

- Disagreements read and explained (one line each):
- Decision: use as gate / use as signal only / rewrite and recalibrate.
- Recalibrate when: the judge model or prompt changes, the task changes, or quarterly.
