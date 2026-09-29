# Instructor notes — 15.2 Company scaffolding and the before state

**Teaching objective.** Students build a company around the demo and a before/after pair that is a fair comparison: empty layer at the before tag, only layer paths changing, the demo ticket open at both tags, the same agent, model, prompt and memory conditions on both sides. They run both sides once and write the sentence that stops the room reading it as a productivity claim.

**Likely confusion.** "Isn't the company scaffold just decoration?" No: PRDs, tickets and the glossary are the inputs the skills read, and the Finance constraint is what the after run visibly respects. Second confusion: students think a before/after needs statistics on stage. It needs honesty on stage: one run shows *how*, the measured runs from 15.1 say *how often*.

**Common misconception.** "A better before/after gap makes a better demo." A gap produced by a crippled before (weaker model, vaguer prompt, no tests) is a straw man, and someone will reproduce the before later. Second: "fixing the stale doc in the layer commit is harmless" — it is a second treatment.

**Key analogy.** A before/after photo for a renovation. If the "after" was also taken on a sunny day with a wide-angle lens, the viewer cannot tell how much was the renovation. Same light, same lens, same angle; only the renovation differs.

**Common failure in the exercise.** A user-level `~/.claude/CLAUDE.md` or saved auto memory leaking into the before run; students then conclude "the layer barely matters". Have them check `/context` and `/memory` before the before run. Second: rehearsing on `main` and tagging afterwards. Third: running the two sides with different agent versions weeks apart.

**Expected exercise outcome.** `DemoCheck before` passes; tickets visible in a tracker; a filled comparison table from two fresh sessions with conditions written down; one sentence with measured numbers and intervals. For the break: all four errors explained, including why the doc fix is a confound even though it is "correct".

**Extension exercise.** Add a third tag, `ai-layer-v1-rules-only`, containing only the grounded `AGENTS.md`. Run BILL-97 five times at each of the three tags. Which part of the layer carries the gap on this ticket, and what does that change about what you show live?

**Discussion question.** A prospective client says: "I'd rather see it on our code." What do you offer instead — and what does it cost you in credibility, time and risk if you agree?
