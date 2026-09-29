# Content piece template (`talks/pieces/P-NN-<slug>.md`)

Introduced in [18.2](../lessons/module-18/lesson-02.md) (evidence-based claims) and [18.3](../lessons/module-18/lesson-03.md) (formats). Keep one file per piece, including the script of a video or the speaker notes of a talk, so that `PostCheck claims` and `PostCheck format` (in `labs/module-18/tools/PostCheck`) can read it. The header ends at the first line that is exactly `---`.

## Header

```markdown
# P-04 · <title>

- Format: post | article | video | talk | lunch-and-learn | demo | case-study
- Concept: C3                      (one; a talk may carry up to three)
- Stage: discover | trust | act
- Audience: <role and situation, not "everyone">
- Evidence: EXP-01, INC-08, <link>
- Disclosure: none | <employer, vendor, free access, payment>
- Target: 5 minutes               (video, talk: the slot; optional for text)
- Q&A: 10 minutes                 (talk, lunch-and-learn)
- Repo: brownfield-demo @ ai-layer-v1   (video, demo: public and pinned)
- Captions: reviewed | auto | none      (video)
- Hook: <the problem the first 15 seconds or first line states>
- Fallback: <recording or pre-baked branch>   (live demo)
- Consent: yes, <who>, <yyyy-mm-dd>           (case-study)
- Quotes: <who approved which quote, date>    (case-study with quotes)
- Published: yyyy-mm-dd

---

<body>
```

## Claims rules (checked by `PostCheck claims`)

1. Every percentage, multiplier or money figure has a source in the same sentence that a reader can follow: an evidence ID from your notes (`EXP-01`, `INC-08`, `FAQ-04`) or a link.
2. An effect from your own experiment carries its interval: "fell by 16% (95% CI 5% to 25%)".
3. "Studies show", "research says", "data proves": only with the study linked in the same sentence.
4. No hype words: guaranteed, 10x, game-changing, revolutionary, zero risk, effortless, the end of programming.
5. No generalizing past the people you measured ("every team", "teams like yours will see").
6. A limits line: what this does not show, where it does not apply, what you have not tested.
7. A disclosure line, even if it says "none".
8. No employer or client names: run with your private deny list.

Place each claim on the claims ladder from [13.6](../lessons/module-13/lesson-06.md): public pieces stay at rung 3 or below.

## Format rules (checked by `PostCheck format`)

| Format | Rules |
|---|---|
| post | ≤ 300 words (error above 600); one concept; the first line states the reader's problem, not your news; end with a question |
| article | 400–2,500 words; two or more `##` headings; one or two concepts |
| video | speaking time at 150 words per minute within the target; target 6 minutes or less for a teaching video; one concept; reviewed captions; public repository pinned to a tag; problem first, introductions later; lines starting with `[` are screen directions and are not counted |
| talk, lunch-and-learn | speaking time at 130 words per minute fits the slot minus Q&A; at most three concepts; a feedback line naming the one question you will read first |
| demo | public pinned repository; a fallback (15.4) |
| case-study | `## Before`, `## Intervention`, `## After`, `## What this does not show`; written consent with a date; approved quotes |
