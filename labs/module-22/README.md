# Module 22 labs — Productization and scaling

Everything the Module 22 labs need. Lessons: [22.1](../../lessons/module-22/lesson-01.md) · [22.2](../../lessons/module-22/lesson-02.md) · [22.3](../../lessons/module-22/lesson-03.md) · [22.4](../../lessons/module-22/lesson-04.md).

The labs turn what you sell and deliver ([Module 20](../module-20/README.md) ladder and pricing, Module 21 engagements) and what you teach ([Module 17](../module-17/README.md) workshop kit, [Module 16](../module-16/README.md) assessment) into a practice that is not only hours: a **service card** for your most-sold rung, a **product catalog** with licences, a versioned **practitioner kit** with an item-analysed assessment, and the module artifact, a **productization plan** with a capacity cap, capped retainers, revenue ranges and a 12-month roadmap.

> [!CAUTION]
> Licensing, contract, tax and worker-status content in this module is **orientation only and jurisdiction-dependent** ([20.5](../../lessons/module-20/lesson-05.md)). Nothing you publish or sell may contain employer or client material: no client workshop recordings, slides, code, data or names without written consent. Scan every kit with a **private** deny list kept outside the kit. Keep your service card, plan and prices private until you choose to publish them.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400.
- No NuGet packages. `ScaleCheck` is read-only and deterministic; it works offline.
- Your time log from at least one delivery (Module 21), your offer ladder and pricing worksheet (Module 20), your workshop kit (Module 17), your post-test answers (Module 16), and your deny list (Module 15).

## Contents

| Path | What it is |
|---|---|
| `tools/ScaleCheck/` | Dependency-free C# checker: `service` (one price, one scope, a standard asset per standard step, custom share, effective day rate against the floor, Wright's learning curve and CV from past deliveries), `catalog` (licence per artifact, software vs document licences, team-licence terms, TASL register, community rules, revenue per live day, capacity at high demand, unbounded promises, certification claims, client recordings), `kit` (semantic versions, licences, LICENSE files, verification age, stranger tests with Wilson intervals, changelog, support line, deny-list scan), `items` (item difficulty and 27% discrimination), `plan` (allocation within the cap, buffer, capped and expiring retainers, Kingman wait at low and high demand, month-12 revenue ranges, 12-month roadmap with exit signals, stop rules). |
| `solution/` | **Illustrative** reference for the same student as Modules 17–20, one year on: [`service-pilot.md`](solution/service-pilot.md), [`catalog.md`](solution/catalog.md), [`practitioner-kit/`](solution/practitioner-kit/kit.md) (templates, starter pack manifest, assessment with `responses.csv`, `CHANGELOG.md`, LICENSE files), [`productization-plan.md`](solution/productization-plan.md), and `deny-terms.example.txt`. Clients (Fabrikam Freight, Wide World Importers, Adventure Works) are fictional; prices are placeholders (USD, as of 2026-09). |
| `break/` | One deliberately flawed artifact per lesson: a "tailored to you" service card (22.1), a course cut from a client recording with "certification" and CC-licensed code (22.2), a stale kit with employer names and a miskeyed item (22.3), a plan with rollover-forever retainers and 120% utilization (22.4). |

## Quick start

From this folder:

```bash
S="dotnet run --project tools/ScaleCheck --"

# 22.1 — from hours to repeatable service
$S service break/22.1-bespoke-service/service.md               # 13 errors, 5 warnings
$S service solution/service-pilot.md                           # clean; learning rate 82%, next delivery ~90 h

# 22.2 — workshop to course to community
$S catalog break/22.2-course-dump/catalog.md                   # 18 errors, 10 warnings
$S catalog solution/catalog.md                                 # clean; 40% to 100% of revenue scales

# 22.3 — templates, kits and assessments as products
$S kit break/22.3-stale-kit solution/deny-terms.example.txt    # 17 errors, 6 warnings (5 deny-list hits)
$S kit solution/practitioner-kit solution/deny-terms.example.txt   # 1 warning (Bound plan stranger test 5/6)
$S items break/22.3-stale-kit/assessment/responses.csv         # 1 error (q4, D = -0.50), 2 warnings
$S items solution/practitioner-kit/assessment/responses.csv    # clean

# 22.4 — capacity-limited consulting and the productization plan
$S plan break/22.4-unlimited-retainers/productization-plan.md  # 16 errors, 13 warnings
$S plan solution/productization-plan.md                        # clean; wait 1.1 to 3.7 months
```

(PowerShell: type the full `dotnet run --project tools/ScaleCheck -- <command>` instead of `$S`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem. `ScaleCheck` checks structure, arithmetic and wording; it cannot tell whether a licence is right for your jurisdiction, a price right for your market, or a forecast likely.

## Your own files

```text
workshop-kit/                    # private, from Modules 17 and 20
├── service-card.md              # 22.1 — templates/service-card.md
├── catalog.md                   # 22.2 — templates/productization-plan.md, part 1
└── productization-plan.md       # 22.4 — templates/productization-plan.md, part 3 (the module artifact)
practitioner-kit/                # 22.3 — sold or published; part 2 of the template
├── kit.md
├── CHANGELOG.md
├── LICENSE-code  LICENSE-docs
├── templates/  starter-pack/  assessment/
~/private/deny-terms.txt         # never inside the kit
```

Formats: [service card](../../templates/service-card.md), [productization plan, catalog and kit manifest](../../templates/productization-plan.md), [stranger test](../../templates/stranger-test.md), [pre/post assessment](../../templates/pre-post-assessment.md).

## Practice (about 4 hours)

1. **Service card (22.1).** From your time log; `service` clean.
2. **Catalog and licences (22.2).** Three to five products, one licence row per artifact, community rules if any; `catalog` clean.
3. **Kit (22.3).** Manifest, changelog, LICENSE files, stranger tests booked or done, item analysis on your post-test answers; `kit` with your private deny list and `items` clean.
4. **Plan (22.4).** Cap, allocation, retainers, queue, month-12 ranges, 12-month roadmap, stop rules; `plan` clean. Read the "What I will not do" section to someone who will hold you to it at month 6.
