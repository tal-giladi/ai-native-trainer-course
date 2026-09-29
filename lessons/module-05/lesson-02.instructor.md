# Instructor notes — 05.2 Plans worth reviewing

**Teaching objective.** Students produce plans a human can approve or reject in five minutes and a script can check: checkable goal, evidence-backed decisions, touch/do-not-touch boundaries, steps with verify clauses, named HUMAN checkpoints with a stop rule, and runnable validation. They can explain what the plan lint and the scope gate do not prove.

**Likely confusion.** "Plan mode already produces a plan, so why a template?" Plan mode produces *a* plan, shaped by the prompt. Without required sections, the model writes the shape it has seen most: a numbered narrative with no boundaries. The template is a contract for the reviewer, not for the model.

**Common misconception.** "Boundaries slow the agent down." They slow *unreviewed* change down. The agent can still need an extra file; the rule is only that the plan changes first. Show the scope output from the break: the out-of-scope change was arguably an improvement, and it was still wrong to merge it under BILL-150.

**Key analogy.** A building permit. You can change the design after approval, but you file an amendment before you move a load-bearing wall, not after the inspector finds it.

**Common failure in the exercise.** Students accept the first draft because it passes the lint. Require at least one substantive edit and a timed review. Second failure: verify clauses that cannot fail ("verify: code looks correct"); push for a command or an observable result.

**Expected exercise outcome.** A 40–70 line `plans/BILL-150.md` passing plan-lint, with assumption A1 (the 7-day boundary) or an equivalent open question surfaced at the HUMAN checkpoint; a review time of 5–10 minutes; implementation with 10 tests; scope reporting exactly the six planned files. On the break branch, scope fails on `Legacy/MonthlyRevenueReport.cs` while tests and arch stay green.

**Extension exercise.** Log $q$ (fraction of plans you rejected or substantially changed) over your next ten plans, and the minutes each rejection saved. Compute whether plan review pays on your own tickets, per ticket size.

**Discussion question.** The reviewer approved the EF plan in 05.4 in three minutes. What would have to be true about your team's review culture for plan approval to be more than a rubber stamp — and is it true today?
