# Worked example — pain map after five interviews

Illustrative and fictional, continuing [the example wedge](wedge-example.md). I02 is "Noa" from [the bad transcript](../transcripts/bad-interview.md), re-interviewed properly with the [discovery script](../../../templates/customer-discovery-script.md).

| Id | Pain (their words) | Heard in | Verbatim quote (anonymized) | Last event | Frequency | Size | Workaround | User | Champion | Buyer | Blocker | AI-related? |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| P1 | "The agent's migrations scare us" | I02, I03, I05 | "It dropped a default constraint on Claims and we found it in staging on the Thursday." (I02) | I02: last month; I03: August; I05: two weeks ago | ~1 per month per team | 8–16 engineer-hours per incident; one near-miss in production (I05) | juniors banned from agent migrations (I03); manual DBA review (I05) | developers | I02 (senior dev) | eng manager (I02's) | DBA team | yes — worse with AI |
| P2 | "Everything waits for the two people who know the engine" | I02, I04, I05 | "Last sprint I had one sit for four days." (I02) | I02: last sprint; I04: this week | weekly | flow: median 26 h to first review, p90 71 h (I04's export); effort 19–56 engineer-hours/month | pairing on Fridays (I04), abandoned | developers, 2 experts | I04 (team lead) | CTO (I04) | — | no — may get worse with more agent PRs |
| P3 | "New people can't touch the procedures for months" | I01, I05 | "Our last hire needed nine weeks before her first solo procedure change." (I01) | I01: hire in spring | per hire (≈ 4 / year) | ~9 weeks of reduced output per hire | a 40-page Confluence page nobody updates | new hires, leads | I01 | eng manager | — | yes — agents could help with explained context |
| P4 | "We tried an AI reviewer and turned it off" | I02, I04 | "Someone turned it off after about three weeks." (I02) | spring | once | 3 weeks of noise; trust loss | none | developers | — | — | — | yes |

## Problem statements

- **P1.** Teams of 10–60 .NET developers on a SQL Server–centred system lose 8–16 engineer-hours per incident, about monthly, because agent-written migrations skip the conventions that live only in senior heads; today they ban or hand-review agent migrations. Engineering managers care because the near-misses reach staging and, once, production.
- **P2.** The review queue waits on one or two experts (median 26 h to first review in one export); agents that produce more PRs will lengthen it. CTOs care about lead time; developers care about context switching.

## Parking lot (not evidence)

"Would attend a two-day workshop" (I02, I03). "AI makes me 30% faster" (I02). "We should use MCP everywhere" (I04's architect, second-hand). "Sounds great" (everyone).

## Verdict

**Change** the wedge: keep the stack and audience, move the pain from "ticket to PR" to "safe schema and procedure changes". P2 is noted for Module 11 (CI review) and P3 for the workshop's onboarding angle. CISO concerns (I02) go to the security module's threat model (Module 9).
