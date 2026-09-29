---
id: "17.2"
module: 17
minutes: 16
practice_minutes: 180
prerequisites: ["17.1", "16.5", "15.3", "15.4"]
objectives:
  - Build a starter pack from the demo repository with a setup check learners run before the day, an offline package path, a no-agent path and catch-up tags that each build and pass their tests.
  - Write every hands-on exercise as a spec with a start state, task, done-criteria, time box, hint ladder, catch-up and paper path.
  - Compute a room's peak token demand against an organization's rate limit and plan the start of agent runs so that the room does not hit it.
  - Maintain a troubleshooting guide and a fallback list from rehearsal evidence, with every fix and every clip tested.
volatility: implementation
sources:
  - title: "Microsoft Learn — Setting up local NuGet feeds"
    url: https://learn.microsoft.com/en-us/nuget/hosting-packages/local-feeds
  - title: "Microsoft Learn — dotnet-install scripts"
    url: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script
  - title: "Anthropic — Rate limits (Claude API documentation)"
    url: https://platform.claude.com/docs/en/api/rate-limits
  - title: "Haynes et al. (2009) — A Surgical Safety Checklist to Reduce Morbidity and Mortality in a Global Population"
    url: https://www.nejm.org/doi/full/10.1056/NEJMsa0810119
last_verified: "2026-09-28"
---

# 17.2 · Workshop materials

## Why it matters

The lab's student had a clean agenda from 17.1 and a demo that had passed its drills. They built the rest of the kit the night before a trial run for the host's engineering guild: a README telling people to "clone, restore, test", notes for the three live segments only ("the rest I know by heart"), two exercise files, a troubleshooting table written from memory and a fallback list without the third clip. It is `labs/module-17/break/17.2-thin-kit/`.

At 0:36, Hands-on 1 began. Of fourteen laptops, one was on .NET 7, one could not restore through the company proxy, one had CRLF line endings that failed a convention test, and two people's agents asked them to log in to an account their company had not enabled. Five of fourteen people spent the twelve minutes watching a neighbour, and the student spent them on one laptop. The exercise that was supposed to teach O2 taught it to nine people.

None of those failures is about teaching, and none is rare. A talk fails when *your* laptop fails. A hands-on workshop fails when *any* of fifteen laptops fails, on networks, policies and accounts you do not control. The materials exist to move those failures out of the room: to the day before (a setup check), to a fallback that costs seconds (an offline feed, a catch-up tag, a paper path), or to the break (a fix you have tested).

> [!NOTE]
> Content tags. **Concept** (stable): failure absorbers for a hands-on room, exercise specs, catch-up states, evidence-based troubleshooting, the room as one customer of a rate limit. **Implementation**: NuGet local feeds, `dotnet-install`, provider rate-limit rules and the setup-check scripts (as of 2026-09).

## How it works

### What fails in a room, and what absorbs it

| Failure | When it surfaces without a kit | What absorbs it | When it surfaces with one |
|---|---|---|---|
| Wrong SDK, missing Git, unbuilt hooks | first exercise | setup check two days before | two days before |
| Proxy or private feed blocks restore | first exercise | offline package feed in the pack | never, or in 40 seconds |
| No agent access (policy, licence, account) | first exercise | pairing rule and a paper path | at the door |
| Learner falls behind | every exercise after | catch-up tags | never blocks the next step |
| "What am I supposed to do?" | every exercise | exercise spec with done-criteria | never |
| Unknown failure | anywhere | troubleshooting guide from rehearsals | known, with a timed fix |
| Demo fails | live segments | fallback recordings and branch (15.4) | 30 seconds, narrated |
| "Did they learn anything?" | never | forms A/B, feedback, follow-up (16.5) | at 1:54 and two weeks later |

The pattern is the one surgeons use. Haynes et al. (2009) introduced a 19-item checklist, read aloud at three fixed moments of an operation, in eight hospitals; in the before/after comparison, complications fell from 11% to 7% and deaths roughly halved. The point for us is not the size of the effect (a workshop is not surgery, and a before/after study is not an RCT) but the mechanism: known failures, checked at a fixed moment, by a procedure that does not depend on anyone remembering. Your setup check runs two days before; your T−30 list from the run sheet runs before the doors open.

### The starter pack

The starter pack is `brownfield-demo` on a `workshop` branch, public because it contains nothing but the fictional company (it passed `DemoCheck credibility` in [15.1](../module-15/lesson-01.md)). It holds four things besides the code.

**A setup check** (`check-setup.ps1` and `check-setup.sh`) that learners run two days before and whose last line they send you: `SETUP OK` or `SETUP FAILED (T-02, T-06)`. It checks what failed in rehearsal: SDK version, Git version, the right tag and a clean tree, restore, build and the test count, the agent hooks, the agent's command-line tool. It prints troubleshooting IDs, not advice, and it never reads or prints a credential. Two days before, you know that 12 of 14 are ready and what the other two need; the fix happens by e-mail, not at 0:36.

**An offline package path.** Corporate proxies and private feeds break `dotnet restore` more than anything else. Restore once with `dotnet restore --packages ./packages` and ship the folder: it has the `id/version/` layout that NuGet accepts as a local feed, so a learner behind a proxy runs `dotnet restore --source ./packages` and moves on. The only thing that then needs the network is the agent.

**A no-agent path.** Some learners will not have access on the day, because of policy, licence or a laptop that belongs to a client. Decide it in advance: they pair with someone who has access (and hold the keyboard for the parts that are writing, not running), or they take the **paper path**: the handout carries the agent's output for every step from a rehearsal run, and the exercise asks them to judge it. Every exercise in the reference kit works on paper because every objective is about judging and writing, not typing a command. Never let anyone log in on someone else's account to "just follow along".

**Catch-up tags.** Each hands-on step has a tag with its finished state: `ws-1-ground` has a reference brief, `ws-2-bound` the approved plan and the build, `ws-3-built` the same with a saved test run. A learner who is behind at the stop time stashes their work and checks out the tag, and starts the next step with everyone else. A tag that does not build is worse than no tag, so verify all of them every time you change the pack:

```bash
for t in ws-0-start ws-1-ground ws-2-bound ws-3-built; do
  git checkout -q "$t" && dotnet test Contoso.Billing.sln --nologo -v q | tail -1
done
```

### Exercises as specs

The Module 16 exercise spec (start state, task with the objective's verb, done-criteria, time box, hint ladder, extension) gets two more lines in a workshop: **catch-up** (which tag, and what to read there) and **paper path** (the same judgment from saved output). The done-criteria carry more weight in a room of fifteen than in a room of six: without "the brief names every existing void rule with a path and one open question for Finance", pairs finish between minute four and never, and your debrief compares nothing.

Write the reference answer under each exercise, marked instructor-only. For BILL-180 it lists what Ground should find: `InvoiceStatus.Void` in `Invoice.cs`, the tinyint `Status` column from migration V003, the glossary's "cancelled after issue", and the open ticket BILL-155, which says only issued invoices can be voided while BILL-180 says drafts can be too. That conflict is the point of the exercise: an agent that does not find BILL-155 writes a rule that BILL-155 will later duplicate.

### Troubleshooting: evidence, not memory

Every entry comes from a rehearsal, a setup-check reply or a delivery, and every fix has been run on a clean machine (the **Tested** column is a date). Two rules keep it usable in the room:

- **Symptoms in the learner's words** ("asks me to log in"), not causes ("SSO not provisioned"). The co-host scans the symptom column.
- **The three-minute rule.** A fix that takes longer than about three minutes is not done during an exercise. The learner moves to pairing, the catch-up tag or the paper path, and you fix the laptop at the break. Installing an SDK (four minutes with the non-admin `dotnet-install` script, where laptop policy allows it at all) is a break-time fix.

### The room is one customer

Intuition: fifteen people starting an agent in the same minute look, to the model provider, like one organization's traffic jumping from nothing to a lot. Providers limit an organization's traffic per minute; Anthropic's documentation, for example, sets limits at the organization level, per model class, in requests, input tokens and output tokens per minute, and warns that a sharp increase can also trip separate acceleration limits.

Equation. With $n$ learners starting in the same minute and $t$ uncached input tokens in each first call, the peak demand is

$$D = n \cdot t \quad\text{input tokens per minute},$$

and with starts staggered across $k$ one-minute groups it falls to about $D/k$. Compare $D$ with the organization's limit $L$.

Tiny example. Fourteen learners, $t \approx 40{,}000$ (rules, brief and the files the planner reads): $D = 14 \times 40{,}000 = 560{,}000$ tokens in the first minute of Hands-on 2. If the host's organization has $L = 400{,}000$ for that model, about 160,000 tokens' worth of requests get 429 errors, and every rejected learner retries at the same moment. With $k = 2$ (odd tables now, even tables a minute later), $D/k = 280{,}000 < L$.

Implementation. Ask the host for the organization's limits and which workspace the agent uses (an administrator may have set a lower workspace limit); redo the arithmetic with their numbers; write the stagger into the Hands-on 2 cue card and into troubleshooting entry T-05 ("retry once after 60 seconds").

Interpretation. The estimate is rough on purpose. Later calls cost less against most limits because cached input is not counted, and learners on personal subscriptions are separate customers. What matters is the order of magnitude: a room can plausibly exceed a small organization's limit in its worst minute, and staggering is free.

### Fallbacks and forms

The **fallback list** names every clip the agenda falls back to, the timestamp to start from, the take it was cut from, and the date you last played it end to end on the presenting laptop. A clip you have not played on that laptop, with sound, is not a fallback ([15.4](../module-15/lesson-04.md)). The **forms** are Module 16's: parallel forms A and B with two items per objective, counterbalanced, the five-question feedback form and the two-week follow-up message ([16.5](../module-16/lesson-05.md)). With four objectives, the pre-test takes six minutes; that is why it has its own row in the agenda.

## Show me

`KitCheck kit` on the reference kit (condensed):

```text
workshop-kit/
  ok      agenda.md
  ok      instructor-notes.md
  ...
Instructor notes
  19 of 19 agenda steps have a notes section
Starter pack
  setup check script: yes
Exercises
  3 exercise file(s) for 3 hands-on step(s)
  ex1-ground.md                complete
  ex2-bound-build.md           complete
  ex3-prove.md                 complete
Troubleshooting
  10 entries, 0 untested, 0 slower than 3 minutes
Fallback recordings
  listed  01-before-BILL-97.mp4
  listed  02-ground.mp4
  listed  03-bound.mp4
Question bank (details: KitCheck questions)
  0 error(s), 0 warning(s)
Rehearsals: 3 log(s) (details and exit standard: KitCheck rehearsal)

0 error(s), 0 warning(s)
```

## Try it

Budget: 180 minutes.

1. **Starter pack (60 min).** Create the `workshop` branch in a copy of your `brownfield-demo`; commit the hands-on ticket; tag `ws-0-start`. Do the hands-on steps yourself and tag each finished state. Run the tag loop above. Add `packages/` with `dotnet restore --packages ./packages`, and adapt the setup-check scripts from `labs/module-17/solution/workshop-kit/starter-pack/`.
2. **Clean machine (20 min).** Run the setup check on a machine or account that has never seen the repository: a colleague's laptop, a fresh VM, a new user account. Write every failure into `troubleshooting.md` with a tested fix.
3. **Exercises (45 min).** One file per hands-on step in the [workshop template](../../templates/workshop-template.md) format, including catch-up, paper path and the instructor-only reference answer. Print the paper path and do it yourself without a laptop.
4. **Rate limit (15 min).** Get your target organization's limits (or your own account's), count the first-call tokens of your busiest step from a rehearsal log, and compute $D$ and the stagger you need.
5. **Fallbacks and forms (40 min).** `fallback/README.md` with every clip played on the presenting laptop; `forms.md` from your Module 16 forms extended to the workshop's objectives. Then:

```bash
cd labs/module-17
dotnet run --project tools/KitCheck -- kit ~/workshop-kit
```

<details>
<summary>Hint: my hands-on step cannot be finished in 15 minutes by a stranger</summary>

Then the step is too big, not the stranger too slow. Your Module 15 stranger test timed a whole ticket at 25–30 minutes; a workshop gives each phase 7–14 minutes. Split the ticket by phase, give each phase a catch-up tag, and let the tags carry the learners across the phases they did not finish.
</details>

## Break it

```bash
cd labs/module-17
dotnet run --project tools/KitCheck -- kit break/17.2-thin-kit
```

Before running it, open the thin kit's `starter-pack/README.md` and its `troubleshooting.md` and write down what happens to a learner behind a proxy, a learner with no agent access and a learner who is five minutes behind at 1:18.

## Fix it

**Diagnose.** 22 errors, 14 warnings. Fifteen agenda steps have no notes, and the credibility notes are a monologue. No setup check, no offline path, no no-agent path; two of the agenda's three catch-up tags are not in the starter pack. Hands-on 3 has no exercise, and Hands-on 2's has no done-criteria, hints or catch-up ("time: until it is done"). Three troubleshooting entries, all untested, two of them 10 and 30 minutes long ("ask IT to open nuget.org"). The third fallback clip is not listed, and no clip has been played back. No question bank, no rehearsals. The student prepared the parts that frightened them (the live segments) and trusted the parts that fail in rooms: laptops, networks, accounts and time.

**Modify.** Rebuild from the failure table: setup check with IDs, `packages/` and the offline command, pairing and paper path, all four tags in the README and verified; three complete exercise specs; troubleshooting from the trial run's actual failures with tested fixes under three minutes; the full fallback list with play-back dates; cue-card notes for every row. That is the reference kit.

**Rerun.** `kit solution/workshop-kit`: 0 errors, 0 warnings.

## How do I know it works?

- [ ] Someone who has never seen your repository ran the setup check and got `SETUP OK`, or an ID whose fix worked.
- [ ] Every catch-up tag builds and passes its tests, verified after your last change to the pack.
- [ ] You did every exercise on paper, from the paper path alone.
- [ ] Every troubleshooting fix has a test date and takes under three minutes, or says where the learner goes instead.
- [ ] You have written down $D$, the host's $L$ and your stagger.
- [ ] `KitCheck kit` reports no errors other than the question bank and rehearsals you have not written yet.

## Use / don't use

**Use** a starter pack with a setup check for any session where learners run code on their own machines. **Use** catch-up tags whenever a later step depends on an earlier one.

**Don't** hand out the pack on the day. **Don't** fix a laptop for ten minutes while fourteen people wait. **Don't** use a real client's repository as a starter pack, even inside that client: the pack gets copied, forwarded and kept.

**Limitations.**

- The setup check verifies what it checks. It cannot see a proxy that only blocks the model provider on the day, or a laptop that will be swapped; the troubleshooting guide covers what the check cannot.
- Rate limits, their tiers and how cached tokens count are provider decisions that change; the arithmetic stays, the numbers must be looked up for each host.
- The checklist evidence comes from surgery; its transfer to workshops is an analogy for a mechanism, not a measured effect.

## Reflect

1. Which failure from your clean-machine run would have cost you an exercise if you had found it on the day?
2. What does a learner on the paper path miss, compared with one who runs the agent, and does your post-test notice?
3. Which of your troubleshooting fixes is still a guess?

## Sources

- [Microsoft Learn — Setting up local NuGet feeds](https://learn.microsoft.com/en-us/nuget/hosting-packages/local-feeds) — a local feed is a hierarchical `id/version/` folder usable as a package source.
- [Microsoft Learn — dotnet-install scripts](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script) — non-admin installation of the .NET SDK to a user folder.
- [Anthropic — Rate limits](https://platform.claude.com/docs/en/api/rate-limits) — limits set per organization and model class in requests and tokens per minute, token-bucket enforcement, acceleration limits, cached input not counted for most models (as of 2026-09).
- [Haynes et al. (2009) — A Surgical Safety Checklist to Reduce Morbidity and Mortality in a Global Population](https://www.nejm.org/doi/full/10.1056/NEJMsa0810119) — a checklist at fixed moments in eight hospitals; complications 11% to 7% in a before/after comparison.
