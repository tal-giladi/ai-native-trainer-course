# Instructor notes — 16.5 Measuring learning

**Teaching objective.** Students compute and interpret $\langle g \rangle$, $\bar c$ and Cohen's d with a bootstrap interval, spot the measurement errors that fake or hide a gain, read feedback next to learning, measure behaviour with a follow-up and a Wilson interval, and deliver and honestly report their own 30-minute mini-workshop.

**Likely confusion.** $\langle g \rangle$ versus $\bar c$: the first is computed from class averages, the second is the average of per-learner values. They differ, and both are legitimate; the tool prints both. Second: students think a bootstrap interval with six learners is precise because it has two decimals. Remind them of 13.5's small-sample warning.

**Common misconception.** "A pre/post gain proves the session works." It shows that scores rose in one group under one set of conditions, including the pre-test itself. Second: "relative improvement is just another way to say the same thing." Run the 10% → 30% example: a "200% improvement" with a lower normalized gain than a 70% → 90% group.

**Key analogy.** Test coverage. A green build with 20% coverage says little; a gain on items that were already at ceiling, or on the same items twice, is a green build on untested code. Normalized gain is "how much of the uncovered code got covered", not "how many lines were added".

**Common failure in the exercise.** Delivering to friends who helped build the concept, so pre-test scores are at ceiling. Second: forgetting the counterbalancing and giving everyone form A first. Third: scoring open items knowing which sheet is which. Fourth: writing `results.md` before the follow-up and never adding level 3.

**Expected exercise outcome.** A complete `talks/mini-workshop/` folder: session plan passing `align`, forms A/B with key, handout, three CSVs, and a `results.md` whose numbers all come from `LearnCheck`. Typical first-delivery gains range widely, often 0.3 to 0.7 with intervals spanning 0.2 or more; a low gain with a clear item-level diagnosis is a good outcome. For the break: at least eight of the nine listed errors found, and a rewritten summary close to the reference.

**Extension exercise.** Deliver the revised session to a second group and compare the two deliveries with both $\langle g \rangle$ and Cohen's d. If the groups started at different pre-test levels, which metric would you lead with, and why (Nissen et al., 2018)?

**Discussion question.** A client offers to pay only if the normalized gain from your workshop is at least 0.5. What would you agree to measure, how, and what would you refuse to promise?
