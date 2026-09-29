---
id: "21.3"
module: 21
minutes: 16
practice_minutes: 180
prerequisites: ["21.2", "03.1", "06.1", "07.6", "12.5"]
objectives:
  - Separate recommending from deciding, and record each architecture decision with a client decider and an ADR or PR.
  - Plan a build in which the consultant's share of AI-layer changes falls phase by phase, using pairing, then review, then stepping back.
  - Compute the client-authored share of AI-layer changes by thirds of the build and the client truck factor over the AI-layer files, and interpret both.
  - Check a build's PR log with EngageCheck build and change the plan when the team is not the author.
volatility: concept
sources:
  - title: "Team Topologies — Key concepts (enabling teams and the facilitating interaction mode)"
    url: https://teamtopologies.com/key-concepts
  - title: "Avelino, Passos, Hora and Valente (2016) — A Novel Approach for Estimating Truck Factors (ICPC)"
    url: https://arxiv.org/abs/1604.06766
  - title: "Bacchelli and Bird (2013) — Expectations, Outcomes, and Challenges of Modern Code Review (ICSE)"
    url: https://www.microsoft.com/en-us/research/publication/expectations-outcomes-and-challenges-of-modern-code-review/
last_verified: "2026-09-28"
---

# 21.3 · Architecture and build with the team

## Why it matters

Look at how your own `brownfield-demo` got its AI layer ([Module 15](../../labs/module-15/README.md)): seven commits, one author, one week. Grounded rules, loop gates, skills, the eval harness, the schema MCP server, hooks, CI review, a changelog. For a public demo that is exactly right. Delivered that way to a client, it is the most expensive mistake in this module.

Contoso's tech lead told you why in the interview: in 2024 an external contractor configured a static-analysis tool; nobody understood its rules, and it was switched off after four months. An AI layer is more fragile than that. It is prose, skills and gates that must change every time the codebase, the model or the tool changes (Module 11's system-evolution loop). If the only person who has ever written a line of it leaves in week 12, it starts decaying in week 13.

The build phase has one gate that matters more than the layer's quality: **the team wrote most of it.** This lesson shows how to plan for that, how to measure it from the PR log, and what to do when the numbers say you are building for them instead of with them.

> [!NOTE]
> Content tags. **Concept** (stable): recommend vs decide, the enabling stance, the fading scaffold, review as knowledge transfer, client-authored share, truck factor. **Implementation**: `EngageCheck build`, the `prs.csv` format, and the illustrative Contoso PR log.

## How it works

### Recommend, then let them decide

Every architecture choice in the layer is one you have made before: `AGENTS.md` canonical with `CLAUDE.md` importing it ([03.2](../module-03/lesson-02.md)), a schema MCP server in snapshot mode rather than a live database ([Module 8](../../labs/module-08/README.md)), an eval gate on AI-layer changes ([07.6](../module-07/lesson-06.md)). You know the answer. You still do not decide.

The pattern: you write the options and a recommendation as a draft ADR ([12.5](../module-12/lesson-05.md)); the client person who will live with the consequence decides; the record shows both. At Contoso the security lead decides the MCP server's mode, the Platform lead decides whether the eval gate blocks merges, and the Billing tech lead decides where the rules live. A decision recorded as "decided by the consultant" is a decision nobody at the client can defend or change.

### The enabling stance

Team Topologies describes an **enabling team** as one that helps a stream-aligned team overcome obstacles and gain missing capabilities, temporarily, and then moves on; its interaction mode is facilitating, not building on the team's behalf ([Team Topologies](https://teamtopologies.com/key-concepts)). That is the consultant's role in the build. The capability you are transferring is not "has an AI layer"; it is "can change the AI layer on a Tuesday".

### The fading scaffold

Module 16's show–do–reflect ([16.3](../module-16/lesson-03.md)) applied to four weeks of build:

| Build weeks | Who types | Who navigates | Your role |
|---|---|---|---|
| First third | You, in pairs or a mob | Client developers | Model the reasoning out loud; they choose names and wording |
| Middle third | Client developers | You | Pair, then review; you write only what nobody else can yet |
| Last third | Client developers | Client developers | Review when asked; fix nothing silently |

Code review helps, but it is not enough on its own. Bacchelli and Bird found that, although finding defects is the main stated motivation for code review at Microsoft, reviews deliver fewer defect findings than expected and more knowledge transfer and team awareness ([Bacchelli and Bird](https://www.microsoft.com/en-us/research/publication/expectations-outcomes-and-challenges-of-modern-code-review/)). Reviewing your PR makes a developer aware of the layer. Writing the next rule makes her able to change it. Plan for both, in that order.

### Two numbers from the PR log

Keep one row per merged change in `prs.csv`: week, author and side, reviewer and side, area (`ai-layer`, `code`, `ci`, `docs`), mode (`solo`, `pair`, `mob`), files.

**Client-authored share by third.**

*Intuition.* If the scaffold is fading, the share of AI-layer changes written by client developers rises from one third of the build to the next.

*Equation.* For third $t$: $s_t = c_t / n_t$, where $n_t$ is the number of AI-layer changes and $c_t$ those with a client author. Gate: $s_3 \ge 0.7$.

*Tiny example.* Contoso reference log, weeks 3–8: $2/6 = 33\%$, then $5/6 = 83\%$, then $5/6 = 83\%$. The consultant's last change (the office-hours page) was paired with the developer who will run office hours.

**Client truck factor.**

*Intuition.* Avelino and colleagues define the truck factor as the minimal number of developers who would have to leave before a project is incapacitated; in their study of 133 popular GitHub projects, 65% had a truck factor of two or less ([Avelino et al.](https://arxiv.org/abs/1604.06766)). For an engagement, compute it over the AI-layer files, counting only client people, because the consultant is certain to leave.

*Equation (the greedy version).* A client person "knows" a file if they authored a change to it or paired on one. Repeatedly remove the person who knows the most files; the truck factor is the number removed when more than half of the files have nobody left who knows them. If more than half already have no client knower, it is 0.

*Tiny example.* The reference log touches 18 AI-layer files. Remove the tech lead (knows 9): 6 of 18 files have no one left, 33%. Remove the migrations developer (knows 6): 11 of 18, 61%, over half. Truck factor **2**.

*Implementation.* `EngageCheck build <record> --log prs.csv` prints the shares by third, the client authors, the files known only to the consultant, and the truck factor; it fails below 70% in the last third, below two client authors, or below a truck factor of 2.

*Interpretation.* A truck factor of 2 means the layer survives one person leaving, not two. That is why the handover (21.4) names a backup for every owner. And a high client share with a truck factor of 1 means one enthusiastic developer wrote everything; the team still does not own it.

### Quality gates do not change

"With the team" is not "lower quality". Every change goes through the same gates you built: the rules lint (Module 3), skill tests ([06.1](../module-06/lesson-01.md) onward), the eval gate on AI-layer PRs. D3's acceptance criterion in the SOW makes this checkable: the gate runs in the client's CI and fails on two seeded regressions described in the PR.

## Show me

The build as `brownfield-demo` did it ([`break/21.3-built-for-them`](../../labs/module-21/break/21.3-built-for-them/prs.csv)), mapped onto a client's weeks: the seven layer commits of `layer.tsv`, all by the consultant, most pushed without review. Before running: what is the client truck factor?

```bash
cd labs/module-21
dotnet run --project tools/EngageCheck -- build break/21.3-built-for-them/engagement-record.md --log break/21.3-built-for-them/prs.csv
```

```text
ERROR 1 (W03, S): consultant change to the AI layer merged without a client reviewer
...
  third        changes  client  share  pair
  W03-W04          2       0     0%     0
  W05-W06          2       0     0%     0
  W07-W08          3       0     0%     0
ERROR client-authored share of AI-layer changes in the last third is 0% (target 70% or more): ...
      AI-layer files: 29; known only to the consultant: 29; client truck factor: 0
ERROR client truck factor 0: if the consultant leaves, more than half of the AI layer has nobody who has worked on it
ERROR decision "AGENTS.md canonical, CLAUDE.md imports it" decided by the consultant; recommend, and let a client person decide
...
12 error(s), 8 warning(s)
```

Zero. Twenty-nine files, all complete and all correct, and nobody at the client has ever edited one. Every decision is recorded as the consultant's.

The reference build ([`solution/prs.csv`](../../labs/module-21/solution/prs.csv)):

```text
  third        changes  client  share  pair
  W03-W04          6       2    33%     5
  W05-W06          6       5    83%     1
  W07-W08          6       5    83%     2
      client authors of AI-layer changes: Avi Ben-David, Dana Katz, Yossi Mizrahi, Omer Peretz, Lior Ben-Ami, D5
      AI-layer files: 18; known only to the consultant: 0; client truck factor: 2

0 error(s), 0 warning(s)
```

Three things made the difference. In week 3 the tech lead merged the first layer PR himself, after pairing on it. The migrations developer, the skeptic, co-wrote the migration rule and the CI check from her own incident, so the rule she did not trust became one she owns. And the enthusiast's personal prompts went through the skill review and the eval gate like everyone else's, which turned a personal gist into two team skills.

## Try it

Budget: 180 minutes, ideally with one or two peers playing Billing developers.

1. **Decisions (30 min).** For four decisions (rules location, MCP server mode, eval gate, where the enthusiast's prompts go), write a one-paragraph draft ADR with options and your recommendation. Have the role-played client person decide. Fill the decisions table.
2. **Build plan (20 min).** Map the fading scaffold onto weeks 3–8: which changes you type, which they type, which you only review.
3. **Build (2 h).** On a branch of `before-ai-layer`, rebuild a slice of the layer (rules, one skill, the migration check, the eval gate) with your peers at the keyboard for most of it. Log every merged change in `prs.csv`. If you have no peer, work through the reference log instead and write down, for each row, what you would have said while pairing.
4. **Check (10 min).** `EngageCheck build` until clean.

<details>
<summary>Hint: what if the client developers have no time to pair?</summary>

That is a client responsibility in the SOW ("four hours per Billing developer per week in weeks 3–6"). Raise it at the Monday check-in, then with the sponsor in writing. Do not fill the gap by writing the layer yourself; that trades a schedule slip now for a dead layer in March.
</details>

## Break it

> [!CAUTION]
> In a real engagement, the PR log is about named people's work. Use it to check the transfer, never to rate individuals, and keep it in the private record.

In a copy of `solution/prs.csv`, make the tech lead leave in week 6: change the author of PRs 17 and 24 to `S` with `author_side` `consultant` (you "cover" for him). Rerun `build`. What happens to the last-third share and to the truck factor? Then, instead, add a second author to the files only he knows by making PR 24 a `pair` with Dana Katz as reviewer. What changes?

## Fix it

**Diagnose.** In the break, the layer is correct and nobody owns it: 0% client-authored in every third, five changes merged without any client review, no client author, truck factor 0, four decisions taken by the consultant.

**Modify.** Rewrite the plan: draft ADRs decided by client people; pairing in the first third with the client choosing names and wording; client developers typing from the middle third; your remaining changes paired with the person who will own them. If the calendar cannot fit that, cut scope (fewer skills), not ownership.

**Rerun.** `EngageCheck build` on the reference log: 0 errors, 0 warnings, truck factor 2.

<details>
<summary>Solution: the break exercise</summary>

With PRs 17 and 24 moved to the consultant, the last third drops to 4 of 6 (67%) and fails the 70% gate, and `AI-LAYER-CHANGELOG.md` is now known only to the consultant (a warning). The truck factor still reads 2, because the log still credits the tech lead's earlier changes; the tool cannot know he left, so in a real handover you would recompute without him. Making PR 24 a pair with Dana Katz removes the orphan file, since she now knows the changelog, settings and MCP config, but the share gate still fails: pairing on your change is not client authorship. The fix that passes both is Dana authoring PR 24 with you reviewing. Covering for a departing owner by typing yourself is the moment the layer starts becoming yours again.
</details>

## How do I know it works?

- [ ] Every architecture decision has a client decider and a record; you only recommended.
- [ ] Client-authored share of AI-layer changes is at least 70% in the last third of the build.
- [ ] At least two client developers have authored AI-layer changes, and the client truck factor is at least 2.
- [ ] No consultant change to the AI layer was merged without a client reviewer.
- [ ] The layer still passes every gate (lint, skill tests, eval gate).
- [ ] `EngageCheck build` is clean.

## Use / don't use

**Use** pairing early and deliberately with the skeptic; a rule she co-wrote is a rule she defends. **Use** the truck factor as the question "who else could change this file?", asked every week.

**Don't** write the layer at night to "save them time". **Don't** count reviews as authorship. **Don't** let one enthusiast become the new single point of failure.

**Limitations.**

- Authorship is a proxy for understanding. A developer can author a change you dictated; watch for who explains the layer in office hours, not only who commits.
- The greedy truck factor is a simplified version of Avelino et al.'s method, which weighs degree of authorship; for a layer of 20 files it is good enough to start a conversation, not to rank people.
- With-the-team builds are slower in weeks 3–4. Say so at kickoff and put it in the plan; the sponsor who expects the demo speed will otherwise read it as a problem.

## Reflect

1. Which part of your own AI layer could nobody else on your team change today?
2. What would you have to stop doing yourself for your team's truck factor to reach 2?
3. When did someone else's "helpful" build leave you with something you could not maintain?

## Sources

- [Team Topologies — Key concepts](https://teamtopologies.com/key-concepts) — enabling teams help stream-aligned teams overcome obstacles and detect missing capabilities; facilitating interaction; temporary, then move on.
- [Avelino et al. (2016)](https://arxiv.org/abs/1604.06766) — truck factor as the minimal number of developers whose departure incapacitates a project; 65% of 133 GitHub systems had a truck factor of two or less.
- [Bacchelli and Bird (2013)](https://www.microsoft.com/en-us/research/publication/expectations-outcomes-and-challenges-of-modern-code-review/) — modern code review at Microsoft: fewer defect findings than expected; knowledge transfer and team awareness as outcomes.
