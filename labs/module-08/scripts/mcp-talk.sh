#!/usr/bin/env bash
# Talk to a stdio MCP server by hand (lesson 08.1): sends each line of a .jsonl file, keeps stdin open
# long enough for the answers, prints one JSON response per line. Server logs (stderr) go to mcp-talk.log.
#   bash scripts/mcp-talk.sh samples/handshake.jsonl dotnet server/ContosoSchema.Mcp/bin/Release/net8.0/ContosoSchema.Mcp.dll --source snapshot:samples/catalog-v006.json
set -euo pipefail
file=$1; shift
{ cat "$file"; sleep "${MCP_TALK_WAIT:-3}"; } | "$@" 2>mcp-talk.log
