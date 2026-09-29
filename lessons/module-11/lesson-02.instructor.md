# Instructor notes — 11.2 CI review: signal vs noise

**Teaching objective.** Students measure a CI reviewer against humans as precision and recall with intervals, understand that comment volume against a low defect base rate caps precision, build ground truth from three incomplete sources, and ship a policy chosen on dev and confirmed on holdout.

**Likely confusion.** Which precision/recall is which. In 07.3 the grader's "positive" was *pass*; here the reviewer's "positive" is *a comment*. Write both confusion matrices side by side. Also: recall's denominator is known defects, not human comments; most human comments are not about defects.

**Common misconception.** "A better model will fix the flood." Run the ceiling calculation on the board first: 14 defects, 64 comments, 22% maximum. No model beats arithmetic. The flood is a prompt and policy problem.

**Key analogy.** A smoke alarm that goes off every time someone makes toast. It has excellent recall for fires. Within a month the battery is out, and then its recall is zero. Precision is what keeps recall alive.

**Common failure in the exercise.** Students tune on all 20 PRs, then "confirm" on the same 20. Insist on writing the policy down before running `--split holdout`, and ask them to run the holdout only once. Second failure: in their own repository they count every human comment as a defect, which inflates D and makes the agent look better than it is.

**Expected exercise outcome.** Reproduced numbers (v1 9/64, 9/14; v2 + medium 11/14, 11/14; holdout 4/6); the ceiling calculation (90% precision needs C ≤ 15 across 20 PRs); a written dev policy and a single holdout run; a completed CI review report; for their own repository, a first 20-PR scoring with ground-truth provenance and the workflow wired with the measured thresholds.

**Extension exercise.** Add a narrow second reviewer for `db/migrations/**` only (a migration-specific prompt from the Module 6 `new-migration` skill) and score the combination: does it raise recall on migration defects without dropping the combined precision below the team bar? Report both reviewers separately and together.

**Discussion question.** Tricorder's bar is 90% precision for automated checks. Should an LLM reviewer be held to the same bar, a lower one, or a higher one? Argue from what a developer does with a comment, not from what the tool is.
