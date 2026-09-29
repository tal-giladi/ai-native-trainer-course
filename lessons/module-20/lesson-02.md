---
id: "20.2"
module: 20
minutes: 17
practice_minutes: 75
prerequisites: ["20.1", "13.5", "13.6"]
objectives:
  - Set a price between a cost floor, a market reference and a value ceiling, and choose between day rate, fixed fee, retainer and success fee for each rung.
  - Compute a client's value range from your own measured effect and its interval, including the quality guardrail's cost, adoption and tool spend, at the corners and with a seeded Monte Carlo.
  - Report ROI as an interval with the probability that the client loses money and the payback at the median, never as a single number.
  - Price a pilot as a measurement when one team's value range straddles zero, and say so in writing.
volatility: concept
sources:
  - title: "JCGM 101:2008 — Evaluation of measurement data, Supplement 1 to the GUM: Propagation of distributions using a Monte Carlo method (BIPM)"
    url: https://www.bipm.org/documents/20126/2071204/JCGM_101_2008_E.pdf
  - title: "U.S. Bureau of Labor Statistics — Employer Costs for Employee Compensation (news release)"
    url: https://www.bls.gov/news.release/ecec.nr0.htm
  - title: "Peng, Kalliamvakou, Cihon and Demirer (2023) — The Impact of AI on Developer Productivity: Evidence from GitHub Copilot"
    url: https://arxiv.org/abs/2302.06590
  - title: "FTC — Policy Statement Regarding Advertising Substantiation"
    url: https://www.ftc.gov/legal-library/browse/ftc-policy-statement-regarding-advertising-substantiation
last_verified: "2026-09-28"
---

# 20.2 · Pricing with uncertainty

## Why it matters

"What's the ROI?" is the question every budget owner asks, and the one where honest trainers lose credibility fastest, in either direction. Answer with a borrowed headline ("studies show 55% faster") and the first engineer in the room who has read the study takes your number apart. Answer with a single figure from your own data ("ROI 149%") and you have promised a point your own interval does not support. Refuse to answer and the budget goes to someone who will.

This course's rule, stated since Module 13, is that ROI claims may cite only your own measured numbers, and money is always a range ([13.6](../module-13/lesson-06.md)). This lesson turns that rule into a price. You will compute what one client could gain from your measured effect, with its interval, the cost of the quality guardrail that failed, the share of tickets that will actually use the method, and the tool spend, and you will find, as the reference student does, that for one team the honest answer is "about break-even, with a real chance of loss". That finding does not kill the sale. It changes what you are selling.

> [!NOTE]
> Content tags. **Concept** (stable): cost floor, market reference and value ceiling; pricing models; value and ROI as intervals; corners vs Monte Carlo; pricing a measurement. **Implementation**: `OfferCheck price`, the example rates and tool spend (USD, as of 2026-09).

## How it works

### Three numbers bound the price

- **Cost floor**: what you need per day to make the work worth doing. Target annual income plus business costs (insurance, tools, accountant, unpaid selling time), divided by the days you can actually sell. With 120,000 + 30,000 over 100 sellable days, the floor is 1,500 a day. Below it, every engagement makes you poorer.
- **Market reference**: what comparable trainers and consultancies charge for comparable work in your market. It tells you where buyers will start comparing, not what you are worth.
- **Value ceiling**: what the work could be worth to this client, as a range. Above the lower part of that range, you are asking the client to carry risk you created.

Price between the floor and a small share of the value range. If the floor is above the value, the offer is wrong for this client, not the price.

### Pricing models, by rung

| Model | Fits | Risk sits with | Watch out for |
|---|---|---|---|
| Day rate | open-ended advisory work | the client (more days, more cost) | rewards slowness; invites micromanagement of hours |
| Fixed fee | workshop, pilot, implementation with a SOW | you (overruns are yours) | needs tight scope and change control (20.4) |
| Retainer | capped monthly upkeep | shared | must state days per month and what rolls over |
| Success fee | rarely, and only on a pre-registered measure | you, on a number you do not control | ties your income to the result, which pressures the measurement itself |

The reference ladder uses fixed fees for rungs 1–3 and a capped retainer for rung 4. It uses no success fee: EXP-01's quality guardrail failed, and a fee that grows with speed would pay the student for the very trade-off the pilot must detect.

### The value range

*Intuition.* The saving is the hours the method removes from the tickets that use it, minus the hours lost to extra escaped defects, minus what the tools cost. Each of those inputs is a range. The effect's range is your experiment's interval; the others are the client's own numbers.

*Equation.* With $N$ tickets a month, adoption share $a$, baseline median $m$ hours per ticket, time ratio $\rho$ (0.84 means 16% shorter), loaded rate $r$, escaped-defect difference $\Delta_d$, hours per escaped defect $h_d$ and agent spend $S$:

$$\text{net per month} = N a\, m\,(1-\rho)\, r \;-\; N a\, \Delta_d\, h_d\, r \;-\; S$$

and, for a fee $P$ over a horizon of $T$ months,

$$\text{ROI} = \frac{T \cdot \text{net} - P}{P}, \qquad \text{payback} = \frac{P}{\text{net}}.$$

*Tiny example.* EXP-01's own team: $N = 40$, $a = 1$, $m = 13.8$, $r = 95$, $h_d = 6$, $S = 1{,}800$. At the point estimates ($\rho = 0.84$, $\Delta_d = 0.125$): $552 \times 0.16 \times 95 = 8{,}390$ saved, $5 \times 6 \times 95 = 2{,}850$ lost to defects, net **3,740 a month**. A 12-month ROI on an 18,000 fee would be $(44{,}880 - 18{,}000)/18{,}000 = 149\%$: the single number to refuse. At the ends of the intervals: $\rho = 0.75$ and $\Delta_d = 0$ give $+11{,}310$; $\rho = 0.95$ and $\Delta_d = 0.25$ give $2{,}622 - 5{,}700 - 1{,}800 = -4{,}878$. The honest monthly statement for the team that was measured is **about −4,900 to +11,300**.

*Implementation.* Two ways to combine ranges:

- **Corners**: evaluate every input at its low or high end and take the worst and best result. Easy to explain, and a hard bound, but it assumes every bad (or good) thing happens at once, which is rare.
- **Monte Carlo**: draw each input from a distribution, compute the net, repeat 20,000 times and read percentiles. This is the method of JCGM 101, the supplement to the international guide on measurement uncertainty, for propagating distributions through a model. `OfferCheck price` draws the time ratio from a log-normal whose 95% interval matches your experiment's, the defect difference from a normal matching its interval, and the client's planning ranges uniformly; the seed is fixed, so the numbers are reproducible.

Two corrections the worksheet makes before any of that:

- **Transfer.** EXP-01 was measured on another team. For a new client, widen the ratio's high end past 1.0 (the effect there may be zero or negative) until their own pilot measures it.
- **Adoption.** Not every ticket gets the method in the first months. Module 19's adoption metrics ([19.4](../module-19/lesson-04.md)) give a realistic share; 50–90% is a plausible planning range, not 100%.

*Interpretation.* Read three numbers: the 90% interval of monthly net, the probability it is negative, and, after your fee, the probability the client loses money over the horizon. When that probability is material, it goes into the proposal in plain words.

### The loaded rate

Use the client's own figure. If they have none, remember that salary is not cost: in the US private sector, benefits were 30% of employer compensation costs in June 2026 (BLS), before overhead such as space and management. A loaded rate is a range too.

## Show me

The reference worksheet for Fabrikam Freight, a fictional prospect, [`pricing.md`](../../labs/module-20/solution/workshop-kit/pricing.md), prices the measured pilot for one team of eight. The ratio row is EXP-01's interval widened to 1.05 for transfer; volume, baseline, rate and defect cost are the engineering manager's ranges from the discovery call (20.3).

```bash
cd labs/module-20
dotnet run --project tools/OfferCheck -- price solution/workshop-kit/pricing.md
```

```text
Monthly net value to the client (before your fee)
  corners, every input at its worst or best:  -15,706 to 17,808
  Monte Carlo 90% interval (20,000 draws, seed 20): -4,842 to 5,311 (median 85)
  P(monthly net < 0): 49%
Over 12 months, after the fee of 18,000:
  net 90% interval: -76,100 to 45,727 (median -16,980)
  ROI 90% interval: -423% to +254% (median -94%)
  P(the client loses money over the horizon): 69%
  payback at the median: not within ten horizons (median monthly net near or below 0)
  note: the median value does not cover the fee; this rung is priced as a measurement, and the proposal must say it may not pay back on its own
```

For one team, EXP-01's effect, if it transfers at all, roughly pays for the tools and the defects it may add. The pilot will very likely not repay itself from that one team's saving. Selling it on ROI would be dishonest.

So the student prices it as what it is: a measurement for a decision. Fabrikam spends about 150,000 a year on seats and is deciding in January whether to expand to six teams or cut back. [`pricing-rollout-scenario.md`](../../labs/module-20/solution/workshop-kit/pricing-rollout-scenario.md) shows what the rollout would be worth *if* the pilot meets its pre-registered go criteria (ratio below 1.0 on their tickets, defects no worse than +5 points):

```text
  Monte Carlo 90% interval (20,000 draws, seed 20): -264 to 45,046 (median 18,610)
  P(monthly net < 0): 5%
Over 12 months, after the fee of 64,000:
  ROI 90% interval: -105% to +745% (median +249%)
  P(the client loses money over the horizon): 13%
  payback at the median: 3.4 months
```

The pilot fee is 12 days at the 1,500 floor. It is about one month of the rollout's median value in the good case, and in the bad case it prevents a rollout that would lose money. The worksheet warns that the scenario's defect row is a go criterion, not a measurement, and the proposal says exactly that.

## Try it

Budget: about 75 minutes.

1. **Floor (10 min).** Your target income, costs and sellable days, honestly counted (selling and admin are not sellable). Compute your day rate floor.
2. **Market reference (15 min).** Three public price points for comparable work in your market, with dates. Note what each includes.
3. **Worksheet (35 min).** Copy the [pricing worksheet template](../../templates/pricing-worksheet.md) for the person you will pitch (your employer counts). Effect and guardrail from your own EXP report; widen for transfer; client ranges from what they told you, marked as estimates.
4. **Run and read (15 min).** `price` clean. Write two sentences you would put in a proposal: the value range, and the chance of loss. If the median does not cover the fee, decide: smaller scope, lower price, or sell it as a measurement.

<details>
<summary>Hint: my EXP interval includes zero</summary>

Then your value range includes losses at the median, and no price makes an ROI pitch honest. Sell the workshop on its learning evidence, and the pilot as the measurement your own data could not settle. Say "my own data could not rule out no effect; that is why I measure it on yours". Buyers who have been burned by vendor numbers tend to trust this more, not less.
</details>

## Break it

[`break/20.2-borrowed-roi/pricing.md`](../../labs/module-20/break/20.2-borrowed-roi/pricing.md) prices a 150,000 implementation for all 60 Fabrikam developers. Its effect is "Peng et al. 2023, 55.8% faster", entered as a point estimate. It has no defect row, no adoption row and no agent spend. Its summary reads: "2,350 hours saved every month, which is $3.1 million a year: an ROI of 20x … We guarantee a 30% productivity improvement or your money back."

```bash
dotnet run --project tools/OfferCheck -- price break/20.2-borrowed-roi/pricing.md
```

Before running it: what did Peng et al. actually measure, and on whom (13.2)?

## Fix it

**Diagnose.** Nine errors and five warnings, and the value range is not even computed:

- **Borrowed effect.** Peng et al. measured a controlled task, implementing an HTTP server in JavaScript, with recruited developers using an early code-completion tool: 55.8% faster on that task. It is not Fabrikam's tickets, not your method and not your measurement. The tool rejects any effect not sourced to your own `EXP-nn`.
- **Point estimate.** Low equals high; no interval.
- **No quality cost.** The guardrail that failed in EXP-01 is simply missing.
- **Everyone, from day one.** No adoption row, no tool spend; 60 developers × 5 tickets each at a flat 14 hours.
- **Claims.** A multiplier ("20x"), a single-number ROI, a guarantee, and effects without intervals. In advertising, an objective claim such as a speed-up needs a reasonable basis before it is made, and "studies show" needs the studies (FTC). A borrowed study about a different task is not a basis for a claim about this client.

**Modify.** Replace the effect with your EXP interval, widened for transfer. Add the defect row from your guardrail, an adoption range and the agent spend. Price the rung the client should buy first (a pilot), not the rung you would like to sell. Delete the summary and write the two sentences from Try it step 4.

**Rerun.** `price` with no errors. Compare the median with the break's "$3.1 million": the gap is the size of the promise you nearly made.

## How do I know it works?

- [ ] You can state your day-rate floor and how you computed it.
- [ ] Every rung's price is between your floor and a small share of the client's value range, or is labelled as a measurement.
- [ ] Your worksheet's effect and guardrail rows cite your own EXP report with their intervals, widened for transfer where needed.
- [ ] `OfferCheck price` is clean, and you can read out the 90% interval, the probability of loss and the payback at the median.
- [ ] The sentence you will put in the proposal contains a range and, if material, the probability of loss, and no single ROI figure.

## Use / don't use

**Use** the worksheet for every rung above the workshop, and rerun it when the client gives you real baseline numbers. **Use** the probability of loss as a reason to sell a pilot, not as something to hide.

**Don't** quote a published study as the client's expected effect; quote it, if at all, as context with its design. **Don't** accept a success fee tied to a speed metric when your own guardrail failed. **Don't** discount below your floor to win a first client; if you want to invest in a reference, run a free pilot openly (Module 18) instead of a cheap paid one.

**Limitations.**

- The Monte Carlo treats inputs as independent. In reality, a team with high ticket volume may have smaller tickets; correlated inputs change the tails.
- The formula ignores second-order effects: learning time in the first weeks, review load shifting to seniors, and morale. They belong in the pilot's measurement, not in the worksheet.
- Rates, tool prices and benefit shares change; the BLS figure is US private industry, June 2026. Mark worksheets "as of" and revisit them.

## Reflect

1. What is your day-rate floor, and which of your current prices sits below it?
2. What is the probability of loss for your first prospect, and how will you say it out loud?
3. Which published number were you most tempted to borrow, and what would you say instead?

## Sources

- [JCGM 101:2008 — Propagation of distributions using a Monte Carlo method](https://www.bipm.org/documents/20126/2071204/JCGM_101_2008_E.pdf) — Supplement 1 to the Guide to the Expression of Uncertainty in Measurement: propagate input distributions through a model by Monte Carlo and report a coverage interval.
- [BLS — Employer Costs for Employee Compensation](https://www.bls.gov/news.release/ecec.nr0.htm) — June 2026, private industry: wages and salaries 70.0% and benefits 30.0% of employer compensation costs ($46.89 per hour worked).
- [Peng et al. (2023) — The Impact of AI on Developer Productivity: Evidence from GitHub Copilot](https://arxiv.org/abs/2302.06590) — a controlled experiment on one task (an HTTP server in JavaScript): the treated group finished 55.8% faster; the source of the borrowed number in the break.
- [FTC — Policy Statement Regarding Advertising Substantiation](https://www.ftc.gov/legal-library/browse/ftc-policy-statement-regarding-advertising-substantiation) — objective claims need a reasonable basis before they are made; claims of specific support need that support.
