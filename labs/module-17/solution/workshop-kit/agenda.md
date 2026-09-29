# Workshop agenda — Ground before you generate (2 hours)

> Reference answer for Module 17, **illustrative**. One student's 2-hour workshop built on their method (Ground-Bound-Build-Prove, [`labs/module-14/solution/method-v1.0.0.md`](../../../module-14/solution/method-v1.0.0.md)) and their demo repository (`brownfield-demo`, [Module 15](../../../module-15/README.md)). Format: [workshop template](../../../../templates/workshop-template.md). Check it with `KitCheck agenda` and `LearnCheck align`.

- Workshop: Ground before you generate — agent work on legacy .NET that survives review
- Method: Ground-Bound-Build-Prove v1.0.0 (concepts C1 Shadow rule, C2 Self-graded green)
- Audience: 12–16 developers and leads from one company's .NET teams; all use a coding agent at least weekly; 2–15 years' experience
- Room: in person, one projector, laptops with the starter pack installed the day before; one co-host
- Slot: 120
- Budget: 120
- Demo: brownfield-demo, BILL-97 (live, from tag `ai-layer-v1`); hands-on ticket BILL-180 in the starter pack
- Date: 2026-10-14

## Learning claim

After two hours, a developer who uses a coding agent on a legacy .NET and SQL Server codebase can spot a shadow rule or a self-graded test in an agent's diff, write a Ground brief and a Bound plan that prevent both on their next ticket, and say which checks count as proof.

## Prior knowledge (from four 10-minute calls with the host's leads, two weeks earlier)

- All four teams use an agent; two have a rules file, none has a research or plan step written down.
- "The agent duplicated code" is known; that it happens because the rule was not in its context is not.
- Tests written by the agent in the same run are counted as evidence by everyone asked.
- Two leads expect a talk about prompts. Say in the first five minutes that it is not.

## Objectives

| ID | Objective | Level |
|---|---|---|
| O1 | Given an agent's diff on a legacy .NET repository, decide whether it added a shadow rule or a self-graded test, and name the evidence. | analyze |
| O2 | Write a Ground brief for a ticket that names every existing implementation of its business term across C# and T-SQL, with a path for each. | create |
| O3 | Write a Bound plan with a do-not-touch list and one check per acceptance criterion. | create |
| O4 | Given a finished change and its test run, decide which checks count as proof and which do not. | evaluate |

## Assessment

Parallel forms A and B (BILL-240 and BILL-251 surfaces), counterbalanced. Item rules: [pre/post assessment template](../../../../templates/pre-post-assessment.md). Forms and key: [`forms.md`](forms.md).

| Item | Objective | Level | What it asks |
|---|---|---|---|
| I1 | O1 | analyze | Diff adds a second "can be voided" check next to `InvoiceService`: shadow rule? |
| I2 | O1 | analyze | Diff extends the existing rule with a parameter; its tests were written in the same run: which problem, if any? |
| I3 | O2 | apply | Which search finds the term in C# **and** T-SQL |
| I4 | O2 | create | Write the Ground brief lines for a ticket (rubric: term, every implementation with a path, "reuse, do not re-implement") |
| I5 | O3 | create | Write the do-not-touch list for a plan (rubric: names files, names the public contract, names the SQL object) |
| I6 | O3 | create | Pair each acceptance criterion with a check (rubric: one check per criterion, at least one the agent cannot edit) |
| I7 | O4 | evaluate | "All 9 tests pass": which of the listed checks are evidence? |
| I8 | O4 | evaluate | The agent changed an expected value in an existing test: pass or reject, and why |

## Agenda

| Start | Step | Minutes | Mode | Delivery | Arc | Objective | New terms | Fallback |
|---|---|---|---|---|---|---|---|---|
| 0:00 | Pre-assessment (form A or B), no discussion | 6 | assess | paper | assess | - | 0 | - |
| 0:06 | Credibility: INC-08, two balances on one invoice; who I am and what I measured | 4 | show | talk | credibility | O1 | 1 | - |
| 0:10 | Write first: your worst agent change this month, one line; three read aloud | 3 | do | paper | credibility | O1 | 0 | - |
| 0:13 | Problem: EXP-01 with its intervals and the failed defect guardrail; what public studies found | 6 | show | talk | problem | O1 | 1 | - |
| 0:19 | Before: BILL-97 with no AI layer (recorded clip, 90 s), what went wrong | 4 | show | recorded | demo | O1 | 0 | rec:fallback/01-before-BILL-97.mp4@00:00 |
| 0:23 | Pairs: the before-diff, find the shadow rule and the self-graded test | 6 | do | paper | exercise | O1 | 0 | - |
| 0:29 | Live 1, Ground: /prime BILL-97; prediction question and Q&A while it runs | 7 | show | live | demo | O2 | 1 | rec:fallback/02-ground.mp4@00:40 |
| 0:36 | Hands-on 1: Ground brief for BILL-180 in the starter pack | 12 | do | hands-on | exercise | O2 | 0 | tag:ws-1-ground |
| 0:48 | Debrief from three learners' briefs on screen | 4 | reflect | talk | exercise | O2 | 0 | - |
| 0:52 | Break | 8 | break | - | break | - | 0 | - |
| 1:00 | Live 2, Bound: /plan-feature and the human checkpoint; prediction question while it runs | 6 | show | live | demo | O3 | 1 | rec:fallback/03-bound.mp4@00:30 |
| 1:06 | Hands-on 2: Bound plan and Build for BILL-180 | 14 | do | hands-on | exercise | O3 | 0 | tag:ws-2-bound |
| 1:20 | Live 3, Prove: /validate and dotnet test, which tests count; Q&A while tests run | 6 | show | live | demo | O4 | 1 | branch:demo/BILL-97-done |
| 1:26 | Hands-on 3: Prove your BILL-180 change | 7 | do | hands-on | exercise | O4 | 0 | tag:ws-3-built |
| 1:33 | Q&A: the parking lot and the hard questions | 7 | show | talk | qa | - | 0 | - |
| 1:40 | Reflect: muddiest point; when would you skip Ground; your Monday ticket | 4 | reflect | paper | reflect | O1 | 0 | - |
| 1:44 | Buffer (unused minutes go to Q&A) | 7 | buffer | - | buffer | - | 0 | - |
| 1:51 | Close: the follow-up, where the kit lives, how to reach me | 3 | show | talk | close | - | 0 | - |
| 1:54 | Post-assessment (other form) and feedback form | 6 | assess | paper | assess | - | 0 | - |

## Materials

- Starter pack: [`starter-pack/README.md`](starter-pack/README.md) with the setup check and catch-up tags.
- Exercises: [`exercises/`](exercises/ex1-ground.md); troubleshooting: [`troubleshooting.md`](troubleshooting.md).
- Demo run sheet: `brownfield-demo/demo/runsheet.md` ([Module 15 reference](../../../module-15/solution/runsheet.md)); fallback clips: [`fallback/README.md`](fallback/README.md).
- Forms A/B, key, feedback form, follow-up message: [`forms.md`](forms.md).
- Question bank: [`question-bank.md`](question-bank.md). Instructor notes: [`instructor-notes.md`](instructor-notes.md).

## Room plan

- Mixed levels: pairs mix one lead with one developer; the developer types. Every exercise has an extension.
- Skeptics: the question bank's cards for "isn't this DRY?", "will we get 16%?" and "does our code go to the vendor?"
- Agents: the host confirmed which agent and plan the teams use; learners without access pair with someone who has it, or follow the paper path.
- Co-host: watches for raised hands during hands-on, keeps the parking lot, calls time at one minute left.
