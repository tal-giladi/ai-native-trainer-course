# MCP TypeScript SDK: shared server/transport instance routed messages across clients

- **Topic ID:** mcp-ts-sdk-shared-transport-routing
- **Status:** REJECT
- **Next review:** on new evidence
- **Course change:** none
- **Candidates:** C-20261105-02

## Summary

In `@modelcontextprotocol/server` 2.0.0–2.2.0, an app that shared one stateless HTTP transport or one server instance across HTTP clients could deliver one client's responses or notifications to another, because routing used only the JSON-RPC request id (CVE-2026-111770, GHSA-jm63-cq47-j5jp, fixed 2.3.0, which refuses to be shared).

## Evidence

- https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-jm63-cq47-j5jp — primary advisory, read by the daily run (2026-11-05).
- The course's labs use the MCP C# SDK over stdio; its advisories page (checked 2026-11-08) has no analogous shared-instance advisory.

## What would change the decision

An analogous flaw in the C# SDK or another SDK the course uses, or MCP spec / security-best-practices guidance that names per-client server instances as a rule — then a "common mistakes building an MCP server" line in 08.3.

## History

- 2026-11-08 — REJECT — an implementation bug in an SDK the course does not use, fixed by the SDK refusing the pattern; no course claim affected — weekly/2026-W45.md — candidates C-20261105-02
