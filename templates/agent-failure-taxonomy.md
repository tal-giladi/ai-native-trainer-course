# Agent failure taxonomy — classification sheet

Introduced in [lesson 02.5](../lessons/module-02/lesson-05.md). Use it on any agent transcript: your own, a teammate's, a client's. Classify every failure twice — the **primary cause** (the earliest point where a correct system would have diverged) and the **escape cause** (why nothing caught it).

## The ten classes

| # | Class | Symptom in the output | Evidence to look for in the trace | First fix to try |
|---|---|---|---|---|
| 1 | Hallucination | Asserts an API, column, package, file or fact that does not exist | The claim has no source anywhere in the trace | Give it a tool or source; require citations; allow "I don't know" |
| 2 | Incorrect reasoning | Right facts, wrong conclusion | Facts in the trace are correct; the inference from them is not | Decompose the task; tests that exercise the logic; more reasoning effort |
| 3 | Incomplete reasoning | Handles part of the problem (one caller of two, happy path only) | Search or analysis stopped before it was exhaustive | Explicit completeness criteria ("find every caller") and a checklist |
| 4 | Stale context | Uses a fact that used to be true | The fact is traceable to an outdated source in context | Remove or archive the source; add freshness metadata |
| 5 | Insufficient context | Fills a gap with a plausible guess | The needed fact appears nowhere in the trace | Add the fact or a way to retrieve it |
| 6 | Conflicting context | Follows one of two disagreeing sources — the wrong one | Two sources in context disagree | Remove the contradiction at its source |
| 7 | Tool failure | Wrong result after a tool errored or returned bad data | `is_error`, empty or wrong tool output | Fix the tool; harness error handling; freshness checks |
| 8 | Instruction failure | Ignores or misapplies a clear instruction that was in context | The instruction is present and unambiguous | Specificity, scope, rationale; enforce invariants with a gate |
| 9 | Planning failure | Wrong approach from the start (wrong layer, duplicate service, wrong pattern) | The first plan or first edits already point the wrong way | Research and plan phases with a human checkpoint |
| 10 | Verification failure | Claims success it did not check, or skips an available check | No test/build/lint call, or a claim with no tool evidence | Validation gates that run whether or not the agent chooses to |

## Log

| Date | Task / ticket | Agent + model + version | What went wrong (one line) | Primary class | Escape class | Evidence (quoted trace line) | Fix applied | Rerun result |
|---|---|---|---|---|---|---|---|---|
| | | | | | | | | |

## Rules of thumb

- "Hallucination" is usually a symptom. Ask whether the fact was in context (if not: insufficient context) or in context in two versions (stale / conflicting) before settling on class 1.
- If a check existed and was not run, the escape class is 10 even when the primary is something else.
- One line of quoted evidence per classification, or the classification does not count.
