# Instructor notes — 05.4 Failure diagnosis across the loop

**Teaching objective.** Students locate a defect's origin phase by walking back diff → plan → brief → repository, list the escape phases, fix at the origin, add the gate with the largest effect, and start a six-ticket comparison with alternated arm order and honest limits.

**Likely confusion.** Origin versus escape. Students say "validation failed" because the missing architecture gate is the most visible gap. Ask: "If the gate had existed, would the brief still have been wrong?" Yes — so the gate is an escape; the brief is the origin. Both get fixed.

**Common misconception.** "The model should have known ADR 0007." It could only know what its context contained. The seeded research ran in the built-in Explore sub-agent, which does not load `CLAUDE.md`, with a prompt that did not name the ADR folder. This is the most transferable insight of the module: every context boundary (sub-agent, new session, compaction) is a place where rules silently stop applying.

**Key analogy.** Root-cause analysis on a production incident. The outage showed up in the web tier; the root cause was a config change two days earlier; the escapes were the canary that was not watching the right metric and the alert with the wrong threshold. Nobody fixes only the web tier.

**Common failure in the exercise.** Answering "does the diff follow the plan?" too generously, which makes everything an implementation defect. Enforce the "could a reviewer have predicted this line from the plan?" test. In part B, students run the loop arm second every time; check the order column in NOTES.md.

**Expected exercise outcome.** A diagnosis row for the seeded case with origin research, at least three escapes (plan checkpoint, plan-lint, tests; optionally code review), a fixed research prompt that makes 3 of 3 fresh briefs cite ADR 0007, and a green `LoopGate all` on the re-implementation. For part B, after 1–2 weeks: 12 rows (6 tickets × 2 arms), typically showing more research + planning minutes in the loop arm, fewer defects found in review and later, and a honest statement that six tickets and one developer cannot establish a causal effect.

**Extension exercise.** From the failure-diagnosis log, estimate $c_i$ per gate per defect class (caught / seen). Recompute the escape probability for your most common class and decide which single gate to add next. Revisit the estimate after Module 7.

**Discussion question.** If most of your team's agent defects originate in research, is the right fix a better research process, a better always-loaded rules file, or a better ticket-writing practice upstream? Who owns each option?
