---
id: "17.4"
module: 17
minutes: 15
practice_minutes: 600
prerequisites: ["17.1", "17.2", "17.3", "15.4"]
objectives:
  - Run three rehearsals with different jobs (a solo technical run, a friendly run with a drilled failure, a dress rehearsal with outsiders) and log what each one measured.
  - Split timing drift into the parts you control and the parts you estimated, and decide from it what to change in the agenda.
  - Score a rehearsal with two observers on an anchored rubric, read their agreement, and calibrate where they differ by two points or more.
  - Decide from the logs whether the workshop meets the exit standard: the full two hours from the agenda with notes checked at most twice.
volatility: concept
sources:
  - title: "Ericsson, Krampe and Tesch-Römer (1993) — The role of deliberate practice in the acquisition of expert performance"
    url: https://doi.org/10.1037/0033-295X.100.3.363
  - title: "Macnamara and Maitra (2019) — The role of deliberate practice in expert performance: revisiting Ericsson, Krampe & Tesch-Römer (1993)"
    url: https://royalsocietypublishing.org/doi/10.1098/rsos.190327
  - title: "Brown, Cai and DasGupta (2001) — Interval Estimation for a Binomial Proportion"
    url: https://projecteuclid.org/journals/statistical-science/volume-16/issue-2/Interval-Estimation-for-a-Binomial-Proportion/10.1214/ss/1009213286.full
last_verified: "2026-09-28"
---

# 17.4 · Rehearsal protocol

## Why it matters

The lab's student rehearsed three times before their first paid workshop, as the career path advises. The logs are in `labs/module-17/break/17.4-rehearsals/`: three evenings in the living room, slides on the laptop, no audience, the hands-on steps "assumed" at a few minutes each, every criterion self-scored 4 out of 4, "everything went fine". The third log shows the whole workshop 4 minutes over plan, which looked acceptable.

On the day, the hands-on steps took the minutes the agenda gave them, because fourteen real people were doing them. The talk and live parts took what the rehearsals had quietly shown they take. The workshop reached the post-test at 2:04, the room was booked until 2:00, and half the group left without taking it. The session that was supposed to produce the student's first evidence of learning in a paid room produced ratings and no scores.

A rehearsal is a measurement. Rehearsing alone measures some things well (how long you talk, whether each fallback plays) and cannot measure others at all (how long fifteen people take to write a brief, what the room asks, whether your notes are readable when you are nervous). This lesson is the protocol: three rehearsals, each designed to measure something different, logged, scored by people who are not you, and judged against a standard written before you start.

> [!NOTE]
> Content tags. **Concept** (stable): deliberate practice with feedback, rehearsals as measurements, drift decomposition, anchored rubrics and observer agreement, an exit standard. **Implementation**: `KitCheck rehearsal` and the log format; `DemoCheck drill` from Module 15.

## How it works

### Practice with feedback, not repetition

Ericsson, Krampe and Tesch-Römer (1993) introduced **deliberate practice**: activities designed to improve performance, usually with a teacher, focused on specific weaknesses, with immediate feedback. Running through your talk three times is repetition. Running through it with someone scoring the exercise launch, and changing the launch before the next run, is deliberate practice.

Keep the claim modest. The original study said accumulated practice "largely" accounted for differences between violinists; a replication with double-blind procedures (Macnamara & Maitra, 2019) found practice explained about 26% of the variance among skill groups, and the best violinists had practised *less* than the good ones. Practice matters, and it is not everything. What transfers to a workshop is the design: specific criteria, outside feedback, a change after every run.

### Three rehearsals, three jobs

| | Rehearsal 1: solo technical run | Rehearsal 2: friendly run | Rehearsal 3: dress rehearsal |
|---|---|---|---|
| Who | you, a timer, a recorder | 3–5 colleagues on their own laptops; 1 observer | 3–8 people who did not build it; 2 observers |
| Measures | talk and live minutes; every fallback switched once; notes checks | setup check on real laptops; hands-on minutes; a failure recovered in front of people | the whole workshop against the exit standard; Q&A; pre/post |
| Cannot measure | hands-on minutes, questions, the room | an outsider's questions; your nerves with strangers | anything after one delivery (behaviour, results) |
| Special move | switch to every fallback once, on purpose | `DemoCheck drill` picks a failure; only the observer knows | full forms A/B; observers score independently |
| Typical output | a restore in front of the room, silent waits, missing prediction questions | a new troubleshooting entry, a changed exercise launch | READY, or the one criterion to fix |

**Rehearsal 1** finds the problems you can fix alone. In the reference log, `/prime` ran 4:10 with 70 silent seconds, the plan landed with no prediction question ready, and `dotnet test` restored packages in front of the (empty) room. Three changes, all to the kit: two prediction questions on cue cards, `--no-restore` and a restore at T−30. Count the notes checks from the recording: every time your eyes go to the page. Eleven is normal for a first run.

**Rehearsal 2** puts real laptops in the room. Learners run the setup check from the starter pack two days before, then do every hands-on step. Before it starts, run the drill from [15.4](../module-15/lesson-04.md) and tell only the observer which segment will fail; recover in front of people, by the run sheet's clock. In the reference log, a Windows laptop failed a convention test on line endings (a new troubleshooting entry, T-04) and the drilled API failure in Live 2 was recovered in 50 seconds. The observer scored the exercise launch 2: the task and done-criteria were said, the time box was not.

**Rehearsal 3** is the workshop, from the doors opening to the last form, in front of people who have never seen the kit: a meetup group, another department, a study group. Two observers score independently with the rubric. Forms A/B are real, and `LearnCheck gain` runs on them afterwards, knowing that six learners give a wide interval ([16.5](../module-16/lesson-05.md)).

### Timing: split the drift

The total drift of a rehearsal is $\sum_i (a_i - p_i)$ over steps $i$, actual minus planned. It hides as much as it shows, because positive and negative errors cancel. Split it:

$$\Delta_{\text{controlled}} = \sum_{i \in \text{talk, live}} (a_i - p_i), \qquad \Delta_{\text{estimated}} = \sum_{i \in \text{hands-on, pairs}} (a_i - p_i).$$

The break's third log: credibility +3, problem +3, Live 1 +4, Live 2 +3, Live 3 +2, Q&A +2, so $\Delta_{\text{controlled}} = +17$. Pairs −3 and the three hands-on steps −4, −4, −2 because they were "assumed", so $\Delta_{\text{estimated}} = -13$. Total: +4, which looked fine. On the day, $\Delta_{\text{estimated}}$ became about zero, because real people used the minutes the agenda gave them, and the workshop ran +17 into a 7-minute buffer. That is the post-test at 2:04, predicted by the log.

Interpretation: a solo rehearsal measures $\Delta_{\text{controlled}}$ well and $\Delta_{\text{estimated}}$ not at all. Fix the controlled overrun in the talk and live parts, which are yours; measure the estimated part only with people doing the exercises.

### Live segments: rates, not memories

The run sheet keeps a count of clean rehearsals per live segment ([15.4](../module-15/lesson-04.md)). The chance that all three live segments run cleanly is roughly the product of their rates. With 9/10, 10/10 and 10/10 the point estimate is $0.9 \times 1.0 \times 1.0 = 0.90$; with ten runs each, the Wilson lower bounds are 0.60, 0.72 and 0.72, and their product is about 0.31. Ten rehearsals cannot tell "almost always" from "most of the time", which is why every live segment keeps its fallback cued, however well the rehearsals went.

### The rubric and two observers

Eight criteria, each scored 1–4 (1 not seen, 2 attempted and did not work, 3 meets the standard, 4 a model for others), with the "3" written down so that observers score behaviour, not impressions: **timing**, **objectives**, **narration**, **wait-time**, **recovery**, **exercise launch**, **questions** and **claims**. The anchors are in the [workshop template](../../templates/workshop-template.md#observation-rubric). The criteria are the lessons of Modules 15–17 in observable form: a live segment narrates decisions and never goes silent for more than 45 seconds; every number has a source and an interval; a question is repeated, triaged and answered in about a minute.

Self-scores are not observations. You cannot see your own silent gaps or your own glances at the notes, and in the break every criterion scored 4.

Two observers score **independently**, then compare. Read two numbers: **exact agreement** (same score) and **agreement within one point**. In the reference dress rehearsal, the observers agreed exactly on 5 of 8 criteria (63%) and within one point on 8 of 8. Treat these as a calibration check, not a statistic: eight ratings on a four-point scale will agree by chance fairly often, and a formal coefficient on eight items would not be stable. What matters is a **difference of two points or more** on any criterion. It means the observers are reading the anchor differently; watch that stretch of the recording together, agree what a 2 and a 4 look like, and rewrite the anchor if needed before the next run.

### The exit standard

Write it before rehearsal 1, so that you cannot lower it after rehearsal 3. The course's standard, from the outline: the full two hours from your agenda, notes checked at most twice. `KitCheck rehearsal` checks it on the last log together with what makes it meaningful:

- at least three rehearsals, the last a dress rehearsal before at least three people who did not build the kit;
- two observers other than you, all eight criteria scored, none below 3 from either;
- notes checked at most twice, counted from the recording;
- total within 5 minutes of the plan;
- at least one failure recovered in front of people, with the time it took.

If rehearsal 3 misses the standard on one criterion, fix it and run that block again with an observer; if it misses on several, run a fourth dress rehearsal. A paid room is not the place to find out.

## Show me

`KitCheck rehearsal` on the three reference logs (condensed):

```text
r1.md: kind solo, audience 0, notes checks 11
  timing: planned 120 min, actual 126 min, drift +6 min
  incidents: 3, recovered with a time: 1
r2.md: kind friendly, audience 3, notes checks 5
  timing: planned 120 min, actual 121 min, drift +1 min
  dana: mean 3.0 of 4 over 8 criteria, lowest 2
r3.md: kind dress, audience 6, notes checks 2
  timing: planned 120 min, actual 118 min, drift -2 min
  agreement dana/omer: exact 5 of 8 (63%), within one point 8 of 8 (100%)

Exit standard (on r3.md)
  PASS  three rehearsals or more (3)
  PASS  the last one is a dress rehearsal (dress)
  PASS  in front of at least 3 people who did not build it (6)
  PASS  two observers other than you (2)
  PASS  notes checked at most twice (2)
  PASS  total within 5 minutes of plan (-2)
  PASS  all eight rubric criteria scored
  PASS  no criterion below 3 from either observer
  PASS  a failure recovered in front of people, with the time it took

READY for a paid room
```

Notes checks fell from 11 to 5 to 2 across the three runs. That is the cue-card notes (17.1) doing their job once you know the arc: the page is for the two moments you cannot afford to get wrong, the fallback switch and the stop time.

## Try it

Budget: about 10 hours over two to three weeks.

1. **Before rehearsal 1 (30 min).** Write your exit standard and your observers' rubric anchors in your kit. Recruit two observers (a colleague who teaches, a lead who knows the audience) and a dress-rehearsal audience now; they are the hardest part to schedule.
2. **Rehearsal 1 (3 h including watch-back).** Full run alone, recorded, every fallback switched once. Log timing, incidents and notes checks from the recording. Change the kit.
3. **Rehearsal 2 (3 h).** Three to five people, setup check two days before, `DemoCheck drill` told only to the observer. Log, score, change.
4. **Rehearsal 3 (3 h).** The full workshop with outsiders, two observers, forms A/B. Log, score independently, compare, calibrate.
5. **Check (30 min).**

```bash
cd labs/module-17
dotnet run --project tools/KitCheck -- rehearsal ~/workshop-kit/rehearsals/r1.md ~/workshop-kit/rehearsals/r2.md ~/workshop-kit/rehearsals/r3.md
dotnet run --project ../module-16/tools/LearnCheck -- gain ~/workshop-kit/rehearsals/r3-responses.csv --plan ~/workshop-kit/agenda.md
```

<details>
<summary>Hint: I cannot find outsiders for a 2-hour dress rehearsal</summary>

Offer it as what it is: a free workshop for a meetup, a study group, a team in another department or a local university's software engineering course, clearly labelled as a rehearsal, with the observers introduced. Engineers give two hours for a hands-on session on something they use every day more readily than for a talk. The career path's advice is the same: deliver it to a meetup or an internal guild as the dress rehearsal for a paid client.
</details>

## Break it

```bash
cd labs/module-17
dotnet run --project tools/KitCheck -- rehearsal break/17.4-rehearsals/r1.md break/17.4-rehearsals/r2.md break/17.4-rehearsals/r3.md
```

Before you run it, compute $\Delta_{\text{controlled}}$ and $\Delta_{\text{estimated}}$ for `r3.md` by hand, and write down when the post-test would start on the day.

## Fix it

**Diagnose.** NOT READY, 6 failed gates. Three solo runs: no audience, no observers, self-scores excluded, notes checked 9 times on the last run, no failure recovered in front of anyone. Total drift +4 passes the timing gate, and that is the trap: $\Delta_{\text{controlled}} = +17$ and $\Delta_{\text{estimated}} = -13$, because the hands-on steps were assumed rather than done. The three rehearsals measured the same thing three times: the student talking to an empty room.

**Modify.** Keep one solo technical run and use it for what it measures: cut the credibility and problem parts back to their minutes, write prediction questions for the live waits, switch every fallback once. Replace the other two with a friendly run (real laptops, a drilled failure, one observer) and a dress rehearsal (outsiders, two observers, forms). That is the reference sequence.

**Rerun.** `rehearsal solution/workshop-kit/rehearsals/r1.md r2.md r3.md`: READY; drift −2 minutes; notes checked twice; observers within one point on all eight criteria.

## How do I know it works?

- [ ] Your exit standard was written before rehearsal 1 and did not change.
- [ ] Each rehearsal log shows what it measured and at least one change to the kit.
- [ ] For your dress rehearsal you have $\Delta_{\text{controlled}}$ and $\Delta_{\text{estimated}}$, both within a few minutes.
- [ ] Two observers scored independently; every two-point difference was discussed and the anchor clarified.
- [ ] `KitCheck rehearsal` says READY, and `LearnCheck gain` ran on the dress rehearsal's forms.

## Use / don't use

**Use** the three-rehearsal protocol before the first delivery of any new workshop, and a dress rehearsal again after any change to the method, the demo repository or the agent version that alters a live segment.

**Don't** count solo runs as rehearsals of the room. **Don't** use a friendly run as the dress rehearsal: colleagues who helped you build the kit will not ask a stranger's questions. **Don't** lower the standard after rehearsal 3 because the date is close; move the date or cut a block.

**Limitations.**

- Deliberate-practice research comes from music, sport and chess, and its effect is smaller than first claimed. The case for the protocol rests as much on what it measures as on how practice builds skill.
- Two observers with eight criteria give a rough signal. Agreement within one point can hide a shared bias, such as observers who like you; choose at least one who does not.
- A dress rehearsal with six friendly outsiders is still not a paid room with a skeptical manager. The question bank (17.3) covers part of that gap; your first deliveries cover the rest, and they get logged like rehearsals.

## Reflect

1. What did your solo run measure well, and what did it tell you nothing about?
2. On which criterion did your observers disagree most, and what did you learn when you watched that stretch together?
3. What is your notes-check count across the three runs, and which two moments are still on the page?

## Sources

- [Ericsson, Krampe and Tesch-Römer (1993) — The role of deliberate practice in the acquisition of expert performance](https://doi.org/10.1037/0033-295X.100.3.363) — practice designed to improve performance, with feedback, as the core of expertise.
- [Macnamara and Maitra (2019) — revisiting Ericsson, Krampe & Tesch-Römer (1993)](https://royalsocietypublishing.org/doi/10.1098/rsos.190327) — replication with double-blind procedures: accumulated practice explained about 26% of variance among skill groups, not most of it.
- [Brown, Cai and DasGupta (2001) — Interval Estimation for a Binomial Proportion](https://projecteuclid.org/journals/statistical-science/volume-16/issue-2/Interval-Estimation-for-a-Binomial-Proportion/10.1214/ss/1009213286.full) — the Wilson interval used for the live-segment rates.
