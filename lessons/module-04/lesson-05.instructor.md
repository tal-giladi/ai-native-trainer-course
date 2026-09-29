# Instructor notes — 04.5 The context audit

**Teaching objective.** Students run a complete context audit as a project: bucket every block (must always know / retrieve when needed / never load / generate dynamically), cut the always-loaded layer by ≥ 70%, prove on a fixed 10-task set that results held or improved, and report tokens, pass rate and cost per passing answer before and after.

**Likely confusion.** "Pass rate went from 24/30 to 25/30, so the new layer is better." Not with 30 runs. Teach the vocabulary "held" / "clearly improved" / "one task collapsed" now; Module 7 gives the intervals.

**Common misconception.** "Fewer tokens is the goal." The goal is the same or better results at lower cost and attention; tokens are the constraint. A 95% cut that drops one critical fact is a regression. Conversely, a layer that saves little money can still be worth it because contradictions and distractors are gone.

**Key analogy.** Refactoring with a test suite. You would not delete 700 lines of production code because they "look unused" without running the tests; the task set is the test suite for the context layer, and ablation is `git bisect` for facts.

**Common failure in the exercise.** (1) Running "before" and "after" on different days with a silent model or agent upgrade in between — have them record `claude --version` and the model in the results. (2) Counting grader false negatives as context failures and re-adding lines that were never needed. Insist that every failure is read and labeled. (3) Measuring in one interactive session instead of fresh runs.

**Expected exercise outcome.** A reduced layer of roughly 25–60 always-loaded lines (≥ 70% cut in lines and tokens), a completed audit table, `results.csv` with a before and an after row, a report where the after pass rate is at least the before rate (typically higher, because contradictions on F1–F3 are gone), three ablations, and every failure labeled. Most students find at least one grader false negative and one fact they dropped too eagerly.

**Extension exercise.** Add a no-context baseline: run the task set with `claude --bare -p …` (skips CLAUDE.md; needs `ANTHROPIC_API_KEY`) and compare three rows — none, bloated, reduced. Which facts does the model get right with no layer at all? That is your list of lines that did not need to exist.

**Discussion question.** A client's manager asks for "the percentage productivity gain" from your context audit. What can you honestly claim from this project's numbers, what can't you, and what would you need (Modules 7 and 13) to say more?
