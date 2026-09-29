# Module 18 labs — Teaching in public

Everything the Module 18 labs need. Lessons: [18.1](../../lessons/module-18/lesson-01.md) · [18.2](../../lessons/module-18/lesson-02.md) · [18.3](../../lessons/module-18/lesson-03.md) · [18.4](../../lessons/module-18/lesson-04.md).

The labs turn your method ([Module 14](../module-14/README.md)), your measured evidence ([Module 13](../module-13/README.md)) and your public demo ([Module 15](../module-15/README.md)) into a steady stream of small public pieces, one concept at a time, and measure the stream by the questions it brings back rather than by its reach: `talks/` with a content calendar, at least four published pieces with links, a lunch-and-learn deck with feedback, a free pilot with feedback, and a questions log.

> [!WARNING]
> Publishing is permanent: screenshots and archives outlive a deleted post. Before anything goes out, run `PostCheck claims` with a deny list of employer and client names that lives **outside** the repository, and read every finding. Never show employer or client code, tickets or dashboards; film only `brownfield-demo`. Write nothing about a pilot, a client or a colleague without their written consent, anonymize askers in the questions log, and follow your employer's rules on public statements and side activities (lesson 14.4). Keep `talks/` private; only the pieces themselves are public.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400.
- No NuGet packages. `PostCheck` is read-only and deterministic; it works offline.
- The Module 14 evidence and concept files (`../module-14/evidence/`, `../module-14/solution/concepts.md`, `../module-14/solution/faq.md`) and the Module 15 example deny list (`../module-15/samples/deny-terms.example.txt`). Nothing is copied into this folder; the commands below point at them.

## Contents

| Path | What it is |
|---|---|
| `tools/PostCheck/` | Dependency-free C# checker: `calendar` (a calendar as a funnel: concept and evidence per piece, share of asks, cadence, the outside-question signal and how many pieces it takes to hear from a stranger), `claims` (sources, intervals, hype, "studies show", generalization, limits, disclosure, deny list, readability), `format` (per-format rules for posts, articles, videos, talks, demos and case studies), `questions` (themes, private-only answers, question-to-piece time, anonymized askers, exit signal). |
| `solution/talks/` | **Illustrative** reference: `content-calendar.md`, `questions.csv`, four published pieces in `pieces/` (a post, an article, a video script, a pilot case study), `lunch-and-learn/` (slides and feedback) and `pilot/` (plan and results). One student teaching Ground-Bound-Build-Prove; reach and replies are made-up numbers of a plausible size. |
| `break/` | One deliberately flawed artifact per lesson: a promotion plan posing as a calendar (18.1), a hype post (18.2), a three-concept video script filmed on employer code (18.3), a questions log answered only in private and a pilot post without consent (18.4). |

## Quick start

From this folder:

```bash
P="dotnet run --project tools/PostCheck --"
EV="--evidence ../module-14/evidence/experiments.md ../module-14/evidence/NOTES-contoso.md ../module-14/solution/concepts.md ../module-14/solution/faq.md"
DENY="--deny ../module-15/samples/deny-terms.example.txt"

# 18.1 — content as a funnel
$P calendar break/18.1-promo-calendar/content-calendar.md --concepts ../module-14/solution/concepts.md   # 9 errors, 3 warnings
$P calendar solution/talks/content-calendar.md --concepts ../module-14/solution/concepts.md              # clean, 4 of 7 pieces with an outside question

# 18.2 — evidence-based writing
$P claims break/18.2-hype-post/draft-post.md $EV $DENY             # 11 errors, 4 warnings
$P claims solution/talks/pieces/P-04-late-pr-article.md $EV $DENY  # clean, 3 effects, 3 sourced, 3 with an interval

# 18.3 — formats
$P format break/18.3-long-video/script.md                          # 4 errors, 3 warnings
$P format solution/talks/pieces/P-05-shadow-rule-video.md          # clean

# 18.4 — lunch-and-learn, pilot, questions
$P questions break/18.4-private-answers/questions.csv              # 5 errors, exit signal not yet
$P format break/18.4-private-answers/pilot-post.md                 # 2 errors
$P questions solution/talks/questions.csv --pieces solution/talks/content-calendar.md   # 1 warning, exit signal seen 2026-08-12
```

(PowerShell: type the full `dotnet run --project tools/PostCheck -- <command>` and the file lists instead of `$P`, `$EV` and `$DENY`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem. Run `claims` and `format` on every piece before it goes out; they take a second.

## Your own `talks/`

```text
talks/
├── content-calendar.md     # 18.1 — templates/content-calendar.md
├── questions.csv           # 18.4 — same template, questions log
├── pieces/
│   └── P-NN-<slug>.md      # 18.2, 18.3 — templates/content-piece.md, one per piece, incl. scripts
├── lunch-and-learn/
│   ├── slides.md           # 18.4 — speaker notes with the piece header
│   └── feedback.md
├── pilot/
│   ├── plan.md             # 18.4 — run like a paid engagement
│   └── feedback.md
└── mini-workshop/          # from Module 16
```

Formats: [content calendar and questions log](../../templates/content-calendar.md), [content piece](../../templates/content-piece.md).

## Field work (ongoing, start now)

1. **Calendar (18.1).** Audience sentence, two or three places to publish, cadence you can keep for six months, the first six pieces planned from your supported concepts. `calendar` clean.
2. **Every piece (18.2, 18.3).** Write it into `pieces/` with the header; `claims` with your private deny list and `format`, both clean; publish; add the link and the date.
3. **Lunch-and-learn (18.4).** 20–30 minutes for your own team or a neighbouring one, on `brownfield-demo`, with a three-question form and a questions log at the door.
4. **Free pilot (18.4).** One 60-minute workshop for a small company or open-source project you have a real connection to: prep call, plan, form, two-week follow-up, written consent before any write-up.
5. **Questions log (18.4).** Every question, the day it is asked. Once a theme is asked three times, the next piece answers it.
6. **Review monthly.** Rerun `calendar` and `questions`; fill in reach, replies and outside questions; move one theme from private to public.

Done means: at least four published pieces with links, each passing `claims` and `format`; a delivered lunch-and-learn with feedback; a pilot with a plan, results and consent; a questions log; and, eventually, the exit signal.

**Exit test (outline).** Someone outside your network asks a follow-up question unprompted: the first `outside,no` row in `questions.csv`, confirmed by `PostCheck questions`. It can take months. Keep the cadence.

## Expected results

| Check | Break | Reference |
|---|---|---|
| `calendar` (18.1) | 9 errors, 3 warnings: 2 pieces without a concept, 3 without evidence, 1 without a link, 50% asks, 42-day silence, reach without the outside-question column | clean; 7 pieces out, 0 asks, mean gap 12.8 days; 4 of 7 pieces with an outside question (95% CI 25% to 84%) |
| `claims` (18.2) | 11 errors, 4 warnings: 3 unsourced numbers, an experiment number without its interval, "studies show", 3 hype terms, an unknown ID, no limits line, an employer name | P-04: 3 effect claims, all sourced, all with an interval |
| `format` (18.3) | 4 errors, 3 warnings: 4.2 min of speech for a 3-minute target, 3 concepts, no captions, employer repository | P-05: 1.9 min of speech plus screen time, target 5, reviewed captions, pinned repository |
| `questions` (18.4) | 5 errors, 2 warnings: 3 identifiable askers, 2 themes asked 3+ times and answered only in private, 0 outside, exit signal not yet | 1 warning (a theme answered in a drafted piece), median 20 days from question to piece, exit signal seen |
| `format` pilot post (18.4) | 2 errors, 1 warning: no limits section, no consent, unapproved quote | P-07: clean |
