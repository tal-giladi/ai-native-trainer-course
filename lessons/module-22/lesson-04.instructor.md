# Instructor notes — 22.4 Capacity-limited consulting and the productization plan

**Teaching objective.** Students write the module artifact: a 12-month productization plan with a cap and allocation that fit, capped and expiring retainers priced above the floor, a queue check at their planned utilization, month-12 revenue as ranges, a roadmap with measurable exit signals, and stop rules. `ScaleCheck plan` clean.

**Likely confusion.** Utilization vs busyness. Students hear "85% utilization" as "a bit of slack". The queueing curve is the point: waits grow with rho/(1-rho), so the last 10% of utilization costs more waiting than the first 70%. Draw the curve on the board with rho 0.5, 0.7, 0.8, 0.9. Second: retainers priced per day used vs per capped day.

**Common misconception.** "Products are passive income." Courses need support, updates when tools change (22.3), refunds; communities need office hours and moderation. Second: "A waitlist is good marketing." Only if it is real and short; a long waitlist is lost clients, and advertising it as scarcity is the fake-scarcity problem from 20.1.

**Key analogy.** Capacity planning for a service. You would not run a production database at 95% CPU and call it efficient; latency explodes and one spike takes it down. Your calendar is the same server.

**Common failure in the exercise.** No buffer, justified by "I will work harder that month". Second: demand guessed with no link to the funnel or the application log. Third: the roadmap front-loads everything (course, community, kit, two retainers) into months 1–3; ask them to compare month-by-month days needed with the allocation.

**Expected exercise outcome.** `productization-plan.md` with an allocation that sums to the cap including buffer and product days, at most one or two retainers each with cap/expiry/notice/scope, a queue output they can explain aloud, month-12 streams as ranges that fit the cap at the high case, 12 roadmap rows with numeric or checkable exit signals and evidence files, three or more stop rules, and a "what I will not do" section.

**Extension exercise.** Simulate month 4 going wrong: a pilot runs 30% over and a retainer client asks for its full day twice. Rewrite that month's allocation and state which stop rule or "When full" line you apply. Then do the same for a month when a large team licence sells.

**Discussion question.** A former client offers a retainer of 4 days a month at a good rate, which would fill half your cap for a year. What does it do to the next pilot's wait, to your product roadmap and to your employer permission, and would you take it?
