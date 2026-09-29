# Instructor notes — 17.2 Workshop materials

**Teaching objective.** Students build the parts of the kit that absorb room failures: a starter pack with a setup check run two days before, an offline package feed, a no-agent path and verified catch-up tags; exercise specs with catch-up and paper path; a troubleshooting guide from tested evidence; a fallback list played on the presenting laptop; forms from Module 16; and the rate-limit arithmetic for their busiest minute. `KitCheck kit` passes apart from the question bank and rehearsals.

**Likely confusion.** "The setup check replaces the troubleshooting guide." The check finds known problems early; the guide covers what the check cannot see (the day's network, a swapped laptop, the room's rate limit). Second confusion: catch-up tags as "the answers". They are start states for the next step; the reference answers stay in the instructor-only section.

**Common misconception.** "Engineers can set up their own machines." They can, at home, with an hour. In a room, on a managed laptop behind a proxy with a policy-restricted agent, the best engineer in the company loses twelve minutes. Second: "the rate limit is the vendor's problem." To the provider, the room is one customer that went from idle to busy in one minute.

**Key analogy.** A surgical team's checklist and instrument tray. Nobody in the theatre is less skilled for using a checklist; the list exists because known failures happen to skilled people at predictable moments. Your setup check and T−30 list are read at fixed moments for the same reason.

**Common failure in the exercise.** Tags created but not verified after a later change to the pack, so `ws-2-bound` fails to build on the day. A setup check tested only on the author's machine. Paper paths that are really "watch a neighbour". Troubleshooting entries phrased as causes that learners cannot recognize.

**Expected exercise outcome.** A `workshop` branch with four tags that all pass their tests; setup check scripts adapted and run on a clean machine; at least six troubleshooting entries with test dates, all under three minutes or routed elsewhere; three or four exercise specs with catch-up, paper path and a reference answer; a fallback list with play-back dates; forms for every objective; a written $D$, $L$ and stagger. For the break: the 22 errors grouped into notes, starter pack, exercises, troubleshooting, fallbacks and missing files.

**Extension exercise.** Build a dev container for the starter pack (SDK, Git, hooks prebuilt) and run the setup check inside it. What does it fix, what new failure does it introduce on locked-down Windows laptops, and would you offer it as the default or as a second path?

**Discussion question.** A host insists that learners use their real repositories in the hands-on steps "so that it is relevant". What do you gain, what could go wrong (confidentiality, uneven setups, no catch-up), and what would you propose instead?
