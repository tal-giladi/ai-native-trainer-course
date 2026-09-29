# Instructor notes — 13.3 Experiment design

**Teaching objective.** Students design a controlled comparison their own team can actually run: ticket-level randomization within developer × size, size set before assignment, a recorded seed, a power estimate that honestly states the detectable effect, and a committed pre-registration. They can diagnose a self-selected comparison with a balance check and a stratified comparison.

**Likely confusion.** Blocking vs stratified analysis. Blocking is a design step (how arms are assigned); stratification is an analysis step (how arms are compared). Randomized-and-blocked data should be analyzed stratified by the same blocks; observational data can only be stratified, which repairs measured confounders and nothing else.

**Common misconception.** "A tiny p-value means the effect is real." The self-selected data gives p = 0.002 for a difference that is entirely size mix. Statistical tests answer "could chance alone produce this?", not "is this the treatment?". The permutation test within size gives p = 0.87.

**Key analogy.** Comparing a hospital's survival rates for patients who chose surgery with those who chose medication: the healthier patients choose surgery. Compare within severity, or better, let a coin decide.

**Common failure in the exercise.** Students estimate size after assignment (or let the tracker default fill it in later). Second: they compute power from the SD of raw hours or of log hours across all sizes (about 0.9 here) instead of within size (about 0.3–0.4), which inflates the required sample six- to sevenfold and makes them give up.

**Expected exercise outcome.** An assignment file for 30–60 tickets with a recorded seed and a balance check with at most one ticket of imbalance per block; a power table with the detectable effect at the achievable sample (typically 25–35% for 20 per arm); sections 1–5 of the pre-registration committed. For the break: balance shows 74% vs 33% agent use for S vs L; stratified ratio 1.02 [0.86, 1.20].

**Extension exercise.** Simulate in a spreadsheet: 40 tickets, no true effect, sizes S/M/L with medians 5/16/48 h, developers choose the agent with probability 0.8/0.45/0.25. Repeat 20 times. How often does the naive comparison show more than 30% "speed-up"? How often does the stratified one?

**Discussion question.** Developers say randomization is "unfair" because some tickets obviously suit the agent. What is true in that objection, what does it cost the experiment to honour it, and what compromise keeps the result interpretable?
