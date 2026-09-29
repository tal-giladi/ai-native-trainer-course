# P-01 · The rule your agent cannot see

- Format: post
- Concept: C1
- Stage: discover
- Audience: developers on legacy .NET systems who use a coding agent
- Evidence: INC-01, INC-08 (NOTES), C1 card
- Disclosure: none; I am not paid by any tool vendor
- Published: 2026-07-08

---

Your agent just wrote a customer-balance calculation in C#. It compiles, and its tests pass.

There was already a stored procedure that does it: `usp_GetCustomerBalance`. The new code ignores credit notes. The procedure does not.

I call this a shadow rule: when the agent cannot see a business rule that already exists, it writes a second copy with a slightly different meaning, and both stay in the code (INC-08).

It was not a one-off. In twelve weeks on one legacy billing system I logged four of them: an overdue check, the meaning of "issued", the balance, and VAT rounding (INC-01, INC-05, INC-08, INC-12).

What stopped it for me was one row in the research brief, before any plan: "Reuse: which existing class, procedure or view already implements this term?" The agent has to name it, with a file path, or say it searched and found nothing.

Where this does not apply: new code with no existing rules, or a term the agent already has in its context.

Where did your agent last duplicate a rule it could not see?
