# Lesson template (session plan for one teaching session)

A plan for one session that teaches one to three objectives: a lunch-and-learn, a 30-minute mini-workshop, one block of a longer workshop. Introduced in [16.2](../lessons/module-16/lesson-02.md) (objectives, alignment, cognitive load) and [16.3](../lessons/module-16/lesson-03.md) (show, do, reflect). The three tables are what `LearnCheck align` in `labs/module-16/tools/LearnCheck` reads, so keep the column names. A worked example: [`labs/module-16/solution/mini-workshop/session.md`](../labs/module-16/solution/mini-workshop/session.md).

Fill it in **backwards**: learning claim → objectives → assessment → activities. The activities are the last thing you write, not the first.

Rules:

- **Objectives** start with an observable verb (decide, find, write, explain why, choose, predict). Never *understand*, *know*, *learn*, *appreciate*, *be aware of*: you cannot see them happen. Two or three per 30 minutes.
- **Level** is one of remember · understand · apply · analyze · evaluate · create (revised Bloom). Every objective has at least one item at its level, preferably two items.
- **Every objective is practised** in at least one `do` step before it is assessed.
- **Learners are active** (`do` + `reflect`) for at least 40% of the teaching time (assessment minutes excluded), and never watch for more than 10 minutes in a row.
- **New terms:** at most three per step. If a step needs more, it needs splitting or the terms need cutting.
- **Mode** is one of assess · show · do · reflect.

```markdown
# Session plan — <concept>

- Concept: <C-id · name, method version>
- Audience: <who, how many, experience range, what tools they use>
- Room: <in person / remote / hybrid; screens; laptops; repo>
- Budget: <minutes>
- Date: YYYY-MM-DD

## Learning claim

<One sentence: after this session, a <role> who <situation> can <observable performance>.>

## Prior knowledge

<What they already know and believe, from 2–3 short conversations beforehand. List each
misconception you will surface on purpose.>

## Objectives

| ID | Objective | Level |
|---|---|---|
| O1 | <Given <condition>, <observable verb> <what>, <criterion>.> | analyze |
| O2 | | apply |

## Assessment

<Parallel forms A and B, counterbalanced. Item rules: templates/pre-post-assessment.md.>

| Item | Objective | Level | What it asks |
|---|---|---|---|
| I1 | O1 | analyze | |
| I2 | O1 | analyze | <misconception check> |
| I3 | O2 | apply | |
| I4 | O2 | apply | |

## Activities

| Step | Minutes | Mode | Objective | New terms |
|---|---|---|---|---|
| Pre-assessment, no discussion | 4 | assess | - | 0 |
| Hook: one real incident | 2 | show | O1 | 1 |
| Worked example | 3 | show | O1, O2 | 1 |
| Pairs: faded examples (first half-solved) | 6 | do | O1 | 0 |
| Live: <the step that needs the tool>; prediction question during agent wait-time | 4 | show | O2 | 1 |
| Solo: own ticket, hint ladder on handout | 5 | do | O2 | 0 |
| Reflect: muddiest point; "when would you not bother?" | 2 | reflect | O1 | 0 |
| Post-assessment (other form) and feedback form | 4 | assess | - | 0 |

## Materials

- Handout: <exercises, hint ladder, rubric>
- Repo at tag <tag>; fallback recording of every live step
- Forms A/B and key; feedback form; follow-up message

## Room plan

- Mixed levels: <extension task for the fast; pairing rule>
- Skeptics: <the boundary and counter-evidence lines you will quote; the evidence you will show>
- Remote: <co-host or not; breakout links pasted before rooms open; chat watcher>

## After the session

- Scores → `responses.csv`, forms → `feedback.csv`, two-week follow-up → `followup.csv`.
- `LearnCheck gain`, `feedback`, `followup`; write `results.md`; change the plan; bump its date.
```

Check it:

```bash
cd labs/module-16
dotnet run --project tools/LearnCheck -- align path/to/session.md
```
