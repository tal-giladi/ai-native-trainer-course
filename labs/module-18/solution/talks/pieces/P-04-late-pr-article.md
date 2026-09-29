# P-04 · Why PR review time lies

- Format: article
- Concept: C3
- Stage: trust
- Audience: tech leads and engineering managers who report delivery metrics
- Evidence: EXP-01 (randomized comparison, 96 tickets); report linked below
- Disclosure: none; the experiment ran on my own team's tickets with my manager's agreement
- Published: 2026-08-12

---

If your dashboard says pull requests are reviewed much faster since the team started using a coding agent, check one more clock before you celebrate.

## What we measured

Over twelve weeks, four developers on one legacy .NET billing system worked 96 tickets. Each ticket was randomly assigned, within developer and ticket size, to "with the agent" or "without" (EXP-01). We recorded the whole ticket, from In Progress to merged, and the two parts it splits into: time until the PR opens, and time from PR to merge.

The full [report](https://example.com/experiment-01) has the design, the ticket flow and the analysis commands.

## The result

With the agent, PR review time fell by 49% (95% CI 42% to 55%) in EXP-01. That is the number a dashboard shows.

Time to open the PR rose by 35% (95% CI 15% to 57%) in the same comparison (EXP-01). Developers kept working with the agent longer before asking anyone to look.

The whole ticket, cycle time, fell by 16% (95% CI 5% to 25%) in EXP-01. That is still a real improvement on this team's tickets. It is about a third of what the review clock alone suggests.

## Why it happens

Review time starts when the PR opens. If the work before that point gets longer and more complete, the part after it shrinks, even if nothing about reviewing changed. I call this the late-PR illusion: review looks much faster because PRs open later with more done.

The fix is not a new metric. It is the clock you already have: measure from In Progress to merged, and show the two halves next to it.

## What to do on Monday

1. Find where your dashboard starts its review clock. If it starts at "PR opened", add cycle time from "In Progress" to "merged" next to it.
2. Split every agent-assisted ticket into the two halves and look at them together, per ticket size. A shrinking second half with a growing first half is this pattern.
3. Before you report a speed-up to anyone, write down which clock it comes from. If it is the review clock alone, do not report it.

None of this needs a new tool. It needs the timestamps your tracker already stores.

## What this does not show

- One team, one repository, four developers and one agent: I do not know what you will see.
- Escaped defects were possibly higher with the agent: 10 of 48 tickets against 4 of 48, an interval from 0 to +25 points (EXP-01). That guardrail failed, so we did not roll out further.
- Teams that open draft PRs at the start of every ticket should not see this effect; we did not test that.

If your team reports review time, which clock does your dashboard start?
