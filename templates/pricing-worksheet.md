# Pricing worksheet (and offer ladder)

Two formats: the **offer ladder** that fixes what you sell and at what price (introduced in [20.1](../lessons/module-20/lesson-01.md)), and the **pricing worksheet** that turns your own measured effect and its interval into a value range for one client (introduced in [20.2](../lessons/module-20/lesson-02.md)). Check them with `OfferCheck ladder` and `OfferCheck price` ([Module 20 labs](../labs/module-20/README.md)).

## Offer ladder

```markdown
# Offer ladder — <your offer name>

- Positioning: For <one audience> who <one problem, in their words>, unlike <what they do today>, I <what you do> and <how they will know>.
- Wedge: <from Module 1>
- Capacity: <real cap: days a month, engagements at a time>
- Currency: <currency, taxes included or not>

| Rung | Offer | Problem (buyer's words) | Deliverables | Duration | Price | Evidence | Entry | Next |
|---|---|---|---|---|---|---|---|---|
| 1 | <first step, e.g. workshop> | "…" | … | … | <currency> <fixed> | <ID: result with interval> | <who qualifies> | 2 |
| 2 | <measured pilot> | "…" | … | … | … | EXP-nn | <evidence from rung 1, sponsor, data> | 3 |
| 3 | <implementation> | "…" | … | … | <range> | <client's own pilot result> | <pilot met go criteria> | 4 |
| 4 | <retainer> | "…" | … | monthly, minimum, notice | <currency> <n> per month | … | rung 3 delivered | - |
```

Three to five rungs. Each rung produces the evidence the next one needs, and a client can stop after any rung with something useful.

## Pricing worksheet

```markdown
# Pricing worksheet — <client>, <rung>

- Offer: <rung and scope>
- Client: <client>
- Price: <fee>
- Currency: <currency>
- Horizon months: 12
- Effect source: EXP-nn
- Effect scope: same-team | other-team
- Seed: 20

| input | low | high | unit | source |
|---|---|---|---|---|
| tickets_per_month | | | tickets | client's tracker, or their estimate until measured |
| baseline_median_hours | | | h | client's baseline (13.1), or their estimate |
| ratio | | | time ratio | EXP-nn 95% interval; widen the high end towards or past 1.0 when it was measured on another team |
| defect_delta | | | share of tickets | EXP-nn guardrail interval (escaped-defect difference) |
| hours_per_defect | | | h | client's incident log |
| loaded_rate | | | currency/h | client finance; wages plus benefits and overhead |
| agent_spend_per_month | | | currency | tool pricing as of YYYY-MM |
| adoption_share | | | share of tickets | adoption plan target (Module 19) |
```

**What the tool computes.** Monthly net value to the client:

$$\text{net} = N \cdot a \cdot m\,(1-\rho)\,r \;-\; N \cdot a \cdot \Delta_d \cdot h_d \cdot r \;-\; S$$

with $N$ tickets a month, adoption share $a$, baseline median $m$ hours, time ratio $\rho$, loaded rate $r$, escaped-defect difference $\Delta_d$, hours per escaped defect $h_d$ and agent spend $S$. It reports the corner range (every input at its worst or best), a seeded Monte Carlo 90% interval, the probability the monthly net is negative, and, after your fee over the horizon, the ROI interval, the probability the client loses money, and payback at the median.

## Rules

- The effect comes from **your own** experiment (EXP-nn), as an interval. No published study, vendor number or point estimate.
- The quality guardrail's cost is always in the worksheet, even when its interval includes zero.
- Client inputs are ranges they gave you, marked as estimates until measured.
- The proposal quotes the range and, when it is material, the probability of loss. It never quotes a single ROI.
- Prices are "as of" a date. Tool costs and market rates change; review the worksheet when they do.
