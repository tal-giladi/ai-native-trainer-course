# Module 7 quiz — Agent evaluation

Nine scenario questions on evaluating an agent system: a Contoso Billing AI layer, its skills, its model and its agent version. They test reasoning, not recall: most describe a result, a task, a grader or a pipeline and ask what it shows, what is wrong with it, or what to run next.

**Covers**

- [07.1 · Why evaluate agents](../lessons/module-07/lesson-01.md) — the one-run trap; task, trial, grader, transcript, outcome; qualitative vs quantitative.
- [07.2 · Task datasets](../lessons/module-07/lesson-02.md) — representative, golden and regression tasks; validity checks; holdout; contamination and answer leakage.
- [07.3 · Graders](../lessons/module-07/lesson-03.md) — outcome checks first; grader precision and recall; correcting for grader error; calibrating an LLM judge.
- [07.4 · Statistics for stochastic systems](../lessons/module-07/lesson-04.md) — Wilson intervals, pass@k vs pass^k, runs needed, clustering by task.
- [07.5 · Comparisons and paired designs](../lessons/module-07/lesson-05.md) — one treatment, pinned versions, interleaving, pairing, A/A.
- [07.6 · Evals in the loop](../lessons/module-07/lesson-06.md) — gate policy, golden floors, CI, incidents as regression tasks.

**Pass mark:** 70% (7 of 9). Unlimited retries; answers and explanations appear after you submit.

Before you start, it helps to have done the labs in [`labs/module-07`](../labs/module-07/README.md) — several questions use `EvalHarness` output and the tasks-v1 set.
