# Confidence intervals for judge-corrected pass rates

- **Topic ID:** llm-judge-corrected-score-intervals
- **Status:** ADD
- **Next review:** n/a
- **Course change:** lessons/module-07/lesson-03.md (new subsection "How sure is the corrected number?", objective 3 extended, one Limitations bullet extended, one checklist line, one source; quiz q5 added; instructor-notes paragraph; glossary "Rogan–Gladen correction"; no new lesson)
- **Candidates:** C-20261031-01

## Summary

07.3 already corrects an LLM judge's observed pass rate for its TPR/FPR, which is the Rogan–Gladen (1978) estimator. That corrected number inherits the noise of the small human-labelled calibration set that measured TPR and FPR. Lee et al. (ICML 2026) give the delta-method interval with test-set and calibration-set terms, a small-sample adjusted-Wald version, an uneven label-allocation rule, and they show the estimator stays unbiased under a shift in pass rate between the test and calibration sets, provided the judge's error rates given the true label do not change. With the course's own 24-label calibration set the interval is about ±19 points at n = 100, so the learner should report corrected scores with an interval and size the label set to the precision the client needs.

## Evidence

- https://proceedings.mlr.press/v306/lee26bl.html — peer-reviewed ICML 2026 (PMLR 306:65231–65252), fetched 2026-11-01; full PDF (https://raw.githubusercontent.com/mlresearch/v306/main/assets/lee26bl/lee26bl.pdf) read for eqs. (4)–(10): estimator, variance, adjusted CI, allocation, distribution-shift assumption.
- https://arxiv.org/abs/2511.21140 — preprint since Nov 2025 (v3), so about a year of public scrutiny before the proceedings (found via search).
- https://github.com/UW-Madison-Lee-Lab/LLM-judge-reporting — public implementation (86 stars at fetch; license not shown).
- Method foundations are long-established and independent of this paper: Rogan & Gladen (1978) prevalence correction; Lang & Reiczigel (2014) confidence interval, both cited by the paper. The course's worked numbers were computed for this review from eq. (10) and eqs. (6)–(7).

## What would change the decision

If `EvalHarness calibrate` later gains an interval output (lab code is .NET, not built here), point the lesson's Implementation paragraph at it. Pending lab idea: add a `--n` option to `calibrate` that prints the corrected rate with this interval.

## History

- 2026-11-01 — ADD — peer-reviewed, classical-foundation method that closes a gap in 07.3 (corrected score reported without uncertainty); added as one subsection plus quiz q5, not a new lesson — weekly/2026-W44.md — candidates C-20261031-01
