# P-05 · One shadow rule, found in five minutes (script)

- Format: video
- Target: 5 minutes
- Concept: C1
- Stage: trust
- Audience: developers on legacy .NET and SQL Server systems who use a coding agent
- Evidence: INC-08 (NOTES), C1 card
- Repo: brownfield-demo @ ai-layer-v1 (public, ticket BILL-97)
- Captions: reviewed (edited by hand, 2026-08-25)
- Hook: your agent's new code passes its tests and still disagrees with a stored procedure you forgot existed
- Disclosure: none
- Published: 2026-08-26

---

[screen: brownfield-demo, the diff of a C# balance method, tests green]

Here is a pull request from a coding agent. The build is green, the tests pass. Is it right? Take five seconds and look at the diff.

[pause 5 s]

It is not. There is already a stored procedure that computes the customer balance, and it handles credit notes. This method does not. Now there are two definitions of "balance" in the system, and they disagree.

I call that a shadow rule. The agent could not see the existing rule, so it wrote its own.

[screen: brownfield-demo at tag before-ai-layer, terminal]

Let me show you how it happens. This is the repository before I added anything for the agent. I give it the ticket, BILL-97, and nothing else. Watch the plan.

[screen: agent plan, highlight step 2]

Step two says "calculate the balance". It does not ask whether a balance already exists. Nothing in its context told it to.

[screen: brownfield-demo at tag ai-layer-v1, research brief]

Now the same ticket with the research step. The brief has one row that matters here: reuse. The agent must name the class, procedure or view that already implements each business term in the ticket, with a path, or say it searched and found none.

[screen: brief row "balance: db/procs/usp_GetCustomerBalance.sql"]

It found the procedure. The plan now calls it instead of writing a new one.

[screen: the smaller diff]

Same ticket, smaller diff, one definition of balance.

When does this not matter? On new code with no rules to duplicate, and when the rule is already in the agent's context. And one caveat: this is one repository and four incidents in twelve weeks, so it is a pattern I trust on this code, not a law.

Try it on your next ticket. Before the plan, ask: which existing code already implements this word? The repository is public, pinned to the tag in the description, so you can run exactly what you just watched.
