# Instructor notes — 13.4 Threats to validity

**Teaching objective.** Students can list the threats to an engineering comparison with the direction of each bias, check the checkable ones against data in minutes (balance, by-version, ITT vs per protocol, sensitivity without outliers), compute expected regression to the mean, and use difference-in-differences with placebo units while knowing when it fails.

**Likely confusion.** Intention to treat vs per protocol. Students feel ITT is "wrong" because it counts contaminated tickets in the arm they did not really get. Reframe: ITT answers "what happens when we *assign* the agent?", which is the policy decision; per protocol answers a question about people who chose to comply, and choice is exactly what randomization was protecting against.

**Common misconception.** "Difference-in-differences controls for everything." It controls for changes common to all units. Regression to the mean is unit-specific, so a unit selected for a shock breaks the parallel-trends assumption. The break is built so that DiD *and* a placebo test both "confirm" a zero effect as −54%.

**Key analogy.** Sports Illustrated cover jinx and "sophomore slump": athletes get on the cover after an exceptional season, and the next season is, on average, less exceptional. Nothing about the cover caused it.

**Common failure in the exercise.** Threat registers with mitigations like "be careful" or "keep in mind". Every row needs an observable check ("compare by `agent_version`", "count exclusions per arm") or an explicit "residual risk: not checkable in this design". Second: students try to separate learning from the version change in the Contoso data and "find" an answer; the correct answer is that the design cannot separate them.

**Expected exercise outcome.** A register with 8–10 threats; the Show me table reproduced (−17%/−15% by version; ITT −16% vs per protocol −13%; −13% without BILL-503); a written sentence that learning and version change are confounded in time; a placebo DiD for Billing (+16%, placebo p = 0.50, ranked 3 of 12 in Q2), showing what an unremarkable DiD looks like.

**Extension exercise.** Using `prepost-teams.csv`, compute for every team the change from its worst quarter among the first three to the fourth quarter. What is the median "improvement" from worst quarter onward, with no treatment anywhere? That number is the regression-to-the-mean baseline for any "we fixed our worst team" story in this organization.

**Discussion question.** METR's 2026 update found developers refusing to work without AI and withholding tasks. If your own team does the same in week 3 of your experiment, what do you change, and what do you write in the deviations table?
