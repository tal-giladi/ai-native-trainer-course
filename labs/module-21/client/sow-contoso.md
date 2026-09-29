# Statement of work — AI layer with a measured pilot, Contoso Ltd (dry run)

> **Simulated client.** Contoso Ltd, its people and every term here are fictional; this SOW exists so you can run a complete engagement against `brownfield-demo` before you run one for money. It follows the [SOW template](../../../templates/sow.md) and the structure of the Module 20 reference SOW ([20.4](../../../lessons/module-20/lesson-04.md)). The legal clauses are placeholders, not advice. Check with `OfferCheck sow` ([Module 20 labs](../../module-20/README.md)).

- Version: 1.0 (2026-09-28)
- Parties: the student ("Supplier"), Contoso Ltd ("Client")
- Governed by: the terms in this SOW
- Client sponsor: Head of Engineering; day-to-day contact: Billing tech lead
- Proposal: dry run, no proposal (the offer ladder's implementation rung)

## 1. Objectives

Give Contoso's Billing team an AI layer grounded in the conventions its code and database already have, built with the team so that they own it; measure on Billing's own tickets, with a pre-registered randomized comparison, whether working this way changes cycle time and escaped defects; then enable the Collections and Platform teams and hand every recurring activity to a named Contoso owner.

## 2. Scope

- The Billing repository (`brownfield-demo` at tag `before-ai-layer`) and its SQL Server migrations.
- Three teams, 19 developers in total: Billing (6), Collections (7), Platform (6).
- Audit and baseline; the AI layer built in pairs with Billing developers; one eval regression gate in CI.
- A 2-hour workshop for Collections and Platform, and office hours until the handover.
- Pre-registration, randomized assignment of eligible Billing tickets in weeks 3–6, analysis, report and read-out.
- Follow-up check-ins 30, 60 and 90 days after the handover.

## 3. Deliverables

| Deliverable | Acceptance criterion | Due |
|---|---|---|
| D1 Audit and baseline | Report lists each finding with at least two evidence sources, and each baseline metric with its definition, source, 12-week window, spread and capture date before week 3; accepted by the Billing tech lead | Week 2 |
| D2 Pre-registration | Document lists outcomes, arms, assignment method, detectable effect and stop rule; signed off by the sponsor before the first ticket is assigned | Week 2 |
| D3 AI layer v1 in the Billing repository | Merged via PRs reviewed by Billing developers, at least 70% of AI-layer PRs in weeks 5–6 authored by Billing developers; the eval gate runs in CI and fails on the two seeded regressions described in the PR | Week 6 |
| D4 Enablement | Workshop delivered to Collections and Platform with pre/post results in writing within 5 working days; office hours run twice by a named Client owner without the Supplier | Week 10 |
| D5 Results report and read-out | Report states every D2 outcome with an interval, the ticket flow per arm, deviations and what the result does not show; read-out held with the sponsor | Week 11 |
| D6 Handover pack | Every recurring activity has a Client owner and backup who has run it once without the Supplier; follow-up dates booked | Week 12 |

## 4. Out of scope

- Other repositories, the Collections and Platform codebases, and any production system or production data; the Supplier has no production access of any kind.
- Writing product features or fixing Client defects outside the pilot's own tickets.
- Selecting, buying or administering agent licences or model-provider contracts.
- Legal review of the agent tool's terms, and the Client's data-protection assessment.
- Support after the 90-day follow-up (available separately as a retainer).

## 5. Assumptions

- Billing works on at least 24 tickets in weeks 3–6, most of them eligible for the comparison.
- Jira status history and GitHub PR data can be exported by the Client in anonymized form.
- The agent tool Contoso already licenses is used; no new tools are introduced without a change request.

## 6. Client responsibilities

- Read-only repository access under NDA on a Contoso laptop by 2026-10-05, after the IT and security lead's review.
- Four hours per Billing developer per week in weeks 3–6 for pairing, and a named reviewer for every AI-layer PR.
- Weekly anonymized Jira and GitHub exports; escaped-defect tickets tagged by the Client's usual process.
- Decisions on D2 sign-off within 3 working days. Delays move the schedule day for day.

## 7. Schedule and milestones

- Kickoff 2026-10-05; D1 and D2 in weeks 1–2; comparison weeks 3–6 (to 2026-11-13); enablement weeks 6–10; D5 in week 11 after the 30-day defect window; handover 2026-12-23; follow-ups by 2027-03-23.

## 8. Measurement

Outcomes, arms and the go/no-go rule are fixed in D2 before the first ticket. The baseline is Billing's previous 12 weeks from the Jira export. Results are reported as estimates with 95% intervals, including when they show no effect or a harmful one. The Supplier does not promise any value for any outcome; a null or negative result is a valid, delivered result. If escaped defects in the treatment arm exceed the D2 stop threshold, the comparison stops and D5 reports what was found.

## 9. Fees and payment

Fixed fee: USD 42,000, excluding taxes (a placeholder of plausible size, as of 2026-09; not market advice). Payment terms: net 30 from invoice date.

| Milestone | Amount | Invoice on |
|---|---|---|
| Signature of this SOW | 14,000 | signature |
| Acceptance of D3 | 14,000 | D3 acceptance |
| Delivery of D5 | 14,000 | read-out |

Total: 42,000

## 10. Change control

Any change to scope, deliverables or schedule is made by a written change request (email is enough) that states the change, its effect on fee and schedule, and is approved by the sponsor before work on it starts. The Supplier logs every request, including ones declined or done at no charge.

## 11. Acceptance

The Client has 5 working days after each deliverable to accept it against its criterion in section 3 or to list, in writing, which criterion is not met. Silence after 5 working days counts as acceptance.

## 12. Intellectual property

The Supplier keeps its pre-existing material: the method, templates, tools and workshop materials. On full payment, the Client owns the AI-layer files committed to its repository and the D1 and D5 reports, and receives a perpetual, non-exclusive licence to use the pre-existing material embedded in them internally. The Supplier may describe the engagement in anonymized form only with the Client's written approval of the text.

## 13. Confidentiality and data

The Supplier works only on the Contoso laptop, stores no Client code or tickets elsewhere, and receives exports with developer names replaced by codes. Measurement data is about tickets and teams, never about ranking individuals. On completion, the Supplier deletes any exports within 30 days and confirms in writing.

## 14. Term and termination

This SOW runs from signature to the 90-day follow-up. Either party may terminate with 10 working days' written notice; the Client pays for deliverables accepted and a pro-rata share of work in progress.
