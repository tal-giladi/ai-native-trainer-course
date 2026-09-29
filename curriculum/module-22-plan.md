# Module 22 plan — Productization and Scaling

4 lessons · ~62 min instruction · practice 4 h · depends on Module 20 (offer ladder, cost floor, pricing worksheet, SOW, background IP, admin register), Module 17 (workshop kit and starter pack), Module 14 (method versioning, attribution and IP orientation), Module 16 (pre/post assessment, normalized gain), Module 15 (stranger test, deny list), Module 18 (questions log, free pilot), Module 21 (engagement playbook and case study; written in parallel, linked by manifest path).

Thesis: private work caps revenue at the calendar, and that cap is a feature. Productizing is not "passive income"; it is turning what repeats into standard assets (a service card, a kit, an assessment), selling the teachable part without your hours (course, team licence, community), licensing what you publish so it travels correctly, and setting a capacity cap you actually keep, with retainers that are capped and expire. Every claim still comes from your own numbers: hours from your time log, gains from your pre/post, forecasts as ranges.

Running example: the same illustrative student (Ground-Bound-Build-Prove v1.2.0, workshop *Ground before you generate*), one year on: three paid pilots (Fabrikam Freight, Wide World Importers, Adventure Works — fictional), a workshop, and a questions log. Capacity 8 days a month beside a full-time job.

Shared lab material: `labs/module-22/` — `tools/ScaleCheck` (dependency-free C#: `service`, `catalog`, `kit`, `items`, `plan`), the reference `solution/` (service card, catalog, practitioner kit, productization plan), four breaks.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 22.1 From hours to repeatable service | Distinguish consulting, productized service and product; turn a delivered engagement into a service card with fixed price, scope, standard assets per step and a budgeted custom step; effective day rate against the floor; Wright's learning curve and delivery variability from a time log | 20.1, 20.2, 20.4 | Service card from your own time log; `service` | "Tailored to you": from-price, bespoke, 69% custom, hours rising with repetition | `service-card.md` |
| 22.2 Workshop to course to community | What each format can and cannot carry (live gain vs self-paced completion, MOOC completion data); revenue per live day; licences per artifact (MIT for code, CC BY for docs, own terms for paid material, team-licence terms); TASL third-party register; community rules — orientation only | 22.1, 14.4, 16.5, 18.4 | Catalog with licence table; `catalog` | Course cut from a client workshop recording, "certified", 24/7 access, CC BY-NC code, "free to use", unlimited perpetual company licence | `catalog.md` |
| 22.3 Templates, kits and assessments as products | Kit as a versioned product: SemVer, Keep a Changelog, LICENSE files, support policy, verification dates, stranger tests with Wilson intervals, deny-list scan; assessments: item difficulty and the 27% discrimination index; what a certificate may claim (Standards 2014) | 22.2, 15.3, 16.5, 14.3, 07.4 | Kit manifest + changelog + item analysis; `kit`, `items` | Stale kit: no versions, no licence, employer and client names, "certified" exam, miskeyed item | `practitioner-kit/` |
| 22.4 Capacity-limited consulting and the productization plan | Capacity cap and allocation with a buffer; retainers with day caps, expiry, notice and scope; queueing: Kingman's approximation, why utilization and variability drive waiting; revenue mix as ranges; 12-month roadmap with exit signals and stop rules | 22.1–22.3, 20.5 | Productization plan; `plan` | Retainers that roll over forever, on-call, 9.5 of 8 days, utilization 120%, 6-month roadmap, no stop rules | `productization-plan.md` (the module artifact) |

Math (§10 lists none for M22; used where it pays): 22.1 — effective day rate $P/(h/8)$, Wright's curve $T_n = T_1 n^{-b}$, learning rate $2^{-b}$, CV; 22.2 — revenue per live day; 22.3 — Wilson interval (reused from 07.4), item difficulty $p$ and discrimination $D = p_U - p_L$ (Kelley's 27%); 22.4 — Kingman $W_q \approx \frac{\rho}{1-\rho}\cdot\frac{c_a^2+c_s^2}{2}\cdot\tau$.

Simulation: none for this module (§11).

Templates created: `templates/service-card.md`, `templates/productization-plan.md` (includes catalog, licence table and kit manifest formats).

Links to Module 21 (written in parallel) use manifest paths `lessons/module-21/lesson-0N.md`; they show as broken in `check.py` until Module 21 lands.
