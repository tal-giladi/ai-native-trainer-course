---
id: "01.3"
module: 1
minutes: 15
practice_minutes: 60
prerequisites: ["01.2"]
objectives:
  - Write a wedge as stack × audience × pain in one sentence that a stranger can use to tell whether they are in or out.
  - Score three or more candidate wedges on five weighted criteria and justify the choice, including what you are giving up.
  - Build a stack inventory for the wedge audience and name the three or four platforms you will build against for the rest of the course.
  - Estimate how many people in the wedge you can actually reach, without multiplying correlated survey shares as if they were independent.
volatility: concept
sources:
  - title: "Stack Overflow Developer Survey 2025 — Technology"
    url: https://survey.stackoverflow.co/2025/technology
  - title: "Microsoft Lifecycle — Microsoft .NET Framework"
    url: https://learn.microsoft.com/en-us/lifecycle/products/microsoft-net-framework
  - title: "Microsoft Lifecycle — SQL Server 2016"
    url: https://learn.microsoft.com/en-us/lifecycle/products/sql-server-2016
  - title: "Christensen, Hall, Dillon, Duncan (2016) — Know Your Customers' Jobs to Be Done (Harvard Business Review)"
    url: https://hbr.org/2016/09/know-your-customers-jobs-to-be-done
last_verified: "2026-09-28"
---

# 01.3 · Finding your wedge

## Why it matters

"AI-native SDLC training" describes thousands of people. It covers every tool vendor's developer-relations team, every YouTube channel, and every consultancy that added "AI" to its homepage in 2024. A CTO who hears it cannot tell why you rather than any of them. A **wedge** is the narrow end that gets you in the door. It is a specific stack, audience and pain that you understand better than most trainers, because you have lived it.

Narrow feels risky. It is the opposite. Your examples match the prospect's code, so the demo looks like *their* repository. Your war stories match their incidents, so they believe you. There are fewer people to compare you with, so you are easier to remember. The teardown this course is built on describes a workshop that won with Jira, Confluence, GitHub and a coding agent. That stack isn't the only valid one. It won because it is what that trainer's audience actually ran.

The wedge also decides much of your course. The stack inventory you write today is what you integrate in [Module 8](../module-08/lesson-02.md), what your demo repository imitates later in the course, and what your eval tasks ([Module 7](../module-07/lesson-02.md)) are drawn from. A vague wedge means you build a generic lab and teach a generic workshop.

> [!NOTE]
> Content tags. **Concept** (stable): stack × audience × pain, the five criteria, the reachability funnel, tool-agnostic anchoring. **Implementation**: survey shares and product lifecycle dates (as of 2026-09).

## How it works

### The formula

A wedge is one sentence with three parts:

> AI-native **⟨what you help with⟩** for **⟨audience⟩** on **⟨stack⟩**, who struggle with **⟨pain, in their words⟩**.

- **Stack**: language and runtime, database, tracker, docs, code host and CI. These are the platforms where the work happens.
- **Audience**: role, team size, industry or situation, such as "10–60 developers maintaining a 15-year-old system". Size and situation predict pain better than industry does.
- **Pain**: one from 01.2, ideally one you have seen in data or lived yourself. After interviews (01.4) it should be in *their* words.

### Five criteria

| Criterion | Weight | Scores 5 when… | Scores 1 when… |
|---|---|---|---|
| **Specific** | 0.20 | a stranger can tell in one read whether they are in or out | everyone who writes code is in |
| **Your advantage** | 0.25 | you have years in this stack and incidents you personally lived | you read about it |
| **Reachable** | 0.20 | ≥ 15 people you can actually talk to this month | you know nobody in it |
| **Pain recurring and sized** | 0.20 | a pain passing the four tests, sized in hours and flow | "they'd probably like AI" |
| **Stack durable and common** | 0.15 | the stack outlives the next three tool releases | the wedge is one tool's feature set |

The weights are a starting point, not a law. Your advantage carries the most weight because it is the hardest criterion for a competitor to copy.

### Anchor on the stack, not the tool

Coding-agent tools change quarterly. The stack changes over years. Microsoft's lifecycle page lists no end date for .NET Framework 4.8 and 4.8.1, which follow the Windows component policy. SQL Server 2016 left extended support on 2026-07-15, with paid security updates available until 2029. Many shops will run those systems, and need to change them safely, for years. A wedge built on "legacy .NET and SQL Server" survives the next three agent releases. A wedge built on "Tool X power users" ends when Tool X changes its pricing or its competitor catches up.

This is the course's principle, applied to you: **problem → architecture → requirements → tool**, never favorite tool → problem. The AI layer you build in [Module 3](../module-03/lesson-02.md) is deliberately portable across tools for the same reason.

### Reachability: a funnel, with one trap

*Intuition.* You need 3–5 interviews now and a free pilot later. Both come from people you can actually reach. A wedge that is perfect on paper but contains nobody who will take your call can't be tested.

*Equation.* Starting from the $N$ people you can realistically contact, apply the share $s$ that matches the wedge and a reply rate $r$:

$$R = N \cdot s_{\text{wedge}} \cdot r$$

*The trap.* The 2025 Stack Overflow survey reports C# at 27.8% of respondents, SQL Server at 30.1% and Jira at 46.4%. It is tempting to compute $0.278 \times 0.301 \times 0.464 = 3.9\%$ and call that the ".NET + SQL Server + Jira" share. That multiplication assumes the three are independent. They are not: C# developers use SQL Server far more often than average. Estimate $s_{\text{wedge}}$ **directly** from your own network (search your contacts for the combination) instead of multiplying marginals.

*Tiny example.* 640 first-degree contacts. A search for .NET or C# in title or skills gives 141. Checking those by hand, about 85 work where SQL Server is central. You would genuinely message about half of them: 42. At a 30% reply rate, that's about 13 conversations. That's enough for five interviews and a pilot lead.

*Implementation.* A spreadsheet with one row per funnel step and its source (search query, hand count, assumption). Mark every step as counted or assumed.

*Interpretation.* Under 15 reachable people means the wedge is too narrow **to test now**. It may still be a good niche later, once content brings people to you. It is not a verdict on the market.

## Show me

A fictional student scores four candidates. The student has 14 years of C#/.NET, the last 6 as tech lead on a SQL Server-heavy platform, and uses Claude Code daily:

| Candidate wedge | Specific (0.20) | Advantage (0.25) | Reachable (0.20) | Pain (0.20) | Durable (0.15) | Total |
|---|---|---|---|---|---|---|
| A. AI-native SDLC training for software teams | 1 | 2 | 5 | 2 | 3 | 2.55 |
| B. AI-native delivery for .NET teams (10–60 devs) on a SQL Server–centred brownfield system, Jira + GitHub | 4 | 5 | 4 | 4 | 4 | **4.25** |
| C. Agents for Blazor WebAssembly migrations in fintech | 5 | 3 | 1 | 2 | 2 | 2.65 |
| D. Power-user training for one popular AI editor | 3 | 3 | 4 | 2 | 1 | 2.70 |

Check one row: B = 0.2·4 + 0.25·5 + 0.2·4 + 0.2·4 + 0.15·4 = 0.8 + 1.25 + 0.8 + 0.8 + 0.6 = 4.25.

A is everyone and therefore no one. C is precise, but its funnel gives 640 → 19 → 3 conversations, too few to test. D scores 1 on durability because its content expires with the tool's next release, and the vendor offers the same training for free. B wins, but its pain column is still a hypothesis (4, not 5) until interviews confirm it.

The resulting `wedge.md` v1 (the [worked example](../../labs/module-01/examples/wedge-example.md) shows v2, after interviews):

```markdown
## One sentence
AI-native delivery for .NET teams of 10–60 developers maintaining a SQL Server–centred
brownfield system on Jira and GitHub, who lose days to agent changes that break conventions
nobody wrote down.

## What I am giving up
Greenfield startups; Java and Node shops; executive "AI strategy" talks; Azure DevOps-only shops for now.
```

And the four platforms the student will build against: **Jira** (tracker), **Confluence** (docs), **GitHub + Actions** (code host and CI), **SQL Server** (database, with stored procedures as a first-class code artifact).

## Try it

Budget: 60 minutes. Worksheets in [`labs/module-01/worksheets/`](../../labs/module-01/README.md).

1. Write **three** candidate wedges in the one-sentence form. Make at least one narrower and one broader than your instinct.
2. Score them on the five criteria. Write one line of evidence per score. "I think" is not evidence; "I owned this database for six years" is.
3. For the top two, run the reachability funnel on your real network. Mark each step as counted or assumed.
4. Choose one. Write `wedge/wedge.md` v1, including **what you are giving up**.
5. Fill `wedge/stack-inventory.md`. Mark every cell you are guessing with `?`. Those cells become interview questions in 01.4.
6. Say the sentence out loud to someone outside tech. Can they tell you who it is *not* for?

<details>
<summary>Hint: your advantage feels small</summary>

List incidents, not skills. "Rolled back a migration that locked the Orders table at month-end" beats "SQL Server expert". Every incident you lived is a story a prospect recognizes and a competitor who has only read about it cannot tell. If you have no incidents in a stack, it is not your wedge yet.
</details>

## Break it

A first draft from a student in a hurry:

```text
I help developers of all kinds, mostly .NET but also Python and JavaScript, get the most out of
Cursor and Claude Code, and I also advise teams that are considering AI-first development.
```

Score it on the five criteria before reading on. How many audiences does it name? What happens to it when either tool ships a major release?

## Fix it

**Diagnose.** Specific scores 1: it names three language communities and two situations (using tools, considering AI), so nobody can tell whether they are out. It is **tool-first**, so durability scores 1, and both vendors document their own tools for free. It names **no pain**. And it **hedges**: "mostly", "also", "considering". The source material's exit signal for this step is blunt. You are done when you can say your wedge in one sentence without hedging. This draft hedges three times.

**Modify.** Keep what is true: the student's depth is in .NET. Anchor on stack and pain, move the tools to the stack inventory, and write down who is out:

```text
AI-native delivery for .NET teams maintaining SQL Server-centred brownfield systems on Jira and
GitHub, who lose days to agent-written changes that break conventions nobody wrote down.
Out: Python/JS shops, greenfield teams, "should we adopt AI?" strategy work.
Tools (in the stack inventory, not the wedge): whichever agents the team already licenses.
```

**Rerun.** Rescore it: Specific 4, Advantage 5, Reachable 4, Pain 4 (hypothesis), Durable 4, for a total of 4.25, up from about 2. Say it aloud in one breath. The "Out" line is the test: if you can't name who is out, the wedge isn't narrow yet.

<details>
<summary>Solution notes: other ways a wedge fails</summary>

- **Too narrow to test.** Fewer than 15 reachable people. Keep it as a future niche; widen one dimension (industry → situation) for now.
- **Employer-shaped.** The wedge is exactly your current employer. It is a fine first audience (01.1), but check you can reach at least ten people outside it. Also check your employment contract for side-work and IP clauses before you talk to anyone who could become a paying client; the course returns to this with legal and admin later.
- **Pain-free.** Stack and audience are right, but the pain is "they'd benefit from AI". Go back to 01.2.
- **Borrowed.** Someone else's wedge in your words. If your advantage score depends on their incidents, it is theirs.
</details>

## How do I know it works?

- [ ] The wedge is one sentence with stack, audience and pain, and has an explicit "out" list.
- [ ] Three candidates were scored with one line of evidence per score; the choice is written down with what it gives up.
- [ ] The reachability funnel for the chosen wedge gives ≥ 15 people, with counted vs assumed steps marked.
- [ ] `stack-inventory.md` names the 3–4 platforms you will build against, and every guess is marked `?`.
- [ ] Someone outside tech can repeat who it is for and who it is not for.

## Use / don't use

**Use** the wedge to decide what to build, what to demo, whom to interview and what to say no to. **Use** it as a *hypothesis*: 01.4 will keep it, change it or kill it, and version 2 is normal.

**Don't** pick a wedge by favorite tool. **Don't** pick one where you have no incidents of your own. **Don't** multiply survey percentages to size a market; ask your network. **Don't** treat the wedge as a permanent identity. Widen it once evidence and reputation carry you further (Module 22 covers scaling).

**Limitations.**

- Survey shares describe survey respondents (self-selected Stack Overflow users), not the population of companies. Use them as rough context, never as market size.
- The criteria weights are judgment. Change them if you have a reason, but write the reason down before you score, not after.
- A wedge is chosen with little data. Most of the value of this lesson is making the hypothesis explicit enough for interviews to falsify.

## Reflect

1. What is your wedge sentence, and who is explicitly out?
2. Which of your personal incidents is the strongest evidence for your advantage score?
3. Which cell of your stack inventory are you least sure about, and whom will you ask?

## Sources

- [Stack Overflow Developer Survey 2025 — Technology](https://survey.stackoverflow.co/2025/technology) — share of all respondents using C# (27.8%), Microsoft SQL Server (30.1%), Jira (46.4%), Confluence (32.8%) and GitHub (81.1%); respondent-level, self-selected.
- [Microsoft Lifecycle — Microsoft .NET Framework](https://learn.microsoft.com/en-us/lifecycle/products/microsoft-net-framework) — .NET Framework follows the Component Lifecycle Policy; 4.8 and 4.8.1 list no end date.
- [Microsoft Lifecycle — SQL Server 2016](https://learn.microsoft.com/en-us/lifecycle/products/sql-server-2016) — extended support ended 2026-07-15; Extended Security Updates available through July 2029.
- [Christensen et al. (2016) — Know Your Customers' Jobs to Be Done](https://hbr.org/2016/09/know-your-customers-jobs-to-be-done) — define the market by the job customers need done, not by product category or demographics.
