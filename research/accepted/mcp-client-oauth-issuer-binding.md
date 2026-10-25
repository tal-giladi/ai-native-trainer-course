# MCP clients must bind OAuth credentials to their issuer

- **Topic ID:** mcp-client-oauth-issuer-binding
- **Status:** ADD
- **Next review:** n/a
- **Course change:** lessons/module-08/lesson-01.md ("Where credentials enter": one bullet on the client side of credential binding, one source; no new lesson, quiz unchanged)
- **Candidates:** C-20261020-01

## Summary

In MCP's OAuth flow the server's Protected Resource Metadata names the authorization server, so a malicious or compromised MCP server can name its own. A client that does not bind each stored credential to the authorization server that issued it will hand over refresh tokens or client secrets from earlier legitimate sign-ins without user interaction. The official MCP TypeScript SDK had this flaw (CVE-2026-104850); the durable lesson is the client-side mirror of the server-side "no token passthrough" rule 08.1 already teaches.

## Evidence

- https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-6qxp-vccf-f47h — primary advisory, fetched 2026-10-25: CVE-2026-104850, CVSS 3.1 7.5, sdk 1.12.0–1.30.1 affected, fixed sdk 1.31.0 and client/core 2.2.0; remediation needs `issuer` on saved credentials and `expectedIssuer` on bundled providers; new interactive sign-ins not covered by the fix.
- Independent advisory databases carry the same record: https://advisories.gitlab.com/npm/@modelcontextprotocol/sdk/CVE-2026-104850/, https://app.opencve.io/cve/CVE-2026-104850, https://www.mend.io/vulnerability-database/CVE-2026-104850/ (found via search, not fetched).
- modelcontextprotocol.io (spec authorization page) was DNS-unreachable from the 2026-10-25 session, so the spec's own wording on issuer checks was not re-read.

## What would change the decision

If a later MCP spec revision makes issuer binding a client MUST, cite the spec instead of the advisory in 08.1. If the official C# SDK (used in module 08 labs) publishes a similar advisory, add it to the same bullet.

## History

- 2026-10-25 — ADD — primary, patched CVE on the reference SDK shows a client-side credential-binding rule 08.1 did not state; added as one bullet in 08.1, not a new lesson — weekly/2026-W43.md — candidates C-20261020-01
