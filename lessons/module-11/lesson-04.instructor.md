# Instructor notes — 11.4 Operating agents: approval, rollback, observability and cost

**Teaching objective.** Students run agent jobs as a service: approval chosen by reversibility, rollback defined for every versioned part, a kill switch and shadow runs, a per-run ledger with cost per useful outcome, spend bounded at run, job and day level, and retries by failure class.

**Likely confusion.** "Useful outcome" vs "successful run". A run can end with `success` and produce nothing anyone uses (a review with zero comments, a changelog PR that is closed). $/useful is the business number; $/run is the infrastructure number.

**Common misconception.** "`--max-budget-usd` means we cannot overspend." Walk through the $137 day: no run broke the per-run cap. Most cost incidents are counts of runs, not size of a run.

**Key analogy.** Circuit breakers in a house. Each appliance has a fuse (per-run cap), each room has a breaker (per-job timeout and concurrency), and the house has a main switch (the kill switch) and a monthly bill limit with the utility (the provider spend limit). A space heater in a loop trips the room breaker, not the appliance fuse.

**Common failure in the exercise.** The budget guard sees nothing because only `agent-fix.yml` writes ledger lines; students forget step 2. Second: the kill switch drill passes for `workflow_dispatch` but a scheduled workflow was never checked; ask them to wait for one scheduled run with the switch off.

**Expected exercise outcome.** Ledger read with a written "cut first" decision per job; ledger steps in every agent workflow; a dated kill-switch drill; `budget-guard.yml` installed with a ceiling; the `agents` environment with a second-person approval; a rollback table with commands and no empty row. After the break and fix: a single capped run that fails, alerts, and costs under $2.

**Extension exercise.** Run the review prompt from 11.2 in shadow mode for two weeks: a second job step that runs v3 of the prompt and uploads its comments as an artifact without posting. Score shadow and production against the acted-on outcomes of the posted comments and decide on evidence.

**Discussion question.** Who should be paged when the budget breaker trips at 3 a.m., and should the breaker disable one job or all agents? Argue from blast radius and from how often a single job has caused the incidents you have seen.
