# Instructor notes — 08.1 MCP architecture, authentication and authorization

**Teaching objective.** Students can draw host, clients and servers for a real setup, say where each credential enters (environment for stdio, OAuth 2.1 for HTTP), and reason about power as the intersection of credential scope, server tool surface and host rules, putting least privilege at the lowest layer first.

**Likely confusion.** "The MCP server" versus "the API". Students say "the Jira MCP can only read" when they mean the token can only read, or the reverse. Make them name the layer every time they state a permission. Second confusion: the two protocol revisions. Show both handshakes side by side; the concepts (tools, results, errors) did not change, the lifecycle did.

**Common misconception.** "MCP handles security; it has OAuth." The protocol standardizes how an HTTP client obtains a token for a server. It does not decide what the token may do, whether the server's tools are narrow, or whether returned data is true or safe. Stdio servers do not use it at all.

**Key analogy.** A contractor badge. The credential is what the badge opens (every door in the building, or one room). The server is the toolbox the contractor brings (a screwdriver, or a sledgehammer labelled "screwdriver"). The host rules are the site manager who says which tools may be used today. The badge matters most, because it works even when the site manager is not there.

**Common failure in the exercise.** Redesigns that only add deny rules in `.claude/settings.json` and keep the classic token and the `sa` login. Ask: "the nightly script uses the same token; what stops it?" Second: students trust `McpCheck` as proof. Point at `cleanup_invoices`: the scanner only caught it because the description happened to begin with "Deletes".

**Expected exercise outcome.** Both handshakes run and the student can point to annotations, `serverInfo` and `isError`; token numbers close to 394 (schema) and 2,900 (GitHub sample) with $T_{\text{tools}}$ over 30 requests; a checklist per server with a three-layer redesign table; the break diagnosis lists the seven config problems plus the lying annotation and the missing host rules.

**Extension exercise.** Run `claude mcp add` for the schema server in local scope, then use `/mcp` and `/context` to find what Claude Code actually loaded (tool names deferred or not). Compare with the `McpCheck tokens` estimate and explain the gap (tokenizer, tool search).

**Discussion question.** A vendor offers a hosted MCP server that "wraps our entire REST API, 180 tools". What would you ask about its credential model, its tool surface and its context cost before letting a team install it?
