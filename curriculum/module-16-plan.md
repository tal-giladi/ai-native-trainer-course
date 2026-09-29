# Module 16 plan — Instructional Design for Engineers

5 lessons · ~80 min instruction · ~5 h practice incl. the 30-minute mini-workshop · depends on Module 14 (`concepts.md`: the concept the student teaches), Module 13 (bootstrap, effect sizes, threats to validity), Module 7 (Wilson interval), Module 1 (discovery interviews, reused for prior-knowledge chats), Module 15 (demo recovery and fallback recordings, linked from 16.3).

Thesis: people enjoying a workshop ≠ people learning something. The module teaches the student to design a short session backwards from observable objectives, to put learners to work for most of it, to handle the room, and to prove (or disprove) learning with parallel pre/post forms, normalized gain, a feedback form read next to the scores, and a two-week behaviour follow-up. The running example is one student teaching **Shadow rule** (C1 from the Module 14 reference `concepts.md`) three times: a polished demo that was loved and taught almost nothing (g = 0.11), a redesign (g = 0.67), and a second delivery reported badly.

Shared lab material: `labs/module-16/` — `tools/LearnCheck` (dependency-free C#: `align`, `gain`, `feedback`, `followup`), the reference `solution/mini-workshop/` (session plan, forms A/B with key, responses, feedback, follow-up, results), five breaks.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 16.1 How adults and engineers learn | Kirkpatrick levels; reaction ≠ learning (Deslauriers, Uttl); working memory and prior knowledge; ICAP; retrieval; engineers' specifics; learning claim | 14.2, 01.4 | Pick the concept; 3 prior-knowledge chats; learning claim | Demo-only session: 4.86/5, g = 0.11, confidence gap +46 | `talks/mini-workshop/session.md` header + learning claim |
| 16.2 Objectives, alignment and cognitive load | Backward design (UbD) and constructive alignment; observable objectives with levels; items at the objective's level; intrinsic/extraneous load, worked examples, expertise reversal; progressive difficulty; misconception items | 16.1 | Objectives + items tables; forms A/B draft | Vague verbs, recall items, orphan item, 9 terms on a slide: 12 errors | objectives, assessment, forms A/B |
| 16.3 Show, do, reflect | Rosenshine; worked → faded → solo; live coding rules; agent wait-time; fallback (15.4); exercise design with hint ladders; reflection and retrieval; 30-min timing | 16.2, 15.4 | Activities table; handout; rehearse once | 19 min passive, 14% active, two objectives never practised | complete `session.md` passing `align` |
| 16.4 Managing the room | Q&A protocol; mixed levels (floor/ceiling tasks, expertise reversal); skeptics with boundaries and evidence; silence and wait time; remote vs in-person; think-pair-share | 16.3 | Room plan; answers to 3 likely objections from own FAQ | Remote room log with 8 incidents | room plan section; objections cards |
| 16.5 Measuring learning | Normalized gain math, c, d, bootstrap; relative-change trap; ceiling; parallel forms; pre-test effect; Kirkpatrick 1–3 instruments; feedback analysis; follow-up with Wilson | 16.2, 13.5, 07.4 | **Field:** deliver the mini-workshop to 3–8 people; run `gain`, `feedback`, `followup`; write results | Manager write-up: "71% improvement", same form, late joiner, 100%-pre learner as 1.0 | `talks/mini-workshop/` complete |

Math (§10): normalized learning gain in 16.5 (intuition → equation → example → LearnCheck → interpretation), plus mean normalized change, Cohen's d, bootstrap interval, reused Wilson interval for the follow-up.

Simulation: none for this module.

Templates created: `templates/lesson-template.md` (session plan, read by `LearnCheck align`), `templates/pre-post-assessment.md` (item rules, CSV formats, feedback form, follow-up message, results outline). The workshop template is left for Module 17.

Field assignment (outline): 30-minute mini-workshop on one concept to 3–8 people: pre-assessment → workshop → exercise → post-assessment → feedback analysis. Protocol in `labs/module-16/README.md`, run in 16.5.

Links to Module 15 (written in parallel): 16.3 links `lessons/module-15/lesson-04.md` (demo recovery) and `lesson-01.md`.
