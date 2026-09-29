# Forms — Ground before you generate

> Reference (illustrative). Parallel forms A and B for the eight items in [`agenda.md`](agenda.md), the key and rubrics, the feedback form and the follow-up message. Rules and CSV formats: [pre/post assessment template](../../../../templates/pre-post-assessment.md). Scores go into `responses.csv`, `feedback.csv` and `followup.csv` for `LearnCheck gain`, `feedback` and `followup` ([Module 16 lab](../../../module-16/README.md)).

Instructions on both forms: "This is not a test of you; it tells me what to teach. 'I don't know' is a fine answer. Use the ID on your card, not your name." Time: 6 minutes.

## Form A (ticket BILL-240: "skip reminders for invoices overdue by more than 90 days")

| Item | Question | Key or rubric |
|---|---|---|
| I1 | The agent's diff adds `if (inv.Status == 1 && inv.DueUtc < now)` in `ReminderJob`. `InvoiceService.IsOverdue` exists. Shadow rule? Name the evidence. | Yes; names `IsOverdue` (1 point) |
| I2 | The diff adds a `graceDays` parameter to `IsOverdue`; the three new tests were written by the agent in the same run. Which problem, if any? | Not a shadow rule; the tests are self-graded, so they prove nothing about existing callers (1 point) |
| I3 | Which search finds "overdue" in C# and T-SQL? (four options) | `git grep -n -i "overdue" -- "*.cs" "*.sql"` (1 point) |
| I4 | Write the Ground brief lines for BILL-240. | Rubric, all three for 1 point: names the term; lists every implementation with a path; says reuse, do not re-implement |
| I5 | Write the do-not-touch list for BILL-240's plan. | Rubric, 2 of 3 for 1 point: files; public contract; SQL object |
| I6 | Pair each of BILL-240's three criteria with a check. | Rubric: one per criterion, and at least one the agent cannot edit (1 point) |
| I7 | "All 9 tests pass." Which of five listed checks are evidence? | The unchanged existing tests and the convention test; not the agent's summary (1 point) |
| I8 | The agent changed an expected value in an existing test to make it pass. Pass or reject? Why? | Reject; an existing test was weakened to agree with new code (1 point) |

## Form B (ticket BILL-251: "show the customer balance on the statement")

Same eight items on a different surface: `usp_GetCustomerBalance` and `InvoiceService.OutstandingAsync` instead of overdue; a `StatementBuilder` diff; the same rubrics.

## Feedback form (one minute)

1. Overall, how would you rate this workshop? (1–5)
2. How relevant was it to your work this month? (1–5)
3. How confident are you that you could write a Ground brief for your next ticket without help? (1–5)
4. What was the muddiest point? (free text)
5. What will you try first, on which ticket? (free text)

## Follow-up message (two weeks later, individually)

> "Two weeks ago we did the Ground before you generate workshop. Have you written a Ground brief or a do-not-touch list on real work since? If yes, can you point me to the ticket, PR or brief? If no, what got in the way? One line is plenty."
