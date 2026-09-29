# Pricing worksheet — Fabrikam Freight, rollout scenario (if the pilot passes)

> **Illustrative planning scenario, not evidence.** What a six-team rollout would be worth *if* the pilot on Fabrikam's own tickets meets its pre-registered go criteria (cycle-time ratio interval below 1.0, escaped-defect difference no worse than +5 points). Until then, the effect row is EXP-01's interval and the defect row is the go criterion, not a measurement. Check with `OfferCheck price`.

- Offer: Implementation (rung 3), six teams, 10 weeks
- Client: Fabrikam Freight (fictional)
- Price: 64,000
- Currency: USD
- Horizon months: 12
- Effect source: EXP-01 (planning); replaced by the pilot's EXP-F1
- Effect scope: same-team (scenario)
- Seed: 20

| input | low | high | unit | source |
|---|---|---|---|---|
| tickets_per_month | 190 | 290 | tickets | six teams × the pilot team's range |
| baseline_median_hours | 11 | 16 | h | pilot week-0 baseline (estimate until measured) |
| ratio | 0.75 | 0.95 | time ratio | EXP-01 interval; scenario assumes the pilot reproduces it |
| defect_delta | 0 | 0.05 | share of tickets | pilot go criterion: no worse than +5 points (EXP-F1 to confirm) |
| hours_per_defect | 4 | 8 | h | Fabrikam incident log |
| loaded_rate | 80 | 110 | USD/h | Fabrikam finance range |
| agent_spend_per_month | 7200 | 14400 | USD | 48 developers, as of 2026-09 |
| adoption_share | 0.5 | 0.9 | share of tickets | Module 19 adoption plan targets |
