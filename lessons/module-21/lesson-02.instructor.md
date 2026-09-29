# Instructor notes — 21.2 Technical audit and baseline

**Teaching objective.** Students audit a client's before state with triangulated evidence and client owners, capture a baseline from history before the build starts (definitions, 12 weeks, spread, guardrail, eval and usage baselines, no per-person metrics), and compute and explain the engagement's minimum detectable effect.

**Likely confusion.** Why history is preferred over a fresh prospective baseline: it is longer, free, and uninfluenced by the consultant. Second: the detectable effect is not a target and not the expected effect; it is the resolution of the instrument. Third: sd(log) must be the within-size spread (about 0.44), not the pooled one (0.67), because the comparison is stratified by size.

**Common misconception.** "Randomization means any real effect will show." Randomization removes bias, not noise; with 27 tickets only large effects are detectable. Second: "An inconclusive result means the engagement failed." It is a true result, and the SOW already says so.

**Key analogy.** A thermometer's resolution. If it reads to the nearest 5 degrees, you cannot use it to detect a 1-degree fever; you either buy a better instrument (more tickets, paired designs, evals) or you report "no detectable change at this resolution".

**Common failure in the exercise.** Copying findings from the wiki without running the tests. Using the pooled sd(log) 0.67 in the power calculation (which gives about 52% detectable). Forgetting the escaped-defect Wilson interval, or computing it with the normal approximation, which fails near 0.

**Expected exercise outcome.** Six to eight findings, one labelled hypothesis (`.claude/` content); a baseline with cycle time median 18.5 h (p25 11.3, p75 29.3), review time by size, escaped defects 6/60 (Wilson 5%–20%), eval 53% (CI 44%–63%), usage 3–4 of 19; detectable effect about 38%; `EngageCheck baseline` clean. Break exercise: the two-week window (ISO 39–40, 10 tickets) has a median of 28.1 h against 18.5 h for 12 weeks; with that baseline, an unchanged team would look about a third faster in week 11.

**Extension exercise.** Compute the detectable effect for your own team from your tracker's last 12 weeks. How many weeks of randomized tickets would you need to detect 20%?

**Discussion question.** The sponsor says, "If the pilot can only detect 38%, why run it at all?" Give two reasons that are honest, and one condition under which you would recommend not running it.
