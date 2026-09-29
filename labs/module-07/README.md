# Module 7 labs — Agent evaluation

Everything the Module 7 labs need. Lessons: [07.1](../../lessons/module-07/lesson-01.md) · [07.2](../../lessons/module-07/lesson-02.md) · [07.3](../../lessons/module-07/lesson-03.md) · [07.4](../../lessons/module-07/lesson-04.md) · [07.5](../../lessons/module-07/lesson-05.md) · [07.6](../../lessons/module-07/lesson-06.md).

The labs formalize the Module 4 check ([`labs/module-04/tasks-v0`](../module-04/tasks-v0/README.md): 10 tasks, regex grading, 3 runs) into an evaluation harness, and run it on **Contoso Billing** from [`labs/module-03/brownfield`](../module-03/README.md) with the AI layers you built in Modules 3–6.

> [!WARNING]
> `EvalHarness run` and `judge` send prompts, and whatever context your agent loads from the working copy, to your model provider, and they cost money: 24 tasks × 5 trials × 2 arms is 240 headless sessions plus judge calls. Estimate first (`stats` prints cost per trial after a small run), run only on the fictional Contoso code or on a repository your provider agreement covers, and keep results from employer code in a **private** repository. Code tasks let the agent edit files: the harness runs them in a copy under the output folder, never in your repository.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`, so a newer runtime works too). Verified with SDK 10.0.400.
- Claude Code with `claude` on your PATH, signed in or with `ANTHROPIC_API_KEY` set (for `run` and `judge`). Pin the version and turn off auto-update for the duration of a comparison (lesson 07.5). Another agent works if you save its answers as `T01.r1.json` in the same shape (`{"result": "...", "is_error": false}`) and use `--agent replay:<dir>`. `run` and `judge` pass `--setting-sources project,local`, which keeps your `~/.claude/settings.json` and personal skills out but not your personal `~/.claude/CLAUDE.md` (checked 2026-09-29), so they also pass `--settings` with a small temporary file that sets `claudeMdExcludes` to that file and `~/.claude/rules/**` and turns auto memory off: personal instructions change results, and runs must be reproducible on anyone's machine. `judge` saves each raw judge result as `<answer>.judge.json` next to the answer; `grade` records its cost and `stats` reports it.
- No NuGet packages for the harness. Code tasks restore the Contoso test project's pinned packages (xunit 2.9.2, Dapper 2.1.35) once.
- Everything except `run` and `judge` works offline on the illustrative data in `samples/` and `calibration/`.

## Contents

| Path | What it is |
|---|---|
| `tasks-v1/` | 24 tasks with references, counterexamples, splits and tags; `golden/` tests and reference solutions for the three code tasks; `graders/` rubrics and the biased judge v1. Dataset card in `tasks-v1/README.md`. |
| `tools/EvalHarness/` | Dependency-free C# tool: `validate`, `run`, `judge`, `grade`, `stats`, `compare`, `calibrate`, `gate`, `leak`, `runs`. Read-only on your repository. |
| `calibration/` | 24 answers to task T21, human labels, and **illustrative** verdicts from judge v1 (biased) and v2 (rubric). |
| `samples/` | **Illustrative** result sets (simulated, not measurements) for the statistics, comparison and gate exercises, and the script that generated them. |
| `breaks/` | The leak section for the 07.2 break, and the index of all six breaks. |
| `ci/agent-evals.yml` | GitHub Actions workflow: smoke on PRs that touch the AI layer, full suite nightly, regression gate. |

## Quick start (offline, no API key)

From this folder:

```bash
H="dotnet run --project tools/EvalHarness --"

# 1. The task set is well-formed; with --repo, code tasks are checked against the real app (about 1 minute)
$H validate tasks-v1/tasks.json --repo ../module-03/brownfield

# 2. End-to-end on replayed answers: the Module 4 illustrative "after" runs, graded by the v1 graders
$H run tasks-v1/tasks.json --repo ../module-03/brownfield --out runs/replay --trials 1 --agent replay:../module-04/samples/illustrative-after
$H grade tasks-v1/tasks.json runs/replay --config replay --out results.csv
$H stats results.csv --k 1

# 3. Grader calibration, statistics, comparison and gate on illustrative data
$H calibrate calibration/human-labels.csv calibration/illustrative-judge-v1.csv --answers calibration/answers
$H compare samples/skill-compare/results.csv --a skill-v1-once --b skill-v2-once
$H compare samples/context-reduction/results.csv --a bloated --b reduced
$H gate samples/gate/results.csv --baseline layer-v1.3 --candidate layer-v1.4
$H runs --p 0.7 --halfwidth 0.1
```

(PowerShell: `$H = "dotnet run --project tools/EvalHarness --"` does not expand as a command; type the full `dotnet run --project tools/EvalHarness -- <command>` instead.)

## With a real agent

```bash
# a working copy with your AI layer (from Module 4 or 6) on top of Contoso Billing
cp -r ../module-03/brownfield ~/m7-work && cp -r ../module-04/solution/. ~/m7-work/

$H run tasks-v1/tasks.json --repo ~/m7-work --out runs/reduced --trials 5
$H judge tasks-v1/graders/rubric-T21.md runs/reduced --only T21 --out runs/reduced/verdicts.csv
$H judge tasks-v1/graders/rubric-T22.md runs/reduced --only T22 --out runs/reduced/verdicts.csv
$H grade tasks-v1/tasks.json runs/reduced --config reduced --out results.csv --verdicts runs/reduced/verdicts.csv
```

`run` is resumable: rerun the same command after a crash and it skips trials that already have a file. It writes `manifest.json` (task-set hash, AI-layer hash, agent version, model, seed) so `compare` and `gate` can refuse comparisons in which more than the treatment changed.

## Lab sequence

1. **07.1 — Why evaluate.** Set up `agent-evals/`, run the harness end-to-end on replayed answers, write `CHARTER.md`. *Break:* a one-run demo "proves" skill v2; the five-trial data says otherwise.
2. **07.2 — Task datasets.** `validate` tasks-v0 (10 errors), then tasks-v1; add four tasks of your own from `NOTES.md` incidents. *Break:* someone pastes task answers into `AGENTS.md` (`breaks/eval-hints-section.md`); dev score jumps, holdout does not; `leak` finds it.
3. **07.3 — Graders.** Label the 24 calibration answers yourself, run judge v1 and the v2 rubric, `calibrate`. *Break:* judge v1 rewards verbosity.
4. **07.4 — Statistics.** `stats` with Wilson and task-clustered intervals, pass@k and pass^k; `runs`. *Break:* "100 trials" that are 4 tasks × 25.
5. **07.5 — Comparisons (project).** Re-score the Module 4 context reduction: 24 tasks × 5 trials, bloated vs reduced, interleaved, pinned, with an A/A run. *Break:* arm B ran later on a newer agent without the code tasks.
6. **07.6 — Evals in the loop.** Baseline results, `gate`, CI workflow, regression task per incident. *Break:* an aggregate-only gate lets a golden task collapse.

## Artifacts to commit to `agent-evals`

- `tasks/tasks-v1.json` (+ your own tasks), `tasks/README.md` dataset card ([template](../../templates/evaluation-dataset.md))
- `graders/` rubrics, `graders/calibration.md` ([template](../../templates/evaluation-rubric.md)) with your confusion matrix
- `harness/` (EvalHarness), `CHARTER.md`
- `reports/context-reduction.md` — the first comparison report ([template](../../templates/benchmark.md)) with `results.csv`
- `baseline/results.csv` and the CI workflow in `ai-layer-lab`

See the portfolio scaffold: [agent-evals](../../projects/agent-evals/README.md).
