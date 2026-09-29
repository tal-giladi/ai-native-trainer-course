---
id: "13.4"
module: 13
minutes: 17
practice_minutes: 60
prerequisites: ["13.3"]
objectives:
  - Name the main threats to validity in an engineering comparison (selection, task difficulty, developer differences, learning, Hawthorne, regression to the mean, version changes, contamination, measurement) and the direction each would bias the result.
  - Compute the regression to the mean expected for a team selected for an extreme quarter.
  - Run a difference-in-differences comparison with placebo units, and explain when its parallel-trends assumption fails.
  - Write a threat register with a design mitigation and an analysis check for each threat.
volatility: concept
sources:
  - title: "Barnett, van der Pols and Dobson (2005) — Regression to the mean: what it is and how to deal with it"
    url: https://academic.oup.com/ije/article-abstract/34/1/215/638499
  - title: "McCambridge, Witton and Elbourne (2014) — Systematic review of the Hawthorne effect"
    url: https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3969247/
  - title: "METR (2026) — We are Changing our Developer Productivity Experiment Design"
    url: https://metr.org/blog/2026-02-24-uplift-update/
  - title: "He, Miller, Agarwal, Kästner and Vasilescu — Speed at the Cost of Quality (difference-in-differences on Cursor adoption)"
    url: https://arxiv.org/abs/2511.04427
  - title: "Hernán and Robins — Causal Inference: What If (free book)"
    url: https://miguelhernan.org/whatifbook
last_verified: "2026-09-28"
---

# 13.4 · Threats to validity

## Why it matters

A randomized design (13.3) removes the biggest threat — selection — but not all of them. And most comparisons you will be shown are not randomized: they are "the pilot team, before and after". Each has a short list of ways to produce a convincing number without any effect at all. Your value in the room is knowing that list by heart, checking it against the data in minutes, and saying which threats remain after the checks.

The fictional post in `labs/module-13/claims/claim-b-cto-post.md` is typical: a pilot team's median cycle time "halved in one quarter" while other teams got slightly slower. By the end of this lesson you will show, with the same data, that the pilot team simply returned to normal.

> [!NOTE]
> Content tags. **Concept** (stable): threats to internal, construct and external validity; regression to the mean; difference-in-differences and placebo tests; intention to treat. **Implementation**: `ImpactStats did` and `compare --where/--exclude`.

## How it works

### Three questions of validity

- **Internal validity**: is the difference caused by the treatment in *this* study? (Selection, regression to the mean, version changes, contamination.)
- **Construct validity**: does the metric measure what you claim? (13.1's PR clock; self-reported time; "velocity" as lines added.)
- **External validity**: does the result transfer to other teams, code bases and tool versions? (13.2's whole debate.)

### The threat register

| Threat | Mechanism | Usual direction | Design mitigation | Analysis check |
|---|---|---|---|---|
| **Selection** | who or what gets the agent is chosen by someone | favours the agent | randomize within blocks | `balance` |
| **Task difficulty** | ticket mix differs between arms or periods | either | block on size set in refinement | per-stratum table |
| **Developer differences** | faster developers adopt first | favours the agent | block on developer | per-developer ratios |
| **Learning effects** | people get better with the tool over weeks; skills spill over to control tickets | effect grows over time; spillover shrinks it | run long enough; alternate arms | early vs late weeks |
| **Hawthorne / novelty** | being observed or having a new toy changes behaviour | favours whatever is new | same observation in both arms; ≥3 weeks | first vs last weeks |
| **Regression to the mean** | a team or period selected for being extreme drifts back | favours the "fix" | don't select on extremes; baseline from ≥2 periods | rank of the selected unit; earlier baselines |
| **Model / agent version change** | the treatment itself changes mid-study | either | pin versions; record per row | compare by version |
| **Contamination** | control arm uses the agent anyway; treatment arm skips it | shrinks the effect | explicit rules; record `used_ai` | intention to treat vs per protocol |
| **Attrition / withholding** | tickets or people leave the study non-randomly | either | count and report every exclusion | compare excluded vs included |
| **Measurement** | clocks, missing timestamps, time reporting while agents run in parallel | either | same clock in both arms (13.1) | count missing rows per arm |

Two of these deserve a closer look because they fool experienced people.

**Hawthorne.** McCambridge et al.'s systematic review found evidence that being studied changes behaviour, but the effects vary and depend on the setting; they recommend thinking in terms of *research participation effects* rather than a single named effect. The practical defence is symmetry: observe both arms the same way, for long enough that novelty wears off.

**Selection that happens after you designed the study.** METR's 2026 update is a lesson in humility from careful researchers: in their late-2025 continuation, developers increasingly declined to participate because they did not want to work without AI, and 30–50% said they were not submitting some tasks because they did not want to do them without AI. Randomization was intact on paper; the tasks that reached randomization were no longer representative, and METR concluded their estimate was likely biased toward *less* speed-up. They also found time reports unreliable for developers running several agents at once. Your threat register must include the ways people will route around your design.

### Regression to the mean

*Intuition.* Every team's quarterly cycle time is its long-run level plus luck. Pick the team with the worst quarter and you have almost certainly picked a team with bad luck that quarter. Next quarter its luck is average again, so it "improves" — whatever you did to it.

*Equation.* With the organization mean $\mu$ and $r$ the correlation between one quarter and the next across teams, the expected next value of a unit observed at $x_1$ is

$$E[x_2] = \mu + r\,(x_1 - \mu)$$

so the expected "improvement" with no treatment is $(1 - r)(x_1 - \mu)$. Barnett et al. show the effect grows with measurement noise (smaller $r$) and with how extreme the selection was.

*Tiny example.* In `prepost-teams.csv` the 12 teams' 2026-Q2 mean is 26.1 h and quarter-to-quarter correlations run 0.4–0.6. Forms had 45.2 h, the worst. With $r = 0.45$: $E[x_2] = 26.1 + 0.45 \times 19.1 = 34.7$ h — an expected 23% "improvement" with no intervention at all.

*Interpretation.* Never evaluate an intervention on units chosen because they were extreme. If you must (the worst team *is* the one that needs help), take the baseline from several earlier periods, not the one that got the team selected.

### Difference-in-differences

*Intuition.* Compare the pilot team's change with other teams' change over the same period. Whatever affected everyone (a holiday, a platform outage, a new release process) cancels.

*Equation* (on the log scale, so the result is a ratio):

$$\text{DiD} = \big(\ln T_{\text{post}} - \ln T_{\text{pre}}\big) - \frac{1}{k}\sum_{j}\big(\ln C_{j,\text{post}} - \ln C_{j,\text{pre}}\big)$$

*Placebo check.* Pretend each comparison team was the treated one and compute its DiD the same way. If the real DiD is not more extreme than most placebos, the data cannot distinguish it from ordinary team-to-team movement. With $k$ comparison teams the smallest possible placebo p-value is $1/(k+1)$ — 0.08 with 11 teams.

*Assumption.* **Parallel trends**: without the treatment, the treated team would have moved like the others. He et al. use a careful version (matched control repositories, a modern staggered-adoption estimator) to study Cursor adoption. The assumption fails exactly when the treated unit was selected *because* its pre-period was unusual — which is the break below.

## Show me

Checking the randomized Contoso experiment (`contoso-randomized.csv`) against its threat register:

| Threat | Check | Result |
|---|---|---|
| Selection, difficulty | `balance --by size` | 20/20, 20/20, 8/8 — balanced by construction |
| Version change (week 7) | `compare ... --where "agent_version=agent 1.8"` and `1.9` | −17% [−31%, −2%] and −15% [−27%, −1%]: consistent |
| Learning | weeks 1–6 vs 7–12 | same split as the version change — **the two threats are confounded in time** and cannot be separated in this data; say so |
| Contamination | 3 manual tickets used the agent; ITT vs `--arm used_ai` | −16% [−25%, −5%] vs −13% [−23%, −2%] |
| Outlier / measurement | `--exclude ticket=BILL-503` (blocked on DBA) | −13% [−22%, −3%] |

```text
$ dotnet run --project tools/ImpactStats -- compare data/contoso-randomized.csv --metric cycle_hours --strata size --arm used_ai --a no --b yes
stratified ratio of geometric means B/A: 0.868 (-13%), bootstrap 95% CI [0.770, 0.977] = [-23%, -2%]
```

The primary analysis stays **intention to treat** (by assigned arm): it compares the groups the coin made, so it keeps the protection of randomization. Per protocol (by actual use) compares groups that people chose into, which reintroduces selection; it is a sensitivity check, not the answer. Here contamination dilutes the effect slightly, the direction you would expect.

## Try it

Budget: 60 minutes.

1. Fill section 6 (threat register) of your `experiment-01-prereg.md`: at least eight threats, each with direction, mitigation and check.
2. Reproduce the Show me table on `contoso-randomized.csv`. For the learning row, write the sentence you would put in the report.
3. Run `did` on `prepost-teams.csv` for a team that was **not** the pilot (for example `--treated Billing --pre 2026-Q2 --post 2026-Q3`). What does a placebo DiD look like?

<details>
<summary>Hint: separating learning from the version change</summary>

You can't, in this data: both happen at week 7. In a real design you would either pin the version for the whole study (and record any forced upgrade as a deviation), or schedule the upgrade in the middle of a block so both arms straddle it equally — which they do here, so the *comparison* is still fair even though the two explanations for any time trend are not separable.
</details>

## Break it

The data behind Claim B (`labs/module-13/data/prepost-teams.csv`, illustrative). The VP asks for "something more rigorous than before/after", so an analyst runs a difference-in-differences with 11 comparison teams:

```text
$ dotnet run --project tools/ImpactStats -- did data/prepost-teams.csv --treated Forms --pre 2026-Q2 --post 2026-Q3
median_cycle_hours: Forms 2026-Q2 -> 2026-Q3: 45.2 -> 23.3 h (-48%)
other 11 teams, mean change on the log scale: +11%
difference-in-differences (ratio): 0.464 (-54%)
placebo DiDs for the other teams: -22%, -20%, -4%, -3%, -3%, -2%, +14%, +16%, +16%, +44%, +77%
0 of 11 placebo teams are at least as extreme (placebo p = 0.08)
```

A 54% DiD, more extreme than every placebo. Before reading on: what did the analyst not check?

## Fix it

**Diagnose.** The tool's last line, which the analyst skipped:

```text
selection check: Forms ranked 1 of 12 on median_cycle_hours in 2026-Q2 — the most extreme value; expect regression to the mean.
```

Forms was piloted *because* 2026-Q2 was the worst quarter in the organization. Its history: 21.9 h (2025-Q4), 22.0 h (2026-Q1), **45.2 h** (2026-Q2), 23.3 h (2026-Q3). The pre-period chosen for the DiD is the one bad-luck quarter, so the parallel-trends assumption fails: without any agent, Forms was expected to fall back far more than the other teams. DiD removes what is common to all teams; it cannot remove a shock specific to the unit you selected.

**Modify.** Take the baseline from periods *before* the selection:

```text
$ dotnet run --project tools/ImpactStats -- did data/prepost-teams.csv --treated Forms --pre 2026-Q1 --post 2026-Q3
median_cycle_hours: Forms 2026-Q1 -> 2026-Q3: 22.0 -> 23.3 h (+6%)
difference-in-differences (ratio): 1.005 (+1%)
11 of 11 placebo teams are at least as extreme (placebo p = 1.00)

$ dotnet run --project tools/ImpactStats -- did data/prepost-teams.csv --treated Forms --pre 2025-Q4 --post 2026-Q3
difference-in-differences (ratio): 1.039 (+4%)
```

Against either earlier baseline, Forms moved like everyone else: +1% and +4%. (In the simulation the true effect is zero; see `data/README.md`.)

For the design going forward: don't pick pilot teams for extreme numbers; if several teams need help, randomize which gets the pilot first; better still, randomize tickets within the team (13.3). Add to your threat register: *"Baseline = mean of at least two periods before the selection decision."*

**Rerun.** Rewrite Claim B: *"Forms' cycle time returned from an unusually slow Q2 to its usual level (22 h → 23 h compared with the two quarters before). We cannot attribute any change to the agent pilot."*

[Simulation: Measurement — selection bias and regression to the mean](../../simulations/measurement/index.html?preset=selection-bias)

<details>
<summary>Solution notes: why the placebo test did not save the analyst</summary>

The placebo test asks whether Forms' change is unusual *compared with teams that were not selected*. It was — because Forms was selected for an unusual quarter and none of the placebo teams were. A placebo that respects the selection would ask: "among teams whose quarter was their worst in a year, how much did they improve next quarter?" With 12 teams you rarely have enough such teams; that is why the fix is a baseline from earlier periods, not a cleverer test.
</details>

## How do I know it works?

- [ ] Your threat register lists at least eight threats with direction, mitigation and check, and at least one "residual risk" you cannot remove.
- [ ] You can compute the expected regression to the mean for a selected unit from $\mu$, $r$ and $x_1$.
- [ ] Your primary analysis is intention to treat; per protocol is labelled as a sensitivity check.
- [ ] Every before/after number you report says which periods form the baseline and why they were not selected on the outcome.

## Use / don't use

**Use** the register in every pre-registration and every report. **Use** DiD when you cannot randomize and have several comparison units and several pre-periods; always print the placebo DiDs and the selection rank.

**Don't** evaluate a pilot on the unit or period that was chosen for being worst (or best). **Don't** treat per-protocol results as the effect of the policy "we give developers the agent" — that policy's effect includes the people who don't use it. **Don't** claim you "controlled for" threats that you only listed.

**Limitations.**

- A register makes threats visible; it does not remove them. Some (no blinding, learning vs version change) remain in every practical design and belong in "what this does not show".
- With a dozen teams, DiD and placebo tests have little power: the smallest achievable placebo p-value is $1/(k+1)$.
- External validity cannot be checked inside one study; only replication in other teams can.

## Reflect

1. Which pilot or rollout in your organization was chosen because a team was struggling — and how was its success measured?
2. Which threat in the register would your own team's data be most exposed to?
3. What would you need to see to believe Claim B?

## Sources

- [Barnett, van der Pols and Dobson (2005) — Regression to the mean](https://academic.oup.com/ije/article-abstract/34/1/215/638499) — natural variation in repeated data can look like real change; larger with more measurement error and when follow-up is limited to units selected on a baseline value; design and analysis remedies.
- [McCambridge, Witton and Elbourne (2014) — Systematic review of the Hawthorne effect](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3969247/) — evidence of research participation effects, variable in size and conditions; new concepts needed.
- [METR (2026) — We are Changing our Developer Productivity Experiment Design](https://metr.org/blog/2026-02-24-uplift-update/) — developers declining to work without AI, 30–50% withholding tasks, unreliable time reports with concurrent agents; estimates likely biased downward.
- [He et al. — Speed at the Cost of Quality](https://arxiv.org/abs/2511.04427) — difference-in-differences with matched controls on 807 Cursor-adopting repositories.
- [Hernán and Robins — Causal Inference: What If](https://miguelhernan.org/whatifbook) — intention-to-treat vs per-protocol effects; exchangeability; the assumptions behind comparisons of changes.
