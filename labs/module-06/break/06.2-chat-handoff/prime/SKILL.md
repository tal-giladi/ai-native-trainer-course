---
name: prime
description: >-
  Researches one Contoso Billing ticket before any code is written: code to reuse, architecture
  constraints, blast radius and open questions. Use when starting work on a BILL-### ticket, or
  when the user asks to prime, research or load context for a ticket.
argument-hint: "[ticket-id]"
metadata:
  version: "0.9.0"
---

# Prime a ticket

## Inputs

- `$0`: ticket id. Read `tickets/$0.md`.

## Output contract

- A research brief in the shape of `templates/research-brief.md`, shown in the chat so the user can read it and the next skill can use it.

## Steps

1. Read `tickets/$0.md` and search the code for its nouns and verbs.
2. Show the brief in the chat.
