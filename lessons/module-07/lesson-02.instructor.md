# Instructor notes — 07.2 Task datasets

**Teaching objective.** Students treat each task as a small piece of tested software: reference and counterexample written first, validated mechanically, sourced, split and tagged. They can detect and remove answer leakage without deleting legitimate facts.

**Likely confusion.** Golden vs regression. Golden = must never regress, checked individually by the gate. Regression = born from a failure. Many tasks are both; the tags drive different treatment later (07.6).

**Common misconception.** "Holdout is for big ML datasets." Any time the same person tunes the layer and reads the scores, they overfit. A holdout of five tasks is enough to see a gap opening.

**Key analogy.** An exam and its answer key. A teacher who writes exam questions from the answer key, or leaves the key on the desk, learns nothing about what students know. The holdout is the question the students have never seen.

**Common failure in the exercise.** Students write counterexamples that are absurd ("banana") and conclude their graders are validated. Second: code tasks whose golden tests pass on the unmodified repository because they only test existing behavior. `validate --repo` catches both; make them show the output.

**Expected exercise outcome.** 28 tasks, `validate --repo` clean, a dataset card, and at least one grader bug caught by validation. For the break, students with an agent see dev scores rise on the leaked tasks and holdout unchanged; offline students run `leak` and get six LEAK lines and several `info` lines, and should be able to explain why the `info` lines are not leaks.

**Extension exercise.** Paraphrase the leaked section so that `leak` no longer flags it (change word order, synonyms). Confirm the holdout still exposes it. What does that say about relying on detectors versus design?

**Discussion question.** A client wants to publish your task set so vendors can compete on it. What happens to its value as a regression suite, and what would you keep private?
