# Content calendar and questions log template (`talks/content-calendar.md`, `talks/questions.csv`)

Introduced in [18.1](../lessons/module-18/lesson-01.md) (calendar as a funnel) and [18.4](../lessons/module-18/lesson-04.md) (questions log). The formats are what `PostCheck calendar` and `PostCheck questions` (in `labs/module-18/tools/PostCheck`) read, so keep the column names.

Rules:

- **One piece, one concept.** Every piece that is not an ask names the card from your `method-notes/concepts.md` it teaches (`C1`), and the evidence IDs it rests on (`INC-08`, `EXP-01`, `FAQ-04`, or a link).
- **Stages.** `discover` = a stranger meets one idea; `trust` = they can use it and come back; `act` = the piece asks for something (a seat, a call, a download). At most one piece in five is `act`.
- **Cadence.** Pick a gap you can keep for six months (14 days is a sensible start). A burst followed by silence is worse than a slower steady rhythm.
- **Status.** `idea`, `planned`, `drafted`, `published` (public, with a link), `delivered` (a talk in a room; link the deck).
- **Measure the signal.** Reach is attention. The column that matters is **Outside questions**: questions from people who did not know you before, asked without being prompted. Record reach if you like; never record it alone.

## Calendar

```markdown
# Content calendar

- Owner: <you>
- Cadence: 14
- Audience: <one sentence: role, situation, what they already use>
- Where: <the two or three places you publish, and why those>

| Date | Piece | Format | Stage | Concept | Evidence | From question | Status | Link | Reach | Replies | Outside questions | Conversations |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 2026-07-08 | P-01 · <title> | post | discover | C1 | INC-01, INC-08 | - | published | <url or relative path> | 2140 | 14 | 0 | 0 |
| 2026-07-15 | P-02 · <title> | lunch-and-learn | trust | C1, C2 | INC-08 | - | delivered | lunch-and-learn/slides.md | 11 | 5 | 0 | 1 |
| 2026-10-21 | P-09 · <title> | post | act | - | - | - | planned | - | | | | |
```

Formats `PostCheck format` knows: `post`, `article`, `video`, `talk`, `lunch-and-learn`, `demo`, `case-study`. Header lines and rules per format: [content piece template](content-piece.md).

## Questions log

One row per question, written down the day it was asked, in the asker's words.

```csv
date,channel,network,prompted,asker,question,theme,answered_in
2026-08-12,article,outside,no,R1,"Did reviewers push back on getting PRs later?",review-time,P-04 thread
2026-09-02,email,outside,no,R4,"Can the agent do Ground on a repo with no tests?",no-test-suite,email
```

| Column | Values |
|---|---|
| `channel` | `post`, `article`, `video`, `talk`, `lunch-and-learn`, `pilot`, `meetup`, `dm`, `email`, `case-study` |
| `network` | `inside` (knew you before) or `outside` |
| `prompted` | `yes` if you asked for questions ("any questions?"), `no` if they came on their own |
| `asker` | an anonymous id (`R1`) or a role (`tech lead`); never a name or an address |
| `theme` | a short slug you reuse; the theme, not the wording, is what repeats |
| `answered_in` | `P-04` (a piece), `FAQ-02` (your objections FAQ), a URL, or `dm` / `email` / `verbal` when answered only in private |

A theme asked three times and answered only in private is your next piece. The first `outside,no` row is the module's exit signal.
