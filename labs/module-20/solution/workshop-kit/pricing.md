# Pricing worksheet — Fabrikam Freight, measured pilot

> **Illustrative.** Fabrikam Freight is a fictional prospect. The effect and guardrail come from the student's EXP-01 (the Module 13 worked example); the client's inputs are the ranges its engineering manager gave on the discovery call and must be replaced by its own baseline in pilot week 0. Format: [pricing worksheet template](../../../../templates/pricing-worksheet.md). Check with `OfferCheck price`.

- Offer: Measured pilot (rung 2), one team of eight
- Client: Fabrikam Freight (fictional)
- Price: 18,000
- Currency: USD
- Horizon months: 12
- Effect source: EXP-01
- Effect scope: other-team
- Seed: 20

| input | low | high | unit | source |
|---|---|---|---|---|
| tickets_per_month | 32 | 48 | tickets | Fabrikam EM on the discovery call, "35 to 45 a month"; widened |
| baseline_median_hours | 11 | 16 | h | Fabrikam EM estimate; replaced by the week-0 Jira export |
| ratio | 0.75 | 1.05 | time ratio | EXP-01 interval 0.75–0.95, high end widened to 1.05 because it was measured on another team |
| defect_delta | 0 | 0.25 | share of tickets | EXP-01 guardrail, +0 to +25 points (failed) |
| hours_per_defect | 4 | 8 | h | Fabrikam incident log, two recent escaped defects |
| loaded_rate | 80 | 110 | USD/h | Fabrikam finance range; BLS ECEC benefits ≈ 30% of compensation as a sanity check |
| agent_spend_per_month | 1200 | 2400 | USD | vendor seat price plus usage for eight developers, as of 2026-09 |
| adoption_share | 0.5 | 0.9 | share of tickets | Module 19: not every ticket gets the method in the first months |

## Reading

The value range is the client's saving *if EXP-01's effect transfers*, which the pilot exists to find out. The fee is priced on the decision the pilot informs (whether to roll out to six teams), not on a saving we cannot yet promise. The worksheet makes no promise of any return; the proposal quotes the range and the probability of loss as they come out of `OfferCheck price`.
