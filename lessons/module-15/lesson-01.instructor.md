# Instructor notes — 15.1 Designing a credible brownfield demo

**Teaching objective.** Students leave with an assembled `brownfield-demo` that passes `DemoCheck credibility` with their own private deny list, and a demo ticket chosen from measured runs with an interval, a before/after gap, a duration and a recognizability check.

**Likely confusion.** "Isn't reconstructing a history with fake dates dishonest?" It is honest when the README says the history is reconstructed and the company fictional; it is dishonest when presented as a real company's repository. The lesson is about showing the *shape* of real legacy work without using anyone's real work. Second confusion: students think the confidentiality scan checks the working tree. It reads every added line on every ref, and author metadata.

**Common misconception.** "The best demo ticket is the most impressive one." The best one is repeatable, shows a gap, and is recognized. An impressive ticket that fails half the time is a gamble with the room's trust. Second misconception: "a clean repo looks more professional" — the stale doc is the point.

**Key analogy.** A flight simulator. Pilots do not train on a cartoon plane; they train on a replica of the real cockpit with realistic failures, precisely because the real plane cannot be used for practice. The demo repo is a replica of a legacy codebase that nobody minds you breaking in public.

**Common failure in the exercise.** Picking the ticket before measuring, then "measuring" two runs that confirm it. Insist on ten runs with the layer and five without, graded by checks. Second: the deny list saved inside the demo folder "for convenience" — the tool fails it on purpose; ask why. Third: commits authored with the work e-mail because `git config --global user.email` is set that way.

**Expected exercise outcome.** An assembled repository with 26 commits on `main`, two tags and the fallback branch; `credibility` 0 errors with a private deny list; a `ticket-selection.csv` with at least two candidates measured (10 + 5 runs each). For the break: four errors named, the order "rotate first, then rewrite" stated, and the repository rebuilt clean.

**Extension exercise.** Add a third era to the demo: a 2026 feature half-migrated to a new pattern (for example, one repository using a new async API), with an ADR that is still "Proposed". Measure whether BILL-97's success rate changes. What does that tell you about how much mess a demo can carry before it stops being repeatable?

**Discussion question.** An attendee asks: "Your demo repo was built to make the agent look good. Why should I believe any of it transfers to our code?" What do you concede, what evidence from this module do you show, and where do you send them next (Module 13's claims ladder is a good answer)?
