# Instructor notes — 07.4 Statistics for stochastic systems

**Teaching objective.** Students attach a Wilson interval to every rate, choose pass@k or pass^k by use case, size an eval before running it, and recognize pseudo-replication and fix it by sampling more tasks.

**Likely confusion.** pass@k vs pass^k. Anchor both to a person: pass@k is "a reviewer picks the one good attempt out of k"; pass^k is "nobody is watching, every attempt ships". Then ask which one their CI review agent from Module 11 needs.

**Common misconception.** "More trials always means more precision." Only for the per-trial SE, which is the wrong one when tasks differ. The pseudo-replication break shows a 2.5x underestimate from 4 tasks x 25 trials.

**Key analogy.** Estimating a school's average grade by testing four pupils twenty-five times each. You learn those four pupils very well and the school hardly at all.

**Common failure in the exercise.** Students compute Wilson with $n$ = number of tasks while using the pass count over all trials, or vice versa. Make them write the unit (tasks x trials) next to every number. Second: using the harness's `runs` output as a per-arm figure for a paired design; point forward to 07.5.

**Expected exercise outcome.** Hand-computed Wilson intervals matching `stats` to one decimal point (14/20: 48-85%; 0/5: 0-43%); a pass^3 per golden task with at least one below 0.5 flagged as not yet safe for unattended use; a trial count in the charter with the half-width it buys (typically 3-5 trials x 24-30 tasks, about ±8-10 points task-clustered).

**Extension exercise.** Simulate: draw 24 task difficulties from a wide distribution (e.g. uniform 0.2-0.95), simulate k = 1, 3, 5, 10, 25 trials per task, compute both the naive and the clustered SE, and plot the ratio against k. Where does adding trials stop helping?

**Discussion question.** A vendor reports pass@10 for their coding agent and you plan to run it unattended in CI. What number do you ask them for instead, and what do you expect it to look like?
