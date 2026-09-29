# MCP security checklist

Fill this in for every MCP server before it goes into a shared `.mcp.json`, and again when its version, token or toolsets change. One copy per server, kept next to the configuration (for example `docs/mcp-security.md`) and reviewed by the AI-layer owners. Introduced in [08.1 · MCP architecture, authentication and authorization](../lessons/module-08/lesson-01.md); used in [08.2](../lessons/module-08/lesson-02.md) and [08.3](../lessons/module-08/lesson-03.md).

## 1. Why this server at all?

| Question | Answer |
|---|---|
| What task needs it (ticket or workflow, not "it would be nice")? | … |
| Would a CLI the agent already has (`gh`, `sqlcmd`, `az`) or a script do the same job? | yes / no, because … |
| Which hosts must use it (Claude Code, Cursor, Copilot, CI)? More than one favours MCP. | … |

Decision: MCP server · CLI with permission rules · direct API call in code · nothing. Reason in one sentence: …

## 2. Identity and credentials

| Item | Value |
|---|---|
| Whose identity does the server act as? (a person, a bot account, a service principal) | … |
| Transport | stdio (credentials from the environment) · Streamable HTTP (OAuth 2.1 or token header) |
| Credential type and scope (e.g. fine-grained PAT: one repository, Issues read-only) | … |
| Where the secret lives (never literal in `.mcp.json`; `${ENV_VAR}`, OS keychain, OAuth store) | … |
| Expiry and rotation | … |
| For HTTP: does the server validate that tokens were issued for it (audience), and never pass a client's token through to another API? | yes / no / unknown |

## 3. Tool surface

Run `McpCheck tools <tools.json>` on a saved `tools/list` response and paste the summary line.

| Tool | Read / write / destructive | Open-ended input? (sql, jql, command) | Needed for the task? | Host rule (allow / ask / deny) |
|---|---|---|---|---|
| … | … | … | … | … |

- [ ] Server-side read-only mode or toolset limit is on where the server offers one.
- [ ] Every write tool is either denied or set to `ask`; no destructive tool is on `allow`.
- [ ] Annotations (`readOnlyHint`, `destructiveHint`) were checked against names and descriptions; they are hints from the server, not guarantees.
- [ ] Tool-definition cost measured (`McpCheck tokens`): … tokens; deferred loading on or off: …

## 4. Data and freshness

| Question | Answer |
|---|---|
| What data can reach the model through this server (and therefore the provider)? | … |
| Can any of it be written by people outside the team (tickets, comments, docs)? That is untrusted input; see Module 9. | … |
| How fresh is it, and does every answer say so (source, version, age)? | … |
| What happens when the source is stale or down: error the model must handle, or silent old data? | … |

## 5. Supply chain

- [ ] Package or image pinned to a version or digest (`McpCheck config` passes).
- [ ] Source reviewed or publisher trusted; who approves upgrades: …
- [ ] Runs with the least OS privileges practical (container, no home-directory mounts it does not need).

## 6. Observability and exit

- [ ] Every call is logged by an audit hook with secrets redacted (08.4); log location and retention: …
- [ ] Eval tasks that exercise this server exist (task ids): …
- [ ] How to disable it in one change (remove from `.mcp.json`, revoke the token): …

## 7. Residual risk

What can still go wrong with everything above in place, and who accepted it: …
