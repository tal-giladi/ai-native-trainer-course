# Instructor notes — 03.4 Writing a grounded rules file

**Teaching objective.** Students write a short rules file where every line is a directive with scope and evidence, check it mechanically (lint) and behaviorally (probes, convention tests, a real ticket), and can diagnose an agent that follows a stale rule.

**Likely confusion.** "Evidence markers waste tokens." A path in parentheses is 5–15 tokens; it lets the lint re-check the line forever and gives the agent a pointer to detail. Compare with the thousands of tokens spent on generic advice that fails the "actionable" test.

**Common misconception.** "The agent made a mistake." In the break, the agent did exactly what it was told. Students who blame the model will try to fix it with a stronger prompt; students who see stale context fix the rule and add a check. Make the class say the failure class out loud.

**Key analogy.** A rules line is like a unit test assertion about the codebase: it states something specific that must stay true, and it should fail loudly (lint) when the code moves on — not silently mislead the next reader.

**Common failure in the exercise.** Students keep lines "just in case" and end up with 150 lines where 30 would do. Make them run the deletion spot-check (delete a line, rerun its probe 5 times). Second failure: probing in the same session after editing the rules, so the old context is still loaded — always use fresh sessions.

**Expected exercise outcome.** An `AGENTS.md` of roughly 20–40 lines for Contoso, lint clean, probes 12/12, BILL-142 implemented with a Dapper repository method, `IClock`, SQL-side filtering and green tests; a changelog v1.1.0 entry; lint running in CI. For their own repository: a first grounded file with a before/after lint and probe log.

**Extension exercise.** Add a lint rule of your own: flag any rules line containing words like "always", "clean", "best practices" without a backticked token (a proxy for non-actionable advice). Measure how many lines of a public open-source `AGENTS.md` it flags.

**Discussion question.** If the model improves and infers most of your rules, should the file shrink toward zero? What would still need to be written down, and why?
