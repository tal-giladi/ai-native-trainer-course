# tasks-v1 — dataset card (Contoso Billing)

The Module 4 task set ([`tasks-v0`](../../module-04/tasks-v0/README.md)) turned into an evaluation dataset: 24 tasks, each with a written reference answer, a grader that has been checked against that reference and against at least one wrong answer, a source, a split and tags. Filled in from the [evaluation dataset template](../../../templates/evaluation-dataset.md).

| Field | Value |
|---|---|
| Version | `tasks-v1` (the harness prints a short SHA-256 of `tasks.json`; any edit makes a new version) |
| Repository under test | Contoso Billing, `labs/module-03/brownfield`, with whatever AI layer you put on top |
| Purpose | Regression and comparison suite for AI-layer changes (rules, context, skills, model, agent version) |
| Size | 24 tasks: 19 question tasks graded by regex, 3 code tasks graded by golden tests and scope, 2 rubric tasks graded by a calibrated LLM judge |
| Split | dev 16 (you may look at failures and tune the layer against them) · holdout 8 (T05, T08, T13, T16, T20, T22, T23, T24: run them, read only the score) |
| Tags | `golden` (4: T04, T06, T10, T18; must never regress) · `regression` (7: born from a real failure) · `representative` (18: typical daily questions and tickets) |
| Trials | 3 per task for a smoke run, 5 for a comparison (lesson 07.4 explains why) |
| Owner | whoever owns the AI layer (see `CODEOWNERS` from lesson 03.1) |

## Where the tasks came from

- **T01–T10**: tasks-v0 unchanged in prompt, so old and new results can be compared. T02's grader was repaired after the Module 4 false negative (a correct answer that named `ListIssuedByCustomerAsync` without the interface name failed); it now checks for a `Task<…>` signature, a `CancellationToken` and a status filter.
- **T11–T17, T23, T24**: new questions about facts the Module 3 audit found and the Module 5 tickets depended on.
- **T18–T20**: code tasks from tickets BILL-152, BILL-151 and BILL-142 (criterion 3). The agent edits a copy of the repository; the harness then adds golden tests the agent never saw and checks which files changed.
- **T21, T22**: open answers (an onboarding explanation and a review comment) that no regex can grade fairly. They use the rubrics in `graders/`.

## Graders

| Type | Pass condition | Checked by `validate` |
|---|---|---|
| `regex` | every `must` pattern matches and no `mustNot` pattern matches (case-insensitive, multiline) | the reference passes; every counterexample fails |
| `tests` | golden tests in `golden/Txx/` pass in the agent's working copy, none skipped; only `scope` paths changed | with `--repo`: golden tests fail on the unmodified repository and pass with the reference overlay |
| `judge` | the judge's last line is `VERDICT: PASS` | the rubric file exists; calibrate the judge separately (lesson 07.3) |

## Known limits

- Regex graders check wording, not meaning. `validate` proves they accept one right answer and reject a few wrong ones, not that they accept every right answer. T12, for example, fails "Issued (not Paid, Void or Draft)", a correct answer. Read failing transcripts before believing them.
- The question tasks ask for terse answers to keep grading reliable. Real work is longer; that is why the code tasks exist.
- 24 tasks detect large differences, not small ones. See lesson 07.4 before you quote a percentage.
- Everything is fictional. Do not add tasks built from employer code to a public copy.

## Changelog

- **v1 (2026-09-28)** — from tasks-v0: references and counterexamples added to T01–T10; T02 grader repaired; T11–T24 added; dev/holdout split and tags added.
