# Module 4 labs — Context engineering

Everything the Module 4 labs need. Lessons: [04.1](../../lessons/module-04/lesson-01.md) · [04.2](../../lessons/module-04/lesson-02.md) · [04.3](../../lessons/module-04/lesson-03.md) · [04.4](../../lessons/module-04/lesson-04.md) · [04.5](../../lessons/module-04/lesson-05.md).

The labs reuse **Contoso Billing** from [`labs/module-03/brownfield`](../module-03/README.md) (the application code and tests) and put a different context layer on top of it: first a deliberately terrible one, then yours.

> [!WARNING]
> `run-tasks` sends each task prompt, plus whatever context your agent loads from the working copy, to your model provider, and it costs money: 10 tasks × 3 runs × 2 configurations is 60 headless sessions. Run it only on the fictional Contoso code or on a repository your provider agreement covers. When you repeat the audit on your employer's repository, keep all output in a **private** repository.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`, so a newer runtime works too).
- Claude Code with the `claude` CLI on your PATH (for `run-tasks`), signed in or with `ANTHROPIC_API_KEY` set. Any other agent works if you save its answers as `.txt` files named `T01.r1.txt`, `T01.r2.txt`, …
- Pinned packages (ContextLab only): `Microsoft.ML.Tokenizers` 2.0.0 and `Microsoft.ML.Tokenizers.Data.O200kBase` 2.0.0, the same as `labs/module-02/01-tokens`. Token counts are an o200k_base **proxy**; Claude's tokenizer gives different numbers.

## Contents

| Path | What it is |
|---|---|
| `bloated/` | The "before" context layer: a 306-line `CLAUDE.md` that `@`-imports a pasted architecture page, a company-wide style guide and a team handbook, an always-loaded `.claude/rules/sql.md`, and a diverging `AGENTS.md` that Claude Code does not load. 905 lines in total, 807 of them always loaded. Every pathology from lesson 04.3 is in it, and the ten critical facts are scattered and partly contradicted. |
| `tasks-v0/` | Ten read-only tasks (T01–T10), one per critical fact, with deterministic regex grading. The seed of your `agent-evals/tasks-v0/`. |
| `tools/ContextLab/` | Read-only C# tool: `budget` (what loads when, in tokens; $W - S - R - T - H - O$; monthly cost), `audit` (pathology leads), `prompts`, `grade`, `report`. |
| `scripts/run-tasks.sh`, `.ps1` | Runs every task K times in fresh headless Claude Code sessions (`claude -p --output-format json --permission-mode dontAsk --setting-sources project,local --settings <isolation file>`, so nothing can be edited) and saves one JSON file per run. `--setting-sources project,local` keeps your `~/.claude/settings.json` and personal skills out, but not your personal `~/.claude/CLAUDE.md` (checked 2026-09-29), so the script also passes `--settings` with a small temporary file that sets `claudeMdExcludes` to that file and `~/.claude/rules/**` and turns auto memory off: personal instructions change results, and runs must be reproducible on anyone's machine. |
| `breaks/` | The five deliberate failures of this module (table in `breaks/README.md`). |
| `solution/` | Reference "after" layer: 34 always-loaded lines (`AGENTS.md` + `CLAUDE.md` import and compact instructions), a path-scoped `.claude/rules/migrations.md`, an on-demand `docs/ai/topology.md`, and a filled `context/context-audit.md`. Try the project before opening it. |
| `samples/` | Hand-written **illustrative** runs for practising `grade` and `report` without an API key. Not measurements. |

## Quick start

From this folder (bash; PowerShell equivalents in the lessons):

```bash
# 0. A working copy of Contoso Billing with the bloated layer on top
cp -r ../module-03/brownfield ~/m4-work
cp -r bloated/. ~/m4-work/
dotnet test ~/m4-work/Contoso.Billing.sln            # 6 tests pass: the code is fine, the context is not

# A second copy with the reference layer (lessons 04.2-04.4 use it for breaks)
cp -r ../module-03/brownfield ~/m4-solution
cp -r solution/. ~/m4-solution/

# 1. Budget and audit (lessons 04.1, 04.3)
dotnet run --project tools/ContextLab -- budget ~/m4-work
dotnet run --project tools/ContextLab -- audit ~/m4-work

# 2. Baseline on the task set (lesson 04.5) - 30 headless sessions
sh scripts/run-tasks.sh tasks-v0/tasks.json ~/m4-work runs/before 3
dotnet run --project tools/ContextLab -- grade tasks-v0/tasks.json runs/before --label before --rules-tokens 7028 --out results.csv

# 3. After you have cut the layer (or to see the reference): same commands with --label after, then
dotnet run --project tools/ContextLab -- report results.csv
```

If you already have `ai-layer-lab` from Module 3, work on a branch of it instead of `~/m4-work`: replace its rules with `bloated/` on a branch called `m4-bloated`, and do the reduction on a branch from there.

## Lab sequence

1. **04.1 — Budget.** Run `budget` on the bloated layer and on the Module 3 rules; compute the per-request budget and the monthly cost of $R$ by hand and check the tool. *Break:* a verbose test log enters the history; watch `/context`.
2. **04.2 — Layers.** Sort every block of the bloated layer into always / on-demand / never / generated; write the topology map; move migration details to a path-scoped rule with a pointer in the root. *Break:* the critical `U###` fact lives only in the path-scoped rule → T04 fails when the agent never opens a migration file.
3. **04.3 — Pathologies.** Run `audit`, confirm each lead, label it with one of the five pathologies. *Break:* a contradictory `Invoices/CLAUDE.md` → T02 flips depending on which files the agent reads.
4. **04.4 — Compaction.** Write compact instructions and a handoff note; test what survives `/compact` and `/clear`. *Break:* a constraint stated only in chat is lost after compaction.
5. **04.5 — The context audit (project).** Baseline on tasks-v0, cut the layer by ≥ 70 %, rerun, report tokens, pass rate and cost before/after. *Break:* the cut removes the "never edit a merged `V###`" fact → T06 fails; the task set catches it.

## Artifacts to commit

- `ai-layer-lab/context/context-audit.md` (from the [context audit template](../../templates/context-audit.md)) with your before/after table
- the reduced layer: `AGENTS.md`, `CLAUDE.md`, `.claude/rules/…`, `docs/ai/topology.md`
- `agent-evals/tasks-v0/` — `tasks.json` plus your `results.csv` (see [agent-evals](../../projects/agent-evals/README.md))
