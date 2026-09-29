# Instructor notes — 04.2 Layering context

**Teaching objective.** Students place every piece of project knowledge in one of four layers (always, on demand, generated, never), name the trigger for each on-demand item, and apply the decision-before-trigger rule so that layering saves tokens without losing facts.

**Likely confusion.** "On demand" sounds like "when the agent thinks it is relevant". For path rules and nested files it is mechanical: a matching file was read. Draw the timeline of one T04 run (question → answer) and ask where in that timeline a migration file gets read. Often: never.

**Common misconception.** "`@` imports are lazy references." They are expanded at launch. The only lazy reference is a plain path the agent may choose to read. Have students check with `ContextLab budget` or `/context` rather than take your word for it.

**Key analogy.** A kitchen. The always layer is the counter (salt, oil, knives: used in every dish). On demand is the cupboard above the station where you bake (flour is there because you only need it when baking — but if the recipe card says "preheat to 180°" and that card is inside the cupboard you open *after* you should have preheated, dinner is late). Generated is the fridge inventory list printed each morning. Never is the neighbor's fridge.

**Common failure in the exercise.** Students move almost everything into path rules and celebrate the budget; T04 then fails intermittently and they blame the model. Make them look at the failing run's `result` and at `/context` to prove the rule never loaded. Second failure: a topology map of 200 lines — that is a second rules file; cap it at ~60.

**Expected exercise outcome.** A root layer of roughly 20–40 lines, one path-scoped migrations rule (plus a mirror in Cursor or Copilot format), a 30–60 line topology map referenced by path, and T04/T05/T08 at 3/3 each. The break reproduces at least one T04 failure in three runs; if it does not, have them run 5 more — and note the lesson from 03.3 that passes in 3 runs do not prove reliability.

**Extension exercise.** Convert the migration procedure ("add a column": create V/U, update model, add test) into a skill with a precise description; test whether it is invoked for the prompt "add a PoNumber column" in 5 fresh sessions. Compare that trigger's reliability with the path rule's.

**Discussion question.** Should a team ever put a fact in two layers (root and path rule) on purpose? When is duplication a safety margin, and when is it the start of a contradiction?
