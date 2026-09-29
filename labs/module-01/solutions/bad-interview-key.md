# Answer key — the bad discovery interview

For [`transcripts/bad-interview.md`](../transcripts/bad-interview.md), lesson [01.4](../../../lessons/module-01/lesson-04.md). Try the critique yourself before reading this.

## Defect codes

| Code | Defect | What it produces |
|---|---|---|
| **L** | Leading: the question contains the answer ("don't you find…", "wouldn't it…") | agreement, not information |
| **H** | Hypothetical or future ("would you…", "how much would you pay…") | promises nobody keeps |
| **G** | Generic ("usually", "ever", "in general", scales of 1–10) | opinions and averages instead of events |
| **P** | Pitch: the interviewer describes the solution | politeness; the rest of the call is about you |
| **D** | Double-barrelled: two questions in one | an answer you cannot attribute |
| **C** | Compliment or opinion fishing ("do you like…") | compliments, the most worthless data in discovery |
| **A** | Anchoring: the interviewer offers the number | the number you suggested, returned |
| **M** | Missed thread: the participant offered a real event and the interviewer moved on | lost evidence |

## Question by question

| Q | Codes | Why |
|---|---|---|
| Q1 | P | Opens with a two-sentence pitch and "could be huge for teams like yours". Everything after is colored by it. The closing yes/no question is fine as a warm-up but wasted after a pitch. |
| Q2 | L | Invites agreement with the interviewer's opinion. Noa half-agrees to be polite. |
| Q3 | L, G | "Would you say" + a yes/no opinion about productivity in general. |
| Q4 | A, G | Offers 30% and 50%; Noa picks the lower anchor. Self-estimates of speed-up are unreliable anyway (the METR study in [13.2](../../../lessons/module-13/lesson-02.md): developers estimated +20% while measured −19%). |
| Q5 | G, D | "Usually" + forced choice between two options. Noa's answer contains a real event (AI reviewer tried in spring, turned off after three weeks). |
| Q6 | P, H, M | Ignores the turned-off reviewer (who turned it off? what did "noisy" mean? what replaced it?) and pitches instead. "Would that be useful" is always yes. |
| Q7 | D, G | "Biggest problem … and how would you fix it" asks for a ranking and a design. Noa nonetheless gives the best evidence of the call: the migration incident. |
| Q8 | P, H, M | Drops the migration story (how was it caught? what changed afterwards? who reviewed it?) to pitch rules files, then asks Noa to predict her manager. The answer is a **hype signal** (CEO asking about "AI strategy"), not a pain. |
| Q9 | H | Future attendance. Worth nothing without a commitment. |
| Q10 | G, L | "Do you ever…" — the answer to "ever" is always yes. |
| Q11 | — | The one open question. Could be sharper ("tell me about the last time…"), but it gets a concrete convention: stored procedures, not EF. |
| Q12 | L, P | Proposes the solution and asks for agreement. |
| Q13 | H, A | Hypothetical price with an anchor, asked of someone who says she does not hold the budget. Also a principle violation: no price talk before evidence. |
| Q14 | G | A 1–10 scale "in general" produces a number with no event behind it. |
| Q15 | D, L | Two causes offered in one question. Noa rejects both and gives the real one — a two-person knowledge bottleneck and a PR that waited four days last sprint. |
| Q16 | C | Compliment fishing. "Sounds great" is the output. |
| Q17 | G | Asks the participant to design the product. The answer contains a **blocker** (the CISO), which is valuable, but it arrives by luck. |
| Q18 | P, M | Promises a module instead of asking for anything: no follow-up, no intro to the manager or the CISO, no permission to see PR data. |

Counts: 18 questions; 17 with at least one defect; 1 clean (Q11). Interviewer words ≈ 2× participant words in the opening third.

## Usable evidence (past, specific, from Noa's own experience)

1. **Migration incident** (Q7): last month, AI-written migration dropped a default constraint on `Claims`; caught in staging on a Thursday; two people most of a day. Past, specific, costed (≈ 2 × 6 h = 12 engineer-hours), with an owner (the team). **Pain: unsafe schema changes from agents.**
2. **Review bottleneck** (Q15): two people know the claims engine; a PR waited four days last sprint. Past, specific. **Pain: review queue on a knowledge bottleneck.** Note that it is *not* an AI pain — it exists without agents, and agents that produce more PRs may make it worse.
3. **Abandoned AI reviewer** (Q5): tried in spring, noisy, turned off after three weeks. Past behavior and a failed workaround — the strongest kind of signal that a pain is real.
4. **Convention**: stored procedures, not EF (Q11). A stack fact for the stack inventory and a rules-file line later ([Module 3](../../../lessons/module-03/lesson-04.md)).
5. **Stakeholders**: a manager under CEO pressure (hype, but also a possible buyer) and a CISO who worries about customer data (a blocker to meet early).

## Unusable (opinion, future, flattery)

"30% faster" (anchored guess), "would attend", "maybe" on price, "seven" out of ten, "sounds useful", "sounds great", "that would help". Every one of these was produced by the question, not by Noa's experience.

## Questions that should have been asked

- (after Q5) "Tell me about the AI reviewer you turned off. Who decided? What did 'noisy' look like on a real PR?"
- (after Q7) "Walk me through that migration from the moment it was written. How did it get through review? What did you change afterwards?"
- (after Q15) "Tell me about the PR that waited four days. What happened while it waited?"
- (after Q17) "When did the CISO last raise it? What would she need to see?"
- (close) "Could you introduce me to your manager for 20 minutes?" or "Could you export last quarter's PR open/merge timestamps, no titles, so I can size the review wait?" — a **commitment** that costs Noa something.
