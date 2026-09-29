# Instructor notes — 19.1 Why rollouts fail

**Teaching objective.** Students explain adoption as individual decisions multiplied through a funnel, name the organisational failure modes and the stakeholders behind them, and build a stakeholder map whose answers concede, cite evidence and state boundaries instead of promises, with employee representation, privacy and legal in it before go-live.

**Likely confusion.** Stance versus influence: a low-influence enthusiast is not a champion yet, and a high-influence neutral is more urgent than a low-influence blocker. Second: "evidence" means something the stakeholder can open, not "vendor says" or "industry studies".

**Common misconception.** "Adoption is a communication problem: explain it well and people will use it." Communication moves perceived usefulness a little; time, policy, help and peers (facilitating conditions and social influence) move behaviour. Second: "a mandate is a strong social influence." It produces logins and resentment, not habit.

**Key analogy.** A database migration on a live system. The schema change (the platform) is the easy part; the hard part is every service that reads the table (every stakeholder) and the order in which they are updated. You do not run it without knowing who depends on what, and you do not declare it done when the DDL succeeds.

**Common failure in the exercise.** Writing the map from imagination rather than conversations, so every objection sounds like the student's own worry. Second: answering the replacement objection with a promise because the honest answer feels weak. Third: leaving out the works council or DPO because "it's a tech rollout".

**Expected exercise outcome.** A section 1 with 8–12 rows from at least five conversations, `AdoptCheck stakeholders` clean, every high-influence skeptic owned, and a funnel calculation naming the weakest stage. Break: the three sabotaged rows produce four or more errors; the fixed rows resemble the solution.

**Extension exercise.** Pick the stakeholder whose stance you most want to move and design a two-week "try it on your own code" offer for them alone. Which UTAUT determinant does it target, and how would you know it worked?

**Discussion question.** A sponsor asks you to promise that nobody will lose their job because of the rollout. You cannot. What do you say to the sponsor, and what do you say to the developers?
