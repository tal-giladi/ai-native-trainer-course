# Instructor notes — 13.5 Statistics for engineering comparisons

**Teaching objective.** Students analyze a skewed, blocked engineering comparison with the log scale, a stratified bootstrap interval and a within-strata permutation test; report ratio, Hedges g and Cliff's delta; and decide by reading the interval against a pre-stated minimum effect rather than by p < 0.05.

**Likely confusion.** Bootstrap vs permutation. Both resample, for different questions. The bootstrap asks "how much would my estimate wobble?" (resample *within* each arm, keep the labels). The permutation test asks "how often would a difference this large appear if labels were arbitrary?" (shuffle the labels *across* arms). Draw both on the board with the same eight tickets.

**Common misconception.** "p > 0.05 means no effect" and its twin "p < 0.05 means a large effect". The break puts both errors on two slides about the same data. For guardrails the first error is dangerous: "no significant increase in defects" is routinely read as "safe".

**Key analogy.** A bathroom scale you step on ten times: the bootstrap tells you how much the reading jitters; the permutation test asks whether the difference between wearing shoes and not is bigger than that jitter; the effect size tells you whether it is a kilo or a gram; and only you can say how many grams matter.

**Common failure in the exercise.** Students bootstrap the whole data set ignoring strata and get wide intervals, then conclude "no effect". Second: computing Hedges g with the SD across all sizes. Third: in the hand exercise, resampling *without* replacement (which just reorders the data).

**Expected exercise outcome.** Reproduced outputs; seeds 1 and 2 moving interval ends only in the third decimal ([0.747, 0.952], [0.746, 0.949]); five hand resamples with medians; the blocking table with a one-sentence explanation; a minimum effect written into the pre-registration and the Show me result classified as "real, possibly below the threshold" for a −10% threshold.

**Extension exercise.** Implement a clustered bootstrap: resample developers with replacement, then take all of each sampled developer's tickets. Compare its interval with the ticket-level one on `contoso-randomized.csv`. With only four developers, what goes wrong?

**Discussion question.** Your pre-registration said "act if the effect is at least −10%". The result is −16% [−25%, −5%]. Your sponsor wants to announce "16% faster". What do you agree to, and what do you insist stays in the sentence?
