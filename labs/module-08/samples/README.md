# Samples (Module 8)

| File | What it is | Real or illustrative |
|---|---|---|
| `catalog-v006.json` | Snapshot of the lab database at V006 (current), written by `ContosoSchema.Mcp snapshot --source sql`; capture time set to 2026-09-28 06:00 UTC | real output, date fixed |
| `catalog-nightly.json` | Snapshot of the lab database at V005 (`MIGRATE_TO=5`), capture time set to 2026-08-18 02:00 UTC: the "nightly" file whose refresh job stopped 41 days ago | real output, date fixed |
| `handshake.jsonl` | Three JSON-RPC requests in the 2026-07-28 protocol revision: `server/discover`, `tools/list`, `tools/call` | real protocol messages |
| `handshake-2025-11-25.jsonl` | The same conversation in the older revision (`initialize` handshake first) | real protocol messages |
| `contoso-schema-tools.json` | The `tools/list` response of `ContosoSchema.Mcp` 1.1.0 (four read-only tools) | real output |
| `github-tools-default.json` | A `tools/list` response shaped like the GitHub MCP server's issue, pull-request and file tools with default toolsets, abbreviated to 14 tools | **illustrative** (names from the server's README as of 2026-09; descriptions and schemas shortened) |
| `github-tools-readonly.json` | The read tools from the same list, as seen with read-only mode on | **illustrative** |

Refresh the two catalogs yourself with the lab database (see the lab README): the only thing that changes is the capture time.
