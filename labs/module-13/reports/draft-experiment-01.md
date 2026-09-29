# Experiment 01 — AI agents on Contoso Billing (DRAFT — deliberately flawed)

> **Break exhibit for lesson 13.6.** This draft is written the way many first reports are. Every number in it comes from `data/contoso-selfselected.csv` (illustrative) and is arithmetically correct. Find every claim the data does not support before you read the lesson's Fix it section. Do not reuse its sentences.

## Executive summary

**AI agents made the Contoso Billing team 42% faster** and improved quality at the same time. Over 12 weeks and 120 tickets, median cycle time fell from 14.1 hours to 8.2 hours for AI-assisted work, a highly significant result (p = 0.0017). Escaped defects were cut in half (12% → 6%), and PRs were reviewed 61% faster. We recommend rolling agents out to all 400 developers immediately.

## Business impact

At a loaded rate of $95/hour, 42% of our 14.1-hour median ticket saves 5.9 hours per ticket. With 400 developers each closing about 5 tickets a month, that is **$1.12 million saved per month, or $13.5 million a year**, against a tooling cost of about $0.4 million a year. **ROI: 34x.**

## Method

Developers used the agent whenever they felt it would help. We recorded 120 tickets across 6 developers between weeks 1 and 12. We compared all AI-assisted tickets with all manual tickets. Differences were tested with a permutation test (10,000 shuffles).

## Results

| Metric | Manual (n = 51) | AI (n = 69) | Change |
|---|---|---|---|
| Median cycle time | 14.1 h | 8.2 h | −42% |
| Ratio of geometric means | | | 0.584, 95% CI 0.421–0.807 |
| PR review time (ratio of geometric means) | | | −61% |
| Tickets with escaped defects | 12% | 6% | −6 pt |
| Lines changed per ticket | | | more with AI (developers are more productive) |

## Conclusion

The evidence is clear: AI makes developers faster and produces better code. The effect is statistically significant and large. There are no significant downsides.
