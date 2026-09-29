# Instructor notes — 02.5 Failure taxonomy and model selection

**Teaching objective.** Learners classify failures by mechanism with evidence, and choose models from constraints plus measurements on their own tasks, using cost per successful task.

**Likely confusion.** Primary vs escape cause. Learners want one label. Show the `o.CustomerNumber` example: the invented column is what they *see*; the missing schema is what *caused* it; the absent build is why it *reached review*. Three different fixes follow.

**Common misconception.** "Everything wrong is a hallucination." Make every learner reclassify at least one of their own "hallucinations" after reading the trace. Second misconception: "the best model" exists independent of task. Show the per-task decision column.

**Key analogy.** An incident review in operations. Nobody accepts "the server broke" in a post-mortem; you want root cause *and* why monitoring missed it. The taxonomy is that discipline applied to agents. For model selection: hiring. Legal eligibility to work is a hard constraint, not 20% of the score; after that, you judge candidates on a work sample of *your* job, not on a public exam.

**Common failure.** Learners run ModelBench with N=1 and write up a winner, or they skip the hard-constraint step because "it's only a lab". Require the constraints table and N ≥ 5 before the matrix counts. Another: they weight price per token rather than cost per success; ask them for their team's real $h$.

**Expected exercise outcome.** Ten classified failures with evidence lines, where classes 5, 6 and 10 are common and class 1 is rarer than the learner first assumed. A matrix with ≥3 models from ≥2 providers, constraints applied, N ≥ 5 results per task, a per-task decision, a switch condition and a re-measure date. Often at least one decision flips between N=1 and N=5.

**Extension exercise.** Add a fourth task that exercises tool use (reuse the lesson 02.4 loop with each model) and record malformed tool calls per 100. Or measure $h$ properly: time three real reviews of agent PRs that had to be rejected.

**Discussion question.** "A CTO says: 'We've standardised on one model for everything so procurement is simple.' What does that cost them, how would you show it with their own data, and when is standardising actually the right call?"
