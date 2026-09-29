# Instructor notes — 07.1 Why evaluate agents

**Teaching objective.** Students leave convinced that a single run is not evidence for a stochastic system, can name the parts of an eval precisely, and have written a charter that ties the eval to a decision before any numbers exist.

**Likely confusion.** Transcript vs outcome. Students treat the agent's final message as the result. Use the code tasks: the transcript can say "all tests pass" while the golden tests fail in the working copy. The harness grades the outcome.

**Common misconception.** "We already have evals: our unit tests." Unit tests check the *code*; agent evals check the *agent system* (layer + agent + model) producing code or answers. A green unit-test suite says nothing about whether the agent will keep writing `V005` instead of editing `V004`.

**Key analogy.** A flaky test. Nobody accepts one green run of a known-flaky test as proof it is fixed; everyone reruns it. An agent is a flaky test with a 60–90% pass rate by design, so the same discipline applies to every change.

**Common failure in the exercise.** Students skip the transcript-reading step (step 5) and jump to scores. Make them read all six before looking at grades, and ask for at least one disagreement with a grade. Second failure: a charter with no decision rule, or a rule written after looking at data.

**Expected exercise outcome.** A working `agent-evals` repo with harness, tasks and history; the replay run shows 9/10 (versus 8/10 under ContextLab, because the T02 grader was repaired); a one-page charter; a six-line transcript log. With a real agent, T09 and T18 usually differ in character: T09 failures are context failures (BILL-97 not found), T18 failures are often scope violations or a missing boundary test.

**Extension exercise.** Compute, by simulation or exact binomial sums, how many trials per ticket are needed before an unchanged skill "wins by 2 or more" less than 10% of the time. Compare with the rule in your charter.

**Discussion question.** Your manager says "the demo worked, ship it". What is the shortest honest reply that does not sound like you are refusing to ship?
