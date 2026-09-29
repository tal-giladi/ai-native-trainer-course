# Instructor notes — 10.1 Multi-agent topologies

**Teaching objective.** Students describe any multi-agent design as a topology built from four mechanisms (isolation, parallelism, independent check, specialization), choose a topology from task properties rather than from roles, and recognize coupled work that must not be split.

**Likely confusion.** "Parallel" vs "specialists". Specialists split by *concern* (code vs tests, security vs performance) and may run in parallel; parallel sectioning splits by *part* (folder A vs folder B). The failure modes differ: specialists collide on shared decisions, parallel workers on shared files.

**Common misconception.** "More agents means more checking." A second agent with the same model and the same context mostly re-derives the first agent's view, and in a chain it does not check anything at all. An independent check requires a fresh context *and* something the checker can verify better than the author could.

**Key analogy.** A kitchen. One cook makes a dish faster than two cooks passing it back and forth, unless the dish splits into a salad and a dessert that share nothing. Two cooks both seasoning the same soup without talking is the specialists break.

**Common failure in the exercise.** Students classify (b) "implement BILL-151 with tests" as specialists because code and tests "are different skills". Let them run the break before correcting. Second: they read the trace but not the handoff files; the cause is in the plan text, not in any span.

**Expected exercise outcome.** Classification close to the solution notes (debate on (c) is healthy), both T18 traces read, the handoff folder opened, a design document with section 1 answered. The break produces `CS0246 … 'CollectionSummary'`; the `pw` rerun passes 3/3.

**Extension exercise.** Run the break with a real agent: copy `roles/` to a scratch folder, run `--topology specialists --only T19 --trials 3 --agent claude --budget-usd 1.00`. How often does a real tester guess the names right when it gets only the plan? Then give it the ticket (edit `Program.cs`, one line) and rerun.

**Discussion question.** A client's platform team has already built a seven-agent pipeline and is proud of it. How do you open the conversation without making it about their pride, and which single measurement would you propose first?
