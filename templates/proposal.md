# Proposal

A two-page proposal written after a discovery call, for the person who will take it to the decision-maker. It restates their problem in their words, offers two or three real options with fixed prices, names the evidence behind every claim, and says what is not promised. Introduced in [20.4 · Proposals, SOWs and scope control](../lessons/module-20/lesson-04.md). Check with `OfferCheck proposal` ([Module 20 labs](../labs/module-20/README.md)).

A proposal is not the contract. Once accepted, the scope, deliverables and terms move into a [statement of work](sow.md).

## Rules

- **Their words, not yours.** The situation section quotes the discovery call. If it could be sent to any client, rewrite it.
- **Two or three options**, each with a fixed price or range, including the smallest useful one. No decoy option built to make another look good.
- **Evidence by ID.** Every claim points to one of your experiments, pilots or case studies (EXP-01, P-07…), with its interval.
- **Money as a range**, from your [pricing worksheet](pricing-worksheet.md), with the probability that it does not pay back when that probability is material. Never a single ROI number.
- **What we do not promise** is a section, not a footnote.
- **A validity date is fine; fake urgency is not.** "Valid until 31 October, because my calendar for December fills after that" is a fact. "Expires in 48 hours" is pressure.

## Skeleton

```markdown
# Proposal — <outcome> for <client>

- To: <role> (for <decision-maker's role>)
- From: <you>
- Date: YYYY-MM-DD
- Valid until: YYYY-MM-DD (<the real reason>)

## What we heard
<Their problem, with quotes from the call: the last incident, what it cost, how they measure it today, their deadline.>

## Objectives
1. <What will be true at the end, checkable by someone else.>

## Options
| Option | Scope | Duration | Price |
|---|---|---|---|
| A. <recommended> | … | … | <currency> <fixed fee> |
| B. <smaller> | … | … | <currency> <fixed fee> |

<One sentence each: what this option will and will not tell or give them.>

## Evidence
- <EXP-nn: design, population, effect with interval, guardrails.>
- <Pricing worksheet summary: value range, probability of loss, what it assumes.>

## What we do not promise
- <Any productivity figure for their teams.>
- <Guardrail outcomes (defects, incidents): measured, not promised.>
- <What is out of scope, e.g. production access.>

## Investment
<Fee, payment schedule, payment terms, expenses, taxes.>

## Next steps
1. YYYY-MM-DD: <review together>
2. YYYY-MM-DD: <decision; SOW sent>
3. YYYY-MM-DD: <start, if signed by YYYY-MM-DD>
```
