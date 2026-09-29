# Pre/post assessment template

Measure whether a session taught what it claimed to teach: parallel pre- and post-tests, a feedback form that is read *next to* the scores, and a two-week behaviour follow-up. Introduced in [16.5](../lessons/module-16/lesson-05.md); items are aligned to objectives with the [lesson template](lesson-template.md) ([16.2](../lessons/module-16/lesson-02.md)). The CSV formats are what `LearnCheck gain`, `feedback` and `followup` in `labs/module-16/tools/LearnCheck` read. A worked example: [`labs/module-16/solution/mini-workshop/`](../labs/module-16/solution/mini-workshop/session.md).

## Four levels, four instruments

| Kirkpatrick level | Question | Instrument | When |
|---|---|---|---|
| 1 Reaction | Did they find it relevant and worth their time? | Feedback form | End of session |
| 2 Learning | Can they do what the objectives say? | Pre/post test, parallel forms | Start and end |
| 3 Behaviour | Did they do it at work? | Follow-up message with evidence | 2 weeks later |
| 4 Results | Did the team's outcomes change? | The team's own metrics (Module 13) | Months; usually out of reach for one session |

Level 1 is not evidence of level 2. Report them side by side, never one in place of the other.

## Writing items

- **Every item maps to one objective and one level**; every objective has one or two items (more is better; one item cannot tell a slip from a misconception).
- **Test performance, not recall.** For an objective that says *decide* or *write*, the item makes the learner decide or write. At most one recall item per objective.
- **Distractors are misconceptions.** Each wrong option is something a real person believed in your prior-knowledge chats, not a joke.
- **Include one "correct-looking non-example"** per concept: a case that resembles the concept but is not it (tests over-application).
- **Parallel forms.** Form A and form B have the same items on different surface details (different term, file, ticket). Half the group takes A then B, half B then A. Never give the same form twice.
- **Avoid the ceiling.** If most people get an item right before teaching, it cannot show learning. Replace items that were ≥ 80% correct on the pre-test.
- **Open items get a rubric with 0/1 criteria** written before the session, and are scored blind to phase (shuffle pre and post sheets before scoring).
- **Instructions:** "This is not a test of you; it tells me what to teach. 'I don't know' is a fine answer." Anonymous IDs (P1, P2, …), no names on sheets.
- **Time:** about 30 seconds per multiple-choice item, 90 seconds per open item.

## `responses.csv`

One row per learner per phase; item scores between 0 and 1.

```text
learner,phase,form,I1,I2,I3,I4,I5,I6,I7,I8
P1,pre,A,1,1,0,0,1,0,0,0
P1,post,B,1,1,1,1,1,1,0,1
```

Only learners with both a pre and a post row count. Someone who joined late or left early is excluded, not averaged in.

## The numbers

With class averages $\bar S_{pre}$ and $\bar S_{post}$ in percent, the **class normalized gain** (Hake 1998) is

$$\langle g \rangle = \frac{\bar S_{post} - \bar S_{pre}}{100 - \bar S_{pre}}$$

the share of the *possible* improvement that happened. Bands from Hake: low below 0.3, medium 0.3 to 0.7, high 0.7 and above. Report it with a bootstrap interval over learners (`LearnCheck gain` does both). Also useful: the mean of individual normalized changes $c$ (Marx & Cummings 2007), which handles learners who went down, and Cohen's $d$ for comparison with other fields. Never report $(\bar S_{post} - \bar S_{pre}) / \bar S_{pre}$: it grows as the pre-test gets worse.

## Feedback form (five questions, one minute)

1. Overall, how would you rate this session? (1–5)
2. How relevant was it to your work this month? (1–5)
3. How confident are you that you could ⟨objective O1 in plain words⟩ tomorrow without help? (1–5)
4. What was the muddiest point? (free text)
5. What will you try first, on which ticket? (free text)

`feedback.csv`: `learner,rating,relevance,confidence,comment`. Question 3 is compared with the actual post-test score: a large positive gap is a fluency illusion.

## Two-week follow-up

Send one message, individually:

> "Two weeks ago we did the ⟨concept⟩ session. Have you used it on real work since? If yes, can you point me to the ticket, PR or brief? If no, what got in the way? One line is plenty."

`followup.csv`: `learner,used,evidence`. "Yes" without a pointer counts as no. Report $k$ of $n$ with a Wilson interval ([07.4](../lessons/module-07/lesson-04.md)).

## Results

```markdown
# Results — <concept>, <date>

## Summary
<n> learners, parallel forms (<k> items), counterbalanced. Class average <pre>% → <post>%:
<g> = <x> (95% bootstrap interval <lo> to <hi>), <band>. Follow-up: <k> of <n> used it with evidence
(<p>%, Wilson <lo>–<hi>). What this does and does not show, in one sentence.

## Learning (level 2)
| Objective | Items | Pre | Post | g |
Item notes: ceiling items, items that went down, the misconception item, the weakest item and why.

## Reaction (level 1)
Rating, relevance, confidence vs actual, comments grouped by theme, each theme tied to an item result.

## Behaviour (level 3)
<k> of <n>, with the evidence each pointed to.

## What I change for the next delivery
Numbered; each change traced to an item, a comment or a room incident.

## What this does not show
No control group; pre-test effect; n; timing of the post-test; self-report.
```
