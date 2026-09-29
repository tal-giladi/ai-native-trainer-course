---
name: plan-feature
description: >-
  Turns a researched Contoso Billing ticket into a reviewable implementation plan at
  plans/BILL-###.md (goal, evidence, approach, Touch and Do not touch, verified steps, HUMAN
  checkpoints, validation) and stops for approval. Use after prime, when the user asks to plan a
  BILL-### ticket. Not for one-sentence fixes. Never writes code.
disable-model-invocation: true
argument-hint: "[ticket-id]"
metadata:
  version: "1.0.0"
  owner: "@contoso/billing-leads"
---

# Plan a feature

## Inputs

- `$0`: ticket id. Needs `tickets/$0.md` and the brief `research/$0.md`.
- If the brief is missing, reply "No research brief for $0. Run /prime $0 first." and stop. Do not research here, and do not plan from the conversation.

## Output contract

- `plans/$0.md` in the shape of `templates/implementation-plan.md`, citing `research/$0.md` in its header.
- `LoopGate plan-lint` passes, and `SkillCheck contract` passes with `${CLAUDE_SKILL_DIR}/contract.rules`.
- Every open question in the brief becomes an Assumption named at a HUMAN checkpoint, or blocks the plan.
- A one-sentence change gets no plan file. Reply "One-sentence change: [the sentence]. No plan needed." instead.
- The reply is three lines: the path, the riskiest decision, "Waiting for approval." Nothing is implemented in this turn.

## Steps

1. Read `tickets/$0.md` and `research/$0.md`.
2. If the whole change fits in one sentence (BILL-152 is the example), reply as the contract says and stop.
3. Write `plans/$0.md` from `templates/implementation-plan.md`. Reuse what the brief lists; put owned and legacy areas under Do not touch.
4. Run both checks: `dotnet run --project tools/LoopGate -- plan-lint plans/$0.md --repo .` and `dotnet run --project tools/SkillCheck -- contract plans/$0.md --rules .claude/skills/plan-feature/contract.rules`. Fix the plan and rerun. If a check fails twice, stop and list the failures.
5. Reply with the three lines from the output contract, then wait.
