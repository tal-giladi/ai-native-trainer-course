# Evaluation dataset template

Use when you build or extend a task set for an agent, a skill, a rules file or a model choice — and before any comparison that will be reported to someone else.
Goal: every task has a pass criterion written **before** the first run, a grader that has been checked against a right and a wrong answer, a source, and a split. Introduced in [07.2 · Task datasets](../lessons/module-07/lesson-02.md); graders in [07.3](../lessons/module-07/lesson-03.md).

> [!WARNING]
> Tasks built from employer or client code, tickets or incidents are confidential. Keep the dataset in a private repository, and never publish a task whose prompt or reference answer quotes internal code, names or data.

## Dataset card

| Field | Value |
|---|---|
| Name and version | |
| System under test (repository, commit range, agent) | |
| Decision this dataset informs | (e.g. "ship AI-layer changes", "choose between two models") |
| Size by kind (question / code / open answer) | |
| Split (dev / holdout, which ids) | |
| Tags (golden / regression / representative) | |
| Trials per task (smoke / comparison) | |
| Owner and review | |
| Date created · last changed | |

## Task table

| Id | Kind | Split | Tags | Source (ticket, incident, audit finding) | Pass criterion (one sentence) | Grader type | Reference ready? | Counterexample ready? |
|---|---|---|---|---|---|---|---|---|
| T01 | | | | | | | | |

## Checks before the first run

- [ ] Every task's pass criterion is written down and was written before anyone saw agent output for it.
- [ ] Every grader passes its reference answer and fails at least one realistic wrong answer (`EvalHarness validate`).
- [ ] Code tasks: golden tests fail on the unmodified repository and pass with the reference solution (`validate --repo`).
- [ ] At least 20% of tasks are in a holdout split that nobody tunes the system against.
- [ ] Golden tasks (must never regress) are tagged, and a regression task exists for every incident in the log.
- [ ] Coverage: the task mix resembles the real work (read / write / review; each major area of the codebase).
- [ ] Contamination: no prompt or reference answer appears in the AI layer (`EvalHarness leak`); public-benchmark tasks are marked as possibly seen in training.
- [ ] The task file is under version control, and its hash is recorded with every result.

## Change log

| Date | Version | Change | Why | Results before/after still comparable? |
|---|---|---|---|---|
| | | | | |
