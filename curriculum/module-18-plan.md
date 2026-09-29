# Module 18 plan — Teaching in Public

4 lessons · ~60 min instruction · practice ongoing (a piece every 1–2 weeks, one lunch-and-learn, one free pilot) · depends on Module 14 (`concepts.md`, evidence IDs, objections FAQ, attribution and employer IP), Module 15 (`brownfield-demo`, deny list, pinned tags, fallbacks), Module 13 (claims ladder, "what this does not show"), Module 16 (mini-workshop, Kirkpatrick levels, normalized gain, Wilson follow-up) and Module 17 (hard-questions bank; linked, written in parallel).

Thesis: public teaching is the top of the funnel, not the product, and credibility is built by many small, checkable pieces over months. Each piece teaches one concept from `concepts.md` with its evidence and its limits; the stream is measured by the signal (questions from people outside your network, asked unprompted) rather than by reach; and every question is logged, because repeated questions are the next pieces and the objections a paying room will raise. Honesty is not only an ethic here: in advertising, objective claims need a reasonable basis and "studies show" needs the studies (FTC), and hype in a source propagates downstream (Sumner et al.).

Running example: the same illustrative student and method as Modules 14–17 (Ground-Bound-Build-Prove; C1 Shadow rule, C2 Self-graded green, C3 Late-PR illusion; EXP-01; FAQ-04), publishing from 2026-07-08.

Shared lab material: `labs/module-18/` — `tools/PostCheck` (dependency-free C#: `calendar`, `claims`, `format`, `questions`), the reference `solution/talks/` (calendar, questions log, four pieces, lunch-and-learn, pilot), four breaks. Evidence and deny list are read from Modules 14 and 15, not copied.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 18.1 Content as a funnel | Funnel stages (discover, trust, act) and what each piece is for; audience research from discovery notes and the questions people ask; credibility as a track record (Hovland–Weiss, participation inequality 90-9-1); signal vs vanity metrics; $1-(1-p)^n$ for pieces until a stranger asks | 14.2, 15.1, 01.4 | Audience sentence, channels, cadence, six planned pieces; `calendar` | Promotion plan: 50% asks, no concepts, burst then 42-day silence, reach and likes only | `talks/content-calendar.md` |
| 18.2 Evidence-based technical writing | Claims ladder applied to posts; source and interval in the same sentence; FTC substantiation and "studies show"; hype propagation (Sumner 2014); limits and disclosure lines; plain language; rewrite a hype draft | 18.1, 13.6, 14.4 | Write the first piece; `claims` with private deny list | Hype post: 42% unsourced, 49% without interval, "studies show 55%", zero risk, 10x, $13.5M, employer name | first published piece |
| 18.3 Content formats | Post, article, video, talk, live demo, case-study piece: what each is for and costs; one concept per short piece; video length (Guo et al.), captions (W3C/WCAG), problem-first openings; demo pinned to a public tag with a fallback; repurposing one concept across formats | 18.2, 15.3, 15.4 | Turn one concept into a post and a short video script; `format` + `claims` | Three-concept "short" video: 4.2 min for a 3-min target, no captions, filmed on employer code, intro first | ≥2 formats published |
| 18.4 Lunch-and-learn, free pilot, and questions into content | Internal lunch-and-learn as audience #1; free pilot treated as paid (prep, plan, form, follow-up, consent); questions log; theme threshold → piece; FTC endorsement rules for testimonials; exit signal | 18.1–18.3, 16.5, 17.3 | Deliver a lunch-and-learn; run a pilot; log questions; `questions`; write the pilot piece | Named askers, all inside and prompted, themes answered only in DMs; pilot post without consent or limits | `lunch-and-learn/`, `pilot/`, `questions.csv`, ≥4 published pieces |

Math (§10): none new. 18.1 reuses $1-(1-p)^n$ (15.3) for the number of pieces until an outside question, and the Wilson interval (07.4) for the share of pieces that produced one. 18.2 reuses the claims ladder (13.6). 18.4 reuses normalized gain (16.5) and the Wilson interval for follow-ups.

Simulation: none for this module.

Templates created: `templates/content-calendar.md` (calendar + questions log), `templates/content-piece.md` (piece header, claims rules, format rules).

Exit test (outline): someone outside your network asks a follow-up question unprompted — the first `outside,no` row in `talks/questions.csv`, checked with `PostCheck questions`.

Links to Module 17 (written in parallel) use manifest paths `lessons/module-17/lesson-0N.md`.
