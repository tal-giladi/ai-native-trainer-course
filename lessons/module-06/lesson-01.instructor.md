# Instructor notes — 06.1 Skill anatomy

**Teaching objective.** Students treat a skill as an interface with four parts (trigger, inputs, output contract, steps), move every one-right-answer step into a script, follow each model step with a deterministic check, and measure triggering instead of assuming it.

**Likely confusion.** "A skill is just a saved prompt." A saved prompt has no trigger design, no inputs contract and nothing a script can check. The difference shows when the same request is run 15 times: a prompt-shaped skill loads sometimes and produces something different each time.

**Common misconception.** "If the skill is in the repo, the agent will use it." Model invocation is a probabilistic decision made from the description alone. Students who have only ever typed `/name` have never seen a skill fail to load; the trigger test is usually their first surprise.

**Key analogy.** A skill is a function in a library whose caller is a colleague skimming the index. The description is the index entry (the only thing they read before deciding), the output contract is the return type and postconditions, the scripts are the parts you would never let someone retype by hand.

**Common failure in the exercise.** Descriptions that describe the implementation ("Runs new-migration.sh and writes SQL") instead of the user's words ("add a column", "index", "schema change"). Second: the trigger test is run once per query, a single miss is "noise", a single hit is "works". Insist on 3 runs and on writing the numbers down.

**Expected exercise outcome.** A `new-migration` skill that lints clean, creates V005/U005 through the script, passes `LoopGate arch`, and a trigger run with recall of roughly 0.8–1.0 and false-trigger rate of 0–0.15 (real numbers vary by model and version; the illustrative samples show 0.93/0.07). One changelog entry. The `db-helper` break reproduces low recall and a V005 without U005.

**Extension exercise.** Add `paths: "db/migrations/**, src/**/Invoices/**"` to the skill and rerun the trigger test. Which queries changed, and why is a path scope a trade-off between false triggers and recall on requests that start before any file is open?

**Discussion question.** Your team has a 60-line "how we do releases" section in the root rules file. Using the cost equation, when does moving it into a skill save context, and what do you lose if the model does not load the skill on release day?
