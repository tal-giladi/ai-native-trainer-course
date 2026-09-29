# Case-study template

*Used in [21.5 · Writing the anonymized case study](../lessons/module-21/lesson-05.md). One page, written within a week of the handover or the 90-day follow-up, from the engagement record ([playbook template](engagement-playbook.md)), never from memory. Check with `EngageCheck casestudy <file> --record <engagement-record.md> --deny <private-deny-list.txt>` ([Module 21 labs](../labs/module-21/README.md)). Reference: [`labs/module-21/solution/case-study.md`](../labs/module-21/solution/case-study.md).*

> [!CAUTION]
> Publish only with the client's **written approval of the exact text**, as your SOW's IP or publicity clause requires ([20.4](../lessons/module-20/lesson-04.md), [20.5](../lessons/module-20/lesson-05.md)). Anonymized means a motivated outsider could not work out who the client is from the text plus what is public: no names, no product names, no tracker keys, no exact headcounts, no city plus industry plus size. If the client was your employer's customer or the work used your employer's material, you need your employer's permission too. When in doubt, leave it out.

## Rules

- **Every number comes from the record**, with its interval on the same line or in the same table row. No number the record does not contain; no rounding in your favour.
- **The status is the record's status.** An inconclusive result is written as inconclusive, not as "up to" or "trending towards".
- **Report what is generally expected, not the best case.** A single engagement is one draw; say so. "Results may vary" does not fix an unrepresentative claim (FTC Endorsement Guides, 16 CFR 255.2).
- **Bands, not exact values** for anything that identifies: 15–25 developers, a business-to-business software company, a European country.
- **Say what you did**, specifically enough that a reader could repeat it.
- **Limitations are a heading**, not a footnote.

## Skeleton

```markdown
# Case study — <the problem, in the client's words, no client name>

- Client approval: approved in writing by <role> on YYYY-MM-DD, for this exact text
- Engagement: <length, month and year range>
- Record: <client code>

## Context
<banded description: industry type, size band, stack, what they had tried>

## Problem
<the job to be done, in the sponsor's and the team's words; the past event that made it urgent>

## What we did
- <phase: what, with whom; who owns it now>

## Results
| Outcome | Before | After | 95% interval | Reading |
|---|---|---|---|---|
| <from the record> | | | | improved / inconclusive / worse |

<one paragraph: what the results support and what they do not, including the detectable effect>

## What this does not show
- <at least three bullets>

## What the client did next
<their decision, if they allow you to say it>
```
