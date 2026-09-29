# concepts.md — first draft (deliberately flawed)

> **Break for lesson 14.2.** A student's first concepts file, written the evening after a good week. Run `MethodCheck trace break/14.2-anecdote/concepts.md` and fix every error before comparing with `solution/concepts.md`.

## C1 · Shadow rule
- Status: supported
- Named: 2026-03-01
- Plain words: When the agent cannot see a business rule that already exists, it writes a second copy with a slightly different meaning.
- Claim: On tickets that touch an existing business term, a run without a reuse search produces a second implementation of the term.
- Boundary: Terms with exactly one implementation already in the agent's context; greenfield code.
- Evidence:
  - 2026-01-12 · INC-01 · own overdue check ([NOTES](../../evidence/NOTES-contoso.md))
  - 2026-02-25 · INC-08 · duplicate balance calculation ([NOTES](../../evidence/NOTES-contoso.md))
  - 2026-03-25 · INC-12 · VAT rounding re-implemented ([NOTES](../../evidence/NOTES-contoso.md))

## C2 · The Research Brief Speed Multiplier
- Status: supported
- Named: 2026-01-05
- Plain words: Front-loading a research subagent so the context window holds the brief makes the LLM 3x faster on any ticket.
- Claim: Writing a research brief first makes tickets 3x faster.
- Boundary: none
- Evidence:
  - 2026-02-16 · BILL-157 · loop 35 min vs paste-and-go 110 min ([NOTES](../../evidence/NOTES-contoso.md))

## C3 · Agent amnesia
- Status: hypothesis
- Named: 2026-02-05
- Plain words: After a reset, the agent forgets what the team agreed in the conversation.
- Claim: Anything that lives only in the chat is lost at a reset and silently replaced by a guess.
- Boundary: Decisions written to a file the next step reads.
- Evidence:
  - 2026-02-04 · INC-05 · "issued" redefined after `/clear` ([evidence](../../evidence/experiments.md))
