# Context audit — Contoso Billing (reference answer)

Filled from `templates/context-audit.md`. Before: `bloated/` (905 lines in 6 files; 807 always-loaded lines,
7,028 o200k tokens). After: `solution/` (see the budget table at the end). Task set: `tasks-v0` (10 tasks, 3 runs each).

## 1. Critical facts (from the task set)

| Fact | Statement | Inferable from code? | Evidence | Bucket |
|---|---|---|---|---|
| F1 | Tests run with `dotnet test Contoso.Billing.sln` | partly (one .sln), but the old docs say `Billing.sln` | `Contoso.Billing.sln` | always |
| F2 | New data access = Dapper repository behind interface, `CancellationToken`; no `SqlHelper` | partly; `ARCHITECTURE.md` says the opposite | ADR 0007, `SqlHelper.cs` | always |
| F3 | "Now" = `IClock.UtcNow` | yes, if the agent reads `InvoiceService` | `IClock.cs`, `ConventionTests.cs` | always (cheap, high cost of error) |
| F4 | Every `V###` needs a matching `U###` | no — 2 of 4 existing scripts have none | `V003` header comment | always |
| F5 | Money = `decimal(19,4)` | yes, from `V001` and `Invoice.cs` | `V001__create_invoice.sql` | on demand (path rule) |
| F6 | Never edit a merged `V###` | no | `history-excerpt.txt` | always |
| F7 | Overdue = `IsOverdue` (Issued and past due, UTC) | yes | `InvoiceService.cs` | never (code says it) |
| F8 | `IX_Invoice_CustomerId_Status` exists | yes, if `V003` is read | `V003` | on demand (path rule) |
| F9 | `MonthlyRevenueReport` migration belongs to BILL-97 | yes, code comment + ADR | `SqlHelper.cs`, ADR 0007 | generate (topology map) |
| F10 | Convention test fails → fix the code | partly (test comment) | `ConventionTests.cs` | always |

## 2. Every block of the old layer

| Block (file:lines) | Pathology | Decision | Why |
|---|---|---|---|
| `CLAUDE.md` persona, company history, values | irrelevant, generic | delete | nothing the agent can act on |
| `CLAUDE.md` IMPORTANT rules + Final reminders | redundant, generic | delete | 11 "IMPORTANT"s cancel each other out |
| `CLAUDE.md` Build (`Billing.sln`, `Billing.UnitTests`) | stale, contradicts CI section | replace with F1 | lint + `ls` show the files do not exist |
| `CLAUDE.md` Data access ×2, `sql.md` SqlHelper lines, `architecture-full.md` §2.3 | stale, contradictory | replace with F2 | ADR 0007, `[Obsolete]`, convention test |
| `CLAUDE.md` Dates, `architecture-full.md` §4, `sql.md` "datetime local" | stale | replace with F3 | V003 moved to `datetimeoffset` UTC |
| `sql.md` U### line (line 24 of 36) | buried critical fact | keep, move to root | non-inferable; T04 fails without it |
| `team-handbook.md` "never edit merged V###" | critical fact in the wrong file | keep, move to root | non-inferable; T06 |
| Front-end, Deployment, Python, MongoDB, Kubernetes | irrelevant to this repo | delete | none of it exists here (audit: 35 leads) |
| `coding-standards.md` (205 lines) | inferable / generic | delete | the code shows the style; a formatter enforces it |
| Git workflow, PR rules, rituals, on-call, coffee | irrelevant to code tasks | delete (lives in the handbook for humans) | no task needs it |
| Glossary, domain section | redundant with code | delete | `InvoiceStatus`, `IsOverdue` say it |
| "Things the agent got wrong before" | mostly generic | delete; convert live ones to tests | a changelog is not a rule |
| `AGENTS.md` (not loaded by Claude, diverges from CLAUDE.md) | contradiction across tools | replace by the new canonical `AGENTS.md`; `CLAUDE.md` imports it | one source |

## 3. Result

| | Before | After | Change |
|---|---|---|---|
| Always-loaded lines | 807 | 34 | −96% (≥ 70% required) |
| Always-loaded tokens (o200k) | 7,028 | 531 | −92% |
| Pass rate on tasks-v0 (10 × 3) | your run | your run | must not drop |
| Mean input tokens per run | your run | your run | |
| Cost per passing answer | your run | your run | |

Record your own measured numbers here; do not copy numbers from the lesson.
