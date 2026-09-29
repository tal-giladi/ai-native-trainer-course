# Module 3 labs — The AI layer

Everything the Module 3 labs need. Lessons: [03.1](../../lessons/module-03/lesson-01.md) · [03.2](../../lessons/module-03/lesson-02.md) · [03.3](../../lessons/module-03/lesson-03.md) · [03.4](../../lessons/module-03/lesson-04.md).

> [!WARNING]
> The labs run against the fictional Contoso Billing app in this folder. When you repeat an exercise on your employer's repository, keep all output (audit, rules file, probe logs) in a **private** repository and never paste confidential code into public tools.

## Requirements

- .NET SDK 8 or newer (`global.json` rolls forward to newer SDKs). The test project and the tool set `RollForward=Major`, so they also run when only a newer runtime is installed.
- One coding agent (Claude Code primary) and, for 03.2, one of Cursor or GitHub Copilot.
- A GitHub account (03.1: CODEOWNERS and branch protection).
- No database is needed: the tests are unit tests and the SQL scripts are read, not executed.

## Contents

| Path | What it is |
|---|---|
| `brownfield/` | **Contoso Billing**, a small .NET 8 billing library with a 2016 legacy helper (`SqlHelper`, `[Obsolete]`), a 2024 Dapper repository pattern (ADR 0007), SQL Server migrations with an undo-script convention that starts at V003, a stale 2021 `docs/ARCHITECTURE.md`, a history excerpt, convention tests and ticket `tickets/BILL-142.md`. |
| `starter/CLAUDE.md` | The deliberately poor v1.0.0 rules file "imported from the wiki": generic advice, a nonexistent solution name, and the stale `SqlHelper` / `DateTime.Now` conventions. |
| `solution/` | Reference answers: grounded `AGENTS.md`, `CLAUDE.md` that imports it, `CODEOWNERS`, `AI-LAYER-CHANGELOG.md`, `probes/probes.md`, and `portability/` (one path-scoped rule in Claude Code, Cursor and Copilot formats). Try the exercises before opening it. |
| `tools/AiLayerTool/` | Dependency-free C# console tool: `scan` (brownfield audit leads) and `lint` (rules-file grounding). Read-only: it never modifies files. |

## Quick start

From this folder:

```bash
# 1. The app builds and its 6 tests pass
dotnet test brownfield/Contoso.Billing.sln

# 2. Audit leads (lesson 03.3)
dotnet run --project tools/AiLayerTool -- scan brownfield

# 3. Lint the starter rules (lesson 03.4) - expect 4 errors, 11 warnings, exit code 1
cp starter/CLAUDE.md brownfield/CLAUDE.md
dotnet run --project tools/AiLayerTool -- lint brownfield/CLAUDE.md --repo brownfield

# 4. Lint the reference rules - expect 0 errors, 0 warnings
cp solution/AGENTS.md solution/AI-LAYER-CHANGELOG.md solution/CODEOWNERS brownfield/
dotnet run --project tools/AiLayerTool -- lint brownfield/AGENTS.md --repo brownfield
```

(PowerShell: use `Copy-Item` instead of `cp`.) Remove the copied files afterwards if you want to redo the exercises from the starting point.

## Lab sequence

1. **03.1 — Make the AI layer a citizen.** Create a private `ai-layer-lab` GitHub repository with the contents of `brownfield/`, commit `starter/CLAUDE.md` as v1.0.0, add `.github/CODEOWNERS` (catch-all first), protect `main` with code-owner review, and start `AI-LAYER-CHANGELOG.md`. *Break:* catch-all last → AI layer loses its owners.
2. **03.2 — Map and port.** Inventory components on three axes; make `AGENTS.md` canonical with `CLAUDE.md` importing it; write the migration rule for three tools. *Break:* diverging `CLAUDE.md` and `AGENTS.md` → Claude and Cursor/Copilot give different answers.
3. **03.3 — Audit.** Run `scan`, run the tests yourself, build a findings table with evidence rungs, probe a rules-free agent 5 times per finding. Repeat on your own repository. *Break:* docs-only audit → stale findings.
4. **03.4 — Ground the rules.** Rewrite the rules from the audit, lint to clean, run probes P1–P4 ×3, implement BILL-142 with an agent, add lint to CI. *Break:* the stale `SqlHelper` line is restored → the agent reproduces the abandoned pattern; `ConventionTests` catches it.

Copy `tools/AiLayerTool/` into your `ai-layer-lab` repository (for example as `tools/AiLayerTool/`) so CI can run the lint.

## Artifacts to commit to `ai-layer-lab`

- `.github/CODEOWNERS`, `AI-LAYER-CHANGELOG.md`
- `AGENTS.md` (grounded, lint clean) and `CLAUDE.md` importing it
- `.claude/rules/`, `.cursor/rules/`, `.github/instructions/` path-scoped rule
- `docs/ai-layer.md` from the [AI-layer architecture template](../../templates/ai-layer-architecture.md) (ownership, inventory, portability matrix)
- `audit/brownfield-audit.md` from the [brownfield audit checklist](../../templates/brownfield-audit-checklist.md)
- `audit/probes.md` with your probe results log

See the portfolio scaffold: [ai-layer-lab](../../projects/ai-layer-lab/README.md).
