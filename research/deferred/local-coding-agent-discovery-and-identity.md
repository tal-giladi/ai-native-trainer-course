# Endpoint/network discovery of local coding agents and per-agent identities (Microsoft Ignite 2026)

- **Topic ID:** local-coding-agent-discovery-and-identity
- **Status:** WAIT
- **Next review:** 2027-01-17
- **Course change:** none
- **Candidates:** C-20261121-01

## Summary

At Ignite 2026 Microsoft described Defender and Entra Global Secure Access discovering local AI agents (naming Claude Code and GitHub Copilot CLI) and their configured MCP servers, plus Entra Agent IDs for discovered local agents and Conditional Access that tells a person from an agent acting for them. Most of it is preview and detection-only for coding agents. The practice behind it, inventorying the agents and MCP servers that actually run instead of relying on policy, and giving the agent an identity distinct from the developer, fits 09.1, 12.2, 12.3 and 19.2.

## Evidence

- https://techcommunity.microsoft.com/blog/agent-365-blog/whats-new-in-microsoft-agent-365-ignite-2026/4571753, https://techcommunity.microsoft.com/blog/microsoft-entra-blog/ai-is-changing-access-see-how-microsoft-entra-helps-you-stay-ahead-/4556456, https://learn.microsoft.com/en-us/microsoft-agent-365/admin/agent-local — official Microsoft pages known only from search-result text; learn.microsoft.com did not resolve from the weekly-review session on 2026-11-22 (vendor claim, unverified).
- https://github.com/microsoft/entrabot — research prototype (MIT), fetched by the daily run; "not a production service".

## What would change the decision

The Microsoft pages read directly, with GA status for discovery of coding agents and their MCP servers (and ideally blocking), plus evidence of use outside Microsoft's own announcement, or an equivalent capability from a second vendor (which would make it a practice rather than one product). Then one paragraph in 19.2 or 12.x, marked as vendor-specific and volatile. Not a new lesson.

## History

- 2026-11-22 — WAIT — vendor announcement of mostly-preview, detection-only features; primary pages unreachable from research sessions; the underlying practice may become teachable once GA — weekly/2026-W47.md — candidates C-20261121-01
