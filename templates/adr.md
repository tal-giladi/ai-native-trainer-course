# Architecture decision record (ADR)

*Template. Copy into your project as `docs/adr/NNNN-short-title.md` (four digits, never reused). Introduced in [Module 12, lesson 12.5](../lessons/module-12/lesson-05.md). Based on Michael Nygard's format and MADR 4. The Module 12 lab's `ArchCheck adr` checks the structure below.*

**Rules.** One decision per record, one or two pages. Write it when the decision is made, not after. Never edit an accepted decision's substance: write a new ADR that says `Supersedes: ADR-NNNN` and change the old one's status to `Superseded by ADR-NNNN`. Typos and broken links may be fixed in place.

---

```markdown
# ADR-NNNN: <short noun phrase, e.g. "Central model gateway for all agent traffic">

Status: Proposed | Accepted | Rejected | Deprecated | Superseded by ADR-NNNN
Date: YYYY-MM-DD
Topic: <one stable key for the question this answers, e.g. model-gateway>
Deciders: <names or roles who own the decision>
Consulted: <security, legal, platform, team leads ...>
Supersedes: <ADR-NNNN, only if this replaces one>
Answers: <security-review question ids this ADR is evidence for, e.g. Q05, Q07>

## Context

The forces at play, stated neutrally: the requirement, the constraint (residency, budget, headcount, deadline),
the incident or measurement that triggered the decision, and what is out of scope. Numbers beat adjectives.

## Options considered

### Option A — <name>
What it is, in two or three lines. Pros. Cons. What it would cost (money, people, time, capability).

### Option B — <name>
...

### Option C — Do nothing / keep the current state
Always worth writing down: it is the baseline every other option must beat.

## Decision

We will <active voice, one paragraph>. Name the option and the deciding reasons, in order of weight.
Say which hard constraint eliminated which option before any weighing happened.

## Consequences

- Positive: <what gets better, measurably if possible>
- Negative: <what we give up or pay; every real decision has at least one>
- Risk: <what could make this decision wrong, and the signal that would tell us>
- Follow-up: <work this decision creates, with owners>

## Confirmation

How we will know the decision is being followed and still holds: an automated check (lint rule, policy,
CI gate), an eval, a metric with a threshold, or a review date. "We will review it" alone is not confirmation.
```

---

## Checklist before you mark it Accepted

- [ ] At least two real options, plus the do-nothing baseline where it applies.
- [ ] Hard constraints eliminate options first; only survivors are weighed ([model selection matrix](model-selection-matrix.md) uses the same rule).
- [ ] At least one negative consequence is written down.
- [ ] Confirmation names a check someone other than the author can run.
- [ ] The `Topic:` is not already held by another Accepted ADR (if it is, supersede that one).
- [ ] The ADR is linked from the security-review answers it supports (`Answers:`), and from the reference diagram if it changes a box or a boundary.
