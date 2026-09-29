# Module 17 labs — Workshop design and delivery

Everything the Module 17 labs need. Lessons: [17.1](../../lessons/module-17/lesson-01.md) · [17.2](../../lessons/module-17/lesson-02.md) · [17.3](../../lessons/module-17/lesson-03.md) · [17.4](../../lessons/module-17/lesson-04.md).

The labs turn your method ([Module 14](../module-14/README.md)), your demo repository ([Module 15](../module-15/README.md)) and your instructional design ([Module 16](../module-16/README.md)) into **`workshop-kit`**: a 2-hour hands-on workshop you can deliver on a bad day, with the agenda, instructor notes, starter pack, exercises, troubleshooting guide, fallback recordings, forms, hard-questions bank and three rehearsal logs scored by observers.

> [!WARNING]
> The starter pack is meant to be **public**, so it must be built from `brownfield-demo` after `DemoCheck credibility` passes with your private deny list ([15.1](../../lessons/module-15/lesson-01.md)), never from employer or client code. Learners will run an agent on their own laptops: tell the host in writing that the repository is fictional, which agent and account learners should use, and that nothing from their company's code is needed. Rehearsal recordings and observer scores are about you; forms and scores from learners follow the anonymity rules of the [Module 16 lab](../module-16/README.md).

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400.
- No NuGet packages. `KitCheck` is read-only and deterministic; it works offline.
- `LearnCheck` from [`labs/module-16/tools/LearnCheck`](../module-16/README.md) for the pedagogy checks on the same agenda.
- For the live parts: your `brownfield-demo` with its run sheet ([Module 15](../module-15/README.md)), a screen recorder, and, for rehearsals 2 and 3, people and two observers.

## Contents

| Path | What it is |
|---|---|
| `tools/KitCheck/` | Dependency-free C# checker: `agenda` (timestamps, arc, live segments and fallbacks, hands-on and catch-up tags, buffer, break, close), `kit` (every file the agenda needs exists and agrees with it), `questions` (hard-questions bank), `rehearsal` (drift, notes checks, incidents, observer agreement, exit standard). |
| `solution/workshop-kit/` | **Illustrative** reference kit for a 2-hour workshop, *Ground before you generate*, built on the Module 14 reference method and `brownfield-demo` (BILL-97 demo, BILL-180 hands-on). Compare structure; your method, repository, audience and numbers are your own. |
| `break/` | One deliberately flawed piece per lesson: a demo-marathon agenda (17.1), a kit put together the night before (17.2), a hype-filled question bank (17.3), three solo "rehearsals" scored by the presenter (17.4). |

## Quick start

From this folder:

```bash
K="dotnet run --project tools/KitCheck --"
L="dotnet run --project ../module-16/tools/LearnCheck --"

# 17.1 — the agenda
$K agenda break/17.1-demo-marathon/agenda.md       # 8 errors, 10 warnings
$L align  break/17.1-demo-marathon/agenda.md       # 11 errors, 5 warnings
$K agenda solution/workshop-kit/agenda.md          # clean
$L align  solution/workshop-kit/agenda.md          # clean, 46% active

# 17.2 — the materials
$K kit break/17.2-thin-kit                         # 22 errors, 14 warnings
$K kit solution/workshop-kit                       # clean

# 17.3 — the question bank
$K questions break/17.3-hype-bank/question-bank.md        # 5 errors, 14 warnings
$K questions solution/workshop-kit/question-bank.md       # clean, 13 questions

# 17.4 — rehearsals, in the order they were run
$K rehearsal break/17.4-rehearsals/r1.md break/17.4-rehearsals/r2.md break/17.4-rehearsals/r3.md          # NOT READY, 6 errors
$K rehearsal solution/workshop-kit/rehearsals/r1.md solution/workshop-kit/rehearsals/r2.md solution/workshop-kit/rehearsals/r3.md   # READY
```

(PowerShell: type the full `dotnet run --project tools/KitCheck -- <command>` instead of `$K`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem.

## Your own `workshop-kit/`

Use the [workshop template](../../templates/workshop-template.md). Build it in lesson order:

1. **17.1** — `agenda.md` from your Module 16 `session.md` scaled to two hours and your Module 15 run sheet cut into live segments; `instructor-notes.md`. Both checks clean.
2. **17.2** — `starter-pack/` built from `brownfield-demo` with a `workshop` branch and catch-up tags (each tag must build and pass its tests); the setup check run on a machine that has never seen the repository; `exercises/`, `troubleshooting.md`, `fallback/`, `forms.md`. `kit` clean except the question bank and rehearsals.
3. **17.3** — `question-bank.md`, at least 12 questions from your objections FAQ ([14.3](../../lessons/module-14/lesson-03.md)), discovery calls and rehearsals, each said aloud with a timer.
4. **17.4** — three rehearsals: solo technical run, friendly run with a drilled failure, dress rehearsal with outsiders and two observers. `rehearsal` says READY.

## Expected results

| Check | Break | Reference |
|---|---|---|
| `agenda` (17.1) | 8 errors: timestamp off by 6 min, clock ends at 2:01, hands-on 4%, demo 54%, no buffer, two live segments without fallback, hands-on without catch-up tag | 0 |
| `align` (17.1) | 11 errors: vague objectives, no items, 4% active, 95 passive minutes | 0, 46% active |
| `kit` (17.2) | 22 errors: 15 steps without notes, no setup check, 2 tags not in the starter pack, missing exercise and fields, unlisted fallback, no question bank | 0 |
| `questions` (17.3) | 5 errors: "completely safe", "10x", two answers without evidence, no confidentiality question | 0 |
| `rehearsal` (17.4) | NOT READY: solo, no audience, no observers, notes checked 9 times, self-scores, no recovered failure | READY: dress, 6 people, 2 observers (exact agreement 5 of 8, within one 8 of 8), notes checked twice, −2 min |
