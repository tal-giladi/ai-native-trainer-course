---
id: "07.4"
module: 7
minutes: 17
practice_minutes: 60
prerequisites: ["07.3", "02.2"]
objectives:
  - Compute a Wilson 95% interval for a pass rate and explain why it replaces the textbook $\hat p \pm 1.96\,\text{SE}$ at small n and extreme rates.
  - Distinguish pass@k from pass^k, estimate both from n trials per task, and choose which one a given use of an agent needs.
  - Estimate how many trials a question needs with $n \approx z^2 p(1-p)/h^2$, and explain why more tasks beat more repeats of the same task.
  - Recognize pseudo-replication and compute a task-clustered interval.
volatility: concept
sources:
  - title: "Brown, Cai and DasGupta (2001) — Interval Estimation for a Binomial Proportion"
    url: https://projecteuclid.org/journals/statistical-science/volume-16/issue-2/Interval-Estimation-for-a-Binomial-Proportion/10.1214/ss/1009213286.full
  - title: "Miller (2024) — Adding Error Bars to Evals: A Statistical Approach to Language Model Evaluations"
    url: https://arxiv.org/abs/2411.00640
  - title: "Chen et al. (2021) — Evaluating Large Language Models Trained on Code (pass@k)"
    url: https://arxiv.org/abs/2107.03374
  - title: "Yao et al. (2024) — τ-bench: A Benchmark for Tool-Agent-User Interaction in Real-World Domains (pass^k)"
    url: https://arxiv.org/abs/2406.12045
  - title: "Anthropic Engineering — Demystifying evals for AI agents"
    url: https://www.anthropic.com/engineering/demystifying-evals-for-ai-agents
last_verified: "2026-09-28"
---

# 07.4 · Statistics for stochastic systems

## Why it matters

Since Module 2 the course has told you to say "held" or "clearly improved" and to wait for Module 7 before quoting a number ([02.2](../module-02/lesson-02.md), [04.5](../module-04/lesson-05.md)). This is where the waiting ends. You will attach an interval to every pass rate, choose between two reliability metrics that answer different questions, and know before you spend the API budget how many trials a question needs.

None of it is exotic. It is the statistics of coin flips, plus one correction that most published agent evals still skip: trials of the same task are not independent, so "100 trials" is often much less evidence than it sounds.

> [!NOTE]
> Content tags. **Concept** (stable): binomial variation, Wilson intervals, pass@k and pass^k, sample size, clustering by task. **Implementation**: `EvalHarness stats` and `runs`. Bootstrap and permutation tests for engineering metrics come in Module 13.

## How it works

### One pass rate, one interval

*Intuition.* A task set run $n$ times gives $k$ passes. $\hat p = k/n$ is an estimate; run again and you get a different $k$. The interval is the range of true rates that could plausibly have produced your $k$.

*The textbook interval and its problem.* $\hat p \pm 1.96\sqrt{\hat p(1-\hat p)/n}$ (the Wald interval) misbehaves exactly where agent evals live: small $n$, and rates near 0 or 1. For 5/5 it gives $[100\%, 100\%]$, claiming certainty from five coin flips. Brown, Cai and DasGupta showed its coverage is erratic far beyond the cases textbooks warn about, and recommend the **Wilson** interval for small $n$.

*Equation (Wilson, 95%, $z = 1.96$).*

$$\frac{\hat p + \frac{z^2}{2n} \pm z\sqrt{\frac{\hat p(1-\hat p)}{n} + \frac{z^2}{4n^2}}}{1 + \frac{z^2}{n}}$$

*Tiny example.* 14/20: $\hat p = 0.7$, centre $= (0.7 + 0.096)/1.192 = 0.668$, half-width $= 1.96\sqrt{0.0105 + 0.0024}/1.192 = 0.187$, so **48%–85%**. And 12/20 gives **39%–78%**. The two intervals overlap over most of their width. 5/5 gives **57%–100%**, and 0/5 gives **0%–43%**.

*Implementation.* `EvalHarness stats` prints the Wilson interval for all tasks, per split, and for golden tasks.

*Interpretation.* Twenty trials locate a pass rate within roughly ±18 points. That is enough to tell 30% from 80%, and not enough to tell 60% from 70%.

### pass@k and pass^k

[02.2](../module-02/lesson-02.md) gave you $1-(1-p)^k$ ("at least one of $k$ succeeds") and $p^k$ ("all $k$ succeed"). They now get their names:

- **pass@k** — the probability that at least one of $k$ attempts passes. It is the right metric when a **verifier** picks the winner: generate three candidate fixes, run the tests, keep the one that passes. Chen et al. introduced it for code generation, with an unbiased estimator from $n \ge k$ trials with $c$ passes: $\text{pass@}k = 1 - \binom{n-c}{k}/\binom{n}{k}$.
- **pass^k** — the probability that all $k$ attempts pass. It is the right metric for **reliability**: an unattended agent in CI runs once per PR, every PR, and each failure costs someone's time. Yao et al. introduced it with τ-bench, where even the best function-calling agent of the time succeeded on under 50% of tasks and had pass^8 under 25% in the retail domain. Estimator: $\text{pass}^k = \binom{c}{k}/\binom{n}{k}$.

*Tiny example.* A task with 4 passes in 5 trials: pass@3 $= 1 - \binom{1}{3}/\binom{5}{3} = 1$ (any three trials include a pass) and pass^3 $= \binom{4}{3}/\binom{5}{3} = 4/10 = 0.4$. Anthropic's guide gives the population version: at 75% per trial, pass^3 is about 42%.

*Interpretation.* The same results can be reported as "pass@3 = 100%" or "pass^3 = 40%". Neither is wrong; they answer different questions. Report the one your use case pays for, and name it.

### How many trials?

*Equation.* To estimate a rate near $p$ within $\pm h$ at 95%: $n \approx z^2 p(1-p)/h^2$.

*Tiny example.* $p = 0.7$, $h = 0.10$: $3.84 \times 0.21 / 0.01 \approx 81$ trials. For $\pm 5$ points: about 323. To detect a difference between two *independent* arms, say 70% → 85% with 80% power, `EvalHarness runs --p 0.7 --delta 0.15` gives about 118 trials per arm. A paired design needs fewer (07.5).

*Implementation.* `EvalHarness runs --p 0.7 --halfwidth 0.1` prints $n$ and a table of Wilson widths from 5 to 400 trials.

### Trials are clustered by task

The binomial formulas assume independent trials. Trials of the *same task* are not independent: an easy task passes almost every time, a task that needs a missing fact fails almost every time. What you actually sampled is a set of tasks from all the tasks you care about, and each task's rate is measured with some noise. Miller (2024) makes this the central point of "Adding Error Bars to Evals": treat questions as drawn from a super-population and use **clustered** standard errors.

*Equation.* With $T$ tasks and per-task pass rates $\hat p_i$:

$$\bar p = \frac{1}{T}\sum_i \hat p_i \qquad \text{SE}_\text{task} = \frac{s(\hat p_i)}{\sqrt{T}} \qquad \text{CI} = \bar p \pm t_{T-1}\,\text{SE}_\text{task}$$

where $s$ is the sample standard deviation across tasks and $t_{T-1}$ the t critical value for $T-1$ degrees of freedom.

*Interpretation.* When tasks differ a lot, the task-level SE is much larger than the per-trial one, and piling on repeats of the same few tasks barely shrinks it. Past about 3–5 trials per task, **add tasks, not trials.**

```mermaid
flowchart TD
    Q{What do you need?} -->|one success is enough,<br/>a verifier picks it| PA[pass@k]
    Q -->|every run must succeed,<br/>unattended| PH[pass^k]
    Q -->|a rate with honest error| W[Wilson interval<br/>+ task-clustered interval]
    W --> N{Interval too wide?}
    N -->|each task has < 3 trials| MT[More trials per task]
    N -->|each task has ≥ 3–5| MK[More tasks]
```

[Simulation: Evaluation — trials vs interval width](../../simulations/evaluation/index.html?preset=one-run)

## Show me

The 07.1 skill comparison, `skill-v2` at five trials per ticket (illustrative data):

```text
$ dotnet run --project tools/EvalHarness -- stats samples/skill-compare/results.csv --config skill-v2 --k 3
all        54/100  =  54%   95% Wilson [44%, 63%]   width 19%
| S02 | 4/5 | 0.800 | 1.000 | 0.400 |
| S11 | 2/5 | 0.400 | 0.900 | 0.000 |
| S20 | 0/5 | 0.000 | 0.000 | 0.000 |
mean over 20 tasks: pass@1 0.540 | pass@3 0.865 | pass^3 0.190
task-clustered: mean of 20 task rates 54%, SE 0.060, 95% CI 41%-67% | naive per-trial SE 0.050 (ratio 1.2x)
```

Three readings of the same 100 trials: a plan passes about half the time (interval 41–67% once you respect clustering); with a verifier choosing among three attempts you would get a passing plan 87% of the time; and if the skill ran unattended three times in a row on the same kind of ticket, all three would pass only 19% of the time. The "one-run" configuration from 07.1 — 14/20 — has a Wilson interval of 48–85%, which contains the five-trial estimate: the one run was not wrong, just nearly uninformative.

## Try it

Budget: 60 minutes.

1. Compute the Wilson interval for 14/20 and for 0/5 by hand (or in a spreadsheet) and check them against `stats`.
2. Run `stats --k 3` on your own results from 07.1–07.3 (or on `samples/context-reduction/results.csv --config reduced`). For each golden task, write down pass^3. Which golden task would you not yet trust in an unattended CI job?
3. Run `runs --p <your pass rate> --halfwidth 0.1` and `--halfwidth 0.05`. Put the trial count you can afford into `CHARTER.md`, with the half-width it buys.
4. Decide, for each of these uses, whether you report pass@k or pass^k, and why: (a) an agent that proposes three candidate fixes that CI tests; (b) a nightly unattended dependency-update agent; (c) a developer who reruns the agent when it fails.

<details>
<summary>Hint: step 4</summary>

(a) pass@3 — a verifier (CI) selects. (b) pass^k over the number of nights you care about — there is no one to pick the good run. (c) pass@k with k equal to the reruns the developer will tolerate, but also report the cost: each rerun is time, and 02.2's independence assumption often fails (a missing fact fails every rerun).
</details>

## Break it

A teammate's PR description: "Reduced layer: **92% pass rate on 100 trials, 95% CI 85–96%**. Ready to merge." The results are in `labs/module-07/samples/pseudo-replication/results.csv` (illustrative).

Before opening the file: what would you want to know about those 100 trials before believing an 11-point-wide interval?

## Fix it

**Diagnose.**

1. *Look at the structure:* `stats samples/pseudo-replication/results.csv` shows 4 tasks × 25 trials: T01 25/25, T03 25/25, T07 24/25, T12 18/25.
2. *Recompute with the right unit:* the task-clustered line reads **mean 92%, SE 0.067, 95% CI 71%–100%**, with the naive per-trial SE 2.5 times too small. The honest interval is almost four times as wide.
3. *And the bigger problem:* four tasks cannot represent the codebase. T12 at 18/25 is the only informative task, and it is not a golden one. The claim is "the agent reliably answers four questions".

**Modify.** Rerun on the full 24-task set with 3–5 trials per task (72–120 trials, the same budget) and report the task-clustered interval next to Wilson. Add to `CHARTER.md`: "every reported interval names its unit (tasks × trials) and uses the task-clustered interval when tasks have more than one trial".

**Rerun.** Compare the widths: 24 × 5 trials of the reduced layer (`samples/context-reduction`, config `reduced`) gives a task-clustered interval of 80–92% — narrower than 4 × 25, from more *tasks*, not more trials.

<details>
<summary>Solution notes</summary>

This error has a name in experimental science: **pseudo-replication** — treating repeated measurements of the same unit as independent units. It is common in published agent evals too, which is why Miller's paper exists. The fix is not a statistical trick; it is sampling what you want to generalize over. You want to generalize over tickets, so sample tickets.
</details>

## How do I know it works?

- [ ] You can compute a Wilson interval by hand and your numbers match `stats`.
- [ ] Every pass rate in your notes carries an interval and its unit (tasks × trials).
- [ ] You have chosen pass@k or pass^k for each use of your agent, in writing, with the reason.
- [ ] `CHARTER.md` states trials per task, the task count, and the half-width that buys.

## Use / don't use

**Use** Wilson intervals for any single rate; **use** task-clustered intervals whenever tasks have several trials; **use** `runs` before an experiment to size it, not after to explain it. **Use** pass^k for anything unattended.

**Don't** report "p < 0.05" for an eval: report the interval and the effect size, which is what a decision needs. **Don't** keep adding trials to a small task set to shrink an interval: it shrinks the wrong interval. **Don't** compare pass@k of one system with pass^k of another.

**Limitations.**

- All of this assumes the tasks are a fair sample of the work you care about. Statistics cannot fix an unrepresentative task set (07.2).
- Grader error (07.3) adds bias that no interval shows.
- With fewer than about 10 tasks, the t-based clustered interval is itself rough; treat it as a warning, not a precise bound.

## Reflect

1. What is the widest interval hiding behind a number you have quoted this year?
2. Which of your agents' uses is really a pass^k problem that you have been measuring as pass@1?
3. With your budget, which is cheaper for you: more tasks or more trials — and which does your question need?

## Sources

- [Brown, Cai and DasGupta (2001) — Interval Estimation for a Binomial Proportion](https://projecteuclid.org/journals/statistical-science/volume-16/issue-2/Interval-Estimation-for-a-Binomial-Proportion/10.1214/ss/1009213286.full) — the Wald interval's coverage is erratic far beyond the usual warnings; Wilson (or Jeffreys) recommended for small n.
- [Miller (2024) — Adding Error Bars to Evals](https://arxiv.org/abs/2411.00640) — questions drawn from a super-population; CLT and clustered standard errors; paired differences; power analysis.
- [Chen et al. (2021) — Evaluating Large Language Models Trained on Code](https://arxiv.org/abs/2107.03374) — pass@k and its unbiased estimator; repeated sampling raises solved problems from 28.8% to 70.2% with 100 samples.
- [Yao et al. (2024) — τ-bench](https://arxiv.org/abs/2406.12045) — pass^k as a reliability metric; best agents under 50% success and pass^8 under 25% in retail.
- [Anthropic Engineering — Demystifying evals for AI agents](https://www.anthropic.com/engineering/demystifying-evals-for-ai-agents) — pass@k vs pass^k; at 75% per-trial success, pass^3 ≈ 42%.
