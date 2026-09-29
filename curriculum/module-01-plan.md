# Module 1 plan — The AI-Native Trainer Role & Picking Your Wedge

4 lessons · ~60 min instruction · 4–6 h field work · no prerequisites. Written after Modules 2–13, so it links *forward* to them ("you'll do this in Module N") and never assumes their content.

Module 1 is the field-work module. It decides what the student will build against for the rest of the course (the wedge and its stack) and collects the first real evidence (verbatim interview quotes). No pricing content (principle: evidence before price; Module 20 prices). Everything is sized in engineer-hours, never money.

Shared lab material: `labs/module-01/` — `transcripts/bad-interview.md` (18-question transcript full of leading, hypothetical, generic, pitching and double-barrelled questions, with one buried real incident), `solutions/bad-interview-key.md`, `tools/TranscriptLint` (dependency-free C# heuristic linter: flags question defects, counts past-specific evidence, talk ratio), `exercises/hype-or-pain.md` (12 statements), `sql/review-wait.sql` (sizing a review-queue pain from PR data), `worksheets/` (wedge, stack inventory, interview log CSV, pain map), `examples/` (a worked wedge and pain map for a fictional .NET/SQL Server wedge).

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 01.1 The roles and the flywheel (C) | Distinguish trainer / consultant / enablement engineer by deliverable, buyer and success measure; explain the four-stage flywheel and the proof each stage hands the next; trace every proposal sentence to an artifact | — | Proof-chain inventory: 6 sentences you want to say to a client → artifact → module that produces it | A 90-day plan that starts at stage 4 (pricing page, cold outreach) with no evidence | `wedge/role-and-stage.md` |
| 01.2 Engineering pain vs AI hype (C) | Four tests (observable, recurring, costly, owned) + workaround signal; size a pain in engineer-hours with a range; buyer / user / champion / blocker | 01.1 | Classify 12 statements; size one pain from your own PR data | Pain sized as calendar wait × PRs → 3,276 h/month, more than the team's capacity | pain sizing sheet |
| 01.3 Finding your wedge (C) | Wedge = stack × audience × pain in one sentence; five criteria + weighted matrix; stack inventory; reachability count | 01.2 | `wedge.md` + stack inventory; count reachable people | Tool-first, hedged wedge ("Cursor training for developers, mostly .NET…") | `wedge.md`, `stack-inventory.md` |
| 01.4 Customer discovery interviews (I) | Rewrite leading/hypothetical questions into past-specific ones; critique a transcript; run 3–5 real interviews with consent; pain/problem map; support / change / kill decision | 01.3 | Critique `bad-interview.md` by hand, then with `TranscriptLint`; run 3–5 interviews with the discovery script | "5 of 5 loved it" summary with zero past behavior and zero commitments | transcripts with verbatim quotes, `pain-map.md`, updated `wedge.md` |

Math (light, §10 has none for M1): pain sizing $H = n \cdot f \cdot t$ with a low/high range and a capacity sanity bound (01.2); reachability funnel (01.3); probability of hearing a pain held by a fraction $p$ of the audience at least once in $n$ interviews, $1-(1-p)^n$, and why 5 interviews cannot estimate prevalence (01.4).

Template created: `templates/customer-discovery-script.md` (the §13 "customer discovery script"; the "discovery-call script" is Module 20's and is not created here).

No simulation. Forward links only to existing modules 02–13; later modules are named in text.
