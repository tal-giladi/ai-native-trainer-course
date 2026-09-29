# Module 13 labs — Measuring AI engineering impact

Everything the Module 13 labs need. Lessons: [13.1](../../lessons/module-13/lesson-01.md) · [13.2](../../lessons/module-13/lesson-02.md) · [13.3](../../lessons/module-13/lesson-03.md) · [13.4](../../lessons/module-13/lesson-04.md) · [13.5](../../lessons/module-13/lesson-05.md) · [13.6](../../lessons/module-13/lesson-06.md).

The labs turn the `NOTES.md` log you started in [Module 5](../module-05/README.md) into a controlled comparison, and extend the statistics of [Module 7](../module-07/README.md) (Wilson intervals, task clustering, paired designs) from pass rates to skewed engineering metrics: cycle time, review time, rework and escaped defects.

> [!WARNING]
> Your own experiment uses data about real people's work. Get your manager's agreement and tell the team what is measured and why **before** you start; measure tickets and teams, never rank individuals; anonymize developer names (`D1`, `D2`…) before any file leaves your machine; and keep employer data in a **private** repository. `gh pr list` and tracker exports can contain customer names and internal URLs — strip them before committing.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`, so a newer runtime works too). Verified with SDK 10.0.400.
- No NuGet packages. `ImpactStats` is read-only and deterministic (fixed seeds).
- Optional: the `gh` CLI for importing your own PR data; Python 3 only to regenerate the illustrative data (`py data/generate_data.py`).
- Everything except your own experiment works **offline** on the illustrative data.

## Contents

| Path | What it is |
|---|---|
| `tools/ImpactStats/` | Dependency-free C# tool: `describe` (medians, percentiles, geometric means), `balance` (are the arms comparable?), `compare` (stratified ratio of geometric means, bootstrap CI, permutation test, Hedges g, Cliff's delta; `--binary` for rates with Wilson intervals; `--mean` for the naive t-test), `did` (difference-in-differences with placebo units), `assign` (blocked randomization), `power` (tickets per arm), `import-gh` (PR JSON → CSV). |
| `data/` | Three **illustrative** data sets and their deterministic generator; see [`data/README.md`](data/README.md) for what is true in each simulation, and `ticket-log-template.csv` for your own experiment. |
| `claims/` | Three fictional claims to critique (13.2): a vendor slide, a social-media post, a dashboard tile. |
| `reports/` | `draft-experiment-01.md`, a deliberately flawed report (13.6 break), and `worked-example-experiment-01.md`, an honest one. |

## Quick start (offline)

From this folder:

```bash
H="dotnet run --project tools/ImpactStats --"

# 13.1 — describe and the two clocks
$H describe data/contoso-randomized.csv --metric cycle_hours --by size
$H compare data/contoso-randomized.csv --metric review_hours --strata size

# 13.3 — the confounded data set: naive vs within size
$H balance data/contoso-selfselected.csv --arm assigned --by size
$H compare data/contoso-selfselected.csv --metric cycle_hours
$H compare data/contoso-selfselected.csv --metric cycle_hours --strata size

# 13.3 — design tools
$H power --sd 0.35
$H assign data/contoso-selfselected.csv --block developer,size --seed 42 > my-assignment.csv

# 13.4 — regression to the mean
$H did data/prepost-teams.csv --treated Forms --pre 2026-Q2 --post 2026-Q3
$H did data/prepost-teams.csv --treated Forms --pre 2026-Q1 --post 2026-Q3

# 13.5 — statistics
$H compare data/contoso-randomized.csv --metric cycle_hours --strata size --mean
$H compare data/contoso-randomized.csv --metric escaped_defects --binary --strata size
```

(PowerShell: type the full `dotnet run --project tools/ImpactStats -- <command>` instead of `$H`.) Add `--where col=value` or `--exclude col=value` to filter rows, `--seed` to change the resampling seed, `--boot`/`--perm` to change the number of resamples.

## Your own data

```bash
# PRs merged in the last months (review clock only — see 13.1 for why that is not cycle time)
gh pr list --state merged --limit 500 --json number,title,createdAt,mergedAt,additions,deletions,labels > prs.json
$H import-gh prs.json > prs.csv
```

For cycle time you need the tracker's status history (for Jira: the issue changelog, "In Progress" → "Done"); export it, compute hours per ticket, and fill `data/ticket-log-template.csv`. Record size **in refinement, before** assignment.

## Lab sequence

1. **13.1 — Delivery metrics.** Write your team's metric dictionary from the [metrics template](../../templates/metrics.md); `describe` the Contoso data; import your PRs. *Break:* the PR clock says 49% faster; the full clock says 16%.
2. **13.2 — The evidence base.** Critique the three `claims/` against the published studies. *Break:* a vendor's "55% faster" slide sent to a legacy .NET CTO.
3. **13.3 — Experiment design.** `assign` a backlog, `power` from your baseline SD, write the pre-registration ([template](../../templates/experiment-design.md)). *Break:* self-selected data, 42% faster → +2% within size.
4. **13.4 — Threats to validity.** Threat register; `did` with placebo teams. *Break:* the pilot team chosen for its worst quarter "halves" cycle time.
5. **13.5 — Statistics.** Bootstrap, permutation, effect sizes, blocking. *Break:* a mean-based "26% faster" driven by one 170-hour ticket.
6. **13.6 — Running and reporting (project).** Your controlled comparison over 3–6 weeks and the report ([template](../../templates/experiment-report.md)). *Break:* the flawed draft report.

## Artifacts to commit to `method-notes`

- `metrics.md` — your metric dictionary
- `evidence-base.md` — one-page annotated bibliography with your critique of one claim
- `experiment-01-prereg.md` — pre-registration and threat register, committed before the first ticket
- `data/experiment-01.csv` (anonymized) and the exact `ImpactStats` commands
- `experiment-01.md` — the report; `experiment-01-onepager.md` — the internal write-up

See the portfolio scaffold: [method-notes](../../projects/method-notes/README.md).
