# ADR-0003: Per-team, short-lived credentials for agents

Status: Accepted
Date: 2026-09-04
Topic: agent-identity
Deciders: CISO delegate, Head of Platform Engineering
Consulted: Identity team, DevOps guild
Answers: Q05, Q06, Q07, Q08, Q10, Q11

## Context

The v0 design used one gateway key for all 400 developers (distributed in the onboarding `.env` template), one org-level CI secret for every repository, and one Jira service account. Security review found that one leaked key would reach all eight teams, spend could not be attributed, and the CI secret lived 365 days (`ArchCheck lint break/architecture.json --rules IDN,SEC`).

## Options considered

### Option A — Keep one key, add IP allowlisting
Cheap. Does not fix attribution or blast radius; developers work from home networks.

### Option B — One key per developer
Best attribution. 400 keys to issue, rotate and revoke; the gateway then needs a team mapping anyway for quotas.

### Option C — Humans sign in with SSO; gateway keys per team issued from the vault; CI uses workload identity federation
Developers never hold a provider key. Each team key is fetched at run time by `apiKeyHelper` from the vault with a short TTL; CI jobs exchange the platform's OIDC token for a one-hour credential scoped to the repository's team.

## Decision

We will adopt Option C. Humans: SSO with MFA through the IdP; the gateway records the user from the SSO token alongside the team key. Workloads: GitHub Actions OIDC federation, one-hour tokens, trust conditions on repository and environment. MCP writes go through per-team bot identities, reads through each user's own OAuth grant.

## Consequences

- Positive: a leaked team key reaches one team and expires within 30 days; CI holds no stored secret.
- Positive: every call is attributable to a team and, for interactive use, a person.
- Negative: the vault and the IdP become dependencies of every agent session; an outage of either stops agents.
- Cost: platform effort to run `apiKeyHelper` distribution through managed settings (about two weeks).

## Confirmation

`ArchCheck lint --rules IDN,SEC` passes. Monthly: `ArchCheck chargeback` shows attribution coverage above 95%. Offboarding test each quarter: a revoked user's session fails within 5 minutes (the `apiKeyHelper` refresh interval).
