# ADR-0004: Log all prompts and responses at the gateway

Status: Accepted
Date: 2026-08-20
Topic: prompt-logging
Deciders: Platform team
Answers: Q21

## Context

We need to debug the gateway and show auditors what the agents did.

## Options considered

### Option A — Log everything
Every prompt and response in full, 365 days, in the platform team's log workspace.

## Decision

We will log everything (Option A). Storage is cheap and more data is always better for debugging.

## Consequences

- Positive: complete record of every interaction.
- Positive: easy to search.

## Confirmation

