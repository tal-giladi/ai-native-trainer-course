# Instructor notes — 13.1 Delivery metrics

**Teaching objective.** Students write a metric dictionary in which every duration has explicit start and stop events, every speed metric travels with a quality guardrail, and no metric is one the agent inflates by construction. They summarize durations with medians and p90 and compare with ratios of geometric means.

**Likely confusion.** Cycle time vs lead time vs PR review time. Draw the ticket lifecycle on the board and have students place each clock's start and stop on it before giving definitions. DORA's "change lead time" starts at commit, not at ticket start; teams mix these up constantly.

**Common misconception.** "A randomized comparison protects any metric." Randomization makes the arms comparable; it does not stop the treatment from moving the start line of a clock. The PR-clock break shows a tight, significant, randomized −49% that is mostly an artifact.

**Key analogy.** Timing a relay race from when the last runner takes the baton. A team that sprints its first three legs looks no faster; a team that hands over at the last meter looks brilliant.

**Common failure in the exercise.** Students export PR data and call `open_to_merge_hours` "cycle time". Make them write the start event next to the column name. Second: quality bars written so loosely ("no major bugs") that every ticket passes; insist on observable events with windows (21 days, 30 days).

**Expected exercise outcome.** A `metrics.md` with 5–8 metrics, each with start/stop events, source and guardrail; `describe` output for both Contoso files with a sentence explaining the mean/median gap (the 170 h ticket; size mix); a PR export with median and p90; cost per successful task for both arms of the Module 5 notes, with sample size stated.

**Extension exercise.** Pull 90 days of your own PRs, split by whether the author labelled them AI-assisted, and compute the PR-clock ratio. Then find five labelled and five unlabelled PRs and reconstruct their full cycle time from the tracker. Does the gap shrink?

**Discussion question.** DORA 2024 found AI adoption associated with faster code review but lower delivery stability. If you could add only one metric to a team's dashboard to watch that trade-off, which would it be and where would its clock start?
