# Ground-Bound-Build-Prove — a method for agent work on legacy .NET

> **Reference (illustrative).** The fix for lesson 14.3: the same changes as the flawed 1.0.1, released honestly as 2.0.0. Check: `MethodCheck diff solution/method-v1.0.0.md solution/method-v2.0.0.md`.

Version: 2.0.0
Status: in use on one team; one controlled comparison (EXP-01).

## Problem

On a ten-year-old codebase, an agent that only sees the ticket produces changes that compile, pass the tests it wrote and are still wrong: it re-invents rules that exist, ignores decisions it cannot see, and changes things nobody asked for. This method is for teams maintaining a legacy .NET and SQL Server system who already use a coding agent every day and want fewer of those changes to reach review.

It is not for greenfield prototypes, one-line fixes, or teams without a test suite they trust.

## Loop

1. **Ground** — before any plan, find what already exists (the rule, the procedure, the exemplar class), the decisions that constrain the change, and what depends on what will change. Output: a written brief with a source for every line. Why: [C1 Shadow rule](concepts.md).
2. **Bound** — write the plan with an explicit list of what will not change and the checks that will prove each acceptance criterion. Output: a plan a reviewer can approve in five minutes. Why: INC-03, INC-06 in the notes.
3. **Build** — implement one plan step at a time, in a fresh working session that reads the brief and the plan from files. Output: a diff limited to the plan's files.
4. **Prove** — run checks the agent did not write and cannot edit: architecture rules, scope, a human-reviewed test per criterion, the existing suite with a rising test count. Output: evidence a stranger can rerun. Why: [C2 Self-graded green](concepts.md).
5. **Harden** — turn every incident found in Prove or later into a check that fails when the fix goes missing. Output: a new gate or eval task per incident. Why: C4 (retired into this step), INC-09, INC-13.

Skip Ground and Bound when the whole change can be described in one sentence and touches no business term (INC-14).

## Concepts

- **C1 · Shadow rule** (supported) — the agent writes a second copy of a rule it cannot see.
- **C2 · Self-graded green** (supported) — "all tests pass" where the agent wrote all the tests.
- **C3 · Late-PR illusion** (supported) — review looks much faster because PRs open later with more done.
- **C4 · Unguarded rule** (retired) — merged into the Harden step in 2.0.0; the name is not reused.
- **C5 · Unread rule** (hypothesis) — a rule deep in a long root rules file that the agent stops following.

Full cards with evidence: [concepts.md](concepts.md).

## Evidence

- Fourteen diagnosed incidents over twelve weeks on one repository ([notes](../evidence/NOTES-contoso.md)).
- One randomized comparison, 96 tickets ([EXP-01](../evidence/experiments.md)): cycle time −16% (95% CI −25% to −5%); escaped defects possibly higher (0 to +25 points). The method's claim is limited to that.

## What this method does not claim

- That it makes developers faster in general, or on any team but the one measured.
- That Prove catches every defect: the defect guardrail in EXP-01 failed.
- That the step names are new ideas. See Credits.

## Credits

The four-step shape is the common research, plan, implement pattern, described as research -> plan -> implement by HumanLayer (Advanced Context Engineering for Coding Agents) and as explore -> plan -> implement -> commit in Anthropic's Claude Code best practices; this method's contribution is what each step must output on legacy code and the evidence for why. "AI layer" is used as in the AI-Native Trainer course. "Context engineering" is used in its general industry sense.

How the steps map to today's tools is in [implementation-2026-09.md](implementation-2026-09.md), versioned separately.

## Changelog

### 2.0.0 — 2026-09-14
- MAJOR: loop gains a fifth step, **Harden**, because both C4 incidents were fixed by a check, not by a rule (INC-09, INC-13). Workshop slides 3, 7 and 12 and the loop diagram change.
- MAJOR: C4 Unguarded rule retired into Harden. Kept in the list so old posts still resolve.
- MINOR: C5 Unread rule added as a hypothesis (two incidents so far; see concepts.md when it has cards).
- C2 keeps its name. "Green lie" was considered and rejected: it blames the agent for lying, the evidence shows a missing gate.

### 1.0.0 — 2026-08-03
- First version: loop, four concepts, credits, attribution audit.
