# Module 8 labs — MCP, APIs and Hooks

Everything the Module 8 labs need. Lessons: [08.1](../../lessons/module-08/lesson-01.md) · [08.2](../../lessons/module-08/lesson-02.md) · [08.3](../../lessons/module-08/lesson-03.md) · [08.4](../../lessons/module-08/lesson-04.md).

The labs run on **Contoso Billing** from [`labs/module-03/brownfield`](../module-03/README.md), in your `ai-layer-lab` repository, with the Module 5 gates (`gates/architecture.rules`, [`labs/module-05`](../module-05/README.md)) and the Module 7 eval harness ([`labs/module-07`](../module-07/README.md)). This module adds two migrations to Contoso (V005, V006: invoice notes move to their own table) and ticket BILL-161, so there is a schema change for an agent to miss.

> [!WARNING]
> MCP servers and hooks are executable configuration that runs with **your** credentials. Use a throwaway GitHub repository (your private `ai-layer-lab`) and tokens you create for this lab only, with the narrowest scope that works; revoke them when you finish. Never point these labs at an employer's Jira, GitHub organization or database without written permission. The SQL Server container is local, bound to 127.0.0.1, and holds fictional data; its passwords are lab-only defaults.

## Requirements

- .NET SDK 8 or newer (every project sets `RollForward=Major`). Verified with SDK 10.0.400.
- Docker Desktop for the lab database (SQL Server 2022, image `mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04`, about 1.6 GB). Without Docker, every exercise has an offline path using the snapshots in `samples/`.
- Git and `bash` (Git Bash on Windows) for the scripts.
- Claude Code for the lab as written (`.mcp.json`, `.claude/settings.json` hooks and permissions). The server itself is a standard MCP server and works in any MCP host.
- For 08.2: a GitHub account and the GitHub CLI (`gh`); optionally an Atlassian Cloud site (the free tier is enough).
- NuGet packages (pinned): `ModelContextProtocol` 2.2.0, `Microsoft.Extensions.Hosting` 10.0.10, `Microsoft.Data.SqlClient` 5.2.2; tests use xunit 2.9.2. `McpCheck` and `AgentHooks` have no packages.

## Contents

| Path | What it is |
|---|---|
| `overlay/` | Two Contoso migrations and one ticket to copy over the brownfield app: V005 adds `dbo.Invoice.Notes` (2025), V006 moves notes to `dbo.InvoiceNote` with author and time, drops `Notes`, and adds `dbo.usp_InvoiceNote_LatestByCustomer` (2026-09-14); BILL-161 asks for the latest note per open invoice. |
| `db/` | `docker-compose.yml` and `init/`: SQL Server in Docker, migrations applied in order and recorded in `dbo.SchemaHistory`, a `schema_reader` login with `VIEW DEFINITION` and nothing else. `MIGRATE_TO=5` stops at V005. |
| `server/ContosoSchema.Mcp/` | The custom MCP server (08.3): four read-only tools (`list_tables`, `describe_table`, `list_procedures`, `describe_procedure`) over stdio, sources `sql` or `snapshot:<file>`, freshness metadata against `db/migrations`, and a `check-sql` command used by the 08.4 hook. |
| `server/ContosoSchema.Mcp.Tests/` | 17 tests: freshness, refusal of stale data, the SQL check, and one end-to-end test that starts the server over stdio with the SDK's client. No database needed. |
| `tools/McpCheck/` | Read-only static checks: `config` (secrets, pinning, toolsets, admin identities in `.mcp.json`), `tools` (read / write / destructive / open-ended, untrusted annotations), `tokens` (context cost of tool definitions). |
| `hooks/AgentHooks/` | Claude Code hooks (08.4): `guard` (PreToolUse, fails closed), `audit` (PostToolUse, fails open, redacts), `report`, `selftest`. |
| `hooks/payloads/` | Ten recorded hook payloads named `*.deny.json` / `*.defer.json`: the guard's test set. |
| `hooks/settings.example.json` | The three hooks wired into `.claude/settings.json`. |
| `integration/` | `mcp.json` (GitHub read-only + Atlassian + contoso-schema), `settings.permissions.json` (allow / ask / deny per MCP tool), `import-tickets.sh` (tickets → GitHub issues with `gh`), and `break-write-scope/`. |
| `evals/tasks-m8.json` | Four regex-graded tasks (T25–T28) in the Module 7 format that cover the schema server and the integration. |
| `samples/` | Two real catalog snapshots (V006 current, V005 "nightly"), protocol transcripts for both protocol revisions, illustrative GitHub tool lists. See `samples/README.md`. |
| `break/` | One break per lesson; index in `break/README.md`. |
| `solution/BILL-161/` | Reference implementation of BILL-161 on the stored procedure. |
| `scripts/mcp-talk.sh` | Send JSON-RPC lines to a stdio server by hand. |

## Setup (once)

Build and test from this folder:

```bash
dotnet test server/ContosoSchema.Mcp.Tests          # 17 passed, no database needed
dotnet build tools/McpCheck -c Release
dotnet build hooks/AgentHooks -c Release -o hooks/AgentHooks/out
dotnet hooks/AgentHooks/out/AgentHooks.dll selftest hooks/payloads --rules ../module-05/gates/architecture.rules   # 10 passed
```

Start the lab database (optional; skip it and use `--source snapshot:samples/catalog-v006.json` everywhere):

```bash
cd db && docker compose -p m8lab up -d && docker compose -p m8lab logs -f migrate   # ends with "ContosoBilling is at V006"
export CONTOSO_SCHEMA_CONN='Server=127.0.0.1,14338;Database=ContosoBilling;User Id=schema_reader;Password=Lab_only_Reader_2026!;TrustServerCertificate=True'
```

(PowerShell: `$env:CONTOSO_SCHEMA_CONN = '…'`.) Put the variable in your shell profile or a local, git-ignored env file, never in `.mcp.json`.

> [!NOTE]
> Finding while building this lab: `V004__due_not_null.sql` from Module 3 fails on a real SQL Server (Msg 5074, the index created in V003 includes `DueUtc`). Module 3 reads the scripts and never executes them, so nobody noticed. `db/init/before-V004.sql` drops the index and `after-V004.sql` recreates it. That is the argument for running migrations against a real engine in CI; keep it in mind for Module 11.

Then, in the root of `ai-layer-lab` (paths assume this folder is available as `<course>/labs/module-08`):

```bash
cp -r <course>/labs/module-08/overlay/. .                      # V005, U005, V006, U006, tickets/BILL-161.md
mkdir -p tools && cp -r <course>/labs/module-08/server/ContosoSchema.Mcp <course>/labs/module-08/tools/McpCheck <course>/labs/module-08/hooks/AgentHooks tools/
dotnet publish tools/ContosoSchema.Mcp -c Release -o .claude/mcp/contoso-schema
dotnet publish tools/AgentHooks -c Release -o .claude/hooks/bin
printf '.claude/mcp/\n.claude/hooks/bin/\n.claude/audit/\n.mcp-cache/\n' >> .gitignore
git add -A && git commit -m "Module 8: notes migrations, BILL-161, MCP and hook tools"
```

Built binaries stay out of git; the sources are reviewed like any other code (CODEOWNERS from Module 3 covers `.mcp.json`, `.claude/` and `tools/`).

## Verified results

Run from this folder (`M="dotnet tools/McpCheck/bin/Release/net8.0/McpCheck.dll"`, `S="dotnet server/ContosoSchema.Mcp/bin/Release/net8.0/ContosoSchema.Mcp.dll"`; build the server with `-c Release` first):

| Command | Result |
|---|---|
| `dotnet test server/ContosoSchema.Mcp.Tests` | 17 passed |
| `bash scripts/mcp-talk.sh samples/handshake.jsonl $S --source snapshot:samples/catalog-v006.json` | three responses: `supportedVersions ["2026-07-28"]`, four tools all `readOnlyHint: true`, the `dbo.Invoice` columns with a `[schema]` header |
| same with `samples/handshake-2025-11-25.jsonl` | `initialize` answered with `protocolVersion 2025-11-25`; same tools |
| `$M config break/08.1-overbroad/.mcp.json` | FAIL: 7 problems in 2 servers (3 literal secrets, unpinned package, unpinned image, all toolsets, `sa` identity) |
| `$M tools break/08.1-overbroad/contoso-ops-tools.json --expect read-only` | FAIL: 3 write tools (`run_sql`, `jira_update_issue`, `cleanup_invoices` annotated read-only but "Deletes") |
| `$M tools samples/github-tools-default.json` / `…-readonly.json --expect read-only` | 7 read, 7 write (2 destructive) / PASS read-only surface |
| `$M tokens samples/github-tools-default.json` | 14 tools, ~2,900 tokens of definitions |
| `$M tools samples/contoso-schema-tools.json --expect read-only` / `$M tokens …` | PASS read-only surface / 4 tools, ~394 tokens |
| `$M config integration/mcp.json` | PASS: 3 servers, GitHub read-only mode on |
| `$S info --source sql --migrations <work>/db/migrations` (database at V006) | `status=FRESH` |
| `describe_table dbo.Invoice` from `samples/catalog-nightly.json`, no `--migrations` | `status=UNKNOWN`, WARNING, and `Notes nvarchar(400) NULL` (the break) |
| same with `--migrations <work>/db/migrations` | `isError: true`, `REFUSED: the source is at V005 but the repository is at V006`, points to `db/migrations/V006__invoice_note_table.sql` |
| `$S check-sql --source sql --migrations … break/…/InvoiceNotes.cs` | `unknown column Notes (not in dbo.Invoice)`, exit 2 |
| `$S check-sql` with the nightly snapshot | refuses: `Cannot validate SQL against this schema source`, exit 2 |
| `$S check-sql` on `solution/BILL-161` + all Contoso `src/` files, fresh source | PASS: 11 files against V006 |
| `dotnet test` on Contoso + `break/08.3-stale-schema/overlay` / + `solution/BILL-161` | 7 passed / 8 passed (the broken code passes its unit tests) |
| `schema_reader`: `SELECT TOP 1 Amount FROM dbo.Invoice` | `The SELECT permission was denied on the object 'Invoice'` |
| `bash break/08.4-fail-open/guard-v0.sh < write-dbcontext payload` | exit 1 (non-blocking: the write proceeds) |
| `AgentHooks selftest hooks/payloads --rules ../module-05/gates/architecture.rules` | 10 passed, 0 failed |
| hook latency, `dotnet AgentHooks.dll guard` vs `dotnet run --project … guard` | about 0.22 s vs 3.8–5.3 s per call on the author's machine |
| `EvalHarness validate evals/tasks-m8.json` (Module 7 tool) | 0 errors (1 warning: only 4 tasks, merge them into tasks-v1) |

## Lab sequence

1. **08.1 — MCP architecture and authorization.** Talk to the server by hand over stdio in both protocol revisions; measure tool-definition cost; classify tool surfaces with `McpCheck`; fill in the [MCP security checklist](../../templates/mcp-security-checklist.md) for a Jira + GitHub integration. *Break:* `break/08.1-overbroad`.
2. **08.2 — A real integration.** Import the tickets as GitHub issues, connect the GitHub MCP server read-only with a fine-grained token on one repository, optionally Atlassian, add allow / ask / deny rules, run T28. *Break:* `integration/break-write-scope` (classic token, default toolsets, "tidy up" closes an unfinished ticket).
3. **08.3 — A custom MCP server in C#.** Build and test `ContosoSchema.Mcp`, run it against the live database with the least-privilege login, register it in `.mcp.json`, implement BILL-161 with it. *Break (module lab):* the nightly snapshot with no migrations folder; the agent codes against the dropped `Notes` column. *Fix:* freshness metadata, `--migrations`, refuse stale data.
4. **08.4 — Hooks.** Install `guard`, the `check-sql` validation hook and `audit`; run the selftest; read the audit report. *Break:* `guard-v0.sh` fails open on `Write`.

## Artifacts to commit to `ai-layer-lab`

- `.mcp.json` (GitHub read-only, contoso-schema; Atlassian if you have it), permission rules in `.claude/settings.json`, `docs/mcp-security.md` from the [MCP security checklist](../../templates/mcp-security-checklist.md)
- `tools/ContosoSchema.Mcp/` and its tests, `tools/McpCheck/`, `tools/AgentHooks/` and `hooks/payloads/` (the hooks' test set)
- `.claude/settings.json` hooks: `guard` (enforcement), `check-sql` (validation), `audit` (audit)
- BILL-161 implemented on `dbo.usp_InvoiceNote_LatestByCustomer`
- T25–T28 merged into `agent-evals/tasks/` and one harness run with results
- `AI-LAYER-CHANGELOG.md` entries and `NOTES.md` entries for both incidents (write scope, stale schema)

See the portfolio scaffold: [ai-layer-lab](../../projects/ai-layer-lab/README.md).
