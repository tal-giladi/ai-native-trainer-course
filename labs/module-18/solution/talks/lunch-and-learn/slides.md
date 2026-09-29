# P-02 · Lunch-and-learn: ground before you generate

- Format: lunch-and-learn
- Target: 30 minutes
- Q&A: 10 minutes
- Concept: C1, C2
- Stage: trust
- Audience: the student's own platform team and two neighbouring teams (11 people), internal
- Evidence: INC-08, INC-10 (NOTES); C1 and C2 cards
- Repo: brownfield-demo @ ai-layer-v1 (public demo, never the team's own code on screen)
- Feedback: three-question form, read first: "What will you try on a ticket this week?"
- Disclosure: none
- Delivered: 2026-07-15, 12:30, meeting room and call

---

[slide 1 · title, no logo, one line: "Two ways an agent's PR is wrong while everything is green"]

Thanks for giving up lunch. Twenty minutes of me, ten of you. Please interrupt.

[slide 2 · a diff: C# balance method, tests green]

Here is a pull request from the demo repository. Build green, tests pass. Look at it with the person next to you for thirty seconds: would you approve it?

[pause 30 s, take two answers]

The procedure `usp_GetCustomerBalance` already computes the balance and handles credit notes. This method does not. I call that a shadow rule: the agent writes a second copy of a rule it cannot see. I logged four of these in twelve weeks on one repository.

[slide 3 · the brief's reuse row]

What stopped it for me is one row in a research brief, written before any plan: for every business term in the ticket, which existing class, procedure or view implements it. The agent names it with a path or says it searched and found none.

[live · brownfield-demo, BILL-97, the research step; fallback: recording before-BILL-97]

Let me run it. While it searches, a question for you: where would it look first in our kind of code?

[slide 4 · "All 212 tests pass"]

Second pattern. The agent reported that all 212 tests pass. They did. The new test project was not in the solution, so its tests did not run. I call this self-graded green: when the only tests are the ones the agent wrote, green means the agent agrees with itself.

[slide 5 · the gate]

The fix is a check the agent cannot edit: the test count must rise, and every acceptance criterion needs a test a human reviewed.

[slide 6 · limits]

What I do not know: this is one repository and one agent; I have no numbers for your code. The loop costs time on one-line tickets, so skip it there.

[slide 7 · one question]

Before you go, on the form: what will you try on a ticket this week? Now, questions.
