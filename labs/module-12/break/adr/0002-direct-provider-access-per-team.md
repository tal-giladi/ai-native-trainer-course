# ADR-0002: Teams call model providers directly with their own accounts

Status: Accepted
Date: 2026-03-10
Topic: model-gateway
Deciders: Head of Platform Engineering
Consulted: Team leads
Answers: Q13

## Context

In the pilot (March 2026, three teams, 30 developers) each team needed agent access quickly. There was no platform capacity to run a shared service.

## Options considered

### Option A — Each team gets its own provider account and API key
Fast to start; each team sees its own bill.

### Option B — A shared gateway run by platform
Central keys, quotas and logs; needs a team to own it.

## Decision

We will let each pilot team use its own provider account (Option A) until the pilot ends, and revisit before any wider rollout.

## Consequences

- Positive: pilot started in a week.
- Negative: no central audit trail, no org-wide quota, keys managed by each team.
- Risk: the pattern outlives the pilot; keys spread into CI and `.env` files.

## Confirmation

Pilot review on 2026-06-30 decides whether to keep or replace this. (It was replaced by ADR-0007.)
