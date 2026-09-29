# Instructor notes — 11.3 Recurring automation

**Teaching objective.** Students decide which chores deserve an agent by net value and break-even acceptance, build every automation as propose, validate, apply with a single deterministic write step and an idempotency key, and recognize untrusted input in their own automations.

**Likely confusion.** Where the schema ends and the validator begins. The schema is enforced by the agent runtime and shapes what the model can output; the validator is your code and enforces policy that depends on meaning (p0 needs a human, no mentions in summaries). Both are needed; only the validator is under your review process.

**Common misconception.** "It runs by itself, so it is free." Every proposal costs a review. Put the test-generation row of the table on the board and ask who would have guessed it was negative.

**Key analogy.** A junior colleague who prepares the paperwork but has no signing authority. They fill the form; a clerk checks it against the rules; the manager signs. Nobody gives the junior the company stamp because they fill forms quickly.

**Common failure in the exercise.** The live triage run fails with `error_max_structured_output_retries` because the prompt and the schema disagree (a label in one but not the other). Second: students interpolate `${{ github.event.issue.body }}` into the prompt string "for convenience", recreating the script injection from 11.1; `wflint` catches it.

**Expected exercise outcome.** A ranked list of three chores with net value and break-even; one automation card; offline triage verdicts reproduced (ACCEPT, REJECT 6, REJECT 1, SKIP); the safe triage workflow running in the lab repository with BILL-913 ending as `needs-human`; the changelog job skipping on its second run in the same week. After the break: an explanation of why `wflint` only warns about triage-v0.

**Extension exercise.** Build the test-generation automation with a mutation check: for each generated test, revert or mutate the function under test (flip `<` to `<=` in `IsOverdue`) and require the test to fail. Measure acceptance before and after adding the check.

**Discussion question.** Some automations are valuable precisely because they act without a human (e.g. auto-labelling hundreds of issues). Where would you allow an apply step with no human review at all, and what property of the change makes that acceptable?
