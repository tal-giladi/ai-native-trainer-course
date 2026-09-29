# Workshop template (`workshop-kit/`)

Everything you need to deliver a 2-hour hands-on workshop on a bad day: the agenda, notes you can glance at, a starter pack learners set up before the day, exercises with finish lines, a troubleshooting guide, fallback recordings, forms, a hard-questions bank and rehearsal logs. Introduced in Module 17: [17.1](../lessons/module-17/lesson-01.md) (agenda and notes), [17.2](../lessons/module-17/lesson-02.md) (materials), [17.3](../lessons/module-17/lesson-03.md) (question bank), [17.4](../lessons/module-17/lesson-04.md) (rehearsals). It extends the [lesson template](lesson-template.md) (Module 16) from one 30-minute session to a full workshop, and reuses the [demo run sheet](demo-runbook.md) (Module 15) for the live segments and the [pre/post assessment template](pre-post-assessment.md) for the forms.

The tables are what `KitCheck` (in `labs/module-17/tools/KitCheck`) and `LearnCheck align` (in `labs/module-16/tools/LearnCheck`) read, so keep the column names. A worked example: [`labs/module-17/solution/workshop-kit/`](../labs/module-17/solution/workshop-kit/agenda.md).

## Layout

```text
workshop-kit/
├── agenda.md              # claim, objectives, items, the timed agenda (KitCheck agenda, LearnCheck align)
├── instructor-notes.md    # one "## h:mm · step" section per agenda row, ≤ 150 words each
├── starter-pack/
│   ├── README.md          # before the day, offline path, no-agent path, catch-up tags, reset
│   ├── check-setup.ps1    # prints SETUP OK or the troubleshooting IDs
│   └── check-setup.sh
├── exercises/             # one file per hands-on step
├── troubleshooting.md     # | ID | Symptom | Cause | Fix | Seconds | Tested |
├── fallback/README.md     # | Segment | File | Start at | Duration | Source take | Checked |
├── forms.md               # forms A/B + key, feedback form, follow-up message
├── question-bank.md       # hard questions, one ### block each
└── rehearsals/            # r1.md, r2.md, r3.md …
```

`KitCheck kit workshop-kit/` checks that all of it exists and agrees with the agenda.

## Agenda (`agenda.md`)

Write it backwards, as in Module 16: learning claim → objectives → items → agenda. The objectives and assessment tables are exactly those of the lesson template.

```markdown
# Workshop agenda — <title> (2 hours)

- Workshop: <title, one line>
- Method: <name and version; the concepts it teaches>
- Audience: <who, how many, experience range, which agent they use>
- Room: <in person / remote; laptops; co-host>
- Slot: 120
- Budget: 120
- Demo: <demo repo, demo ticket, hands-on ticket>
- Date: YYYY-MM-DD

## Learning claim
## Prior knowledge
## Objectives        (| ID | Objective | Level |)
## Assessment        (| Item | Objective | Level | What it asks |)
## Agenda            (below)
## Materials
## Room plan
```

| Start | Step | Minutes | Mode | Delivery | Arc | Objective | New terms | Fallback |
|---|---|---|---|---|---|---|---|---|
| 0:00 | Pre-assessment | 6 | assess | paper | assess | - | 0 | - |
| 0:06 | Credibility: one incident from your notes | 4 | show | talk | credibility | O1 | 1 | - |
| … | Live 1: ⟨step⟩; prediction question and Q&A while it runs | 7 | show | live | demo | O2 | 1 | rec:fallback/02-x.mp4@00:40 |
| … | Hands-on 1: ⟨the same step on the hands-on ticket⟩ | 12 | do | hands-on | exercise | O2 | 0 | tag:ws-1-x |

- **Start** is `h:mm` from the start of the workshop; each row starts where the previous one ended.
- **Mode** (for `LearnCheck`): assess · show · do · reflect · break · buffer.
- **Delivery**: talk · live · recorded · hands-on · paper · `-`.
- **Arc**: credibility · problem · demo · exercise · qa · close, in that order of first appearance, plus assess · reflect · break · buffer.
- **Fallback**: every `live` row has `rec:⟨file⟩@mm:ss`, `branch:⟨name⟩` or `tag:⟨name⟩`; every `hands-on` row has a catch-up `tag:⟨name⟩`.

Rules `KitCheck agenda` enforces: timestamps add up and end inside the slot; the whole arc is present; hands-on is at least a quarter of the slot and the demo at most 40%; no single live segment over 10 minutes; each live segment has a fallback and a wait-time plan; a buffer of at least 5% of the slot; a break in anything of 90 minutes or more; a close of at most 10% of the slot. `LearnCheck align` adds the Module 16 rules: observable objectives, items at the objective's level, every objective practised, learners active at least 40% of teaching time, no passive run over 10 minutes.

## Instructor notes (`instructor-notes.md`)

One section per agenda row, headed `## ⟨start⟩ · ⟨step⟩`. Cues, not a script:

```markdown
## 0:29 · Live 1, Ground

- Do: run-sheet segment 3. Type `/prime BILL-97`, slowly.
- Say while it runs: "<prediction question>" Then take one parked question.
- If it breaks: name it, then `02-ground.mp4` from 00:40. Say line from the run sheet.
```

Every `live` and `hands-on` section says what to do **if it breaks** or **if the room falls behind**.

## Exercise spec (`exercises/*.md`)

```markdown
# Hands-on N — <title> (<minutes> minutes)

- Start: <tag or previous state; ticket>
- Task: <with the objective's verb>
- Done when: <observable, the same for every pair>
- Time: <minutes; warning at minus one>
- Hints:
  1. <a nudge>
  2. <a pointer>
  3. <the near-answer>
- Extension: <for the fastest pairs>
- Catch-up: <git checkout ws-N-x, and what to read>
- Paper path: <the same judgment, from saved agent output>
```

## Troubleshooting (`troubleshooting.md`)

| ID | Symptom | Cause | Fix | Seconds | Tested |
|---|---|---|---|---|---|
| T-01 | ⟨what the learner sees, in their words⟩ | ⟨why⟩ | ⟨the exact command or move⟩ | ⟨how long the fix takes⟩ | ⟨date you last ran it on a clean machine⟩ |

Every entry comes from a rehearsal, a setup-check reply or a delivery. A fix longer than about three minutes is not done in the room: the learner moves to the catch-up tag or the paper path, and you fix it at the break.

## Question bank

```markdown
### Q01 · "<the question, in the asker's words>"
- Category: replace | security | confidentiality | roi | skeptic | tools | cost | limits
- Answer: <concede what is true; your evidence; the boundary; about 60 seconds spoken>
- Evidence: <INC-/EXP-/C-/FAQ- ids, a lesson, or a link; "opinion" if it is one>
- Not known: <the honest limit>
- Bridge: <one sentence back to the agenda>
```

`KitCheck questions` requires the five categories replace, security, confidentiality, roi and skeptic; evidence on every answer; a source for every number; no hype words (guaranteed, 10x, zero risk, completely safe); and warns on absolutes, answers over about 130 words, and missing limits or bridges.

## Rehearsal log

```markdown
# Rehearsal N — <solo | friendly | dress>

- Date: YYYY-MM-DD
- Kind: solo | friendly | dress
- Audience: <number> (<who; did they build it?>)
- Observers: <names, not you>
- Notes checks: <count, from the recording>
- Recording: yes | no

## Timing
| Start | Step | Planned | Actual | Note |

## Incidents
| Time | What happened | Recovered in | Change |

## Observer scores (1–4; 3 = meets the standard)
| Criterion | <observer 1> | <observer 2> |

## Changes made after this run
```

### Observation rubric

Each observer scores alone, then you compare. 1 = not seen · 2 = attempted, did not work · 3 = meets the standard · 4 = a model for others.

| Criterion | What a 3 looks like |
|---|---|
| Timing | Every block within 2 minutes of plan; cuts made from the show steps, never from hands-on or the post-test |
| Objectives | Learners hear what they will be able to do, in the objective's words, before each block |
| Narration | Live steps narrate decisions, not keystrokes; no silence over 45 seconds |
| Wait-time | Every agent wait has a prediction question or a parked question answered |
| Recovery | A failure is named, a decision made by the clock, and the lesson in it said out loud, within the run sheet's recover budget |
| Exercise launch | Task, done-criteria and time box said and on screen; most learners start within a minute |
| Questions | Repeated, triaged (answer, redirect, park), answered with evidence and a boundary, in about a minute |
| Claims | Every number has a source and an interval; nothing is promised about the room's own team |

### Exit standard (`KitCheck rehearsal r1.md r2.md r3.md`)

Three rehearsals or more; the last a dress rehearsal before at least three people who did not build it; two observers other than you; notes checked at most twice; total within 5 minutes of plan; all eight criteria scored and none below 3 from either observer; at least one failure recovered in front of people, with the time it took.
