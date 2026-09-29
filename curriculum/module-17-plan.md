# Module 17 plan — Workshop Design and Delivery

4 lessons · ~60 min instruction · ~10 h practice (three rehearsals) · depends on Module 16 (backward design, show-do-reflect, room protocols, pre/post measurement with `LearnCheck`), Module 15 (`brownfield-demo`, run sheet, fallbacks, recovery pattern, stranger test), Module 14 (Ground-Bound-Build-Prove, `concepts.md`, objections FAQ), Module 13 (EXP-01 and the claims ladder), Module 9 (security answers), Module 7 (Wilson interval).

Thesis: a 2-hour workshop is not a long talk and not four mini-workshops glued together. It is a timed arc (credibility → problem → live before/after → exercise → Q&A → close) in which the demo is cut into short live segments, each followed by learners doing the same step on their own copy of the repo, with agent wait-time used for prediction questions and Q&A. The kit is what makes it repeatable by the student on a bad day: agenda, instructor notes you can glance at, a starter pack with a setup check and catch-up tags, exercises with finish lines, a troubleshooting guide built from rehearsals, fallback recordings, forms, and a hard-questions bank answered with the student's own evidence. Rehearsal is measured, not felt: three runs, two observers, a rubric, timing drift and a notes-check count, to the exit standard "full 2 hours, notes checked at most twice".

Running example: one student's workshop *Ground before you generate* on `brownfield-demo` (BILL-97 demo, BILL-180 hands-on), built from the Module 14 reference method.

Shared lab material: `labs/module-17/` — `tools/KitCheck` (dependency-free C#: `agenda`, `kit`, `questions`, `rehearsal`), the reference `solution/workshop-kit/` (agenda with objectives/items/activities tables that `LearnCheck align` also reads, instructor notes, starter pack with setup checks, three exercises, troubleshooting, fallback list, forms, question bank, three rehearsal logs), four breaks.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 17.1 The workshop agenda | Timed arc with contiguous timestamps; demo cut into live segments each followed by hands-on; wait-time plan per live segment; buffer, break, short close; instructor notes as cues | 16.3, 15.4 | Write `agenda.md` from the template; `KitCheck agenda` + `LearnCheck align` | Demo marathon: 70 min demo, no hands-on, timestamps off by 6 min, 15-min pitch, no buffer | `workshop-kit/agenda.md`, `instructor-notes.md` |
| 17.2 Workshop materials | Starter pack with setup check, offline path and catch-up tags; exercise specs; troubleshooting from rehearsals; rate-limit arithmetic for a room; fallback list; forms | 17.1, 16.5, 15.3 | Build kit; `KitCheck kit`; run the setup check on a clean machine | Thin kit: online-only restore, exercise without done-criteria, untested fixes, fallback not listed | `starter-pack/`, `exercises/`, `troubleshooting.md`, `fallback/`, `forms.md` |
| 17.3 The hard-questions bank | Categories (replace, security, ROI, skeptic, confidentiality); answer shape: concede, evidence, boundary, don't-know, bridge; ≤ 60 s; no unsourced numbers; Q&A in wait-time | 17.1, 14.3, 13.6, 09.5 | 12+ questions; `KitCheck questions`; rehearse aloud with a timer | Hype bank: "10x", "guaranteed", numbers without sources, 3-minute answers, dodged security | `question-bank.md` |
| 17.4 Rehearsal protocol | Three rehearsals (solo tech run, friendly run with a drilled failure, dress with outsiders); two observers and a rubric; drift and agreement; exit standard | 17.1–17.3, 15.4 | Three logs; `KitCheck rehearsal` | Three solo "rehearsals" self-scored 4/4, notes checked 9 times, 14 min over | `rehearsals/r1–r3.md` |

Math (§10): none new. 17.2 uses token-rate arithmetic for a room (demand vs an organization's rate limit); 17.4 uses timing drift and observer agreement (exact and within-one), and reuses $\prod p_i$ from 15.4 for live segments.

Simulation: none for this module.

Templates created: `templates/workshop-template.md` (agenda + kit layout + question-bank and rehearsal-log formats, read by `KitCheck`).

Exit test (outline): full 2 hours from your outline, notes checked ≤ 2 times — the dress rehearsal log, checked with `KitCheck rehearsal`.

Links to Module 15 (written in parallel) use manifest paths `lessons/module-15/lesson-0N.md` and `labs/module-15/...`.
