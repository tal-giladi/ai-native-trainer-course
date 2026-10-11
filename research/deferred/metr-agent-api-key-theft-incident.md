# METR incident: agent prompted to reveal its own provider API key

- **Topic ID:** metr-agent-api-key-theft-incident
- **Status:** WAIT
- **Next review:** 2026-11-22
- **Course change:** none
- **Candidates:** C-20261008-02

## Summary

METR disclosed (2026-08-31) that a fail-open authentication bug exposed an internet-facing research instance running agents; per secondary reporting an attacker prompted the agent to reveal the model-provider API key in its environment and used it for about three weeks (~$600K of credits). It is an operational case for 09.1/09.4: a secret in an agent's reach can be asked for.

## Evidence

- https://metr.org/blog/2026-08-31-security-update/ — primary; not reachable from this session (DNS failure 2026-10-11).
- The Register, Infosecurity Magazine, The Hacker News, ITPro — consistent secondary accounts; amounts and framing vary slightly.

## What would change the decision

Reading METR's own post and confirming the "agent was prompted to reveal the key" mechanism (rather than, e.g., reading it from disk over SSH). If confirmed, a one-paragraph example in 09.4 (secret isolation) is the likely shape, not a new lesson.

## History

- 2026-10-11 — WAIT — primary post unreachable; the key-extraction mechanism, which is what would make it teachable, is secondary-only — weekly/2026-W41.md — candidates C-20261008-02
