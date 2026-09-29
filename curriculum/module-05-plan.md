# Module 05 plan — Research → Plan → Implement → Validate

4 lessons · ~60 min instruction · ~6 h practice (plus the 6-ticket field log, spread over 1–2 weeks) · depends on Module 3 (AI layer, grounded rules, Contoso Billing) and Module 4 (context budgets, layering, pathologies, compaction).

Shared lab material: `labs/module-05/` on top of `labs/module-03/brownfield` (Contoso Billing). New: tickets BILL-150/151/152, `tools/LoopGate` (plan-lint, scope, arch, tests, all), `gates/architecture.rules`, worked examples (research brief + plan for BILL-150), reference solutions (BILL-150, BILL-151), and four break overlays (one per lesson), each verified against the gates.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 05.1 Research discipline | Explain why paste-and-go fails on brownfield (plausible-but-local solutions); run a scoped, read-only research pass that finds reuse, constraints and blast radius; write a research brief with evidence | 03.3, 04.2 | Research BILL-150 and BILL-151 with a read-only sub-agent; brief from template | Paste-and-go BILL-151 → duplicates `OutstandingAsync`/`IsOverdue` with drifted meaning and `DateTimeOffset.UtcNow`; tests and convention tests green | `research/BILL-151.md`, first NOTES.md entries |
| 05.2 Plans worth reviewing | Apply 7 plan-quality criteria; write boundaries (touch / do-not-touch), steps with verify clauses and HUMAN checkpoints; lint a plan | 05.1, 03.4 | Plan BILL-150 in plan mode, edit, `LoopGate plan-lint`, implement, `LoopGate scope` | Plan without boundaries → agent "cleans up" `Legacy/MonthlyRevenueReport.cs` (BILL-97's scope) | `plans/BILL-150.md` |
| 05.3 Validation gates and reset decisions | Order gates cheap→expensive and make them independent of the agent's self-report; detect skipped/removed tests; decide reset vs continue with expected cost | 05.2, 04.4, 02.2 | Build the validation gate (`LoopGate all`), wire it as a Stop hook / CI step; reset-vs-continue log | Agent skips a failing 7-day test and reports "all tests pass" after 4 attempts in one polluted session | gate command in CI, validation report |
| 05.4 Failure diagnosis across the loop | Locate origin phase and escape phase(s) of a defect; compute escape probability across gates ($\prod(1-c_i)$); fix at origin + add the missing gate; run the 6-ticket field comparison | 05.1–05.3, 02.5 | Seeded EF plan (outline's lab break), diagnose, fix, rerun; 6 real tickets paste-and-go vs loop in NOTES.md | Research never read ADR 0007 → plan "introduce EF Core" (evidence misread) → approved → implementation green on all existing gates | failure-diagnosis log, NOTES.md with 6 entries |

Math: not required by §10 for M5; two light uses: reset vs continue as expected attempts of a geometric process (05.3), and defect escape probability across independent gates (05.4). Both link back to $p^k$ in 02.2 and forward to M7/M13.

Templates created: `templates/research-brief.md`, `templates/implementation-plan.md`, `templates/notes-log.md` (notes log incl. failure-diagnosis table; M13 formalizes it).

Links to Module 4 (written in parallel) use manifest paths `lessons/module-04/lesson-0N.md`.
No simulation in this module.
