# AI layer changelog

One entry per change to AGENTS.md, CLAUDE.md, `.claude/`, `.cursor/rules/`, Copilot instructions or `.mcp.json`.
Each entry says what changed, the incident or evidence that triggered it, and how it was verified.

## 2026-09-28 — v1.1.0

- **Changed:** data-access rule now points to Dapper repositories; `SqlHelper` forbidden outside `Legacy/`.
- **Why:** agent implemented BILL-142 with `SqlHelper.ExecuteDataSet` and `DataSet`, copying the 2021 rule. Convention test `Only_the_Legacy_folder_may_call_SqlHelper` failed.
- **Evidence:** `docs/adr/0007-dapper-repositories.md`, `docs/history-excerpt.txt` (BILL-66, BILL-79).
- **Verified:** `AiLayerTool lint` clean; probes P1–P4 pass 3/3 runs; BILL-142 rerun passes `dotnet test`.
- **Reviewed by:** @contoso/billing-leads

## 2026-09-20 — v1.0.0

- **Changed:** first rules file imported from the team wiki.
- **Why:** agents had no repository instructions.
- **Verified:** not verified (this is what v1.1.0 fixed).
