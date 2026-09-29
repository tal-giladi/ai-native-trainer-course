---
name: plan-feature
description: >-
  Turns a Contoso Billing ticket into an implementation plan at plans/BILL-###.md and stops for
  approval. Use after prime, when the user asks to plan a BILL-### ticket.
disable-model-invocation: true
argument-hint: "[ticket-id]"
metadata:
  version: "0.9.0"
---

# Plan a feature

## Inputs

- `$0`: ticket id. Use the research brief from earlier in this conversation.

## Output contract

- `plans/$0.md` in the shape of `templates/implementation-plan.md`.

## Steps

1. Read `tickets/$0.md` and the research from the conversation. If there is none, work from the ticket.
2. Write `plans/$0.md`.
3. Reply with the path and wait for approval.
