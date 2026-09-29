# Agent security review checklist — [project]

*Template. Copy into your project. Introduced in [Module 9, lesson 09.5](../lessons/module-09/lesson-05.md). Companion: [agent threat model](agent-threat-model.md).*

> [!CAUTION]
> Run security tests only against a local lab or a work copy, never a real system.

A change to the AI layer (rules, skills, MCP config, permissions, hooks) passes review only when every
applicable box is ticked, with evidence.

## Least privilege

- [ ] The agent's tools are the minimum the task needs; no wholesale `Bash(*)` or `Edit(**)`.
- [ ] MCP servers are scoped read-only unless a write is truly required; write tools are named and justified.
- [ ] Secrets are unreadable by the agent: `Read` deny rules **and** sandbox `denyRead` on `.env`, keys, `secrets/`.
- [ ] No credential is passed as a tool argument or lands in a prompt or log.

## Trust boundary

- [ ] Untrusted content sources are enumerated (tickets, docs, notes, dependencies).
- [ ] The lethal trifecta is broken: not all of {untrusted input, private data/powerful tools, outbound channel} are reachable in one session.
- [ ] Third-party tool descriptions are pinned; a rug pull is detected (`pin`).

## Enforcement (not just prompts)

- [ ] Network egress is enforced at the OS level (sandbox allowlist), not only by `Bash(curl*)` deny rules.
- [ ] High-impact actions (push, publish, comment, settings change, package add) require approval or are denied.
- [ ] Guard hooks **fail closed** (a missing interpreter or malformed input denies, not allows).
- [ ] `.claude/**` and workflow files are CODEOWNERS-protected; the agent cannot edit its own permissions.

## Detection & audit

- [ ] A PostToolUse audit hook records every tool call to an append-only log.
- [ ] Generated code is scanned (analyzers / SAST); SQL uses parameters, not string concatenation.
- [ ] Package additions are checked against a source-mapping allowlist.

## Evidence

- [ ] Every threat has a retest: an attack that breached before the fix and is blocked after.
- [ ] The attacks run as a gated regression suite (`gate --golden-min 1.0`) in CI.
- [ ] Residual risk is written down and accepted by a named owner.
