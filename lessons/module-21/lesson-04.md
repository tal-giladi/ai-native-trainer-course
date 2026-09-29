---
id: "21.4"
module: 21
minutes: 17
practice_minutes: 150
prerequisites: ["21.3", "19.4", "13.6", "17.1"]
objectives:
  - Report every pre-registered outcome with its interval and a status that matches the interval, including inconclusive and harmful results, and write what the results do not show.
  - Hand over every recurring activity to an employee owner with a backup who has run it at least once without the consultant before the handover date.
  - Schedule and run follow-ups at 30, 60 and 90 days that read usage per team against control limits and check each owned rhythm.
  - Check the handover section with EngageCheck handover and the post-handover usage with AdoptCheck usage.
volatility: concept
sources:
  - title: "Google SRE Book — Chapter 32: The Evolving SRE Engagement Model (Production Readiness Review)"
    url: https://sre.google/sre-book/evolving-sre-engagement-model/
  - title: "Kirkpatrick Partners — The Kirkpatrick Model"
    url: https://www.kirkpatrickpartners.com/the-kirkpatrick-model/
  - title: "NIST/SEMATECH e-Handbook of Statistical Methods — 6.3.3.2 Proportions control charts"
    url: https://www.itl.nist.gov/div898/handbook/pmc/section3/pmc332.htm
last_verified: "2026-09-28"
---

# 21.4 · Enable, measure, hand over, follow up

## Why it matters

Week 11, the read-out. The sponsor wants one number for the CFO. The pooled median cycle time of the pilot's ai tickets is 23% lower than the manual ones. It is the most tempting number in the engagement, and it is wrong: it mixes ticket sizes, and the randomized, size-stratified estimate is −8% with a 95% interval from −24% to +12%. Which one goes into the report decides whether the case study you write in 21.5 is honest, and whether the sponsor's renewal decision in February rests on something real.

Week 12, the handover. The contract ends on 23 December. If the office hours, the eval gate and the metrics review are still run by you on that day, the handover is to yourself. Module 19 showed what that looks like three months later ([19.4](../module-19/lesson-04.md)); the Contoso data at the end of this lesson shows it again at the scale of 19 developers.

This lesson covers the last four phases: enable the other teams, measure honestly, hand over to owners who have already done the work without you, and follow up until you can see it holding.

> [!NOTE]
> Content tags. **Concept** (stable): enablement with owners, intervals and statuses, the claims ladder, "what this does not show", readiness-style handover, dated follow-ups, per-team control limits. **Implementation**: `ImpactStats`, `EvalHarness`, `AdoptCheck`, `EngageCheck handover` and the illustrative Contoso data.

## How it works

### Enable: the Module 17 workshop, with owners from day one

Enablement for Collections and Platform is the workshop you designed in Module 17 ([17.1](../module-17/lesson-01.md)) and the enablement mechanics of Module 19 ([19.3](../module-19/lesson-03.md)): role-based paths, practice on their own tickets, office hours, champions with a co-champion. The engagement-specific rule is about ownership: office hours are run by a client developer from the first session (Dana, from week 6), with you in the room for the first two and absent for the next two. The workshop is measured beyond reaction, at least to learning (pre/post, [16.5](../module-16/lesson-05.md)) and, through usage, to behavior, levels 2 and 3 of the Kirkpatrick model ([Kirkpatrick](https://www.kirkpatrickpartners.com/the-kirkpatrick-model/)).

### Measure: every outcome, with its interval and a status the interval allows

Report every outcome in the pre-registration (D2), in the order it lists them, not the ones that came out well. Each row: baseline, estimate, interval, source command, status. The status follows from the interval:

| Interval | Status | Contoso example |
|---|---|---|
| entirely on the good side of zero | improved | eval pass rate +21.7 pts, [+10.6, +32.8] |
| includes zero | inconclusive (or "no detectable change" if narrow) | cycle time −8%, [−24%, +12%] |
| entirely on the bad side | worse | — |

Place every sentence on Module 13's claims ladder ([13.6](../module-13/lesson-06.md)). "In a randomized comparison of 27 Billing tickets, the ratio of cycle times was 0.92 (95% interval 0.76 to 1.12)" is rung 2. "The AI layer made Billing 8% faster" is not supported at any rung. Then write **what this does not show**, before anyone asks: the delivery effect below the detectable 38%; defects on 27 tickets; conventions (evals) are not throughput; enabled teams were not measured for delivery.

### Hand over: readiness, not a date

Google's SRE teams do not take over a service because a date arrived. A Production Readiness Review checks the service, and the transfer is progressive: training, gradual transfer of operational responsibilities, the development team available as backup, then continuous improvement ([SRE book, ch. 32](https://sre.google/sre-book/evolving-sre-engagement-model/)). The engagement's handover follows the same logic, with four responsibilities that must each have:

- an **owner** who is an employee, not you;
- a **backup** (succession, because the truck factor of 21.3 is only 2);
- a **date on which the owner ran it without you**, before the handover date.

| Responsibility | Why it dies without an owner |
|---|---|
| AI layer: rules, skills, changelog | Models, tools and code change monthly; an unedited layer drifts |
| Eval regression gate and nightly suite | A red gate nobody understands gets disabled |
| Office hours and questions | Questions go unanswered, people go back to the old way |
| Metrics and usage review | Nobody notices a team sliding until the renewal meeting |

Close the access you were given and set the data-deletion date the SOW promised. Both go in the record.

### Follow up at 30, 60 and 90 days

Habits take longer than engagements ([19.4](../module-19/lesson-04.md) cites a median of 66 days to automaticity for simple behaviors). The SOW includes three short follow-ups. Each has the same agenda: did each rhythm run without you; weekly usage per team against its control limit; one question to each owner, "what did you change in the layer this month?".

*Intuition.* Usage per team bounces from week to week. The lower control limit tells you when a drop is more than bounce.

*Equation.* From baseline weeks with share $\bar p$ and a week with $n$ seats: $\text{LCL} = \bar p - 3\sqrt{\bar p(1-\bar p)/n}$ ([NIST/SEMATECH](https://www.itl.nist.gov/div898/handbook/pmc/section3/pmc332.htm)).

*Tiny example.* Platform, 6 developers, baseline 75%: $0.75 - 3\sqrt{0.75 \times 0.25 / 6} = 0.75 - 0.53 = 0.22$. One active developer out of six (17%) is below the limit; two (33%) is not.

*Implementation.* `AdoptCheck usage` per team with the engagement's events file.

*Interpretation.* With teams of six, the chart only catches collapses. A slide from 5 to 3 of 6 is invisible to it, which is why the follow-up is a conversation with each owner and not only a chart. For the whole group, a Wilson interval says how precisely you know the share: 17 of 19 at 90 days is 89%, 95% interval 69% to 97%.

## Show me

The measurement, Contoso pilot, weeks 3–6 plus the defect window:

```bash
cd labs/module-21
dotnet run --project ../module-13/tools/ImpactStats -- compare client/data/pilot-tickets.csv --metric cycle_hours --a manual --b ai --strata size
dotnet run --project ../module-07/tools/EvalHarness -- compare client/data/eval-results.csv --a before-ai-layer --b contoso-layer-v1
```

```text
pooled medians: A 15.7 h, B 12.1 h (B/A 0.77, -23%) — not adjusted for size
stratified ratio of geometric means B/A: 0.921 (-8%), bootstrap 95% CI [0.756, 1.116] = [-24%, +12%] (10000 resamples, seed 13)
permutation test (10000 shuffles within size, seed 14): two-sided p = 0.4432

pass rate before-ai-layer 53% (120 trials) -> contoso-layer-v1 75% (120 trials)
paired (by task):   diff +21.7 pts, SE 0.054, 95% CI [+10.6 pts, +32.8 pts]
verdict: contoso-layer-v1 is better: the 95% paired interval excludes 0
```

Two honest statuses: the layer improved the agent's adherence to Contoso's conventions; the delivery comparison is inconclusive, as the 38% detectable effect predicted.

Now the calendar handover ([`break/21.4-calendar-handover`](../../labs/module-21/break/21.4-calendar-handover/engagement-record.md)). Before running: in which week does the first team fall below its limit?

```bash
dotnet run --project tools/EngageCheck -- handover break/21.4-calendar-handover/engagement-record.md
dotnet run --project ../module-19/tools/AdoptCheck -- usage break/21.4-calendar-handover/usage.csv --events break/21.4-calendar-handover/events.csv
```

```text
ERROR no owner for the eval regression gate
ERROR Office hours and questions channel: owned by the consultant; a handover to yourself is not a handover
ERROR Cycle time, ai vs manual (pooled medians): no interval; a point estimate from one engagement is mostly noise
ERROR Escaped defects, ai vs manual: interval [-27, 0] pts includes zero but status says "improved"; ...
ERROR no follow-up about 30 days after the handover
ERROR no "What this does not show" section: say what the results cannot support before someone else says it for you
12 error(s), 5 warning(s)

  W12    15/19     79% active    37% engaged  ...  <- Handover on the contract end date; office hours stay with the consultant 'as needed'
  W14    14/19     74% active    47% engaged  ...  <- Consultant office hours cancelled twice (other client)
  W19     6/19     32% active    26% engaged  ...
  W24     4/19     21% active    11% engaged  ...
  team        base   LCL  last first<LCL  pattern
  billing      83%   38%    0%       W19  decay (habit and support fading)
  collections   89%   54%   29%       W19  decay (habit and support fading)
  platform     83%   38%   33%       W19  decay (habit and support fading)
4 error(s), 0 warning(s)
```

All three teams cross their limits in week 19, seven weeks after a handover that handed nothing over, with no follow-up booked to notice. The reference ([`solution/engagement-record.md`](../../labs/module-21/solution/engagement-record.md) and `client/data/usage.csv`) has five owners with backups who each ran their rhythm between 30 November and 16 December, and follow-ups on 22 January, 19 February and 19 March. Its data has one dip: Platform falls below its limit in week 19 after its champion moves; the 60-day follow-up finds it, the co-champion named at kickoff takes over, and it is back in range by week 24.

## Try it

Budget: 150 minutes.

1. **Enable (60 min).** Run a shortened version of your Module 17 workshop for two or three peers playing Collections and Platform, with a pre/post. Have one of them run a 15-minute office hour without you.
2. **Measure (40 min).** Run the three comparisons (cycle time, defects with `--binary`, evals) and `AdoptCheck usage` on the reference data. Fill the results table with statuses that match the intervals. Write "What this does not show" with at least four bullets.
3. **Hand over (30 min).** Fill the owners table: employee owners, backups, the date each ran without you. Access revoked, data deletion date.
4. **Follow-ups (10 min).** Book 30/60/90 days in the record with an agenda.
5. **Check (10 min).** `EngageCheck handover` until clean.

<details>
<summary>Hint: the read-out sentence for cycle time</summary>

"We cannot tell yet whether Billing delivers faster. The estimate is 8% faster, and the data is compatible with anything from 24% faster to 12% slower. That is what we expected from 27 tickets. What we can tell: the agent follows your conventions much more often (53% to 75% of tasks), and 17 of 19 developers use it weekly."
</details>

## Break it

> [!WARNING]
> Follow-ups read usage data about real teams. Keep to team-level counts with at least five people per group, as in Module 19; never ask for per-person usage "to see who dropped off".

Copy the reference record and change the handover date to 2026-11-27, four weeks earlier, without changing the owners table. Run `handover`. Which owners have not yet run their rhythm without you, and what would you tell the sponsor who asked for the earlier date?

## Fix it

**Diagnose.** The calendar handover fails on ownership (the consultant owns office hours and metrics; nobody owns the gate; nothing ran without the consultant), on measurement (a pooled median with no interval called "improved"; a defect interval touching zero called "improved"; no "does not show"), and on follow-up (none booked). The usage decay is the consequence.

**Modify.** Owners who are employees, with backups and rehearsal dates before handover; the stratified estimate with its interval and status "inconclusive"; the defect result "inconclusive"; the four "does not show" bullets; follow-ups at 30, 60 and 90 days with an agenda.

**Rerun.** `EngageCheck handover` on the reference: 0 errors, 0 warnings. `AdoptCheck usage` on the reference data: clean, one dip found and recovered.

<details>
<summary>Solution: the earlier handover date</summary>

With a handover on 27 November, every owner's "ran without consultant" date (30 November to 16 December) is after the handover, so each fails. Tell the sponsor: "We can hand over on 27 November, but none of the rhythms will have run without me yet. Either we keep the date and I attend the first unassisted runs as an observer under a change request, or we keep 23 December." Put the choice in writing.
</details>

## How do I know it works?

- [ ] Every pre-registered outcome is reported with an interval and a status the interval allows, including inconclusive ones.
- [ ] "What this does not show" has at least three bullets.
- [ ] Each of the four responsibilities has an employee owner, a backup and a date it ran without you, before the handover.
- [ ] Follow-ups at about 30, 60 and 90 days are booked with an agenda; access is revoked and a data-deletion date set.
- [ ] `EngageCheck handover` is clean, and you have read `AdoptCheck usage` for each follow-up.

## Use / don't use

**Use** the stratified, pre-registered estimate as the headline, and the pooled median only as a descriptive aside with its caveat. **Use** follow-ups to check the handover, not to keep running the programme.

**Don't** report the outcomes that came out well and omit the rest. **Don't** extend the engagement to cover a missing owner; find the owner. **Don't** let the contract date decide the handover.

**Limitations.**

- Control limits on teams of six only catch collapses; follow-up conversations do the rest.
- The eval improvement is on the course's task set adapted to Contoso; it measures adherence to conventions, not delivery.
- Three follow-ups in 90 days are a check, not maintenance. If the client needs more, that is the retainer rung of your offer ladder ([20.1](../module-20/lesson-01.md)), sold separately.
- Rehearsed ownership is necessary, not sufficient: owners change jobs. The backups and the usage alert are what cover that.

## Reflect

1. Which recurring activity in your current work would stop the week you left?
2. When have you seen a result reported with the wrong status, and who acted on it?
3. What would you want to see at the 60-day follow-up to feel the engagement worked?

## Sources

- [Google SRE Book, ch. 32](https://sre.google/sre-book/evolving-sre-engagement-model/) — Production Readiness Review; onboarding through training, progressive transfer of responsibilities, the development team as backup, continuous improvement.
- [Kirkpatrick Partners — The Kirkpatrick Model](https://www.kirkpatrickpartners.com/the-kirkpatrick-model/) — four levels: reaction, learning, behavior, results.
- [NIST/SEMATECH e-Handbook — Proportions control charts](https://www.itl.nist.gov/div898/handbook/pmc/section3/pmc332.htm) — p-chart limits $\bar p \pm 3\sqrt{\bar p(1-\bar p)/n}$.
