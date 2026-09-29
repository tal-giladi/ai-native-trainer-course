---
id: "21.2"
module: 21
minutes: 16
practice_minutes: 120
prerequisites: ["21.1", "03.3", "13.1", "13.3", "07.4"]
objectives:
  - Audit a client's repository in its before state with triangulated evidence, labelling each finding confirmed or hypothesis and giving it a client owner.
  - Capture a baseline before the build starts, with definitions, sources, a 12-week window, a spread, a quality guardrail and an eval baseline, and no per-person metrics.
  - Compute the minimum detectable effect of the engagement's comparison from the baseline spread and the tickets the SOW window allows, and explain it to a sponsor.
  - Check the audit and baseline with EngageCheck baseline and fix every error.
volatility: concept
sources:
  - title: "DORA — DORA's software delivery performance metrics"
    url: https://dora.dev/guides/dora-metrics/
  - title: "Forsgren, Storey, Maddila, Zimmermann, Houck and Butler (2021) — The SPACE of Developer Productivity"
    url: https://www.microsoft.com/en-us/research/publication/the-space-of-developer-productivity-theres-more-to-it-than-you-think/
  - title: "Becker, Rush, Barnes and Rein (2025) — Measuring the Impact of Early-2025 AI on Experienced Open-Source Developer Productivity"
    url: https://arxiv.org/abs/2507.09089
last_verified: "2026-09-28"
---

# 21.2 · Technical audit and baseline

## Why it matters

There is one thing in an engagement you cannot do later: measure the before. Once the AI layer is in the repository and the team has started working differently, the "before" is gone. A baseline taken in week 4 measures the intervention, and every comparison built on it is a comparison of the change with itself.

The audit has the mirror problem. A client's wiki describes the system as someone understood it years ago. Contoso's `docs/ARCHITECTURE.md` was last edited in 2021; it names a solution file that no longer exists, a test project that was renamed, and a data-access helper that has been `[Obsolete]` since 2024. An AI layer written from that page teaches the agent the conventions the team spent two years retiring.

This lesson is the second and third gates: an audit grounded in executable evidence, and a baseline captured before the build starts, including the number that tells the sponsor what this engagement can and cannot show.

> [!NOTE]
> Content tags. **Concept** (stable): the evidence ladder applied to a client, baseline timing, metric definitions, guardrails, team-level measurement, minimum detectable effect, pre-registration. **Implementation**: `AiLayerTool scan`, `ImpactStats describe` and `power`, `EvalHarness stats`, `EngageCheck baseline`, and the illustrative Contoso data.

## How it works

### The audit is Module 3's audit, on someone else's code

Everything from [03.3](../module-03/lesson-03.md) applies: five areas (build and test, architecture, SQL Server conventions, legacy patterns, tribal knowledge), and the evidence ladder, from strongest to weakest: (1) executable evidence, (2) recent code, (3) decision records, (4) history, (5) repository docs, (6) wiki, (7) memory. A finding is **confirmed** when two sources agree and one is from rungs 1–3; otherwise it is a **hypothesis** with a test.

Three things change when the repository is a client's:

- **You run it yourself.** Run the build and the tests on the client's machine in week 1. "The tests pass" from the tech lead is rung 7.
- **Every finding gets a client owner.** You will leave; the finding stays. The owner decides whether it becomes a rule, a check, a ticket or nothing.
- **Tribal knowledge comes from the interviews.** The stakeholder interviews of [21.1](lesson-01.md) are audit input: Dana's migration without an undo script is an incident, rung 7 on its own, confirmed by the code.

### The baseline

The baseline is the before-state of every outcome the SOW promises to measure. Five rules, each from an earlier module:

1. **Captured before the build starts**, from history, not prospectively. A 12-week export of tickets that were done before you arrived cannot be influenced by your presence.
2. **Defined.** Start and stop events, unit, inclusion rule ([13.1](../module-13/lesson-01.md)). "Cycle time: hours from In Progress to Done, per ticket; size recorded in refinement."
3. **With a spread.** A median alone cannot tell a later change from noise. Record the quartiles and the spread of the log times within size (the `sd(log)` column), which the power calculation needs.
4. **With a guardrail.** Speed without quality is not a result. Escaped defects, change failures or reverts go in the baseline, in the spirit of DORA's pairing of throughput and instability metrics ([DORA](https://dora.dev/guides/dora-metrics/)).
5. **Tickets and teams, never people.** The SPACE framework argues that no single metric captures developer productivity and that measures should span several dimensions and levels ([Forsgren et al.](https://www.microsoft.com/en-us/research/publication/the-space-of-developer-productivity-theres-more-to-it-than-you-think/)); a per-developer number in a client engagement also turns your baseline into a ranking the client's staff will rightly resist.

Two more baselines come from your own tooling. The **eval baseline**: run the Module 7 task set on the before state (no AI layer), so the layer's effect on the agent is measured on the same tasks later ([07.4](../module-07/lesson-04.md)). The **usage baseline**: weekly active developers from the licence export, the starting point for Module 19's adoption curve ([19.4](../module-19/lesson-04.md)).

### What can this engagement detect?

*Intuition.* The SOW's comparison runs for four weeks on one team. Billing does about five tickets a week, so the randomized comparison will have about 27 tickets. Cycle times vary a lot even within one size. With that little data, only a large change will stand out from the noise. You need to know how large before you sign D2, so that "inconclusive" in week 11 is an expected outcome rather than a surprise.

*Equation.* From [13.3](../module-13/lesson-03.md): comparing log cycle times of two arms with $n$ tickets each, spread $\sigma$ within size, 5% two-sided significance and 80% power, the smallest detectable difference in log hours is

$$\delta = (z_{0.975} + z_{0.80})\,\sigma\sqrt{\frac{2}{n}}, \qquad \text{reduction} = 1 - e^{-\delta}$$

*Tiny example.* Baseline $\sigma \approx 0.44$ (the within-size `sd(log)` of Contoso's baseline), $n \approx 13.5$ per arm, $z_{0.975} + z_{0.80} = 1.96 + 0.84 = 2.80$:

$$\delta = 2.80 \times 0.44 \times \sqrt{\tfrac{2}{13.5}} = 2.80 \times 0.44 \times 0.385 = 0.474, \qquad 1 - e^{-0.474} = 0.38$$

So the pilot can reliably detect a cycle-time change of about **38%** or more. To detect 15%, it would need about 116 tickets per arm, eight times what four weeks give.

*Implementation.* `ImpactStats power --sd 0.44 --effect 0.38` prints 14 tickets per arm (27 total); `--effect 0.15` prints 116 per arm.

*Interpretation.* Two consequences for the engagement. First, write the detectable effect into the kickoff record and the pre-registration (D2), in words: "Smaller real effects will read as inconclusive." Second, measure what *can* be measured with this data: the eval comparison is paired by task and far more sensitive, and adoption is measured on all 19 developers. The delivery comparison stays, honestly labelled, because it is the one the sponsor ultimately cares about, and because a large harmful effect on the guardrail would still show.

### The pre-registration

D2 in the SOW is the pre-registration: outcomes, arms, how tickets are randomized (by size, in the PM's refinement meeting), the detectable effect, and the stop rule (escaped defects in the ai arm above a threshold). Use the [experiment design template](../../templates/experiment-design.md) from Module 13. It is signed before the first ticket is assigned, which is why the baseline must exist before it.

## Show me

Contoso, week 1. The audit on the `before-ai-layer` worktree of your `brownfield-demo`:

```bash
dotnet run --project labs/module-03/tools/AiLayerTool -- scan ../demo-before
```

```text
## SQL migrations (V### needs a matching U### undo script?)
- V001__create_invoice.sql: NO undo script
- V002__add_due_date.sql: NO undo script
- V003__utc_offsets_and_status.sql: undo present
...
## Obsolete types and their remaining callers
- `SqlHelper` (src/Contoso.Billing/Legacy/SqlHelper.cs) | "Use a Dapper repository behind an interface. ..." | callers: src/Contoso.Billing/Legacy/MonthlyRevenueReport.cs
## Existing AI-layer files
- AGENTS.md: missing
- .claude: present
## Docs that name things the code no longer has
- `docs/ARCHITECTURE.md` (last edited 2021-06-10):
  - path `Billing.sln` does not exist
```

`dotnet test Contoso.Billing.sln` passes 6 tests. The scan gives leads, the tests give rung 1, the ADR gives rung 3; the reference findings table in [`solution/engagement-record.md`](../../labs/module-21/solution/engagement-record.md) has eight findings, seven confirmed, and one hypothesis: `.claude/` exists and may hold one developer's personal settings, which you test by asking him.

The baseline, from the 12-week export ([`client/data/baseline-tickets.csv`](../../labs/module-21/client/data/baseline-tickets.csv)):

```bash
cd labs/module-21
dotnet run --project ../module-13/tools/ImpactStats -- describe client/data/baseline-tickets.csv --metric cycle_hours --by size
dotnet run --project ../module-07/tools/EvalHarness -- stats client/data/eval-results.csv --config before-ai-layer
```

```text
group                 n   median     p25     p75     p90     mean  geomean  sd(log)     max
L / baseline         10     45.5    32.8    52.1    74.1     49.6     44.1     0.49   118.1
M / baseline         33     20.4    13.9    27.5    39.7     22.4     20.1     0.47    45.4
S / baseline         17      9.2     7.6    11.4    13.1     10.0      9.5     0.33    20.3

mean over 24 tasks: pass@1 0.533 | pass@3 0.908 | pass^3 0.158
task-clustered: mean of 24 task rates 53%, SE 0.046, 95% CI 44%-63%
```

Sixty tickets, median 18.5 hours overall, escaped defects 6 of 60 (10%, Wilson 95% interval 5% to 20%). The weighted within-size `sd(log)` is about 0.44, which gives the 38% detectable effect above. The agent passes 53% of the convention tasks without a layer.

Now the flawed version ([`break/21.2-late-baseline`](../../labs/module-21/break/21.2-late-baseline/engagement-record.md)). Before running: which of its findings would teach the agent the wrong thing?

```text
ERROR Data access goes through SqlHelper.ExecuteDataSet: marked confirmed on one source; ...
ERROR Timestamps use DateTime.Now: marked confirmed on rungs 5,7; confirmed needs two sources, one from rungs 1-3
ERROR PR review time: captured 2026-10-30, on or after the build started (2026-10-19): it measures the intervention, not the before
ERROR PR review time: a per-person metric; measure tickets and teams, never rank people
ERROR PR review time: 2-week window; take at least 6 (ideally 12) weeks ...
ERROR no guardrail metric (escaped defects, change failure, reverts, rework): ...
11 error(s), 10 warning(s)
```

Three of its four findings are wrong, all from the 2021 page, and each would become a rule telling the agent to use `SqlHelper`, `DateTime.Now` and `Billing.sln`. Its "baseline" was taken in engagement weeks 3–4, per developer, from 9 PRs, as a mean without a spread.

## Try it

Budget: 120 minutes.

1. **Audit (45 min).** Create a `before-ai-layer` worktree of your `brownfield-demo`. Run `scan`, run the tests, read the ADR and history. Write at least six findings with rungs, a confirmed/hypothesis label and a client owner (use the names in the brief).
2. **Baseline (30 min).** `describe` cycle time and review time by size; count escaped defects and compute their Wilson interval ([07.4](../module-07/lesson-04.md)); `stats` the eval baseline. Fill the baseline table with capture dates before 2026-10-19.
3. **Detectable effect (15 min).** Compute it by hand from the equation, then check with `power`. Write the sentence you will say to the sponsor.
4. **Pre-registration (20 min).** Fill the [experiment design template](../../templates/experiment-design.md) for the pilot: outcomes, arms, randomization by size, detectable effect, stop rule.
5. **Check (10 min).** `EngageCheck baseline` until clean.

<details>
<summary>Hint: the sponsor sentence</summary>

"With four weeks on one team, we can only see a change of about 38% or more in cycle time. If the real effect is smaller, the result will read 'inconclusive', and that will be a true result, not a failed engagement. The eval comparison and adoption are measured more precisely, so you will have evidence either way."
</details>

## Break it

> [!WARNING]
> In a real engagement, ticket and PR exports contain names, customer references and internal links. Ask for them with developer names replaced by codes, as the SOW's data clause says, and keep them only on the machine the SOW allows.

Copy `client/data/baseline-tickets.csv` and keep only the last two weeks (ISO 39–40). Run `describe`. Then change the baseline window in your record to "2 weeks" and the capture date to 2026-10-26. Run `baseline`. How different is the two-week median from the 12-week one, and what would that have done to the result in week 11?

## Fix it

**Diagnose.** The late baseline fails in time (after the build started), in shape (per person, no spread, no guardrail) and in size (2 weeks, 9 PRs). The docs-only audit fails at confirmation: every wrong finding rests on rung 5 or 7, and the stronger rungs contradict it.

**Modify.** Rerun the audit with triangulation and client owners. Take the baseline from the 12-week history, by ticket, with the spread and the guardrail, captured in week 1. Add the eval and usage baselines. Compute the detectable effect and put it in D2.

**Rerun.** `EngageCheck baseline` on the reference record: 0 errors, 0 warnings.

<details>
<summary>Solution: why history beats a prospective baseline here</summary>

Tickets finished before you arrived were not influenced by your presence, the team's awareness of being measured, or the first layer PRs. A prospective baseline in weeks 1–2 would be short (two weeks), overlapping with kickoff disruption, and still before any randomization; the history gives 12 weeks for free.
</details>

## How do I know it works?

- [ ] Every finding has at least two sources with one from rungs 1–3, or is labelled a hypothesis with a test; every finding has a client owner.
- [ ] You ran the build and tests yourself.
- [ ] Every baseline metric has a definition, source, window of at least 6 (ideally 12) weeks, a spread, and a capture date before the build start.
- [ ] There is a quality guardrail, an eval baseline and a usage baseline; nothing is per person.
- [ ] The detectable effect is computed and written into the record and D2.
- [ ] `EngageCheck baseline` is clean.

## Use / don't use

**Use** historical exports for the baseline whenever the client's tracker has status history. **Use** the detectable effect to choose what else to measure.

**Don't** start the build before the baseline and D2 are signed, even when the client is impatient; a week's delay is cheaper than an engagement with no before. **Don't** audit from the wiki. **Don't** accept a PR-review clock as cycle time ([13.1](../module-13/lesson-01.md)).

**Limitations.**

- A historical baseline inherits whatever changed in those 12 weeks (a holiday, a release, a reorganization); note those events.
- The detectable effect assumes the pilot's spread will match the baseline's and that sizes are balanced; it is a planning estimate, not a guarantee of power.
- Large experienced-developer trials have found effects in both directions ([Becker et al.](https://arxiv.org/abs/2507.09089) measured a 19% slowdown); a small engagement cannot settle that question, and should not pretend to.
- Escaped defects need a defect window (30 days in the SOW) before they can be counted, which is why D5 is in week 11, not week 7.

## Reflect

1. What is the oldest document in your own repository that an agent would treat as current?
2. How many tickets does your team finish in four weeks, and what effect could that detect?
3. Which baseline would you most regret not having taken, if you only noticed it in week 6?

## Sources

- [DORA — software delivery performance metrics](https://dora.dev/guides/dora-metrics/) — throughput (change lead time, deployment frequency, failed deployment recovery time) paired with instability (change fail rate, deployment rework rate).
- [Forsgren et al. (2021), The SPACE of Developer Productivity](https://www.microsoft.com/en-us/research/publication/the-space-of-developer-productivity-theres-more-to-it-than-you-think/) — productivity spans several dimensions and levels; no single metric captures it.
- [Becker et al. (2025)](https://arxiv.org/abs/2507.09089) — randomized trial with experienced developers; measured slowdown despite forecast speed-up.
