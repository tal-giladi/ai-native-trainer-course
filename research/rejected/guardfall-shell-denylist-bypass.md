# GuardFall: command-text denylists bypassed by shell rewriting

- **Topic ID:** guardfall-shell-denylist-bypass
- **Status:** REJECT
- **Next review:** on new evidence
- **Course change:** none
- **Candidates:** C-20261010-01

## Summary

Adversa AI (2026-06-30) reported that pattern-based shell guards in 10 of 11 popular open-source coding agents can be bypassed because the guard inspects the raw command string while the shell rewrites it (e.g. `r''m` runs `rm`) before execution.

## Evidence

- https://adversa.ai/blog/opensource-ai-coding-agents-shell-injection-vulnerability/ — primary; not reachable from this session (DNS failure 2026-10-11).
- https://thehackernews.com/2026/06/guardfall-exposes-open-source-ai-coding.html, https://securityaffairs.com/194546/ai/guardfall-flaw-hits-10-of-11-popular-open-source-ai-agents.html — consistent secondary coverage.

## What would change the decision

Not a new topic: 09.5 already teaches exactly this ("a Bash rule matches the command text as written ... the sandbox is the boundary") with equivalent bypass shapes. Reopen only if 09.5's examples are rewritten and a primary-verified multi-vendor case would replace a hypothetical, or if the bypass class is shown to defeat OS-level sandboxes (which would change what 09.5 teaches).

## History

- 2026-10-11 — REJECT — confirms an existing 09.5 claim without changing it; primary report unreachable — weekly/2026-W41.md — candidates C-20261010-01
