# Module 21 plan — Delivering Engagements

5 lessons · ~80 min instruction · practice: a full dry-run engagement (about 8 h) and then a real one · depends on Module 20 (SOW, acceptance criteria, measurement instead of guarantees; written in parallel, linked by manifest path), Module 19 (adoption plan, handover criteria, p-chart usage), Module 13 (baseline, power, comparison, claims ladder), Module 7 (eval baseline, gate), Module 3 (audit, evidence rungs), Module 15 (`brownfield-demo`), Module 17 (the workshop), Module 14 (attribution, permission).

Thesis: an engagement is a sequence of phases with exit gates — Discovery → Audit → Baseline → Architecture → Build → Enable → Measure → Handover → Follow-up — and each gate exists because of a specific way engagements fail: the stakeholder nobody interviewed, the baseline taken after the change, the layer the consultant wrote alone, the handover to yourself, the case study that claims more than the record. The consultant's job is to leave a team that owns the change and a record that tells the truth about it.

Running example: a dry run with a simulated client, Contoso Ltd, whose code is the student's `brownfield-demo` at `before-ai-layer`: 19 developers in Billing, Collections, Platform; a 12-week SOW (`labs/module-21/client/sow-contoso.md`, `OfferCheck sow` clean); nine stakeholders with interview notes. Illustrative data: baseline 60 tickets (median 18.5 h, sd(log) within size ≈ 0.44, defects 6/60); pilot 27 randomized tickets (−8%, 95% CI −24% to +12%; pooled medians −23% as the trap); eval 53% → 75% (+21.7 pts, CI +10.6 to +32.8); usage W09–W12 79%, 89% at 90 days, platform dip after the champion moved.

Shared lab: `labs/module-21/` — `tools/EngageCheck` (`kickoff`, `baseline`, `build`, `handover`, `casestudy`), `client/` (brief, SOW, data), `solution/` (record, PR log, case study, deny list), `break/` (one per lesson), `scripts/generate_data.py`.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 21.1 Discovery, kickoff and stakeholder mapping | Stakeholder map (power/interest, Mendelow) with the five required groups incl. the business user of the output; past-event interviews before kickoff; acceptor per deliverable; dated access; premortem (Klein); success measure restated as a measurement | 20.4, 19.1, 01.4 | Role-played interviews; record section 1; `kickoff` | Kickoff after one call: 3 stakeholders, "30% faster, guaranteed", acceptors "S"/"client", access TBD | record §1 |
| 21.2 Technical audit and baseline | Audit with evidence rungs on the client's before state; baseline before build start: definitions, 12 weeks, spread, guardrail, eval baseline, no per-person metrics; minimum detectable effect from sd and n | 21.1, 03.3, 13.1, 13.3, 07.2 | `scan` on `before-ai-layer`; `describe`, `power`; record §2; `baseline` | Docs-only audit; baseline taken in weeks 3–4 on 9 PRs per developer | record §2, D1, D2 |
| 21.3 Architecture and build with the team | Recommend vs decide (ADRs by client deciders); pairing and the fading scaffold; client-authored share by thirds; client truck factor (Avelino); review as knowledge transfer (Bacchelli & Bird); enabling-team stance | 21.2, 03.1, 06.1, 07.6, 12.5 | Rebuild the layer with role-played developers; `prs.csv`; `build` | brownfield-demo's own layer.tsv: 7 commits by one author; truck factor 0 | record §3, PR log |
| 21.4 Enable, measure, hand over, follow up | Enablement with owners (19.3); results with intervals and honest statuses; "what this does not show"; owners who ran it without you; PRR-style handover; follow-ups at 30/60/90 days read with p-charts | 21.3, 19.4, 13.6, 17.1 | `compare` (tickets, evals), `usage`; record §4; `handover` | Calendar handover to the consultant; −23% pooled median "improved"; usage decays from W15 | record §4, D5, D6 |
| 21.5 Writing the anonymized case study | Motivated-intruder anonymization (ICO), bands not values; every number traced to the record; typical-results rule (FTC 16 CFR 255.2); limitations as a heading; written approval | 21.4, 14.4, 18.2, 20.5 | Case study from the record; `casestudy` | Evening-of case study: names, e-mail, 23%, "up to 40%", no approval | case study |

Math (light; M21 is not in §10): 21.2 minimum detectable effect $\delta = (z_{1-\alpha/2}+z_{1-\beta})\,\sigma\sqrt{2/n}$, reduction $1-e^{-\delta}$ (reuses 13.3 `power`); 21.3 client-authored share and truck factor (greedy, 50% of files); 21.4 reading intervals and statuses, Wilson for usage (07.4), p-chart limits (19.4); 21.5 identifiability as the size of the matching set ("k").

Simulation: none (§11).

Templates created: `templates/engagement-playbook.md` (playbook + record format read by `EngageCheck`), `templates/case-study.md`. Reused: `sow.md`, `discovery-call-script.md` (M20), `brownfield-audit-checklist.md` (M3), `experiment-design.md`, `experiment-report.md`, `metrics.md` (M13), `adoption-plan.md` (M19), `adr.md` (M12), `workshop-template.md` (M17), `pre-post-assessment.md` (M16).

Field (outline): the dry run, then a real paid or pilot engagement.

Links to Module 20 (written in parallel) use manifest paths `lessons/module-20/lesson-0N.md`; lessons 04 and 05 may show as broken in `check.py` until Module 20 lands.
