# Module 20 plan — Offers, Pricing, Selling and Consulting

5 lessons · ~80 min instruction · practice 5 h plus a real pitch · depends on Module 13 (EXP-01 interval, claims ladder, money as a range), Module 14 (method, `experiments.md`, IP and employer material), Module 17 (the workshop and hard-questions bank), Module 19 (adoption plan; written in parallel, linked by manifest path), Module 1 (wedge, customer discovery) and Module 18 (free pilot P-07).

Thesis: selling is professional qualification plus honest communication of uncertainty. You sell a ladder of offers in which each rung produces the evidence the next one needs; you price from your own measured interval, including the quality guardrail and the chance the client loses money; you qualify out loud, including "not me"; you write scope a third person could check; and you clear employer conflicts before the first pitch. No manipulation: no invented scarcity, deadlines, decoys or guarantees.

Running example: the same illustrative student (Ground-Bound-Build-Prove, workshop *Ground before you generate*, EXP-01: cycle time −16%, 95% CI −25% to −5%, escaped defects +12.5 points, CI 0 to +25) selling to a fictional prospect, Fabrikam Freight (48 developers, .NET Framework + SQL Server, a refund caused by a duplicated credit-hold rule).

Shared lab material: `labs/module-20/` — `tools/OfferCheck` (dependency-free C#: `ladder`, `price`, `qualify`, `call`, `proposal`, `sow`, `changes`, `admin`), the reference `solution/workshop-kit/`, five breaks.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 20.1 Positioning and the offer ladder | Problem in the buyer's words (jobs to be done); positioning sentence; ladder workshop → pilot → implementation → retainer where each rung produces the next rung's evidence; honest scarcity (a stated cap) vs decoys and fake scarcity | 01.3, 13.6, 17.1 | Write positioning and ladder; `ladder` | "Everything" ladder: 7 rungs, "contact us", 10x guarantee, "only 2 spots left" | `offer-ladder.md` |
| 20.2 Pricing with uncertainty | Cost floor, market reference, value ceiling; fixed fee vs day rate vs retainer; value range from EXP interval incl. defect cost and adoption; corners vs Monte Carlo; ROI interval, P(loss), payback; pilot priced as a measurement | 20.1, 13.5, 13.6 | Worksheet for one prospect; `price` | Borrowed 55% (Peng), point estimate, no defects, 400-dev extrapolation, ROI 20x, guarantee | `pricing.md` |
| 20.3 Qualification and discovery calls | Application form (data minimization, conflict question, expectation question); scoring CALL/NURTURE/REFER/DECLINE; discovery call: past event, success measure, decision, budget, timing, constraints; expectation reset; no pressure tactics (FTC dark patterns) | 20.1, 20.2, 01.4 | Form + 1 real discovery call; `qualify`, `call` | Form collecting DOB/salary/address; hard-close call: 86% talk, "only 2 spots", guarantee | `application-form.md`, `discovery/` |
| 20.4 Proposals, SOWs and scope control | Proposal in their words with options, evidence IDs and "not promised"; SOW with results-based deliverables and acceptance criteria (FAR 37.602), exclusions, assumptions, client responsibilities, change control, measurement instead of guarantee; procurement | 20.2, 20.3 | Proposal + SOW for the pitch; `proposal`, `sow`, `changes` | Guarantee proposal with 48-h expiry; SOW with "etc.", unlimited, no acceptance, fees 50k vs 45k; silent-creep change log | `proposal`, `sow`, `change-log.md` |
| 20.5 Legal, admin and difficult clients | Orientation only: employment IP/outside-work clauses (Cal. Lab. Code 2870 as an example), written permission, employer customers; contractor status (IRS, IR35); IP (work made for hire, background IP); liability; payment terms (EU late payment); invoicing and tax; difficult clients: scope pressure, late payment, blame, guarantee requests | 20.4, 14.4 | Admin register before the pitch; `admin` | Moonlighting: work laptop, employer's customer, no contract, all IP to client, net-120 invoices | `admin.md`, the pitch |

Math (§10): 20.2 — value range with uncertainty: net = $N a m(1-\rho) r - N a \Delta_d h_d r - S$; corners vs Monte Carlo (JCGM 101); ROI $=(T\cdot\text{net} - P)/P$ as an interval; P(loss); payback. Reuses 13.6's money-as-a-range and 13.5's intervals.

Simulation: none for this module (§11).

Templates created: `templates/application-form.md`, `templates/discovery-call-script.md`, `templates/proposal.md`, `templates/sow.md`, `templates/pricing-worksheet.md` (includes the offer-ladder format).

Field (outline): pitch your employer (internal budget) or one network contact.

Links to Module 19 (written in parallel) use manifest paths `lessons/module-19/lesson-0N.md`; they show as broken in `check.py` until Module 19 lands.
