# MCP servers must check token audience; SDK defaults may not

- **Topic ID:** mcp-server-sdk-audience-validation
- **Status:** ADD
- **Next review:** n/a
- **Course change:** lessons/module-08/lesson-01.md ("Where credentials enter": one bullet on testing that the server SDK actually enforces audience, one source; no new lesson, quiz unchanged)
- **Candidates:** C-20261105-01

## Summary

The MCP authorization spec says a server MUST accept only tokens issued for itself (08.1 already teaches this). The official MCP TypeScript SDK's server bearer-auth helper had no audience check until October 2026, and the fix is opt-in, so a server built from the SDK's helper accepted tokens meant for other services sharing the same authorization server. The durable lesson: a spec MUST is not enforced just because you used the reference SDK — test it with a token for another service and expect 401. Server-side mirror of [mcp-client-oauth-issuer-binding](mcp-client-oauth-issuer-binding.md).

## Evidence

- https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-rvq5-wwqv-78pq — primary advisory, fetched 2026-11-08: CVE-2026-111771, CWE-863, CVSS 3.1 6.8; affected sdk 1.6.0–1.31.0, server 2.0.0–2.2.0, express 2.0.0–2.0.1, server-legacy 2.0.0–2.3.0; fixed sdk 1.32.0 / server 2.3.0 / express 2.0.2 / server-legacy 2.3.1; `expectedResource` off by default and needs `AuthInfo.resource` from `verifyAccessToken()`; published 2026-10-05.
- https://github.com/modelcontextprotocol/csharp-sdk/security/advisories — checked 2026-11-08: no audience-validation advisory for the C# SDK the module-08 labs use (its one advisory, GHSA-x863-g495-4cjc, is about stateful HTTP session ownership in `ModelContextProtocol.AspNetCore`, which the stdio labs do not use).

## What would change the decision

If the C# SDK publishes a similar audience advisory, add it to the same bullet. If a later spec revision or SDK release makes the check on by default, update the "off by default" sentence.

## History

- 2026-11-08 — ADD — primary, patched CVE on the reference SDK shows a spec MUST that 08.1 teaches was not enforced by default; added as one bullet with a concrete test, not a new lesson — weekly/2026-W45.md — candidates C-20261105-01
