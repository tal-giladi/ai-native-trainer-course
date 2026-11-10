# Instructor notes — 10.5 Changes that cross services and teams

**Teaching objective.** Students plan a change to a shared contract as expand → migrate → contract, prove with `ContractCheck` that every step is safe with both versions live, split verification between repository owners, a contract check and one change owner, and estimate lead time from the critical path.

**Likely confusion.** "Atomic" merges. Students who work in one repository think a coordinated merge is a coordinated deploy. Draw the timeline: three pipelines, three rolling deploys, a queue between them.

**Common misconception.** "The agent updated all three repositories, so it already checked they fit together." It updated each repository's fixtures to match its own change. The fixtures agree with the code in the same repository, not with the other services. That is why every CI is green in the break.

**Key analogy.** Changing the plug on a building's appliances. You do not swap every socket overnight and hope every appliance was changed too. You install sockets that take both plugs, change the appliances team by team, and remove the old sockets only when the last appliance is done.

**Common failure in the exercise.** The agent proposes three pull requests, one per repository (the big-bang plan). Second: students run `rollout` and look only at the "after" lines, missing the "both live" lines. Third: the gate before the contract phase says "after the readers merge" instead of "after the readers are deployed and the queue has drained".

**Expected exercise outcome.** A four-phase plan (`v2-expand`, two reader v2s, `v3-contract`) that passes `rollout`; usually one or two rounds of feeding the checker output back to the agent; an `owners` list showing that billing's two phases need the readers' teams to know; a lead time of 12 days.

**Extension exercise.** Add a fourth service, `analytics`, owned by a team that has not published a contract and reads `invoice.issued.amount` from a data lake export. How does your coordinating session find it, what does `ContractCheck` miss, and what do you add to the gate?

**Discussion question.** A client's CTO says: "Our coding agents are fast, but cross-team changes are not faster." Using the critical-path formula, what would you measure at a client before promising any improvement, and which part would an agent actually shorten?
