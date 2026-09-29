# Module 8 breaks

| Lesson | Folder | What is broken | How you see it |
|---|---|---|---|
| 08.1 | `08.1-overbroad/` | `.mcp.json` with literal secrets, an unpinned package and image, all toolsets, an `sa` connection string; a `contoso-ops` server whose tools include `run_sql`, `jira_update_issue` and a "read-only" `cleanup_invoices` that deletes | `McpCheck config` (7 problems), `McpCheck tools --expect read-only` (3 write tools) |
| 08.2 | `../integration/break-write-scope/` | GitHub integration with a classic `repo` token and default toolsets; "tidy up" closes a ticket that was not finished | `incident.md`; `McpCheck tools samples/github-tools-default.json` (7 write tools, 2 destructive) |
| 08.3 | `08.3-stale-schema/` | The schema server reads a 41-day-old nightly snapshot with no migrations folder, so freshness is `UNKNOWN`; the agent writes BILL-161 against the dropped `dbo.Invoice.Notes` | `transcript-excerpt.md`; unit tests pass; `check-sql` against the live database finds `unknown column Notes` |
| 08.4 | `08.4-fail-open/` | `guard-v0.sh` reads only `new_string`; on a `Write` it exits 1 (a non-blocking error), so the new `BillingDbContext.cs` is written | run the v0 script on `hooks/payloads/write-dbcontext.deny.json`: exit 1, not 2 |
