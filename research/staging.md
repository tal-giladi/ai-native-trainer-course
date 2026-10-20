# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261020-01 · MCP TypeScript SDK OAuth client credential-exposure vulnerability (CVE-2026-104850 / GHSA-6qxp-vccf-f47h)

- **Class:** A
- **Kind:** new-topic
- **Date discovered:** 2026-10-20
- **Date published:** 2026-09-30
- **Source:** https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-6qxp-vccf-f47h (primary GitHub security advisory, fetched this session)
- **Organization/researchers:** Reported/published via the `modelcontextprotocol/typescript-sdk` GitHub repository (credited "maxisbey")
- **Category:** mcp/tool-protocols
- **What changed:** The official MCP TypeScript SDK's OAuth client did not bind a stored credential (refresh token, client secret, or signed assertion) to the authorization server it was issued for. A malicious or compromised MCP server could advertise its own authorization server in its Protected Resource Metadata and, with zero user interaction, have the client send it a previously-stored refresh token/client secret from an earlier sign-in.
- **Technical summary:** CWE-345 (Insufficient Verification of Data Authenticity) + CWE-522 (Insufficiently Protected Credentials). Affected: `@modelcontextprotocol/sdk` (npm) 1.12.0–1.30.1; `@modelcontextprotocol/client` (npm) 2.0.0–2.1.0 in limited configurations. Fixed in `@modelcontextprotocol/sdk` 1.31.0 and `@modelcontextprotocol/client`/`@modelcontextprotocol/core` 2.2.0 — but the advisory says upgrading alone is not sufficient for some bundled providers/stored credentials; callers must also set `expectedIssuer` and add `issuer` to saved credentials. CVSS 3.1: 7.5 High (AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N).
- **Why it might matter:** It is the mirror case of the "token passthrough" / confused-deputy failure the course already teaches on the *server* side (lessons/module-08/lesson-01.md): here the vulnerable component is the *client's* OAuth implementation, exploited by a malicious/compromised server it talks to. It is a concrete, patched, primary-sourced CVE on the reference implementation of the exact OAuth 2.1 flow the course diagrams (remote GitHub/Atlassian MCP servers in 08.1/08.2), so it is good case-study material, not a reason to change any current claim (the course does not pin or name this SDK package anywhere `lookup` finds, so nothing is currently wrong).
- **Evidence of adoption:** N/A (vulnerability disclosure, not a product claim)
- **Major organizations using it:** `@modelcontextprotocol/sdk` is the reference/official TypeScript SDK maintained under the `modelcontextprotocol` GitHub org (Anthropic-stewarded MCP project); widely depended on by third-party MCP client/server implementations.
- **Open-source implementation:** https://github.com/modelcontextprotocol/typescript-sdk
- **Paper:** none
- **Code:** https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-6qxp-vccf-f47h
- **Relationship to existing course material:** lessons/module-08/lesson-01.md (OAuth 2.1 resource-server flow, RFC 9728 metadata, RFC 8707 `resource`, audience validation, "no token passthrough" — client-side credential binding is the gap this CVE fills); lessons/module-08/lesson-02.md (identity/attribution table including "developer through OAuth"); lessons/module-09 (confused-deputy framing in 09.1.instructor.md). `lookup "OAuth"` / `"confused deputy"` / `"TypeScript SDK"` / `"CVE-2026-104850"` confirm no existing lesson names this SDK or this CVE — new, not a duplicate.
- **Potential course lesson:** Not a new lesson. Candidate for a short real-world case-study addition at the next FIX-eligible opportunity: a sentence or callout in lessons/module-08/lesson-01.md's "Security best practices" bullet list (around line 97–99) naming this CVE as a concrete example of a *client-side* confused-deputy failure (the mirror of the server-side token-passthrough case already taught), and/or as a citable example in module-09's MCP security material.
- **Confidence:** high (primary GitHub advisory read directly; CVE/CVSS/version ranges confirmed)
- **Recommendation:** monitor for the next weekly review; likely a small FIX/addition to lessons/module-08/lesson-01.md (and possibly module-09) rather than a new lesson — low cost, concrete, primary-sourced example that extends (does not contradict) current teaching.
