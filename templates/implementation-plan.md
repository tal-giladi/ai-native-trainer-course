# Implementation plan template

A plan is a reviewable artifact: a human should be able to approve or reject it in five minutes, and a script should be able to check its form. The headings below are the ones `LoopGate plan-lint` and `LoopGate scope` read (`labs/module-05/tools/LoopGate`). Introduced in [05.2 · Plans worth reviewing](../lessons/module-05/lesson-02.md). Worked example: [`labs/module-05/examples/plan-BILL-150.md`](../labs/module-05/examples/plan-BILL-150.md).

```markdown
# Plan — <ticket id> <title>

Author: <agent/human> · Reviewed by: ______ on ______ · Research: `<research brief path>`

## Goal

<One sentence a reviewer can check the diff against: which method/endpoint/table, what behavior.>

## Evidence

- `<path>` — <what it tells us> (at least 2 files; every path must exist today)

## Approach

- <Design decision>. (ev: `<path>`)
- Assumption A1: <what we assume and why>. (ev: `<ticket or doc path>`)

## Boundaries

### Touch

- `<path or glob>` (every file the change may create or modify)

### Do not touch

- `<path or glob>` (<reason / owner / ticket>)

## Steps

1. <Small step>. verify: <command or observable result>
2. …

## Checkpoints

- HUMAN: approve this plan (name the risky decision) before step 1.
- HUMAN: review the diff against Boundaries before merge.
- STOP rule: if the same step fails twice, stop and report.

## Validation

- `<gate command>` — expected: <tests count, 0 skipped, arch clean, scope clean>

## Out of scope

<Things a reasonable engineer might do "while here" that this change must not do.>
```

## Seven review questions (the reviewer's five minutes)

1. **Goal:** could I tell from the diff alone whether the goal was met?
2. **Evidence:** does each cited file actually support the decision next to it (not just exist)?
3. **Reuse:** does the plan reuse what the research found, or quietly re-implement it?
4. **Boundaries:** is every file in the eventual diff predictable from the Touch list? Are owned or legacy areas in Do not touch?
5. **Steps:** is each step small enough to verify, and does it say how?
6. **Checkpoints:** is the riskiest decision named at a HUMAN checkpoint, with a stop rule?
7. **Validation:** are the gates runnable commands with expected results, independent of the agent's own report?
