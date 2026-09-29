# Results — Shadow rule mini-workshop, 2026-08-12

> Reference answer for Module 16, **illustrative**. One student's write-up of the reference [session](session.md) with six developers. Every number comes from `LearnCheck` on the CSV files next to this page; the commands are at the bottom. Format: the "Results" section of the [pre/post assessment template](../../../../templates/pre-post-assessment.md).

## Summary (what I would say to a manager)

Six developers took parallel pre- and post-tests (8 items, forms counterbalanced). The class average rose from 37.5% to 79.2%: a normalized gain of **0.67** (95% bootstrap interval 0.55 to 0.85), in the medium band. That is one session with six people, so it tells me the session *can* teach the concept, not how much it will teach the next group. Two weeks later, 3 of 6 had used it at work with evidence (50%, Wilson interval 19% to 81%).

## Learning (level 2)

| Objective | Items | Pre | Post | g |
|---|---|---|---|---|
| O1 · decide whether a diff adds a shadow rule | I1, I2, I3, I8 | 50% | 83% | 0.67 |
| O2 · find every implementation in C# and T-SQL | I4, I5 | 50% | 83% | 0.67 |
| O3 · write the research-brief line | I6, I7 | 0% | 67% | 0.67 |

What the items say:

- **I3 (a correct extension is not a shadow rule): 17% → 83%.** The misconception from the prior-knowledge chats was real and the half-solved pair diff moved it. Keep that diff.
- **I8 (a T-SQL view duplicating a procedure): 33% → 50%, g 0.25.** The weakest item. The pair exercise had no SQL-only diff; everything SQL was shown in the live step. Add one SQL diff to the pair set.
- **I7 (second brief line): 0% → 50%.** Half wrote a line for I6 and then a vaguer one for I7 ("check for existing code"). The rubric's part (b), *name* the implementation, is the one they dropped. Put the rubric on the handout, not just in my head.
- **I2: 83% correct before teaching.** Too easy to show learning. Replace the obvious `IsLate` diff with one whose names differ more from the original.

## Reaction (level 1)

Rating 4.0 / 5 (top-two-box 83%), relevance 4.5 / 5. Self-rated confidence 71% against an actual post-test of 79%: they slightly *under*-rate themselves, the opposite of the first delivery. Comments point at the same two things as the items: the pair diffs were the useful part (P1, P3), the live search was too fast (P5), the brief line still feels awkward (P2). Rating and individual gain are unrelated here (rho 0.00), which says nothing with six people.

## Behaviour (level 3, two weeks later)

3 of 6 used it with evidence: a brief naming `InvoiceService.IsOverdue` (P1), a reuse line added to the team's research skill (P3), a rejected agent balance method (P4). P5 said yes with nothing to point at; counted as no. 50%, 95% Wilson interval 19% to 81%.

## What I change for the next delivery

1. Add one SQL-only diff to the pair exercise (I8).
2. Rubric for the brief line on the handout (I7).
3. Slow the live search: type it, pause, leave it on screen; put the command on the handout (P5).
4. Harder I2 on both forms.
5. Keep: counterbalanced forms, the misconception diff, the reflection question "when would you not bother?"

## What this does not show

- No control group. Some of the gain is the pre-test itself (it tells people what matters). Parallel forms reduce that; they do not remove it.
- Six learners, one team, one day, my own colleagues. The interval is wide and the group is friendly.
- The post-test was taken two minutes after the exercise. It measures what they can do now, not in a month.
- The follow-up is self-report with a pointer to evidence; I checked the three PRs and the skill change, not whether they would have done it anyway.

## Commands

```bash
cd labs/module-16
dotnet run --project tools/LearnCheck -- align    solution/mini-workshop/session.md
dotnet run --project tools/LearnCheck -- gain     solution/mini-workshop/responses.csv --plan solution/mini-workshop/session.md
dotnet run --project tools/LearnCheck -- feedback solution/mini-workshop/feedback.csv --responses solution/mini-workshop/responses.csv
dotnet run --project tools/LearnCheck -- followup solution/mini-workshop/followup.csv
```
