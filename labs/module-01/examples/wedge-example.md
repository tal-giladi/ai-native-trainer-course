# Worked example — wedge.md (v2)

Illustrative. A fictional student: 14 years of C#/.NET, the last 6 as tech lead on a SQL Server-heavy B2B platform, daily Claude Code user. Version 1 was written in 01.3; version 2 after five interviews in 01.4.

```markdown
# Wedge — v2 (2026-10-19)

## One sentence
AI-native delivery for .NET teams of 10–60 developers maintaining a SQL Server–centred
brownfield system on Jira, Confluence and GitHub, who want agents to change schema and
stored-procedure code without breaking production.

## Role and stage
- Role first: trainer (half-day workshop for one team), consultant later.
- Stage: practitioner. Evidence: 9 tickets in NOTES.md (not yet built — Module 5).
- First claim I want to make: "agent-written migrations in your repo get caught before staging."
  Artifact that will back it: the migration hook + eval tasks (Modules 7–8), measured on my own repo.

## Why me
- 14 years .NET; 6 years owning a 900-procedure SQL Server database.
- Personally rolled back two bad migrations in production (2021, 2024).
- 140 first-degree contacts with .NET in their title; 3 former colleagues now lead teams.

## Criteria
| Criterion | Weight | Score | Evidence |
|---|---|---|---|
| Specific | 0.20 | 4 | stack + team size + pain named |
| My advantage | 0.25 | 5 | the two rollbacks; the procedure estate |
| Reachable | 0.20 | 4 | funnel estimate ~13 conversations; 5 held |
| Pain recurring and sized | 0.20 | 4 | migration incidents in 3 of 5 interviews; ~12 engineer-hours each |
| Stack durable and common | 0.15 | 4 | .NET + SQL Server + Jira: long-lived; not tied to one agent |
| **Weighted total** | 1.00 | **4.25** | |

## What I am giving up
Greenfield startups; Java and Node shops; "AI strategy" talks for executives; Azure DevOps-only shops (for now).

## Interview verdict
- Interviews: 5 (2026-10-06 … 2026-10-16).
- Pains in ≥ 2 interviews: unsafe agent-written schema changes (3), review queue on 1–2 experts (3), onboarding into the stored-procedure estate (2).
- Commitments: 2 intros to engineering managers, 1 anonymized PR export.
- Decision: **change**. v1 said "on Jira"; three of five teams hit the pain in *review*, not in the tracker.
  v2 moves the pain to schema/procedure changes and keeps the stack.
```
