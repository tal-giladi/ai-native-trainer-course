# Module 5 labs — Research → Plan → Implement → Validate

Everything the Module 5 labs need. Lessons: [05.1](../../lessons/module-05/lesson-01.md) · [05.2](../../lessons/module-05/lesson-02.md) · [05.3](../../lessons/module-05/lesson-03.md) · [05.4](../../lessons/module-05/lesson-04.md).

The labs run on **Contoso Billing** from [`labs/module-03/brownfield`](../module-03/README.md), in the `ai-layer-lab` repository you created in Module 3 (with the grounded `AGENTS.md` from lesson 03.4). This folder adds new tickets, a gate tool, worked examples, reference solutions and one deliberate break per lesson.

> [!WARNING]
> Run the breaks on throwaway branches only. When you repeat the field comparison (05.4) on your employer's repository, keep `NOTES.md`, briefs, plans and transcripts in a **private** repository, and never paste confidential code into public tools.

## Requirements

- .NET SDK 8 or newer (the tool sets `RollForward=Major`, as in Module 3). Verified with SDK 10.0.400.
- Git, and `bash` for the Stop hook (Git Bash on Windows).
- One coding agent (Claude Code primary; any agent with a plan/read-only mode works for 05.1–05.2).
- NuGet access only for the 05.4 break overlay, which restores EF Core 8.0.11 packages.
- Optional for 05.3: SQL Server in local Docker to try `SET NOEXEC ON` on `V005`.

## Contents

| Path | What it is |
|---|---|
| `tickets/` | BILL-150 (record reminders: new table, first write), BILL-151 (collections summary: reuse trap), BILL-152 (one-sentence boundary fix: no plan needed). |
| `tools/LoopGate/` | Dependency-free C# console tool: `plan-lint`, `scope`, `arch`, `tests`, `all`. Read-only; never modifies the repository. |
| `gates/architecture.rules` | Contoso's architecture rules for `LoopGate arch`, each citing its evidence (ADR 0007, `IClock.cs`, V003). |
| `hooks/` | `stop-gate.sh` (Claude Code Stop hook), `settings.example.json`, `loop-gates.yml` (GitHub Actions). |
| `examples/` | Worked research brief and reviewed plan for BILL-150. |
| `solution/BILL-150/`, `solution/BILL-151/` | Reference implementations as overlays (files at their repo-relative paths). |
| `break/05.1-paste-and-go/` | Overlay: the paste-and-go BILL-151 service (duplicated, drifted "owed", `DateTimeOffset.UtcNow`). |
| `break/05.2-no-boundaries/` | An unbounded plan, and an overlay that "cleans up" `Legacy/MonthlyRevenueReport.cs`. Apply after `solution/BILL-150/`. |
| `break/05.3-green-claim/` | Overlay with a wrong 7-day comparison and a skipped test, plus the agent's "all tests pass" summary. Apply after `solution/BILL-150/`. |
| `break/05.4-wrong-architecture/` | Seeded research brief and approved plan that introduce EF Core, and the resulting overlay. Apply to a clean copy. |

## Setup (once)

In the root of your `ai-layer-lab` repository (paths below assume this folder is available as `<course>/labs/module-05`):

```bash
mkdir -p tickets gates tools plans research templates
cp <course>/labs/module-05/tickets/*.md tickets/
cp <course>/labs/module-05/gates/architecture.rules gates/
cp -r <course>/labs/module-05/tools/LoopGate tools/
cp <course>/templates/research-brief.md <course>/templates/implementation-plan.md templates/
dotnet build tools/LoopGate
dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6
git add -A && git commit -m "Module 5: tickets, loop gates, templates"
```

Expected: `PASS arch`, `tests: total 6, passed 6, failed 0, skipped 0`, `ALL GATES PASSED`.

## Applying an overlay

An overlay is a folder of files at their repository-relative paths. Copy it over your working copy on a branch:

```bash
git checkout -b break/05-4
cp -r <course>/labs/module-05/break/05.4-wrong-architecture/overlay/. .
```

(PowerShell: `Copy-Item -Recurse -Force <course>\labs\module-05\break\05.4-wrong-architecture\overlay\* .`)

## Verified gate results

Each overlay was run through `LoopGate` against a clean copy of Contoso Billing with the tickets committed:

| Working copy | plan-lint | scope | arch | tests |
|---|---|---|---|---|
| clean Contoso | – | – | PASS | 6/6 |
| `solution/BILL-150` with `examples/plan-BILL-150.md` | PASS | PASS (6 files) | PASS | 10/10 |
| `solution/BILL-151` | – | – | PASS | 9/9 |
| `break/05.1` | – | – | FAIL (`DateTimeOffset.UtcNow`) | 9/9 |
| `break/05.2` (on BILL-150 solution) | FAIL (14 problems, unbounded plan) | FAIL (`Legacy/MonthlyRevenueReport.cs`, reviewed plan) | PASS | 10/10 |
| `break/05.3` (on BILL-150 solution) | – | – | PASS | FAIL (1 skipped); unskipped: 1 failed |
| `break/05.4` with its seeded plan | PASS | PASS | FAIL (7: EF packages, `DbContext`) | 10/10 |

## Lab sequence

1. **05.1 — Research.** Read-only research briefs for BILL-150 and BILL-151; start `NOTES.md`. *Break:* paste-and-go BILL-151 duplicates `OutstandingAsync`/`IsOverdue` with a different meaning.
2. **05.2 — Plan.** Plan BILL-150 in plan mode, `plan-lint`, review with the seven questions, implement, `scope`. *Break:* an unbounded plan lets the agent rewrite a finance report owned by BILL-97.
3. **05.3 — Validate.** `LoopGate all`, Stop hook, CI workflow, reset log. *Break:* a skipped test behind an "all tests pass" report after four attempts in one session.
4. **05.4 — Diagnose.** Walk back the seeded EF case to its origin; fix at the origin and add the gate. Then the field comparison: 6 tickets × (paste-and-go, loop), alternating order, logged in `NOTES.md`.

## Artifacts to commit to `ai-layer-lab`

- `research/BILL-150.md`, `research/BILL-151.md` (from the [research brief template](../../templates/research-brief.md))
- `plans/BILL-150.md` (from the [implementation plan template](../../templates/implementation-plan.md))
- `tools/LoopGate/`, `gates/architecture.rules`, `.claude/hooks/stop-gate.sh`, `.claude/settings.json`, `.github/workflows/loop-gates.yml`
- `NOTES.md` with 6 tickets × 2 arms, the failure-diagnosis log and the reset log (from the [notes log template](../../templates/notes-log.md))
- `AI-LAYER-CHANGELOG.md` entries for the research-prompt change and the architecture gate

See the portfolio scaffold: [ai-layer-lab](../../projects/ai-layer-lab/README.md).
