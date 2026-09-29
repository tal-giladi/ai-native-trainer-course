# Instructor notes — 09.6 Red-team your own layer

**Teaching objective.** Students run the full attack → observe → mitigate → retest → residual-risk cycle, score attacks into results.csv, gate them with EvalHarness at a 1.0 golden floor as a permanent regression suite, quantify residual risk with the rule of three and a Wilson interval, and write a register a named owner accepts.

**Likely confusion.** The inverted polarity: a "pass" means the attack was blocked. Say it repeatedly and show the results.csv reason column ("BREACH: canary reached ..." for fails). Students used to capability suites expect pass = task done.

**Common misconception.** "Green gate = secure." Kill it: green covers known attacks, this run, at your trial count. Residual risk is never zero. The rule of three makes this quantitative — 0/5 is a 60% upper bound, almost worthless.

**Key analogy.** A fire drill that passes proves the exits worked *today* for the scenarios you rehearsed. It does not prove the building is fireproof, and you schedule the next drill.

**Common failure in the exercise.** Reusing the 0.8 capability floor (the Break) and passing a 4/5-blocked attack. Also, students write an attack that is already blocked on baseline and think it validates the fix — it tests nothing. Enforce fail-before/pass-after. The mirror failure with a strong model: a live baseline run "holds" because the model ignored the fixture (Claude Opus 5.5 did on 2026-09-29), and students read that as the baseline being safe or the attack being useless. Point them at the model-agnostic fail-before (samples, trifecta, guard hook fed the obedient call) and at the rule of three for the live result.

**Expected exercise outcome.** One attack run through the whole cycle, a combined results.csv gated at --golden-min 1.0 (passes for hardened, fails for partial), a reproduced break, a residual-risk register row with a rule-of-three bound, and the suite wired into the Module 7 CI gate.

**Extension exercise.** Have them add a seventh attack of their own invention (still benign, canary-only) that the current hardened config does NOT block, then run the cycle to close it and record the new residual risk. This makes the "your suite only covers what you imagined" point visceral.

**Discussion question.** Who signs off on residual risk in their organization, and what number would make that person comfortable? How many trials would it take to produce it?
