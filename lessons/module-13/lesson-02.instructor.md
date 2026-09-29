# Instructor notes — 13.2 The evidence base on AI productivity

**Teaching objective.** Students can summarize the main studies accurately (design, population, metric, finding), explain why they disagree, and turn any productivity claim into a conditional statement the evidence supports — without falling into either hype or "nobody knows".

**Likely confusion.** Output metrics vs time metrics. Cui et al.'s +26% is completed tasks (PRs) per developer; Peng et al.'s 55.8% is less time on one task; He et al.'s "velocity" is lines added. Put the three on the board as three columns with their units before discussing any number.

**Common misconception.** "METR proved AI slows developers down." It measured 16 experienced maintainers on their own mature repositories with early-2025 tools; its own 2026 update found a likely speed-up with later tools and declared its measurements unreliable because of selection effects. The opposite misconception ("the big studies proved 26–55%") has the same shape. Both are single-study generalizations.

**Key analogy.** Drug trials: a drug that works in healthy 25-year-olds in a two-week lab trial is not yet shown to work in 70-year-olds with three conditions. The lab result is real; the transfer is the question.

**Common failure in the exercise.** Students write `evidence-base.md` as a list of headline numbers. Require the "transfer to your wedge" column and at least one study each that found no gain, a loss, and a quality cost. Second: citing DORA or Stack Overflow survey percentages as measured productivity.

**Expected exercise outcome.** Claim B: most damaging answer is assignment/selection (pilot team chosen for its worst quarter) — rewrite: "Forms' cycle time returned from an unusually bad Q2 to its usual level; we cannot yet attribute that to agents." Claim C: PR clock plus self-labelling — rewrite: "PRs labelled AI-assisted spent less time in review; we have not measured whether the tickets were faster end to end." A five-plus-study `evidence-base.md` with transfer notes.

**Extension exercise.** Find one study published after 2026-09 (the table's cut-off) and place it on the evidence ladder with the eight-question checklist. Does it change any sentence of the rewritten slide?

**Discussion question.** A client's CFO wants "one number" for the board. You have none of your own yet. What do you say, and what do you offer to have by the next board meeting?
