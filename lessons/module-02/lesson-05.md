---
id: "02.5"
module: 2
minutes: 16
practice_minutes: 90
prerequisites: ["02.1", "02.2", "02.3", "02.4"]
objectives:
  - Classify an agent failure into one of ten classes, naming both its primary cause and why it escaped, with a quoted line of trace evidence.
  - Compute cost per successful task, including the human cost of failures, and explain why it can reverse a ranking by token price.
  - Build a model-selection matrix for three real tasks across at least three models from at least two providers, with hard constraints applied before weights.
  - Explain why a public benchmark score cannot replace a measurement on your own tasks.
volatility: concept
sources:
  - title: "Why Language Models Hallucinate (Kalai et al., 2025)"
    url: https://arxiv.org/abs/2509.04664
  - title: "SWE-bench: Can Language Models Resolve Real-World GitHub Issues? (Jimenez et al., 2024)"
    url: https://arxiv.org/abs/2310.06770
  - title: "Building Effective AI Agents (Anthropic Engineering, 2024)"
    url: https://www.anthropic.com/engineering/building-effective-agents
  - title: "Models overview (Claude API docs)"
    url: https://platform.claude.com/docs/en/models/overview
  - title: "OpenAI compatibility (Ollama docs)"
    url: https://docs.ollama.com/api/openai-compatibility
last_verified: "2026-09-28"
---

# 02.5 · Failure taxonomy and model selection

## Why it matters

"The AI got it wrong" is not a diagnosis, and "we use the best model" is not an architecture decision. Both sentences are everywhere, and both are where a trainer earns their fee by replacing them. The first becomes: *"This was insufficient context — the schema wasn't in the trace — and it escaped because nothing compiled against the database."* That sentence tells the team exactly what to fix. The second becomes: *"For ticket triage we use the small model: 19/20 on our tickets, 0.8 s, a tenth of the cost. For migrations we use the large one, because a failure costs 40 minutes of review."*

This lesson closes Module 2 by turning the mechanisms you have seen — tokens and position (02.1), sampling (02.2), instruction channels (02.3), and the tool loop (02.4) — into two working instruments: a failure taxonomy you apply to transcripts, and a selection matrix you fill with measurements. Both become permanent parts of your practice and your workshop.

> [!NOTE]
> The core ideas in this lesson will stay true for years. There is a fixed list of ways an agent can fail. What matters is the cost of each successful task, not the cost of each attempt. And when choosing a model, you first rule out any that miss a hard requirement, and only then compare the rest with weighted scores.
>
> What will go out of date are model names, prices and benchmark scores, which change within months. Every number you put in a comparison table should carry the date you took it.

## How it works

### Ten failure classes

| # | Class | Mechanism (where you saw it) |
|---|---|---|
| 1 | Hallucination | A plausible continuation with no source; training and benchmarks reward guessing over "I don't know" |
| 2 | Incorrect reasoning | Correct facts in context, wrong inference from them |
| 3 | Incomplete reasoning | Stopped early: one caller of two, happy path only |
| 4 | Stale context | An outdated fact in context was used (02.1 multi-needle) |
| 5 | Insufficient context | The needed fact was absent, so the model filled the gap |
| 6 | Conflicting context | Two sources disagreed and the wrong one won (02.3) |
| 7 | Tool failure | A tool errored or returned bad data (02.4 `toolfail`) |
| 8 | Instruction failure | A clear, present instruction was ignored or misapplied |
| 9 | Planning failure | The approach was wrong from the first step (wrong layer, duplicate service) |
| 10 | Verification failure | Success claimed without checking, or an available check skipped (02.2 "all tests pass") |

On class 1, Kalai et al. (2025) argue that hallucinations persist largely because training and evaluation reward confident guessing: a model that guesses when unsure scores better on most benchmarks than one that abstains. Practically, that means you should expect a guess whenever the context lacks the answer and nothing rewards abstaining — which is why class 1 is so often really class 5.

### Primary cause and escape cause

Classify each failure twice:

- **Primary cause:** the earliest point where a correct system would have diverged.
- **Escape cause:** why nothing caught it. Very often class 10.

```mermaid
flowchart TD
    S["Failure observed"] --> Q1{"Was the needed fact<br/>anywhere in the trace?"}
    Q1 -- no --> C5["5 Insufficient context<br/>(symptom may look like 1)"]
    Q1 -- "yes, in 2+ versions" --> Q2{"Was one version outdated?"}
    Q2 -- yes --> C4["4 Stale context"]
    Q2 -- no --> C6["6 Conflicting context"]
    Q1 -- "yes, once" --> Q3{"Did a tool error or<br/>return wrong data?"}
    Q3 -- yes --> C7["7 Tool failure"]
    Q3 -- no --> Q4{"Did a clear instruction<br/>cover this?"}
    Q4 -- "yes, ignored" --> C8["8 Instruction failure"]
    Q4 -- no --> Q5{"Wrong approach<br/>from the start?"}
    Q5 -- yes --> C9["9 Planning failure"]
    Q5 -- no --> Q6{"Conclusion wrong, or<br/>work unfinished?"}
    Q6 -- wrong --> C2["2 Incorrect reasoning"]
    Q6 -- unfinished --> C3["3 Incomplete reasoning"]
    C5 & C4 & C6 & C7 & C8 & C9 & C2 & C3 --> E{"Was there a check that<br/>would have caught it?"}
    E -- "yes, not run" --> C10["Escape: 10 Verification failure"]
```

A claim with no source anywhere in context, after you have ruled out 5, is a true class 1.

### Selecting a model: constraints, then measurements

**Step 1 — cross out the models you are not allowed to use.** Before you compare anything, ask yes-or-no questions that a model either passes or fails. These are deal-breakers, not points on a scorecard: a model that fails even one is off the list, no matter how smart it is. There is no "it's brilliant, so we'll live with it."

- **What happens to your data?** Does the provider keep your prompts, for how long, and can they train on them? If your contract or your security team says "no storing customer data," a model whose terms allow it is out.
- **Where does it run?** Some companies must keep data inside a certain country, or on their own servers (Module 12 covers this). A model only offered from a data centre abroad fails that test.
- **Can it do what your code needs?** If your agent calls tools, needs answers in an exact JSON shape every time, or sends large jobs in cheap overnight batches, the model's API has to support that. If it doesn't, it doesn't matter how well it writes.
- **Can it hold enough text?** If a typical task means reading 150 pages of code and tickets, a model that only fits 50 pages is out. Count what you really send, not the biggest number you can imagine.
- **For a model you download and run yourself (an "open-weights" model): does its licence allow your use?** Some forbid commercial use or cap the number of users.

Only the models that survive every question move to step 2.

**Step 2 — test the survivors on your own work.** Take 10–20 real tasks from your team, run each remaining model on them, and write down what happens. Don't trust a single try: the same model can pass a task once and fail it the next time, so run each task several times (02.2 shows how). For each model, measure:

- **How often it gets the job done.** Out of all the runs, how many produced a result you would actually accept?
- **How long it takes.** Look at the typical wait, and also at the slowest runs. A model that usually answers in 5 seconds but sometimes takes 2 minutes will feel broken to the person waiting.
- **What one attempt costs.** Count the tokens it reads and writes per task and turn that into money.
- **How much text it really handles.** A model may accept 200 pages but start missing things after 60. Test with inputs the size of yours (02.1 shows the effect).
- **How it copes with your languages.** If your tickets are in Hebrew and your code comments in English, test exactly that mix.
- **How reliably it uses tools.** Out of 100 tool calls, how many were broken (wrong name, missing field, invalid JSON)? 02.4 shows how to count them.

**Step 3 — weight only what is left**, and write down what would change the decision.

Why not just read a leaderboard? SWE-bench, the best-known coding-agent benchmark, is 2,294 issues from 12 popular **Python** repositories. It is a genuine signal about a model's general ability, and it says little about a 15-year-old .NET solution with stored procedures, Hebrew tickets and your team's conventions. Scores also move fast: the original SWE-bench paper's best model resolved 1.96% of issues, a number that was obsolete within a year. And public tasks can leak into training data (Module 7 calls this contamination).

### Cost per successful task

**Intuition.** Nobody pays for tokens; they pay for finished work. A cheap model that fails often is expensive once someone has to review and redo its failures.

**Equation.** With cost per attempt $c$, success probability $p$, and human cost $h$ of reviewing a failed attempt, and assuming failures are detected and retried until success:

$$\text{cost per success} = \frac{c}{p} + \left(\frac{1}{p} - 1\right) h$$

$1/p$ is the expected number of attempts; $1/p - 1$ the expected number of failures.

**Tiny example.** Review of a failed attempt takes 10 minutes at $90/hour, so $h = \$15$.

| | $c$ | $p$ | Tokens: $c/p$ | Failures: $(1/p-1)h$ | Total |
|---|---:|---:|---:|---:|---:|
| Model A (cheap) | $0.12 | 0.60 | $0.20 | $10.00 | **$10.20** |
| Model B (pricier) | $0.40 | 0.90 | $0.44 | $1.67 | **$2.11** |

Model B costs 3.3× more per attempt and is about 4.8× cheaper per finished task.

**Implementation.** ModelBench prints `$/attempt` and `$/success` (token cost divided by passes); the matrix template adds the failure term.

**Interpretation.** When a human reviews failures, pass rate usually dominates price. The ranking flips back only when $h$ is near zero (a deterministic check discards failures automatically) or volume is huge — so measure $h$ honestly instead of assuming it.

## Show me

Five short failures, classified. Each would appear in the log with its quoted evidence line.

| Trace excerpt | Primary | Escape | Why |
|---|---|---|---|
| Agent writes `o.CustomerNumber`; no schema or SQL file was read in the trace; the column does not exist | 5 Insufficient context (symptom: 1) | 10 — no build or query against the schema was run | The fact was never in context; the model guessed a plausible name |
| Agent reads the planted README (02.3), then uses `AppDbContext` in Orders | 6 Conflicting context | 10 — the architecture gate did not exist yet | Two sources disagreed; the recent, rule-framed one won |
| Agent answers "retry count 5" in the multi-needle test (02.1) | 4 Stale context | — (question-answering; no check possible) | A never-deployed proposal was retrieved instead of the current value |
| Agent fixes the date filter in the stored procedure but not the matching filter in `OrderService` | 3 Incomplete reasoning | 10 — the integration test covering both was not run | The first fix was right; the search for other occurrences stopped early |
| Agent adds a new `OrderLookupService` duplicating `OrderService` | 9 Planning failure | Reviewer caught it (the system worked) | The plan never looked for existing services — Module 5's research phase |

Notice how rarely the honest primary class is 1, and how often the escape class is 10. That pattern is the argument for everything in Modules 4, 5 and 7.

## Try it

1. **Taxonomy.** Collect ten real failures: from your own agent sessions this month, the lesson 02.4 "invoice table" question, and your 02.3 D outcomes. Classify each with the [failure taxonomy sheet](../../templates/agent-failure-taxonomy.md): primary, escape, one quoted line of evidence. Save as `ai-layer-lab/fundamentals/failure-taxonomy.md`.
2. **Selection.** Configure ModelBench with at least three models from at least two providers; a local open-weights model via Ollama counts as a provider and gives you a self-hosted data point. Check the pipeline first, then run:

   ```bash
   cd labs/module-02/05-model-selection
   dotnet run -- --fake        # verifies config, grading and arithmetic at zero cost
   dotnet run                  # real run, writes results.csv
   ```

   The three starter tasks (a SQL risk review, a sync-over-async deadlock fix, a Hebrew ticket triage) are there to get you running; replace them with three tasks from your real backlog before you draw conclusions.
3. Fill in the [model-selection matrix](../../templates/model-selection-matrix.md): constraints first, then measured rows, then a decision per task with a re-measure date.

[Simulation: Agent loop — where runs fail and which class it is](../../simulations/agent-loop/index.html?preset=failure-classes)

> [!WARNING]
> Sending employer or client code to a provider is a data-handling decision, not a lab detail. Use the starter tasks, or tasks from a repository you are allowed to send to every provider in your matrix.

## Break it

Make the decision the way it is usually made, then check it:

1. Set `"runsPerTask": 1`. Run once. Pick a winner per task from that single run — and, separately, pick the model with the lowest price per million tokens.
2. Set `"runsPerTask": 5` and run again.
3. Compare: did any single-run winner lose at N=5? Did the cheapest-per-token model stay cheapest per success once you add a realistic $h$ for your team?

A decision that flips between N=1 and N=5 was never a decision; it was a sample of one from the distribution in lesson 02.2.

## Fix it

- **Apply hard constraints before looking at any score**, and write down which rows eliminated which models. This is the part of the matrix a security reviewer will read.
- **Decide on N ≥ 5 per task**, with the pass criterion written before the first run. Where two models are within one or two passes of each other, record "no measurable difference at N=5" rather than a winner; Module 7 gives you the intervals to say how big N must be.
- **Rank by cost per success including $h$**, measured from your team's real review time, not by token price.
- **Allow different models per task.** Triage, code review and migrations have different $p$, $h$ and latency needs. One model everywhere is a convenience, not a requirement.
- **Date everything and set a re-measure date.** A matrix without dates is a rumour.

## How do I know it works?

- A colleague given your `results.csv` and matrix reaches the same decision without talking to you.
- Every eliminated model has a named constraint; every chosen model has a measured row at N ≥ 5.
- Your taxonomy has ten classified failures, each with primary, escape and quoted evidence, and at least one where you initially wrote "hallucination" and changed it after checking the trace.
- The decision includes a sentence of the form "we would switch if …", with a number in it.

## Use / don't use

**Use** the taxonomy in post-mortems, code review of agent PRs and workshop Q&A — it turns blame into a fix list. **Use** the matrix whenever a team standardises on a model, adds a task type, or a provider changes price or version.

**Don't** pick models from leaderboards, launch posts or a single impressive demo. **Don't** turn a deal-breaker into points on a scorecard ("privacy: 20%"). Think of hiring someone who has no work permit: you don't take off 20 points for the permit and hire them anyway because they aced the interview. You just can't hire them. A model that isn't allowed to see your code is the same: it isn't "a little worse", it's off the list. **Don't** classify a failure without evidence from the trace.

**Limitations.** This lab has three weak spots.

- **The checks are rough.** The starter tasks decide pass or fail just by looking for a certain word or phrase in the answer. A wrong answer can contain the word and pass, and a right answer worded differently can fail. Module 7 brings better checks.
- **Five runs only catch big gaps.** Running each task 5 times tells you something only when two models are far apart, like 1 pass out of 5 against 5 out of 5. If they're close, like 3 against 4, it may just be luck.
- **Naming the cause is a judgement call.** Two people can honestly pick different causes for the same failure. That's why every classification comes with the line of evidence behind it: then the two people argue about something concrete they can both check, not just their opinions.

## Reflect

Write three lines in your learning log:

1. The most common primary class and the most common escape class in my ten failures.
2. The model I would have chosen before measuring, and the one I chose after.
3. The value of $h$ (cost of one failed attempt) for my team, and how I measured it.

## Sources

- Kalai et al., [Why Language Models Hallucinate](https://arxiv.org/abs/2509.04664) — training and evaluation incentives reward guessing over abstaining.
- Jimenez et al., [SWE-bench](https://arxiv.org/abs/2310.06770) — 2,294 issues from 12 Python repositories; best original model resolved 1.96%.
- Anthropic Engineering, [Building Effective AI Agents](https://www.anthropic.com/engineering/building-effective-agents) — measure before adding complexity.
- Claude API docs, [Models overview](https://platform.claude.com/docs/en/models/overview) — example of the dimensions a vendor publishes (context, latency class, price) and how quickly they change (as of 2026-09).
- Ollama docs, [OpenAI compatibility](https://docs.ollama.com/api/openai-compatibility) — running a local open-weights model behind the same API shape.
