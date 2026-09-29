# Instructor notes — 21.4 Enable, measure, hand over, follow up

**Teaching objective.** Students enable further teams with client owners from the start, report every pre-registered outcome with an interval and a matching status plus "what this does not show", hand over four recurring responsibilities to employees with backups who have run them without the consultant, and run 30/60/90-day follow-ups reading per-team usage against control limits.

**Likely confusion.** Why the pooled median (−23%) differs so much from the stratified estimate (−8%): pooling ignores size (the ai arm has a slightly larger share of small tickets) and compares medians of two small, skewed samples; the stratified ratio of geometric means compares like with like and uses every ticket. Second: "inconclusive" vs "no detectable change": both mean the interval includes zero; use the second only when the interval is narrow enough to rule out effects that matter.

**Common misconception.** "Handover is a meeting at the end." It is a condition: each owner has already run the rhythm. Second: "Follow-ups are free support." They are scoped checks in the SOW; anything more is the retainer rung.

**Key analogy.** An on-call handover in operations. You do not hand the pager to someone who has never been paged, and the previous on-call stays reachable for the first incidents (the SRE book's "development team backup").

**Common failure in the exercise.** Reporting only the eval and adoption results because they "came out". Writing the office-hours owner as the consultant "until they are ready". Booking follow-ups without an agenda, which turns them into social calls.

**Expected exercise outcome.** A results table with four outcomes (cycle time −8% [−24%, +12%] inconclusive; defects 0/12 vs 2/15, [−27, 0] pts inconclusive; eval +21.7 [+10.6, +32.8] pts improved; usage 89% at 90 days, Wilson 69%–97%), four "does not show" bullets, five owners with backups and rehearsal dates between 30 November and 16 December, follow-ups on 22 January, 19 February, 19 March. `EngageCheck handover` clean. Break: a 27 November handover fails all five owners (each first unassisted run is later) and the 30-day follow-up.

**Extension exercise.** Rewrite the cycle-time result as a money range using 13.6's method, including the −24% to +12% interval and the defect guardrail. How often is the range entirely positive?

**Discussion question.** The sponsor asks you to leave the defects row out of the read-out "because two tickets is not data". Do you agree, and what do you write instead?
