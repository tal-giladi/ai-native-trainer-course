# Prompt-injection CVEs in other vendors' coding agents (case-study pool)

- **Topic ID:** cross-vendor-coding-agent-injection-cves
- **Status:** WAIT
- **Next review:** 2026-11-22
- **Course change:** none
- **Candidates:** C-20261023-01, C-20261025-01

## Summary

October 2026 advisories show patterns module 09 already teaches appearing in non-Anthropic coding agents: GitLab Duo (CVE-2026-96893, injected content in the conversation context triggers command execution attributed to another user — the identity half of 09.1) and Visual Studio / SSMS Copilot (CVE-2026-103572, CVSS 8.8, prompt injection makes Copilot write outside its scope — reportedly MCP and settings files — leading to code execution, the self-escalation pattern of 09.4 attack A03). Related older items in the same pool: Context7 MCP (CVE-2026-75130), Semantic Kernel (CVE-2026-25592/26030), Copilot CVE-2025-53773.

## Evidence

- https://raw.githubusercontent.com/CVEProject/cvelistV5/main/cves/2026/96xxx/CVE-2026-96893.json — CVE record (daily 2026-10-23); GitLab's own advisory unreachable from research sessions.
- https://msrc.microsoft.com/csaf/advisories/2026/msrc_cve-2026-103572.json — MSRC CSAF record (daily 2026-10-25): CWE-94, CVSS 8.8, fixed VS 17.14.42 / 18.10.4.
- https://securityonline.info/cve-2026-103572-visual-studio-copilot-rce-disclosed/, https://cybersecuritynews.com/github-copilot-rce-vulnerability, https://toolnavs.com/article/2557-copilot-flaw-cve-2026-103572-one-trailing-dot-bypasses-validation-visual-studio — secondary coverage (trailing-dot path bypass in `Microsoft.VisualStudio.Copilot.Core.dll`, researcher Johann Rehberger); not confirmed against primary text.

## What would change the decision

A primary technical write-up (vendor or researcher) confirming each mechanism. Then add one sentence to 09.4 (agent editing its own config, citing the VS Copilot case beside lab attack A03) — most relevant to this course's .NET audience — and optionally a vendor-diversity line to 09.1. No new lesson: the patterns are already taught.

## History

- 2026-10-25 — WAIT — real, patched cases of already-taught patterns; mechanisms rest on secondary coverage (VS Copilot) or an unreachable vendor page (GitLab Duo); revisit for a one-sentence 09.4 example — weekly/2026-W43.md — candidates C-20261023-01, C-20261025-01
