# ADR-0006: Per-team quotas and chargeback at the gateway

Status: Accepted
Date: 2026-09-12
Topic: quotas-and-chargeback
Deciders: Head of Platform Engineering, Finance business partner
Consulted: Team leads, SRE
Answers: Q24, Q29

## Context

In August 2026, 33% of $94k model spend was on two keys with no owning team (`shared-ci` and a hackathon key never revoked). In a peak-hour replay, one runaway CI loop on the shared key throttled every team by about 8% over the hour (`ArchCheck simulate break/gateway.json`). The org limit is 7M input tokens per minute.

## Options considered

### Option A — One org-wide limit, monthly bill reviewed by finance
Current state. Noisy neighbours throttle everyone; nobody owns a third of the bill.

### Option B — Per-team keys with a TPM quota and a monthly budget; CI on separate per-team keys with smaller quotas
Quota sized at 17,500 TPM per developer (the vendor's guidance for 100–500 users is 15–20k per user), total allocation 109% of the org limit, because developers are not all active at once.

### Option C — Per-developer quotas
Finest control; 400 numbers to maintain and constant exception requests.

## Decision

We will adopt Option B. Budgets alert the team lead at 80% and stop CI keys (not interactive keys) at 100%. Interactive keys over budget alert only, because blocking a developer mid-incident costs more than the tokens. Any change to the eval-gated model version (ADR-0001) must pass the Module 7 regression gate before the route switches permanently.

## Consequences

- Positive: in the replay, the runaway loop is throttled on its own key (64% of its requests) and every other team sees 0% throttling.
- Positive: September attribution coverage is 100% (`ArchCheck chargeback solution/usage-2026-09.csv`).
- Negative: quotas need re-sizing when teams grow; a team at its limit files a request instead of just working.
- Risk: over-allocation (109%) means the org bucket can still throttle in an extreme peak; watch the org-level 429 rate.

## Confirmation

The monthly chargeback report must show attribution above 95%. A gateway dashboard alert fires on any team with more than 2% 429s over a day. Re-run the simulation when headcount changes by more than 10%.
