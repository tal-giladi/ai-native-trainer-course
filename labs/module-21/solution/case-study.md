# Case study — Grounding a coding agent in a legacy .NET billing system

> **Illustrative reference** for the dry run: the anonymized case study written from `engagement-record.md`. Format: [case-study template](../../../templates/case-study.md). Check with `EngageCheck casestudy case-study.md --record engagement-record.md --deny <your private deny list>`.

- Client approval: approved in writing by the client's Head of Engineering on 2027-04-02, for this exact text
- Engagement: 12 weeks plus 90 days of follow-up, October 2026 to March 2027
- Record: C-01

## Context

A business-to-business software company with 15-25 developers in three teams. The system in scope is a .NET and SQL Server billing service with ten years of history: a legacy data-access helper, a newer repository pattern, and migrations whose undo-script convention started halfway through. The company had bought coding-agent seats for every developer the year before; in the month before we started, a few developers a week used them.

## Problem

In the tech lead's words: the agent kept writing new code against the helper the team had spent two years retiring, and review was the only thing catching it. The sponsor needed a renew-or-cancel decision for the seats at the next budget round and wanted it based on the team's own work.

## What we did

- Interviewed nine stakeholders before kickoff, including the finance controller who reconciles the report the team was about to change, and the security lead whose review had to happen before any repository access.
- Audited the repository against its tests, recent code and decision records, not its wiki page, which was four years out of date.
- Captured a 12-week baseline of cycle time and escaped defects, and ran the course's 24-task evaluation set on the code before any AI layer existed.
- Built the AI layer with the team: the first changes were paired, and by the last third of the build five of every six changes to it were written by the team's own developers.
- Randomized the billing team's eligible tickets for four weeks between working with the new layer and without it.
- Enabled the other two teams, and handed every recurring activity to a named employee who had already run it once without us.

## Results

| Outcome | Before | After | 95% interval | Reading |
|---|---|---|---|---|
| Agent pass rate on the team's own conventions (24 tasks x 5 trials) | 53% | 75% | +10.6 to +32.8 pts | improved |
| Cycle time, randomized tickets with vs without the layer | median 18.5 h | -8% | -24% to +12% | inconclusive |
| Escaped defects, same tickets | 10% | 0 of 12 vs 2 of 15 | -27 to 0 pts | inconclusive |
| Developers using the agent in a week, all teams | 3 to 4 of 19 | 89% at 90 days | Wilson 69% to 97% | improved |

The agent got measurably better at following this team's conventions. Whether the team delivers faster, this engagement cannot say: with 27 tickets, only a change of about 38% or more was likely to be detected, and the estimate is compatible with anything from 24% faster to 12% slower.

## What this does not show

- It does not show a delivery speed-up or fewer defects; both comparisons are inconclusive.
- The evaluation tasks measure conventions, not throughput.
- Usage was measured for the whole engineering group, but delivery only for one team.
- One engagement, one codebase, one agent version; your team's result may be different in either direction.

## What the client did next

The sponsor renewed the seats for one year with a checkpoint at six months, and asked for a larger comparison across two teams.
