# Proposal — Measuring grounded agent work on one Fabrikam team

> **Illustrative.** Fabrikam Freight is fictional. Written from the A01 discovery call and the two pricing worksheets. Format: [proposal template](../../../../templates/proposal.md). Check with `OfferCheck proposal`.

- To: Engineering manager, Fabrikam Freight (for the VP Engineering)
- From: the student, independent trainer
- Date: 2026-09-18
- Valid until: 2026-10-31 (my calendar for the December deadline fills after that; the price does not change)

## What we heard

In your words: agent PRs "re-implement rules that already live in our stored procedures". Last sprint one of them shipped, a customer received an invoice they should not have, and finance issued a refund; the fix and post-mortem took two days. The post-mortem finding was that "the agent never looked at the database". Median time from PR opened to approved has gone from about one day to about one and a half since spring (your GitHub export, uncontrolled). The DBA lead reverts inline SQL weekly. The licence renewal is in January, and you want evidence to expand or cut it by mid-December: "I would rather know after six weeks."

## Objectives

1. One .NET Framework team works with an AI layer that grounds the agent in the rules the code and the database already have (rules file, Ground, Bound and Prove skills, a pre-merge check).
2. You get a pre-registered, randomized measurement on that team's own tickets: cycle time, escaped defects, DBA reverts and PR review time, each with an interval.
3. You get a written go/no-go recommendation under a rule agreed before the first ticket, in time for the January decision.

## Options

| Option | Scope | Duration | Price |
|---|---|---|---|
| A. Measured pilot (recommended) | Workshop for the pilot team; AI layer built with them on one repository; randomized comparison on their tickets with a pre-registered go/no-go rule; report and read-out | 6 weeks from 2026-10-12, 12 of my days | USD 18,000 fixed |
| B. Workshop only | 2-hour hands-on workshop for up to 16 people; pre/post assessment; two-week follow-up report | 1 day, October | USD 6,500 fixed |

Option B improves how people prompt and review; it will not tell you whether the tool pays for itself on your tickets. Option A will, within the limits below.

## Evidence

- EXP-01, my own randomized comparison (one team, 96 tickets, 12 weeks, 2026): cycle time −16% (95% CI −25% to −5%); tickets with an escaped defect +12.5 points (95% CI 0 to +25), so the quality guardrail failed. That is why this pilot measures escaped defects and DBA reverts as primary outcomes, not only speed.
- P-07, a free pilot workshop (nine learners): normalized learning gain 0.57 on parallel pre/post forms.
- Pricing worksheet for one Fabrikam team, using EXP-01's interval widened for transfer to your team and your own ranges for volume, rates and adoption: monthly net value from about −4,800 to +5,300 USD (90% interval, median near zero). On its own, one team's saving may not repay the pilot. The pilot's value is the decision: a six-team rollout scenario, *if* the pilot meets its go criteria, is worth a median of about 18,600 USD a month (90% interval −300 to +45,000).

## What we do not promise

- Any productivity figure for your teams. EXP-01 is one team's result; yours may be larger, smaller or zero, and the pilot is how we find out.
- That escaped defects will not rise. They are measured, with a stop rule if they rise beyond the agreed threshold.
- Results for the .NET 8 teams, or for teams that do not use the method.
- Anything about production systems: I will not have, or need, production access or production data.

## Investment

USD 18,000 fixed for option A: 50% on signature, 50% on delivery of the report; net 30. Option B: USD 6,500, invoiced after the workshop. Expenses: none (remote, plus one on-site day at my cost). Prices exclude taxes.

## Next steps

1. 2026-09-22: we review this proposal together (30 minutes).
2. By 2026-09-30: your decision; if yes, I send the SOW and your procurement's supplier form.
3. 2026-10-12: pilot kickoff and baseline export, if the SOW is signed by 2026-10-07.
