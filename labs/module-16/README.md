# Module 16 labs — Instructional design for engineers

Everything the Module 16 labs need. Lessons: [16.1](../../lessons/module-16/lesson-01.md) · [16.2](../../lessons/module-16/lesson-02.md) · [16.3](../../lessons/module-16/lesson-03.md) · [16.4](../../lessons/module-16/lesson-04.md) · [16.5](../../lessons/module-16/lesson-05.md).

The labs turn one concept from your `method-notes/concepts.md` ([Module 14](../module-14/README.md)) into a 30-minute mini-workshop that you design backwards, deliver to 3–8 real people, and measure: `talks/mini-workshop/` with the session plan, parallel pre/post forms, scores, feedback, a two-week follow-up and a results page.

> [!WARNING]
> You will collect scores and comments from colleagues. Use anonymous IDs (P1, P2, …) on every sheet and CSV, keep the key from IDs to names off the repository or destroy it, tell people before the pre-test that it is not an evaluation of them, and never share individual scores with their manager. If your company has a policy on surveys of employees, follow it. Keep `talks/` private.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`, so a newer runtime works too). Verified with SDK 10.0.400.
- No NuGet packages. `LearnCheck` is read-only and deterministic (the bootstrap uses a fixed seed); it works offline.

## Contents

| Path | What it is |
|---|---|
| `tools/LearnCheck/` | Dependency-free C# checker: `align` (objectives, items and activities in a session plan), `gain` (normalized gain with a bootstrap interval, per item and per objective), `feedback` (ratings next to learning), `followup` (behaviour with a Wilson interval). |
| `solution/mini-workshop/` | **Illustrative** reference: one student's `session.md`, `forms.md` (forms A and B with key), `responses.csv`, `feedback.csv`, `followup.csv`, `results.md`, teaching *Shadow rule* (C1 in [`module-14/solution/concepts.md`](../module-14/solution/concepts.md)). Compare structure; your concept, audience and numbers are your own. |
| `break/` | One deliberately flawed delivery per lesson: a loved demo that taught nothing (16.1), misaligned objectives and items (16.2), a show-heavy plan (16.3), a remote room log (16.4), a manager write-up with four measurement errors (16.5). |

## Quick start

From this folder:

```bash
M="dotnet run --project tools/LearnCheck --"

# 16.1 — enjoyed vs learned
$M gain     break/16.1-enjoyed/responses.csv --plan break/16.1-enjoyed/session.md   # <g> 0.11, low
$M feedback break/16.1-enjoyed/feedback.csv --responses break/16.1-enjoyed/responses.csv   # 1 error

# 16.2 — alignment
$M align break/16.2-misaligned/session.md          # 12 errors, 4 warnings
# 16.3 — show, do, reflect
$M align break/16.3-show-heavy/session.md          # 4 errors, 1 warning
$M align solution/mini-workshop/session.md         # clean

# 16.5 — measuring
$M gain break/16.5-measurement/responses.csv --plan solution/mini-workshop/session.md   # 1 error, 5 warnings
$M gain solution/mini-workshop/responses.csv --plan solution/mini-workshop/session.md   # <g> 0.67 (0.55 to 0.85)
$M feedback solution/mini-workshop/feedback.csv --responses solution/mini-workshop/responses.csv
$M followup solution/mini-workshop/followup.csv    # 3 of 6, 19% to 81%
```

(PowerShell: type the full `dotnet run --project tools/LearnCheck -- <command>` instead of `$M`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem.

## Your own `talks/mini-workshop/`

```text
talks/mini-workshop/
├── session.md        # 16.1 claim, 16.2 objectives + assessment, 16.3 activities, 16.4 room plan
├── forms.md          # 16.2 — forms A and B, key and rubrics
├── handout.md        # 16.3 — exercises, hint ladder, rubric
├── responses.csv     # 16.5 — scores, anonymous IDs
├── feedback.csv      # 16.5
├── followup.csv      # 16.5 — two weeks later
└── results.md        # 16.5 — templates/pre-post-assessment.md "Results"
```

Formats: [lesson template](../../templates/lesson-template.md) and [pre/post assessment template](../../templates/pre-post-assessment.md).

## Field assignment: the 30-minute mini-workshop

1. **Pick one concept** from your `concepts.md`, status *supported*, that your audience will meet on real work this month (16.1).
2. **Recruit 3–8 people** who did not help you build the concept: colleagues from another team, a meetup group, a study group. Not your manager alone, not only friends. Book 35 minutes.
3. **Three prior-knowledge chats** of five minutes, a week before (16.1). Write down misconceptions verbatim.
4. **Design backwards** (16.2, 16.3): claim → 2–3 objectives → forms A and B → activities. `align` clean. Rehearse once, alone, with a timer.
5. **Deliver:** pre-test (4 min, counterbalanced forms) → session → post-test and feedback form (4 min). Record the session if everyone agrees; the recording is for you.
6. **Score** within 24 hours, blind to phase for open items; enter `responses.csv` and `feedback.csv`.
7. **Follow up** at two weeks with the one-line message; enter `followup.csv`.
8. **Analyse and write** `results.md`: level 1, 2, 3 side by side; what you change; what the numbers do not show (16.5).

Done means: `align` clean, `gain` without errors, a `results.md` whose every number comes from the tool, and at least three concrete changes to `session.md` traced to an item, a comment or a room incident.

## Expected results

| Check | Break | Reference |
|---|---|---|
| `gain` + `feedback` (16.1) | `<g>` 0.11 (0.04 to 0.16), rating 4.86, confidence gap +46, 1 error | `<g>` 0.67 (0.55 to 0.85), rating 4.0, gap −8 |
| `align` (16.2) | 12 errors, 4 warnings | 0 |
| `align` (16.3) | 4 errors, 1 warning (14% active, 19 min passive) | 0 (59% active) |
| room log (16.4) | 8 incidents | — |
| `gain` (16.5) | same form for all 6; Q7 post only; 3 ceiling items; Q4 dropped from c | clean except the small-n warning and one ceiling item |
