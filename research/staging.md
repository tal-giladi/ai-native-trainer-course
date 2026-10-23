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

### C-20261023-01 · GitLab Duo AI-agent instruction-injection → command execution as another user (CVE-2026-96893)

- **Class:** A
- **Kind:** new-topic
- **Date discovered:** 2026-10-23
- **Date published:** 2026-10-13
- **Source:** https://raw.githubusercontent.com/CVEProject/cvelistV5/main/cves/2026/96xxx/CVE-2026-96893.json (primary CVE record, fetched this session; GitLab's own advisory/patch-release pages at docs.gitlab.com/about.gitlab.com could not be reached from this environment — DNS failures on every gitlab.com subdomain tried, unlike github.com which resolved fine in the same session)
- **Organization/researchers:** GitLab (vendor-disclosed via its own security-release process; CWE-1427 "Improper Neutralization of Input Used for LLM Prompting")
- **Category:** agent-security
- **What changed:** GitLab EE has a flaw in how Duo (GitLab's AI coding/chat agent) builds AI conversation context. Under certain conditions an authenticated user can inject crafted content into the agent's instructions, which can trigger command execution attributed to a *different* user — i.e. one user's injected text makes the agent act with another user's authority.
- **Technical summary:** CVSS 3.1 7.3 High, `AV:N/AC:L/PR:L/UI:R/S:U/C:H/I:H/A:N`. Affects GitLab EE 19.1 before 19.2.8, 19.3 before 19.3.5, 19.4 before 19.4.2; fixed in 19.2.8 / 19.3.5 / 19.4.2 (GitLab.com already patched). No KEV listing or public PoC found.
- **Why it might matter:** It is a concrete, vendor-confirmed instance of exactly the pattern module-09/lesson-01 already teaches generically (untrusted text reaching an agent that then acts with someone else's authority/privilege — the identity/trust-boundary half of the lethal-trifecta model) but in a second major, real-world coding-agent product (GitLab Duo) rather than Claude Code, GitHub Copilot or the GitHub MCP server, which are the only products the lesson currently cites (Invariant Labs' GitHub MCP case, line 31; Claude Code CVE-2026-103435/CVE-2026-54316 in 09.5 per `references/research-log.md` rows 162–163). Widens the "this happens to every vendor, not just one" point the lesson is making.
- **Evidence of adoption:** N/A (vulnerability disclosure, not a product claim)
- **Major organizations using it:** GitLab Duo ships to all GitLab.com and self-managed EE customers with Duo enabled; GitLab is one of the two dominant Git hosting/CI platforms this course's audience is likely to use.
- **Open-source implementation:** GitLab EE is source-available; fix landed in the 19.2.8/19.3.5/19.4.2 patch releases (gitlab.com/gitlab-org/gitlab).
- **Paper:** none
- **Code:** https://gitlab.com/gitlab-org/gitlab/-/work_items/607968 (referenced by the CVE record; not independently fetched — gitlab.com unreachable from this session)
- **Relationship to existing course material:** lessons/module-09/lesson-01.md (trust boundaries, identity, lethal trifecta/Rule-of-Two, OWASP LLM01/ASI01 mapping — cites the Invariant Labs GitHub MCP case as its one worked real-world example); lessons/module-09/lesson-05.md (security advisories as case studies, per research-log rows 162–163). `lookup "GitLab"` / `"GitLab Duo"` / `"CVE-2026-96893"` / `"command execution attributed"` all returned no match — new to the registry.
- **Potential course lesson:** Not a new lesson. Candidate for a one- or two-sentence addition to module-09/lesson-01.md's real-world-example paragraph (around line 31, alongside the Invariant Labs GitHub MCP case) naming this as a second, different-vendor instance of the same identity/trust-boundary failure, and/or a line in lesson-05's advisory case-study material.
- **Confidence:** medium-high (CVE record read directly from the official CVE List V5 GitHub mirror, which mirrors cve.org; GitLab's own advisory page could not be independently fetched this session due to DNS resolution failures on gitlab.com domains)
- **Recommendation:** monitor for the next weekly review; likely a small FIX/addition to lessons/module-09/lesson-01.md and/or lesson-05.md rather than a new lesson.
