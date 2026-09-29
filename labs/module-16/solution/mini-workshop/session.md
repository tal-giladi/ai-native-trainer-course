# Session plan — Shadow rule (30-minute mini-workshop)

> Reference answer for Module 16, **illustrative**. One student's plan for teaching one concept from their method (`C1 · Shadow rule` in [`labs/module-14/solution/concepts.md`](../../../module-14/solution/concepts.md)) to six colleagues. Format: [lesson template](../../../../templates/lesson-template.md). Check it with `LearnCheck align solution/mini-workshop/session.md`.

- Concept: C1 · Shadow rule (method v1.0.0)
- Audience: 6 developers from the billing and payments teams, 1–12 years' experience, all use a coding agent at least weekly
- Room: in person, one screen, laptops with the lab repo cloned
- Budget: 30 minutes
- Date: 2026-08-12

## Learning claim

After 30 minutes, a developer who reviews an agent's pull request can tell whether it duplicated an existing business rule, find the rule it should have reused, and write the research-brief line that stops it happening on the next ticket.

## Prior knowledge (from three five-minute chats a week earlier)

- Everyone knows DRY. Two of three thought "the agent duplicated code" was the whole story.
- Misconception to surface: *any* agent change to business logic is a shadow rule (so they would reject correct extensions of the existing rule).
- Misconception to surface: a shadow rule is caught by tests (it is not: both copies pass their own tests).
- Nobody searched stored procedures for business terms; they searched C# only.

## Objectives

| ID | Objective | Level |
|---|---|---|
| O1 | Given an agent's diff, decide whether it added a shadow rule and name the existing implementation it duplicates. | analyze |
| O2 | Find every existing implementation of a business term across C# and T-SQL with one repository search before the agent plans. | apply |
| O3 | Write a research-brief line that makes the agent name the existing implementation it will reuse. | create |

## Assessment

Two parallel forms, A and B, with the same items on different surface details (different term, different file). Half the group takes A before and B after, half the reverse. Scored 0/1 by the key.

| Item | Objective | Level | What it asks |
|---|---|---|---|
| I1 | O1 | remember | Which description matches a shadow rule |
| I2 | O1 | analyze | Diff adds a second overdue/late check beside the existing one: shadow rule? |
| I3 | O1 | analyze | Diff extends the existing rule with a parameter: shadow rule? (misconception check) |
| I4 | O2 | apply | Which search finds the existing rule in C# **and** SQL |
| I5 | O2 | apply | Given search output, where the rule lives |
| I6 | O3 | create | Write the brief line for ticket 1 (rubric: names the term, requires the existing implementation by path, forbids a new one) |
| I7 | O3 | create | Write the brief line for ticket 2 (same rubric) |
| I8 | O1 | analyze | Diff adds a T-SQL computation next to a C# rule: shadow rule? |

## Activities

| Step | Minutes | Mode | Objective | New terms |
|---|---|---|---|---|
| Pre-assessment (form A or B), no discussion | 4 | assess | - | 0 |
| Story: INC-08, the balance calculated twice, two numbers on one invoice | 2 | show | O1 | 1 |
| Worked example: read the diff, search the term, find `usp_GetCustomerBalance` | 3 | show | O1, O2 | 1 |
| Pairs: three diffs, decide and justify (first one half-solved, one is a correct extension) | 6 | do | O1 | 0 |
| Live: search `overdue` in C# and SQL, write the brief line, start the agent; prediction question while it runs | 4 | show | O2, O3 | 1 |
| Solo: search and write the brief line for your own ticket (hint ladder on the handout) | 5 | do | O2, O3 | 0 |
| Reflect: muddiest point on a card, then "when would you not bother?" | 2 | reflect | O1 | 0 |
| Post-assessment (other form) and feedback form | 4 | assess | - | 0 |

## Materials

- Handout: the three pair diffs, the hint ladder, the brief-line rubric.
- Lab repo at tag `shadow-start`; recording of the live step as a fallback (see Module 15).
- Forms A and B with the key: `forms.md`; feedback form and follow-up message from the [pre/post assessment template](../../../../templates/pre-post-assessment.md).

## After the session

- Enter scores in `responses.csv`, forms in `feedback.csv`.
- Two weeks later: follow-up message, answers in `followup.csv`.
- Run `gain`, `feedback`, `followup`; write `results.md`.
