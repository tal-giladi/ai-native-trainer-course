---
name: prime
description: >-
  Researches one Contoso Billing ticket before any code is written and saves a research brief to
  research/BILL-###.md: code to reuse, architecture constraints, blast radius and open questions,
  each with a path. Use when starting work on a BILL-### ticket, or when the user asks to prime,
  research or load context for a ticket. Not for one-sentence fixes. Never edits code.
argument-hint: "[ticket-id]"
context: fork
agent: Explore
metadata:
  version: "1.0.0"
  owner: "@contoso/billing-leads"
---

# Prime a ticket

## Inputs

- `$0`: ticket id, for example `BILL-154`. The ticket must exist at `tickets/$0.md`. If it does not, say so and stop.

## Output contract

- A brief in the shape of `templates/research-brief.md`, at most 80 non-empty lines, returned to the main conversation, which saves it to `research/$0.md`.
- Every reuse and constraint row cites a backticked path that exists.
- Open questions are listed, not answered by assumption.
- `SkillCheck contract` passes with `${CLAUDE_SKILL_DIR}/contract.rules`.

## Steps

1. Read `tickets/$0.md`. Write down its nouns and verbs; they are the search terms.
2. Search `src/` and `tests/` for every search term. Read the classes you find and their tests.
3. Find the most recent class of the same kind (the exemplar) and the callers of anything that will change.
4. Return the brief in the shape of `templates/research-brief.md`.
