# concepts.md — reference (illustrative)

> Reference answer for lessons 14.2 and 14.3, mined from [`evidence/NOTES-contoso.md`](../evidence/NOTES-contoso.md) and [`evidence/experiments.md`](../evidence/experiments.md). These are **one student's** concepts from **illustrative** data. Do not copy the names: your concepts come from your own incidents. Check it with `MethodCheck trace solution/concepts.md`.

Card format: [concept card template](../../../templates/concept-card.md).

## C1 · Shadow rule
- Status: supported
- Named: 2026-03-01
- Plain words: When the agent cannot see a business rule that already exists, it writes a second copy with a slightly different meaning, and both stay in the code.
- Claim: On tickets that touch an existing business term (overdue, balance, issued, VAT), a run without a reuse search produces a second implementation of the term; a research step that must name the existing implementation prevents it.
- Boundary: Terms with exactly one implementation that the agent's context already contains; greenfield code with no existing rules.
- Counter-evidence: none yet. Watch for a shadow rule created even though the brief named the existing rule.
- Evidence:
  - 2026-01-12 · INC-01 · own overdue check next to `InvoiceService.IsOverdue` ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
  - 2026-02-04 · INC-05 · plan redefined "issued" after the brief was lost ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
  - 2026-02-25 · INC-08 · C# balance calculation next to `usp_GetCustomerBalance` ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
  - 2026-03-25 · INC-12 · VAT rounding re-implemented with a different midpoint rule ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))

## C2 · Self-graded green
- Status: supported
- Named: 2026-02-20
- Plain words: When the only tests are the ones the agent wrote, "all tests pass" tells you the agent agrees with itself, not that the work is right.
- Claim: A green test run is evidence of correctness only for tests the agent did not write or edit in the same run; gates that the agent cannot change catch defects its own tests pass over.
- Boundary: Pure refactorings covered by an existing, unchanged test suite.
- Counter-evidence: none yet.
- Evidence:
  - 2026-01-12 · INC-02 · off-by-one asserted by the agent's own tests ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
  - 2026-02-18 · INC-07 · expected value changed to make a failing test pass ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
  - 2026-03-11 · INC-10 · "all 212 tests pass" with the new tests never run ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))

## C3 · Late-PR illusion
- Status: supported
- Named: 2026-07-30
- Plain words: With the agent, people open pull requests later and with more done, so review looks much faster even when the whole ticket is only a little faster.
- Claim: In one randomized comparison, PR review time fell 49% (95% CI 42% to 55%) while time to PR rose 35% (95% CI 15% to 57%) and cycle time fell 16% (95% CI 5% to 25%); review time alone overstates the speed-up.
- Boundary: Teams that open draft PRs at the start of every ticket; comparisons that use the full cycle-time clock.
- Counter-evidence: none; one team, one experiment.
- Evidence:
  - 2026-07-27 · EXP-01 · randomized comparison, 96 tickets ([experiments](../evidence/experiments.md))

## C4 · Unguarded rule
- Status: hypothesis
- Named: 2026-04-05
- Plain words: A rule written down after a mistake, with nothing that fails when the rule goes missing, gets deleted in a clean-up and the mistake comes back.
- Claim: Rules added to the rules file after an incident without a regression task are removed or weakened within a few months; rules with a task survive.
- Boundary: Rules enforced by a test, analyzer or gate rather than by the agent reading them.
- Counter-evidence: none yet; two incidents on one repository.
- Evidence:
  - 2026-03-04 · INC-09 · merged-migration rule lost in a context tidy-up ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
  - 2026-04-01 · INC-13 · hotfix rule without a task deleted; incident repeated ([NOTES](../evidence/NOTES-contoso.md#failure-diagnosis-log))
