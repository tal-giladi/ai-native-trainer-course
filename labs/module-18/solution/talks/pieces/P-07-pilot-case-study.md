# P-07 · Pilot: sixty minutes with a fourteen-developer team

- Format: case-study
- Concept: C1
- Stage: trust
- Audience: tech leads considering a short workshop for their team
- Evidence: pilot/feedback.md (form results, anonymized); pilot/plan.md
- Consent: yes, the team's tech lead, written, 2026-09-21; company not named at their request
- Quotes: one quote, approved by the developer quoted, 2026-09-21
- Disclosure: the pilot was free; nobody paid me or was paid for this write-up
- Published: 2026-09-23

---

A free, 60-minute workshop for the developers of a logistics software company with a legacy .NET and SQL Server invoicing module. I treated it exactly like a paid engagement: a prep call, a plan, a feedback form and a follow-up two weeks later.

## Before

The team had used a coding agent for about six months. In the prep call the tech lead described the problem as agent PRs that looked fine until finance found a second version of a rule. They had no research step and no written brief; tickets went straight to the agent.

## Intervention

One concept, the shadow rule, taught in 60 minutes with the structure of my earlier mini-workshop. A two-minute story came first, then a worked example on the public demo repository and pairs deciding on three diffs. Last, each developer searched their own invoicing code for an existing rule that a current ticket touches. Nine of fourteen developers attended.

## After

- On parallel pre- and post-tests of six items, the class average went from 39 to 74 out of 100, a normalized gain of 0.57 ([results](../pilot/feedback.md)).
- During the exercise, seven of nine developers found an existing rule in their own code that a current ticket touched.
- Two weeks later, the team had added a reuse row to their ticket template. The tech lead reported three agent plans in those two weeks that named an existing procedure instead of writing a new one.
- One developer wrote: "The pairs part was the useful bit; the diffs looked like ours."

## What this does not show

- Nine people, one session, one team. The test measured what they could do at the end of the hour, not what they do in six months.
- The follow-up is the tech lead's report, not a measurement. I did not see the plans.
- They chose to host a workshop on this topic, so they were more ready than a typical team.

What would you want measured if this were your team's pilot?
