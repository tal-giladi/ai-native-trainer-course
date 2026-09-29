# Skill test tickets — prime and plan-feature

The ticket test set for the two skills that start the loop (copy this folder to `skill-tests/` in `ai-layer-lab`). Each row is a ticket, the category it represents, and what a correct run must produce. A skill version ships only if every row passes. Introduced in [06.4](../../../lessons/module-06/lesson-04.md).

| Ticket | Category | Since | prime must produce | plan-feature must produce | Golden rules |
|---|---|---|---|---|---|
| BILL-142 | read query, filter in SQL | 1.0.0 | `IsOverdue`, index `IX_Invoice_CustomerId_Status` | Touch includes `IInvoiceRepository.cs` | — |
| BILL-151 | reuse trap ("owed", "overdue") | 1.0.0 | `OutstandingAsync`, `IsOverdue`, `IClock` | no new service | `skill-tests/golden/BILL-151.research.rules` |
| BILL-152 | one-sentence fix | 1.0.0 | (skipped: one-sentence) | no plan file; "No plan needed" reply | — |
| BILL-154 | reuse + new read model | 1.0.0 | `OutstandingAsync` filter | reuses it; no `Statements/` folder | `skill-tests/golden/BILL-154.research.rules`, `skill-tests/golden/BILL-154.plan.rules` |
| BILL-155 | first status write | 1.0.0 | no write method exists yet; ADR 0007 | new repository method, no SQL in service | — |
| BILL-150 | schema: new table | **1.1.0** | `Schema change: yes`, V003 convention | V005 + U005 in Touch | — |
| BILL-153 | schema: NOT NULL column on an existing table | **1.1.0** | `Schema change: yes`, V004 precedent, `FROM dbo.Invoice` readers, backfill as open question | U005 in Touch, backfill, value at HUMAN checkpoint, existing tests untouched | `skill-tests/golden/BILL-153.research.rules`, `skill-tests/golden/BILL-153.plan.rules` |

Categories that 1.0.0 did not cover: schema changes (added in 1.1.0 after the BILL-153 failure). Categories still missing: tickets that touch `Legacy/`, tickets with conflicting acceptance criteria, tickets in a second project. Add a row when one of them appears in a sprint.

## Running the set

For each ticket, in a fresh session on a clean branch: `/prime <id>`, then `/plan-feature <id>`. Then:

```bash
dotnet run --project tools/SkillCheck -- contract research/<id>.md --rules .claude/skills/prime/contract.rules
dotnet run --project tools/SkillCheck -- contract plans/<id>.md --rules .claude/skills/plan-feature/contract.rules
dotnet run --project tools/LoopGate -- plan-lint plans/<id>.md --repo .
dotnet run --project tools/SkillCheck -- contract research/<id>.md --rules skill-tests/golden/<id>.research.rules   # if the row has one
dotnet run --project tools/SkillCheck -- contract plans/<id>.md --rules skill-tests/golden/<id>.plan.rules           # if the row has one
```

Record per ticket: pass/fail per check, minutes, and the skill versions, in the skill test log of `NOTES.md`.
