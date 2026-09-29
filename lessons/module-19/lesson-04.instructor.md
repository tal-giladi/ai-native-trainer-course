# Instructor notes — 19.4 Measuring adoption and preventing regression

**Teaching objective.** Students define reach, habit and outcome metrics from team-level telemetry, detect real drops with per-team p-chart limits, distinguish step, decay, gap and cohort patterns, diagnose in the order real–where–when–why, and build anti-regression mechanisms and handover criteria that do not depend on them.

**Likely confusion.** Why the organisation's limit (55.5%) is so much tighter than a team's (about 50% for billing): the limit width shrinks with $\sqrt{n}$. Second: a share inside the limits is not "fine", only "not yet distinguishable from noise". Third: the p-chart baseline must come from stable weeks, not from the ramp.

**Common misconception.** "Low usage means people did not like it." Step drops are usually changes (tool, licence, quota, telemetry), not opinions. Second: "more telemetry detail is better." Per-person data adds legal and trust cost and little diagnostic value over team counts.

**Key analogy.** Service monitoring. You alert on SLO burn per service, not on the fleet average, and your runbook starts with "what changed?" (deploys, config, dependencies) before "why are users unhappy?". Adoption monitoring is the same: per team, events next to the chart, change first.

**Common failure in the exercise.** Writing the memo as a list of causes without the order (starting with "why"). Second: recommending retraining for everyone. Third: in the break, not noticing how late the 3-sigma alert fires for a gradual slide in a team of 50 (W25, after six weeks of decline) and not proposing a run rule.

**Expected exercise outcome.** A complete plan passing `AdoptCheck plan`, and a half-page memo on the break data that names the three stories (step in mobile and lending, decay in six teams, a cohort that never started), the weeks and events, and three actions with owners. Break: the web alert fires in W25 with web at 38% (19 of 50); the cohort experiment reports "fewer than 5, suppressed".

**Extension exercise.** Add a run rule to your monitoring (six consecutive weekly declines, or two consecutive weeks below $\bar p - 2\sigma$) and test it on the modified web data. How many weeks earlier does it fire, and how many false alarms does it produce on the unmodified solution data?

**Discussion question.** A sponsor wants to keep you on a monthly retainer "to keep usage up". When is that a good offer for them, and when is it a sign that the handover failed?
