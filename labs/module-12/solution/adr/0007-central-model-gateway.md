# ADR-0007: Central model gateway for all agent traffic

Status: Accepted
Date: 2026-09-05
Topic: model-gateway
Deciders: Head of Platform Engineering, CISO delegate
Consulted: SRE, Finance, team leads
Supersedes: ADR-0002
Answers: Q03, Q09, Q13, Q14, Q21, Q24, Q29

## Context

ADR-0002 let pilot teams call providers directly. At 400 developers that leaves no single place to hold provider credentials, enforce quotas, attribute spend, log calls, or switch provider. Two routes (ADR-0001) must be managed as one.

## Options considered

### Option A — Keep direct access (ADR-0002)
No new infrastructure. Every control above must be rebuilt per team, and a provider switch means touching 400 machines.

### Option B — Cloud provider's own API-management gateway
Managed, integrates with that cloud's identity and monitoring. Ties the fallback route to the same cloud's product roadmap.

### Option C — Self-hosted open-source gateway exposing one Anthropic-format endpoint
Provider-neutral, per-key budgets and rate limits, runs in our EU cluster. We operate and patch it, and must keep it current as agent clients add features.

## Decision

We will run Option C in our EU Kubernetes cluster, behind private networking, as the only component allowed to reach model providers. Clients get `ANTHROPIC_BASE_URL` and an `apiKeyHelper` from managed settings. Provider hosts are blocked at the egress proxy for all other subnets.

## Consequences

- Positive: one place for credentials, quotas, redaction, audit, routing and fallback; a provider switch is a gateway config change.
- Negative: the gateway is now critical infrastructure (on-call, capacity, upgrades); if it lags client features, those features break.
- Risk: a gateway outage stops every agent; run two replicas per zone and document a break-glass route.

## Confirmation

`ArchCheck lint --rules NET` passes (no direct provider access). The egress proxy report shows zero connections to provider hosts from non-gateway subnets each week. The gateway is upgraded within 30 days of a client release that needs it.
