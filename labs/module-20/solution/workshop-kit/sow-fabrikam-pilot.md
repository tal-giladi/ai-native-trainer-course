# Statement of work — Measured pilot, Fabrikam Freight

> **Illustrative.** Fabrikam Freight is fictional, and so is every term here. This is a teaching example of structure, not a contract to copy: the legal clauses (IP, liability, confidentiality, termination) must come from, or be checked by, a lawyer in your and the client's jurisdictions. Format: [SOW template](../../../../templates/sow.md). Check with `OfferCheck sow`.

- Version: 1.0 (2026-10-05)
- Parties: the student ("Supplier"), Fabrikam Freight ("Client")
- Governed by: the master services agreement dated 2026-10-05 (or, if none, the terms in this SOW)
- Client sponsor: VP Engineering; day-to-day contact: engineering manager
- Proposal: proposal-fabrikam.md, option A

## 1. Objectives

Give one Client .NET Framework team an AI layer that grounds its coding agent in the rules the code and database already have, and measure on that team's own tickets, with a pre-registered randomized comparison, whether working this way changes cycle time, escaped defects, DBA reverts and PR review time.

## 2. Scope

- One team (up to eight developers) on one repository: the billing service and its SQL Server database project.
- A 2-hour workshop for the pilot team.
- Building, with the team, a rules file, Ground, Bound and Prove skills, and one pre-merge check.
- Pre-registration, randomized assignment of tickets, weekly data-quality checks, analysis, report and read-out.

## 3. Deliverables

| Deliverable | Acceptance criterion | Due |
|---|---|---|
| D1 Pre-registration | Document lists outcomes, arms, assignment method, sample-size target, go/no-go rule and stop rule; signed off by the sponsor before the first ticket is assigned | Week 1 |
| D2 AI layer in the repository | Merged via PR reviewed by a Client developer; the pre-merge check runs in the Client's CI and fails on the two seeded test cases in the PR description | Week 2 |
| D3 Workshop | Delivered to the pilot team; pre/post assessment results and normalized gain reported in writing within 5 working days | Week 2 |
| D4 Weekly data-quality notes | One note per week listing missing timestamps, balance between arms and deviations from D1 | Weeks 3–6 |
| D5 Report and read-out | Report states every D1 outcome with an interval, the ticket flow per arm, deviations, what the result does not show, and the recommendation under D1's rule; read-out held with the sponsor | Week 6 plus 30-day defect window, by 2026-12-11 |

## 4. Out of scope

- Other teams, repositories or the .NET 8 services.
- Any access to production systems or production data; the Supplier has no production access of any kind.
- Writing product features or fixing Client defects.
- Selecting, buying or administering agent licences or model-provider contracts.
- Security review of the agent tool, and legal advice on its terms.
- Support after the read-out (available separately as a retainer).

## 5. Assumptions

- The team works on at least 30 tickets in the six weeks, about half of them eligible for the comparison.
- Jira status history and GitHub PR data can be exported by the Client in anonymized form.
- The agent tool the team already licenses is used; no new tools are introduced without a change request.

## 6. Client responsibilities

- Read-only repository access under NDA on a Client laptop by 2026-10-12.
- Two hours per developer per week for the pilot, and a named Client reviewer for D2.
- Weekly anonymized Jira and GitHub exports; escaped-defect tickets tagged by the Client's usual process.
- Decisions on D1 sign-off within 3 working days. Delays move the schedule day for day.

## 7. Schedule and milestones

- Kickoff 2026-10-12; D1 and D2 in week 1–2; comparison weeks 3–6 (to 2026-11-20); D5 after the 30-day defect window, by 2026-12-11.

## 8. Measurement

Outcomes, arms and the go/no-go rule are fixed in D1 before the first ticket. The baseline is the team's previous 12 weeks from the Jira export. Results are reported as estimates with 95% intervals, including when they show no effect or a harmful one. The Supplier does not promise any value for any outcome; a null or negative result is a valid, delivered result. If escaped defects in the treatment arm exceed the D1 stop threshold, the comparison stops and D5 reports what was found.

## 9. Fees and payment

Fixed fee: USD 18,000, excluding taxes. Payment terms: net 30 from invoice date.

| Milestone | Amount | Invoice on |
|---|---|---|
| Signature of this SOW | 9,000 | signature |
| Delivery of D5 | 9,000 | read-out |

Total: 18,000

## 10. Change control

Any change to scope, deliverables or schedule is made by a written change request (email is enough) that states the change, its effect on fee and schedule, and is approved by the sponsor before work on it starts. The Supplier logs every request, including ones declined or done at no charge.

## 11. Acceptance

The Client has 5 working days after each deliverable to accept it against its criterion in section 3 or to list, in writing, which criterion is not met. Silence after 5 working days counts as acceptance.

## 12. Intellectual property

The Supplier keeps its pre-existing material: the method, templates, tools, workshop materials and the starter pack. On full payment, the Client owns the AI-layer files committed to its repository and the D5 report, and receives a perpetual, non-exclusive licence to use the pre-existing material embedded in them internally. The Supplier may describe the engagement in anonymized form only with the Client's written approval of the text.

## 13. Confidentiality and data

The Supplier works only on the Client laptop, stores no Client code or tickets elsewhere, and receives exports with developer names replaced by codes. Measurement data is about tickets, never about ranking individuals. On completion, the Supplier deletes any exports within 30 days and confirms in writing.

## 14. Term and termination

This SOW runs from signature to acceptance of D5. Either party may terminate with 10 working days' written notice; the Client pays for deliverables accepted and a pro-rata share of work in progress.
