# Instructor notes — 07.3 Graders

**Teaching objective.** Students rank grader types (outcome check first), measure any grader's error as precision, recall and FPR against human labels, understand how grader error distorts reported scores, and can build and calibrate a rubric-based judge.

**Likely confusion.** Which class is "positive". Fix it once: positive = the grader says pass. A false positive is a wrong answer that passed. Students coming from CI-review precision (Module 11) will meet the same definitions with "comment" as positive.

**Common misconception.** "The judge matches the human pass rate, so it's fine." Show judge v1: 54% vs 50% on the set, kappa 0.08. An accurate average can hide near-random verdicts. Always look at the matrix.

**Key analogy.** A smoke detector that goes off whenever you cook and also when there is a fire. Its "alarm rate" might match the fire rate on some weekends, but the alarm tells you nothing. TPR − FPR is the part of the signal that is about fire.

**Common failure in the exercise.** Students peek at `human-labels.csv` before labelling. Insist on labelling first; the disagreements are the most useful output of the lesson. Second: "fixing" the rubric by adding the calibration answers' specifics (for example "BILL-79 is wrong"), which is answer leakage from 07.2 in grader form. The rubric may state ground truth ("the ticket is BILL-97"), not the list of wrong answers.

**Expected exercise outcome.** Students' labels agree with the file on 20–23 of 24; common disagreements are C15 (terse) and C10 (wraps SqlHelper in a repository). Real judge runs with the v2 rubric usually reach precision ≥ 0.9; the v1 prompt usually shows a clear length gap. Students should report a trial-to-trial flip count (often 0–2 of 24 for v2).

**Extension exercise.** Build a pairwise judge ("which answer is better, A or B?") and run all 12 right/wrong pairs in both orders. Count how often the verdict flips with order. Compare with Wang et al.'s position-bias finding.

**Discussion question.** A vendor reports "92% on our internal LLM-judged benchmark". What three questions do you ask before that number means anything?
