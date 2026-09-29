# Instructor notes — 15.4 When the demo breaks

**Teaching objective.** Students leave with a run sheet that passes `DemoCheck runsheet --repo`, a demo whose $P(\text{clean})$ they can state and explain, and one recorded drill in which they recovered from a failure they did not choose, within the segment's budget, using name → decide → teach → log.

**Likely confusion.** "Isn't switching to a recording cheating?" Not when you say so. Silently passing off a recording as live is; "here is Tuesday's run of the same ticket" is honest and keeps the lesson moving. Second confusion: the probability is not a forecast of the talk, it is a planning tool that says where the risk sits.

**Common misconception.** "Good presenters don't have failures." Good presenters have rehearsed failures. Second: "keeping more segments live makes it more authentic". Authenticity comes from the unedited recording and from honest narration, not from exposing the audience to a 1-in-3 segment.

**Key analogy.** A pilot's checklist and go-around. Pilots do not improvise a missed approach; they have a procedure, a decision height and a phrase for the radio. The run sheet's recover budget is your decision height; the say line is your radio call.

**Common failure in the exercise.** Choosing the drill seed until a comfortable card comes up. Insist on the first seed. Second: say lines written in a formal register nobody speaks; have students say them aloud and rewrite. Third: fallbacks that were never opened — the recording timestamp is wrong or the branch fails tests.

**Expected exercise outcome.** A run sheet with 4–6 live segments, P(clean) typically 0.5–0.75, at least three counted rehearsals per live segment, and one recorded drill with an `error` → `recover`/`fallback` pair within budget. For the break: all errors explained, segment 6 cut (not "fixed"), and the implement segment moved to tests-first plus a pre-baked branch.

**Extension exercise.** Run a "correlated failure" drill: the network is gone for the whole demo. Can you deliver the remaining 30 minutes from recordings and the pre-baked branch alone? What would you change in the run sheet so that the answer is yes?

**Discussion question.** A client stakeholder watches your demo fail and recover well, and then asks: "If it fails in your demo, why would I trust it in my CI?" What is your honest answer, and which evidence from Modules 7, 11 and 13 do you show?
