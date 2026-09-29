# Module 20 labs — Offers, pricing, selling and consulting

Everything the Module 20 labs need. Lessons: [20.1](../../lessons/module-20/lesson-01.md) · [20.2](../../lessons/module-20/lesson-02.md) · [20.3](../../lessons/module-20/lesson-03.md) · [20.4](../../lessons/module-20/lesson-04.md) · [20.5](../../lessons/module-20/lesson-05.md).

The labs turn your measured evidence ([Module 13](../module-13/README.md)), your method ([Module 14](../module-14/README.md)), your workshop ([Module 17](../module-17/README.md)) and your adoption plan (Module 19) into the commercial half of **`workshop-kit`**: an offer ladder, pricing worksheets, an application form, a discovery-call script, a proposal, a SOW, a scope-request log and an admin register. The field assignment is a real pitch, to your employer (internal budget) or to one contact in your network.

> [!CAUTION]
> Legal, tax and employment content in this module is **orientation only and jurisdiction-dependent**. Before you pitch anyone outside your employer, read your employment contract (IP assignment, outside work, non-compete, confidentiality) and get written permission where it is required; never sell to your employer's customers, competitors or suppliers without it. Have contracts reviewed by a lawyer and tax set-up checked by an accountant where you work. Never use employer code, metrics or client names in anything you send a prospect. Keep `workshop-kit`'s commercial files private.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400.
- No NuGet packages. `OfferCheck` is read-only and deterministic (the Monte Carlo uses a fixed seed); it works offline.
- Your Module 13 experiment report (the effect and guardrail intervals), your Module 14 `experiments.md`, your Module 17 `workshop-kit/`.

## Contents

| Path | What it is |
|---|---|
| `tools/OfferCheck/` | Dependency-free C# checker: `ladder` (positioning, rungs, prices, evidence, entry and next steps, capacity, promises and pressure), `price` (value range from a measured effect with its interval: corners, seeded Monte Carlo, probability of loss, ROI interval, payback), `qualify` (application answers: score, conflicts, expectation mismatches, personal data the form should not collect), `call` (discovery transcript: talk share, first pitch, decision/budget/timing questions, leading and hypothetical questions, pressure, promises, an honest limit, a dated next step), `proposal`, `sow` (sections, acceptance criteria, exclusions, change control, payment terms and arithmetic, measurement instead of a guarantee, background IP), `changes` (silent scope creep), `admin` (employer conflicts, contract, tax and invoicing register). |
| `solution/workshop-kit/` | **Illustrative** reference for one student selling *Ground before you generate* to a fictional prospect, Fabrikam Freight: `offer-ladder.md`, `pricing.md` and `pricing-rollout-scenario.md`, `application-form.md` and `applications.csv`, `discovery/A01-call.md`, `proposal-fabrikam.md`, `sow-fabrikam-pilot.md`, `change-log.md`, `admin.md`. Prices are placeholders of a plausible size (USD, as of 2026-09), not market advice. |
| `break/` | One deliberately flawed artifact per lesson: an "everything" ladder with fake scarcity (20.1), a pricing sheet built on a borrowed 55% (20.2), a form that collects personal data and a hard-close call (20.3), a guarantee proposal, an open-ended SOW and a silent-creep change log (20.4), a moonlighting admin register (20.5). |

## Quick start

From this folder:

```bash
O="dotnet run --project tools/OfferCheck --"

# 20.1 — positioning and the offer ladder
$O ladder break/20.1-everything-ladder/offer-ladder.md        # 11 errors, 26 warnings
$O ladder solution/workshop-kit/offer-ladder.md               # clean

# 20.2 — pricing with uncertainty
$O price break/20.2-borrowed-roi/pricing.md                   # 9 errors, 5 warnings
$O price solution/workshop-kit/pricing.md                     # clean; median near zero for one team, 69% chance of loss
$O price solution/workshop-kit/pricing-rollout-scenario.md    # 1 warning; the six-team scenario if the pilot passes

# 20.3 — qualification and discovery calls
$O qualify break/20.3-hard-close/applications.csv             # 7 errors
$O qualify solution/workshop-kit/applications.csv             # clean: CALL 3, NURTURE 1, REFER 1, DECLINE 1
$O call break/20.3-hard-close/call.md                         # 5 errors, 10 warnings
$O call solution/workshop-kit/discovery/A01-call.md           # clean, 43% talk share

# 20.4 — proposals, SOWs and scope control
$O proposal break/20.4-open-sow/proposal.md                   # 13 errors
$O proposal solution/workshop-kit/proposal-fabrikam.md        # clean
$O sow break/20.4-open-sow/sow.md                             # 17 errors, 8 warnings
$O sow solution/workshop-kit/sow-fabrikam-pilot.md            # clean
$O changes break/20.4-open-sow/change-log.md                  # 4 errors, 1 warning
$O changes solution/workshop-kit/change-log.md                # clean

# 20.5 — legal, admin and difficult clients
$O admin break/20.5-moonlighting/admin.md                     # 10 errors, 13 warnings
$O admin solution/workshop-kit/admin.md                       # clean
```

(PowerShell: type the full `dotnet run --project tools/OfferCheck -- <command>` instead of `$O`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem. `OfferCheck` checks structure and wording; it cannot tell whether a price is right for your market or a clause is right for your jurisdiction.

## Your own `workshop-kit/` (commercial files)

```text
workshop-kit/
├── offer-ladder.md            # 20.1 — templates/pricing-worksheet.md (ladder part)
├── pricing/<client>.md        # 20.2 — one worksheet per client and rung
├── application-form.md        # 20.3 — templates/application-form.md
├── applications.csv           # 20.3 — answers, no personal data beyond name and work email (kept outside the CSV)
├── discovery/<id>-call.md     # 20.3 — templates/discovery-call-script.md
├── proposals/<client>.md      # 20.4 — templates/proposal.md
├── sow/<client>-v1.md         # 20.4 — templates/sow.md
├── change-log.md              # 20.4 — scope requests, from day one
└── admin.md                   # 20.5 — conflicts, contract, tax, invoices
```

Formats: [pricing worksheet and offer ladder](../../templates/pricing-worksheet.md), [application form](../../templates/application-form.md), [discovery-call script](../../templates/discovery-call-script.md), [proposal](../../templates/proposal.md), [SOW](../../templates/sow.md).

## Field work: the pitch (about 5 hours plus the meeting)

1. **Admin first (20.5).** Fill `admin.md` rows E1–E5 before you talk to anyone outside your employer. If the first pitch is internal, write down who owns the budget and whether the work is part of your job.
2. **Ladder and price (20.1, 20.2).** Ladder clean; one worksheet for the person you will pitch, using your EXP interval.
3. **Qualify and call (20.3).** Send the form; hold a 30-minute discovery call with the script; run `call` on your notes.
4. **Proposal (20.4).** Within the date you promised; `proposal` clean; if accepted, a SOW with `sow` clean, and a change log from day one.
5. **Debrief.** Whatever the answer, write down the objection you did not expect and add it to your hard-questions bank ([17.3](../../lessons/module-17/lesson-03.md)).
