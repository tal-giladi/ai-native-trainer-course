---
id: "16.5"
module: 16
minutes: 18
practice_minutes: 90
prerequisites: ["16.2", "13.5", "07.4"]
objectives:
  - Compute and interpret the class normalized gain, the mean normalized change and Cohen's d from matched pre/post scores, with a bootstrap interval.
  - Identify measurement errors that fake or hide a gain — relative change, unmatched learners, repeated forms, ceiling items, undefined individual gains — and correct them.
  - Read feedback next to learning (ratings, confidence against scores, comments tied to items) and measure behaviour with a two-week follow-up and a Wilson interval.
  - Deliver a 30-minute mini-workshop to 3–8 people and write a results page that says what the numbers do and do not show.
volatility: concept
sources:
  - title: "Hake (1998) — Interactive-engagement versus traditional methods: A six-thousand-student survey of mechanics test data"
    url: https://pubs.aip.org/aapt/ajp/article/66/1/64/1055076/Interactive-engagement-versus-traditional-methods
  - title: "Marx and Cummings (2007) — Normalized change"
    url: https://pubs.aip.org/aapt/ajp/article-abstract/75/1/87/1056280
  - title: "Nissen, Talbot, Nasim Thompson and Van Dusen (2018) — Comparison of normalized gain and Cohen's d for analyzing gains on concept inventories"
    url: https://journals.aps.org/prper/abstract/10.1103/PhysRevPhysEducRes.14.010115
  - title: "Kirkpatrick Partners — The Kirkpatrick Model"
    url: https://www.kirkpatrickpartners.com/the-kirkpatrick-model/
  - title: "Uttl, White and Wong Gonzalez (2017) — SET ratings and student learning are not related"
    url: https://www.sciencedirect.com/science/article/abs/pii/S0191491X16300323
last_verified: "2026-09-28"
---

# 16.5 · Measuring learning

## Why it matters

After the redesign, the lab's student delivered the session again, to the payments team, and sent their manager a write-up the same afternoon: "Learning improved by 71%. Normalized gain 0.66, high band, the same as interactive-engagement physics courses. I propose we roll it out to all four teams." Every number in it was computed from real data. Almost every conclusion was wrong.

You have met this pattern before. Module 13 was about productivity claims that look positive and are confounded; your own teaching is not exempt. A client who pays for training will eventually ask what people learned, and "they loved it" is a level 1 answer to a level 2 question (16.1). This lesson gives you the instruments for levels 1–3, the one piece of math the module needs, and the list of mistakes that make a pre/post comparison say whatever you hoped. It ends with the field assignment: run your mini-workshop for real, and report it honestly.

> [!NOTE]
> Content tags. **Concept** (stable): matched pre/post designs, normalized gain and change, effect size, bootstrap intervals, measurement threats, reading feedback against learning, behaviour follow-up. **Implementation**: `LearnCheck gain`, `feedback`, `followup` and the CSV formats (as of 2026-09).

## How it works

### Normalized gain

*Intuition.* A raw gain of 20 points means something different for a group that started at 20% and one that started at 75%: the second group only had 25 points left to gain. Hake's normalized gain asks what share of the **possible** improvement actually happened.

*Equation.* With class averages $\bar S_{pre}$ and $\bar S_{post}$ in percent, for learners who took both tests:

$$\langle g \rangle = \frac{\bar S_{post} - \bar S_{pre}}{100 - \bar S_{pre}}$$

Hake (1998) used it to compare 62 introductory physics courses with 6,542 students on the same concept tests: traditional courses averaged $\langle g \rangle = 0.23$, interactive-engagement courses 0.48, and he labelled $\langle g \rangle < 0.3$ low, 0.3 to 0.7 medium, and 0.7 or more high.

*Tiny example.* The reference session (`labs/module-16/solution/mini-workshop/`): six learners, class average 37.5% before and 79.2% after.

$$\langle g \rangle = \frac{79.2 - 37.5}{100 - 37.5} = \frac{41.7}{62.5} = 0.67$$

A group that went from 70% to 90% would have the same $\langle g \rangle$ from half the raw gain. Now the number people put on slides instead, the **relative change** $(\bar S_{post} - \bar S_{pre})/\bar S_{pre}$: 111% for the first group, 29% for the second. It rewards a bad pre-test. A group starting at 10% that reaches 30% "improved 200%" while learning less than either. Never report it.

Two companions:

- **Mean normalized change** $c$ (Marx & Cummings, 2007) computes a gain per learner and averages. For a learner who improved, $c = (post - pre)/(100 - pre)$; for one who got worse, $c = (post - pre)/pre$, so a loss is also bounded; a learner with the same score before and after gets 0, except at 0% or 100%, where $c$ is undefined and the learner is dropped, not counted as a perfect 1.0. In the reference session $\bar c = 0.73$. It differs from $\langle g \rangle$ because it weights every learner equally.
- **Cohen's d**, the raw gain divided by the pooled standard deviation $\sqrt{(s_{pre}^2 + s_{post}^2)/2}$: 2.02 here. Nissen et al. (2018) showed that normalized gain is biased in favour of groups with high pre-test scores, while $d$ is not. When you compare two groups that started in different places, report both.

*Implementation.* With six learners, how much of 0.67 is the particular six? Resample learners with replacement 10,000 times and recompute $\langle g \rangle$ each time (the bootstrap from [13.5](../module-13/lesson-05.md)):

```bash
cd labs/module-16
dotnet run --project tools/LearnCheck -- gain solution/mini-workshop/responses.csv --plan solution/mini-workshop/session.md
```

```text
Class
  pre 37.5%  post 79.2%  (raw gain 41.7 points)
  normalized gain <g> = (post - pre) / (100 - pre) = 0.67   95% bootstrap interval 0.55 to 0.85
  band (Hake 1998): medium
  mean normalized change c (Marx & Cummings 2007) = 0.73 over 6 learners
  Cohen's d (pooled SD of pre and post) = 2.02
  relative change (post - pre) / pre = 111%  <- not a learning measure; do not report it
WARN  6 learners: the interval is wide; report it, and treat the gain as one observation, not a result
```

*Interpretation.* "This session closed about two-thirds of the gap between where these six people started and full marks on my eight items; with six people, anything from about half to 85% is plausible." With under ten learners, 13.5's warning applies: the bootstrap interval is if anything too narrow, so read it as optimistic. And do **not** compare 0.67 with Hake's 0.48: his were semester courses measured with a validated test; yours is 30 minutes and your own eight items. The bands are a vocabulary, not a league table.

### What fakes a gain, and what hides one

A pre/post design with no control group is the weakest design in Module 13's hierarchy. It is still worth running, because at 30 minutes most of the usual threats (maturation, history) have no time to act. The ones that remain are about the test:

| Threat | What it does | Guard |
|---|---|---|
| Same form twice | learners remember items; post-test measures memory | parallel forms A/B, counterbalanced |
| Pre-test effect | the pre-test itself tells people what matters | accept and state it; parallel forms reduce it |
| Unmatched learners | a late joiner who aced the post-test inflates the post average | only learners with both tests count |
| Ceiling items | items most people get right before teaching cannot show learning | replace items ≥ 80% on the pre-test |
| Undefined individual gains | a learner at 100% before and after counted as $c = 1$ | drop them from $\bar c$; report how many |
| Scoring bias | you score post-tests more generously | score open items blind to phase, with a rubric |
| Selection and regression | analysing only the low scorers inflates the gain ([13.4](../module-13/lesson-04.md)) | report the whole group |
| Immediate post-test | measures what they can do now, not in a month | the two-week follow-up |

### Reading feedback next to learning

The feedback form is level 1 and cannot stand in for level 2 (Uttl et al., 2017). It is still useful, read **next to** the scores:

- **Rating and relevance** tell you whether people will come back and whether you picked the right concept.
- **Confidence against the actual score.** In 16.1, confidence ran 46 points ahead of performance (a fluency illusion); in the reference session it ran 8 points behind.
- **Comments, tied to items.** "Too fast in the live part, the search scrolled away" (P5) and item I8 (0.25, the weakest) point at the same step. A comment that matches an item result is a fix; a comment that matches nothing is an opinion.
- **Rating against individual gain** is almost always uncorrelated in groups this small (rho 0.00 in the reference). Do not interpret it; do notice when the loudest praise comes from people who learned nothing.

### Behaviour: the two-week follow-up

Level 3 asks whether anyone used it. One individual message, two weeks later: *"Have you used it on real work since? Can you point me to the ticket, PR or brief?"* A yes without a pointer counts as no. In the reference session, 3 of 6 pointed to evidence: 50%, and with the Wilson interval from [07.4](../module-07/lesson-04.md), 19% to 81%. That interval is the honest size of what six people can tell you.

## Show me

Run the check on the payments-team delivery the manager write-up was based on:

```bash
dotnet run --project tools/LearnCheck -- gain break/16.5-measurement/responses.csv --plan solution/mini-workshop/session.md
```

```text
WARN  Q7: post-test only, excluded (joined late?); counting it inflates the post average
ERROR 6 of 6 learners took the same form before and after (Q1, Q2, Q3, Q4, Q5, Q6): the post-test measures memory of the pre-test
  Q4     pre 100.0%  post 100.0%  c n/a (pre = post at 0 or 100)
  normalized gain <g> = (post - pre) / (100 - pre) = 0.60   95% bootstrap interval 0.50 to 0.69
  band (Hake 1998): medium
  mean normalized change c (Marx & Cummings 2007) = 0.60 over 5 learners (1 dropped)
WARN  item I1: 83% correct before teaching; it cannot show learning (ceiling)
WARN  item I2: 83% correct before teaching; it cannot show learning (ceiling)
WARN  item I5: 100% correct before teaching; it cannot show learning (ceiling)
```

Compare with `break/16.5-measurement/manager-email.md`.

## Try it

This is the module's **field assignment**. Budget: 90 minutes of work around a 35-minute session, plus a follow-up two weeks later. The full protocol is in the [labs README](../../labs/module-16/README.md#field-assignment-the-30-minute-mini-workshop).

> [!WARNING]
> You are collecting scores and opinions from colleagues. Anonymous IDs on every sheet, the key to names kept off any repository, no individual scores shared with anyone's manager, and your company's survey policy followed if it has one.

1. **Before.** Recruit 3–8 people who did not help build your concept. Print forms A and B (counterbalanced), the handout and the feedback form from the [pre/post assessment template](../../templates/pre-post-assessment.md). Check `align` once more.
2. **Deliver** the session as planned: pre-test, session, post-test and feedback form.
3. **Score** within 24 hours: shuffle pre and post sheets together before scoring open items, so you do not know which you are marking. Enter `responses.csv` and `feedback.csv`.
4. **Analyse:**

```bash
dotnet run --project tools/LearnCheck -- gain ~/talks/mini-workshop/responses.csv --plan ~/talks/mini-workshop/session.md
dotnet run --project tools/LearnCheck -- feedback ~/talks/mini-workshop/feedback.csv --responses ~/talks/mini-workshop/responses.csv
```

5. **Write `results.md`** from the template: summary, level 2 by objective and item, level 1 tied to items, what you change, what the numbers do not show.
6. **Two weeks later**, send the follow-up, fill `followup.csv`, run `followup`, and add level 3 to `results.md`.

<details>
<summary>Hint: my gain is low and I want to rerun before writing it up</summary>

Write it up first. A low gain with a clear diagnosis (which objective, which item, which step) is the most useful result this module can produce, and it is exactly what 16.1's student learned from. Rerun afterwards with the changes, and report both deliveries.
</details>

## Break it

Read `labs/module-16/break/16.5-measurement/manager-email.md` and list every error you can find before running `gain` on `responses.csv`. There are at least eight.

## Fix it

**Diagnose.**

1. **"Improved by 71%"** is relative change, and it included Q7.
2. **Q7 joined late** and took only the post-test. Counting them moved the post average from 79.2% to 82.1%.
3. **Same form, pre and post.** Every learner saw the identical items twice, 20 minutes apart. The gain mixes learning with memory of the pre-test, and nothing in the data can separate them. `gain` makes this an error, not a warning.
4. **Q4 counted as a perfect 1.0.** At 100% before and after, $c$ is undefined; the honest $\bar c$ is 0.60 over five learners.
5. **"High band."** 0.66 (and the correct 0.60) is medium; Hake's high band starts at 0.7.
6. **"The same as interactive-engagement physics courses."** Different tests, different durations, different populations. Not comparable.
7. **"Every item improved or stayed at 100%."** I1, I2 and I5 were at 83–100% before teaching: three of eight items could not show learning. The informative items are the other five.
8. **No interval, and a rollout from one session of six people.** The corrected interval is 0.50 to 0.69, and it describes this group, not four teams.
9. **"Use the same test next month so the numbers stay comparable."** It keeps the numbers comparable and wrong. Next month's group will also have colleagues who took it.

**Modify.** Rewrite the summary:

```text
Six people took the same eight items before and after (Q7 joined late and is excluded).
Class average 47.9% -> 79.2%: normalized gain 0.60 (95% bootstrap interval 0.50 to 0.69),
medium band. Because the forms were identical, part of this is memory of the pre-test; three
items were at or near ceiling before teaching. The result is consistent with the first
redesigned delivery (0.67) but is weaker evidence. Next delivery: parallel forms, harder I1,
I2 and I5, follow-up at two weeks. No rollout decision from this data.
```

Then replace I1, I2 and I5 on both forms, and switch to counterbalanced A/B.

**Rerun.** `gain solution/mini-workshop/responses.csv` shows no errors: forms counterbalanced, every learner matched, one ceiling item (I2) already on the "change next time" list in `results.md`.

## How do I know it works?

- [ ] `gain` on your own data reports no errors, and every warning is either fixed or discussed in `results.md`.
- [ ] Your summary gives $\langle g \rangle$ with its interval and $n$, and does not contain a relative change or a comparison with other people's gains.
- [ ] Level 1, 2 and 3 appear side by side, and every change you propose traces to an item, a comment or a room incident.
- [ ] "What this does not show" names at least the missing control group, the pre-test effect, $n$ and the timing of the post-test.
- [ ] `talks/mini-workshop/` contains the session plan, forms, handout, the three CSVs and `results.md`.

## Use / don't use

**Use** matched pre/post with parallel forms for every session you will deliver more than once; the second delivery is where the numbers start paying. **Use** $\langle g \rangle$ for one group over time and $d$ as well when you compare groups with different starting points.

**Don't** report a relative change, an unmatched average or a gain without its $n$. **Don't** sell a gain from one session as a property of your method: it is one observation of one delivery. **Don't** skip the follow-up because it is awkward; level 3 is what a client will eventually ask for.

**Limitations.**

- Your items are not a validated instrument. A high gain on an easy, narrow test is still a high gain on an easy, narrow test.
- No control group means you cannot separate teaching from the pre-test, the forms or the day. Module 13's designs apply if you ever need a causal claim, and they need many more learners than a mini-workshop has.
- Normalized gain favours groups that start high (Nissen et al., 2018), and it is undefined for anyone at 100% before.
- Self-reported behaviour with a pointer is better than self-report alone, but it is still what people chose to tell you.

## Reflect

1. What was your $\langle g \rangle$ and interval, and what would you have guessed before scoring?
2. Which item told you the most about your teaching, and what will you change because of it?
3. What did the feedback form say that the scores did not, and the other way round?

## Sources

- [Hake (1998) — Interactive-engagement versus traditional methods](https://pubs.aip.org/aapt/ajp/article/66/1/64/1055076/Interactive-engagement-versus-traditional-methods) — definition of the average normalized gain from class averages; 62 courses, 6,542 students; 0.23 traditional vs 0.48 interactive engagement; low, medium and high bands.
- [Marx and Cummings (2007) — Normalized change](https://pubs.aip.org/aapt/ajp/article-abstract/75/1/87/1056280) — per-student normalized change that handles losses; averaging individual changes; dropping undefined cases.
- [Nissen et al. (2018) — Comparison of normalized gain and Cohen's d](https://journals.aps.org/prper/abstract/10.1103/PhysRevPhysEducRes.14.010115) — 4,551 students in 89 courses; normalized gain is biased in favour of high pre-test populations, Cohen's d is not.
- [Kirkpatrick Partners — The Kirkpatrick Model](https://www.kirkpatrickpartners.com/the-kirkpatrick-model/) — reaction, learning, behaviour and results as separate levels of evaluation.
- [Uttl, White and Wong Gonzalez (2017) — SET ratings and student learning are not related](https://www.sciencedirect.com/science/article/abs/pii/S0191491X16300323) — why a feedback score cannot stand in for a learning measure.
