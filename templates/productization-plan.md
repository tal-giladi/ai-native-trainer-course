# Productization plan (template)

The module 22 artifact: a 12-month plan that fits a capacity cap, with capped retainers, products that are licensed and versioned, revenue as ranges, measurable milestones and stop rules. It has three companion formats in this file: the product catalog (22.2), the kit manifest (22.3) and the plan itself (22.4). The service card has its own [template](service-card.md) (22.1). Introduced in [22.1](../lessons/module-22/lesson-01.md) to [22.4](../lessons/module-22/lesson-04.md); check each part with `ScaleCheck` in [`labs/module-22`](../labs/module-22/README.md). A worked example: [`labs/module-22/solution/`](../labs/module-22/solution/productization-plan.md).

> [!CAUTION]
> Licensing, contract, tax and worker-status content is **orientation only and jurisdiction-dependent**. Your employment contract and client SOWs may already decide who owns what ([20.5](../lessons/module-20/lesson-05.md)). Have your licence terms, team-licence terms and retainer contracts reviewed by a lawyer and an accountant where you work.

## Rules

- **The cap is real and public.** State it in the application form; the allocation never exceeds it; there is a buffer.
- **Retainers are capped and expire.** Day cap, expiry of unused days, notice, scope, fee at or above your floor per capped day. Never on-call.
- **Every product has its own evidence**, a licence, and honest live days. A course does not borrow the workshop's gain.
- **Code under a software licence, documents under a named licence, paid material under written terms.** Team licences state seats, term, resale, attribution and updates.
- **Nothing from an employer or a client** in any product; scan with a private deny list.
- **Forecasts are ranges; milestones have exit signals with a number or a check; stop rules have numbers.**

## Part 1 — Product catalog (`catalog.md`, 22.2)

```markdown
# Product catalog — <your workshop or method>

- Method: <name> v<MAJOR.MINOR.PATCH>
- Currency: <currency, taxes>
- Capacity: <days a month for these products>

## Products

| ID | Product | Format | Price | Units/month | Live days/unit | Fixed live days/month | Licence | Evidence |
|---|---|---|---|---|---|---|---|---|
| S0 | <free starter pack and posts> | public | Free | 0 | 0 | 0 | MIT (code), CC BY 4.0 (docs) | <stranger test k/n> |
| W1 | <workshop> | live | <price> | <low–high> | <days incl. prep> | 0 | <handout terms> | <your measured gain> |
| C1 | <course> | self-paced | <price per seat> | <low–high> | 0 | 0 | personal, one seat | <its own gain, or "none yet"> |

## Licences

| Artifact | Audience | Licence | Notes |
|---|---|---|---|
| <public code> | public | MIT | LICENSE file in the repository |
| <public documents> | public | CC BY 4.0 | attribution: title, author, source, licence |
| <team licence> | one company | internal use, up to <n> named seats, <n>-month term, no resale or sublicensing, attribution kept | updates within the same MAJOR version |

## Third-party material

| Item | Title | Author | Source | Licence |
|---|---|---|---|---|
| <where it is used> | <title> | <author> | <source> | <licence> |

## Community rules

- Response time: <how fast, and what happens to the rest>
- Office hours: <how many, how long, recorded with consent or not>
- Not included: <private code reviews, client-specific advice, NDA work>
- Moderation: <code of conduct; no employer or client code or data>
- If it closes: <notice, refund, export of members' own posts>
```

## Part 2 — Kit manifest (`kit/kit.md`, 22.3)

```markdown
# <Kit name>

- Kit: <name>
- Version: <MAJOR.MINOR.PATCH>
- Method: <name> v<version>
- Checked: <yyyy-mm-dd>
- Support: <channel, response time, which versions receive fixes>

## Items

| Item | Type | Path | Version | Licence | Last verified | Stranger test |
|---|---|---|---|---|---|---|
| <template> | template | templates/<file>.md | <x.y.z> | CC BY 4.0 | <yyyy-mm-dd> | <k>/<n> |
| <starter pack> | starter | starter-pack/README.md | <x.y.z> | MIT | <yyyy-mm-dd> | <k>/<n> |
| <assessment> | assessment | assessment/<file>.md | <x.y.z> | licensed to buyers | <yyyy-mm-dd> | <k>/<n> |
```

Beside it: `CHANGELOG.md` in Keep a Changelog format, LICENSE files, and for the assessment a `responses.csv` (`learner,q1,...,qn`, 0/1) for `ScaleCheck items`, plus one paragraph on what a score means and does not mean.

## Part 3 — The plan (`productization-plan.md`, 22.4)

```markdown
# Productization plan — <period>

- Owner: <you, legal form>
- Start: <yyyy-mm>
- Capacity: <days a month>
- Floor: <day-rate floor>
- Employer: <permission, date, what it covers, review date>
- When full: <waitlist with honest start date; referral after <n> months; stated in the application form>
- Demand: <low>–<high> <engagements> a month
- Job days: <your days per engagement, from the service card>
- Job variability: <CV of delivery hours, from the service card>
- Arrival variability: 1.0

## Allocation

| Activity | Days/month |
|---|---|
| Delivery: <engagements> | <d> |
| Retainers | <d> |
| Workshops and kickoffs | <d> |
| Community and course upkeep | <d> |
| Selling and content | <d> |
| Admin | <d> |
| Buffer | <d> |

## Retainers

| Retainer | Days/month cap | Fee/month | Rollover | Notice | Scope |
|---|---|---|---|---|---|
| <client: purpose> | <d> | <fee> | <unused days expire at month end> | <30 days> | <what the days are for> |

## Revenue streams (month 12)

| Stream | From month | Low | High | Price | Live days/unit |
|---|---|---|---|---|---|
| <stream> | <m> | <units> | <units> | <price> | <d> |

## Roadmap

| Month | Milestone | Exit signal | Evidence |
|---|---|---|---|
| 1 | <milestone> | <number or check> | <file> |
| ... | | | |
| 12 | Year review | <review written; next plan published> | <file> |

## Stop rules

- If <measure> is below <number> by month <m>, <what stops>.

## What I will not do this year

- <e.g. hire, sell a credential, leave the job — and the evidence that would change it>
```

## Reading the checks

- Revenue per live day = revenue / (live days per unit × units + fixed live days). Scalable share = revenue from products with no live days per unit / all revenue.
- Wilson 95% interval for a stranger test of $k$ of $n$ ([07.4](../lessons/module-07/lesson-04.md)).
- Item difficulty $p$ = share correct; discrimination $D = p_U - p_L$ with upper and lower 27% groups.
- Expected wait to start: $W_q \approx \frac{\rho}{1-\rho}\cdot\frac{c_a^2+c_s^2}{2}\cdot\tau$, with $\rho$ = demand / (delivery days ÷ job days) and $\tau$ = job days ÷ delivery days per month.
