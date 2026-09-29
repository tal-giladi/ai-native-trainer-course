# Billing Team Handbook (for humans and agents)

## Team rituals

- Daily stand-up at 09:30, 15 minutes, camera on.
- Sprint planning every second Monday at 10:00.
- Retrospective every second Thursday at 14:00.
- Backlog grooming on Wednesdays at 11:00.
- Demo to stakeholders at the end of every sprint.
- Tech talk on the last Thursday of the month (volunteers welcome).

## On-call

- On-call rotates weekly, Monday to Monday.
- The on-call engineer answers alerts within 15 minutes during business hours.
- Outside business hours, only P1 alerts page the on-call engineer.
- Hand over open incidents in the #billing-oncall channel on Monday morning.
- Write a short post-mortem for every P1 incident within 3 working days.
- Post-mortems are blameless.

## Incident severity

| Severity | Example | Response |
|---|---|---|
| P1 | Invoices not sent, portal down | Page on-call, fix now |
| P2 | Report slow, one customer affected | Fix this sprint |
| P3 | Cosmetic issue | Backlog |

## Jira

- Every piece of work has a BILL ticket.
- Stories need acceptance criteria before they enter a sprint.
- Bugs need steps to reproduce.
- Move tickets across the board yourself.
- Log time on tickets (optional since 2025).
- Use the "Blocked" flag and write why in a comment.

## Definition of done

- Code merged to `main`.
- Tests written and passing.
- Pipeline green.
- Deployed to staging.
- Product owner accepted the story.
- Documentation updated.

## Database changes

- Talk to the DBA before large schema changes.
- The DBA reviews every migration script.
- Never edit a `V###` migration script that is already merged; add a new script instead.
- Big data migrations run at night.
- Test migrations on a copy of production data first (ask the DBA for a copy).

## Environments

| Environment | URL | Notes |
|---|---|---|
| Dev | https://billing-dev.contoso.example | Shared, may be broken |
| Staging | https://billing-staging.contoso.example | Deployed from `main` |
| Production | https://billing.contoso.example | Manual deployment |

## Tools

- IDE: Visual Studio 2022 or Rider (your choice).
- SQL: SQL Server Management Studio or Azure Data Studio.
- API testing: Postman (shared workspace "Billing").
- Diagrams: draw.io, stored next to the code.
- Chat: Slack (#billing-dev, #billing-oncall, #platform).
- Kubernetes: Lens or `kubectl` (ask DevOps for access).

## Onboarding

- Read this handbook.
- Read the architecture page.
- Set up your development environment (see the README).
- Pair with a team member for your first three tickets.
- Ask questions early; there are no stupid questions.

## Vacation and absence

- Put vacations in the team calendar at least two weeks in advance.
- Make sure your on-call week is covered.
- Set an out-of-office message.

## Communication

- Prefer public channels over direct messages for technical questions.
- Summarize long discussions in the ticket.
- Be kind; assume good intent.
- Write in English in all shared channels.

## Coffee

- The coffee machine on the 4th floor is the good one.
- Clean the milk frother after use.
