# Instructor notes — Ground before you generate

> Reference (illustrative). One section per agenda step, headed by its start time, so you can find your place with one glance at the clock. **Say** is a cue, not a script; **Do** is what your hands do; **Watch** is what to look for in the room. `KitCheck kit` checks that every agenda step has a section and that none is longer than about 150 words.

## 0:00 · Pre-assessment

- Say: "This tells me what to teach, not what you know. 'I don't know' is a good answer."
- Do: hand out A to odd seats, B to even. Timer on screen, 6 minutes. Collect face down.
- Watch: anyone still writing at 5:00 gets "finish the sentence you're on".

## 0:06 · Credibility

- Say: INC-08 in three sentences: two balances on one invoice, one in C#, one in `usp_GetCustomerBalance`, found by Finance. Then: "I run this method on a legacy .NET system; everything today is from that notebook, with the numbers that went against me too."
- Do: one slide, the two numbers side by side. No bio slide.

## 0:10 · Write first

- Say: "One line: the worst change an agent made for you this month."
- Do: 90 seconds silent; ask three people by seat, not volunteers. Write their words on the flipchart; you will point back to them.

## 0:13 · Problem

- Say: EXP-01: cycle time −16% (−25% to −5%), and the defect guardrail failed. Then METR's 2025 study and DORA 2024, one sentence each, with the slide showing the source.
- Do: say "one team, one repository" out loud. Park any ROI question for Q&A.

## 0:19 · Before clip

- Do: play `01-before-BILL-97.mp4` from 00:00, 90 seconds. Pause on the diff.
- Say: "What did it add that already existed?" Do not answer; the pairs will.
- If it breaks: the diff is on handout page 2; show it on paper.

## 0:23 · Pairs: the before-diff

- Say: task, done-criteria, 6 minutes. "Find the rule it re-implemented and the test that proves nothing."
- Watch: collect two wrong answers verbatim for the debrief.
- Extension: "Which existing test would have caught it?"

## 0:29 · Live 1, Ground

- Do: run-sheet segment 3. Type `/prime BILL-97`, slowly.
- Say while it runs: "Will the brief name `SqlHelper.ExecuteDataSet` as the thing to replace? Hands up for yes." Then take one parked question.
- If it breaks: name it, then `02-ground.mp4` from 00:40. Say line from the run sheet.

## 0:36 · Hands-on 1

- Say: "BILL-180. Your brief must name every existing void rule with a path. 12 minutes; hints on page 3."
- Watch: co-host scans for red setup checks first. Anyone without an agent pairs or takes the paper path.
- If behind at 0:46: `git checkout ws-1-ground` and read the reference brief.

## 0:48 · Debrief

- Do: three briefs on screen, chosen while walking: one that found BILL-155, one that missed it, one with the draft question.
- Say: "Who found the ticket that disagrees with this one?"

## 0:52 · Break

- Do: restart the demo worktree with the reset command; check the next fallback clip is cued.

## 1:00 · Live 2, Bound

- Do: `/plan-feature BILL-97`; stop at the human checkpoint.
- Say while it runs: "Predict: will the do-not-touch list include `SqlHelper`?" Read the list aloud when it lands.
- If it breaks: `03-bound.mp4` from 00:30, or open `plans/BILL-97.md` on the fallback branch.

## 1:06 · Hands-on 2

- Say: "Plan, approve, build. 14 minutes. Stop at green or at 1:19, whichever comes first."
- Watch: rate-limit errors (T-05). Stagger: odd tables start now, even tables after one minute.
- If behind at 1:18: `git checkout ws-2-bound`.

## 1:20 · Live 3, Prove

- Do: `/validate`, then `dotnet test`. Show which tests the agent wrote.
- Say while tests run: take the top parked question.
- If it breaks: `git checkout demo/BILL-97-done` and run the tests there.

## 1:26 · Hands-on 3

- Say: "Mark each check: evidence or not. 7 minutes."
- If behind: `git checkout ws-3-built` and do the marking only.

## 1:33 · Q&A

- Do: parking lot first, by name. Then the question bank's likeliest two.
- Say: repeat each question before answering; 60 seconds each.

## 1:40 · Reflect

- Say: "Card: muddiest point; one ticket where you would skip Ground; your Monday ticket."

## 1:44 · Buffer

- Do: if unused, extend Q&A; never start new content here.

## 1:51 · Close

- Say: follow-up in two weeks, one line back; the kit's link; how to reach me. No offer unless the host asked for one.

## 1:54 · Post-assessment

- Do: the other form to each seat; then the feedback form. Collect both before anyone leaves.
