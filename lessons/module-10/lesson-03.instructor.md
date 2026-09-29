# Instructor notes — 10.3 Cost, latency and failure propagation

**Teaching objective.** Students estimate a pipeline's cost, latency and success before building it, compute when a reviewer helps or hurts, and debug a multi-agent run from its trace to the originating span, with loop limits in place.

**Likely confusion.** Recall and false-alarm rate of the reviewer vs precision and recall of a grader (07.3). They are the same confusion-matrix quantities; here "positive" means "the reviewer flags a problem". Draw the 2×2 once with "worker right / wrong" against "reviewer approves / flags".

**Common misconception.** "Two checks multiply reliability: 90% and 90% make 99%." That holds only if the second check's misses are independent of the first's errors. A same-model reviewer shares the worker's blind spots; its recall on exactly the errors that matter is low, and its false alarms trigger rework that breaks correct answers.

**Key analogy.** Proof-reading your own essay an hour later vs having an editor read it. The hour-later you misses the same mistakes; the editor catches different ones. A compiler is an editor who checks only grammar, perfectly.

**Common failure in the exercise.** Students stop at the budget stop when asked for the origin. Insist on "first span whose output was wrong". Second: they take `estimate`'s numbers as predictions; they are the consequence of the inputs, which in the pipeline files are invented.

**Expected exercise outcome.** Step 1: 1.2 rounds and $P_1 = 0.912$ (+1.2 points). Step 2: lowering reviewer recall to 0.3 cuts the pipeline's gain over one agent from +21.9 to +9.3 points; raising the worker's `p` to 0.85 halves the reviewer's own contribution (+19.1 to +8.8 points); a false-alarm rate of 0.3 barely moves success but doubles the reviewer's extra cost. In none of the variants does the reviewer lower the cost per success ($0.49 without it, $0.55–0.77 with it). Step 3: two traces with an origin named (for most illustrative failures: an APPROVE on a wrong result, i.e. incorrect verification). Section 5 of the design with numbers.

**Extension exercise.** Add a deterministic gate to the pipeline model: a build-and-test stage with recall 1.0 on the errors tests cover (say 60% of all errors) and false alarm 0. Model it by lowering the worker's error rate the reviewer sees, and compare with adding the model reviewer. Which gives more points per dollar?

**Discussion question.** A vendor quotes "our reviewer agent catches 80% of bugs". Which two numbers from this lesson's equation are missing from that sentence, and how would you measure them on the client's tasks?
