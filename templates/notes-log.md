# Notes log template (`NOTES.md`)

One row per ticket you run through an agent, written the same day. It is the raw evidence your later claims, your method and your workshop stand on: Module 13 turns it into a controlled comparison, Module 14 mines it for named concepts. Introduced in [05.1](../lessons/module-05/lesson-01.md) and [05.4](../lessons/module-05/lesson-04.md).

Rules: log every run, including the embarrassing ones. Record minutes from a timer, not memory. Count a defect when anyone (you, a test, a reviewer, production) finds it, and add it to the ticket's row even if it is found weeks later.

## Ticket log

| Date | Ticket | Size (S/M/L) | Mode (paste-and-go / loop) | Agent + model + rules version | Research min | Plan min | Implement min | Validate + fix min | Total min | Human review min | Gate failures (which) | Defects found in review | Defects found later | Rework commits | Resets | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| | | | | | | | | | | | | | | | | |

## Failure-diagnosis log

One row per defect that got past at least one phase. Classes from the [agent failure taxonomy](agent-failure-taxonomy.md).

| Date | Ticket | Defect (one line) | Origin phase (R/P/I/V) | Escape phase(s) and missing gate | Failure class (primary / escape) | Evidence (artifact + line) | Fix at origin | Gate added | Rerun result |
|---|---|---|---|---|---|---|---|---|---|
| | | | | | | | | | |

## Reset log

| Date | Ticket | Attempt # when reset | Signal (repeated fix, contradiction, compaction, drift) | Minutes lost before reset | Minutes to finish after reset |
|---|---|---|---|---|---|
| | | | | | |

## Weekly summary (5 lines)

- Tickets: [n] loop, [n] paste-and-go. Median total minutes: [loop] vs [paste-and-go].
- Defects per ticket: [loop] vs [paste-and-go] (found in review + later).
- Most common origin phase this week: [R/P/I/V].
- Gate added or changed: [which, why].
- What this sample cannot show (size, selection, learning effects): […].
