# Instructor notes — 10.4 When multi-agent is worse

**Teaching objective.** Students run and report a fair single- vs multi-agent comparison — one treatment, pinned, paired, A/A, compute-matched baseline, cost and latency with intervals — and write a recommendation against a decision rule they set before the runs.

**Likely confusion.** "Compute-matched" does not mean "same number of tokens". It means the single-agent arm is allowed the same *kinds* of work the pipeline does (plan, run tests, re-check). The costs will still differ, and that difference is part of the result.

**Common misconception.** "No detectable difference means the designs are equal, so pick either." With 24 tasks the paired interval is about ±12 points; a real 8-point difference could hide in it. The report must state the smallest effect it could have detected, and "equal quality at double the cost" is still a reason to keep one agent.

**Key analogy.** A pharmaceutical trial that compares a new two-drug combination against no treatment, instead of against the best single drug. It will look great and answer the wrong question. `solo-verify` is the best single drug.

**Common failure in the exercise.** Students run the pipeline arm last, on a different day, after an auto-update (the 07.5 break); the manifest check catches it only if they read `compare`'s warnings. Second: they drop tasks that hit the budget stop in the pipeline arm. Third: they write the decision rule after looking at the numbers — ask to see the timestamp.

**Expected exercise outcome.** On real runs most students find, as in the illustrative data, no detectable pass-rate difference between `single` and `pwr` at roughly 2–3× cost and latency; `solo-verify` at modest extra cost; code-task results too few to decide. Some will see the pipeline win on code tasks; the report must then show the compute-matched comparison and the task-level interval before claiming it.

**Extension exercise.** Add a sixth arm: voting. Run the single agent three times per code task in separate copies and keep the first result whose own tests pass (a short script around `EvalHarness run --keep true`). Compare against `pwr` at matched cost. Which buys more points per dollar, collaboration or sampling?

**Discussion question.** The module's sample quiz: a vendor pitches a seven-agent PR-review pipeline. You ask for three numbers and they send a demo video instead. How do you respond, and what would you offer to measure for free to keep the conversation evidence-based?
