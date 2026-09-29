---
id: "20.5"
module: 20
minutes: 16
practice_minutes: 75
prerequisites: ["20.4", "14.4"]
objectives:
  - Identify, as orientation, the employment-contract clauses (IP assignment, outside work, non-compete, confidentiality) that must be read and cleared in writing before any paid work outside your job.
  - Explain why contractor status, IP ownership, liability and payment terms depend on jurisdiction and on the contract's wording, and name the questions to put to a lawyer and an accountant.
  - Keep an admin register with employer-conflict, contract, tax and invoicing items, and check it with OfferCheck admin before the first pitch.
  - Respond to the common difficult-client patterns (scope pushing, late payment, moving goalposts, guarantee demands, unethical requests) with the documents built in this module.
volatility: implementation
sources:
  - title: "California Labor Code § 2870 — Employee inventions: exclusions from assignment (example of one jurisdiction's rule)"
    url: https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=LAB&sectionNum=2870
  - title: "U.S. Copyright Office — Circular 30: Works Made for Hire"
    url: https://www.copyright.gov/circs/circ30.pdf
  - title: "IRS — Independent contractor (self-employed) or employee?"
    url: https://www.irs.gov/businesses/small-businesses-self-employed/independent-contractor-self-employed-or-employee
  - title: "GOV.UK — Understanding off-payroll working (IR35)"
    url: https://www.gov.uk/guidance/understanding-off-payroll-working-ir35
  - title: "Directive 2011/7/EU on combating late payment in commercial transactions (EUR-Lex)"
    url: https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=CELEX:32011L0007
last_verified: "2026-09-28"
---

# 20.5 · Legal, admin and difficult clients

> [!CAUTION]
> **Orientation only, and jurisdiction-dependent.** This lesson names the questions, not the answers. Employment, IP, contract, tax and worker-status rules differ by country, state and contract wording, and change. The laws cited are examples from the US, the UK and the EU, chosen because they are public and clearly written; they may not apply to you. Before your first paid engagement, have an employment lawyer read your employment contract and a lawyer or accountant where you work check your contract template and tax set-up.

## Why it matters

The fastest way to end this career before it starts is not a bad workshop. It is selling to your employer's customer without permission, building your method on company time and finding the IP assignment clause covers it, starting work before a contract exists and then arguing about what was agreed, or invoicing on terms that leave you financing a large client for four months. None of these is exotic. The source material for this course lists "employer code and metrics" as a guardrail, and puts the employer first in the order of who to sell to, which makes the employer relationship the first thing to get right.

The second half is people. Most clients are reasonable; a few push scope, pay late, move goalposts after the fact, want a guarantee in week five, or ask you to do something you should not (rank developers by the telemetry, say). Every document you built in this module exists partly for those moments. This lesson connects them.

> [!NOTE]
> Content tags. **Concept** (stable): the questions to ask about employer conflicts, IP, status, liability, payment and difficult clients. **Implementation**: the specific statutes and guidance cited (as of 2026-09) and `OfferCheck admin`.

## How it works

### Your employer first

Four clauses in a typical employment contract decide what you may do outside it:

| Clause | Ask | Why |
|---|---|---|
| IP / invention assignment | Does it cover work "relating to the employer's business", or everything you create while employed? | A broad clause can make your method, templates and tools the employer's. Some jurisdictions limit such clauses; California, for example, excludes inventions made entirely on your own time without the employer's equipment or trade secrets, unless they relate to the employer's business or result from work for the employer (Labor Code § 2870). Others do not. |
| Outside work / moonlighting | Is paid outside work allowed, with or without approval? | Many contracts require written approval. A hallway "sounds fine" is not approval. |
| Non-compete and non-solicitation | Does it restrict working for customers, competitors or suppliers, during or after employment? | Enforceability varies enormously by jurisdiction; the reputational risk does not. |
| Confidentiality | What counts as confidential? | Metrics, incidents, customer names and code from your job are almost always confidential (14.4). |

Then four practical rules: get **written permission** that names the kind of work and the exclusions; use **no employer time, equipment, accounts, code or data**; sell to **no customer, competitor or supplier** of your employer unless the permission covers it; and if your first sale is internal, clarify whether the work is **part of your job** (then there is no invoice, and the output is the employer's).

### Contract basics

- **Sign before you start.** Work done before a contract is work whose scope, price and ownership are undefined. The reference register makes it a blocker.
- **IP.** In US copyright law, a contractor's work is generally not a "work made for hire" unless it falls in one of nine categories (instructional texts and tests among them) *and* a written agreement says so (Copyright Office Circular 30); otherwise ownership moves only by assignment. Workshop materials can fall into those categories, which is exactly why the contract should say, in plain words, that your pre-existing method, templates and tools stay yours and the client receives a licence to what is embedded in their deliverables. Other jurisdictions use different doctrines; the clause is what you control.
- **Liability** capped, typically at the fees paid under the SOW, and **professional indemnity insurance** considered.
- **Confidentiality and data**: what you may keep, where, for how long, and deletion on completion (the Fabrikam SOW's section 13).

### Status, tax and invoicing

Whether you are legally an independent contractor is not decided by the word "contractor" in the contract. The IRS looks at behavioural control, financial control and the type of relationship, with no single decisive factor. In the UK, the off-payroll rules (IR35) put the status decision on medium and large clients, and on your own company for small clients. Fixed-scope deliverables, your own tools and several clients point one way; fixed hours under someone's direction for one client point the other.

Tax: register as the law where you live requires, set aside income tax, and find out whether sales tax or VAT applies to cross-border services. Invoices: most tax regimes require specific fields; many require sequential numbering. Payment terms: in the EU, the late-payment directive limits business-to-business payment periods to 60 days unless expressly agreed and not grossly unfair, and provides a fixed minimum compensation of 40 euros per late invoice. Ask for a deposit on signature; it tests that procurement works before you have spent six weeks.

### Difficult clients

| Pattern | Early sign | Your document |
|---|---|---|
| Scope pusher | "While you're here, could you also…" | change log and change control (20.4) |
| Late payer | invoice "lost", then "in the next payment run" | payment terms, a reminder schedule, a pause-work clause, the deposit |
| Moving goalposts | "We expected it to do X" after delivery | acceptance criteria written before work |
| Guarantee seeker | "Can you commit to 30% now that you're in?" | measurement section, "not promised" list (20.2, 20.4) |
| Disappearing sponsor | the VP who signed leaves | sponsor named in the SOW; Module 19's ownership and succession ([19.4](../module-19/lesson-04.md)) |
| Unethical request | "Use the telemetry to rank the developers" | the measurement rules from Module 13 (tickets, not people); decline in writing |

The response has the same shape each time: acknowledge, point to the document, offer the proper route (a change request, a new SOW, a conversation with the sponsor), and write it down the same day. If the relationship still does not work, the termination clause is there to be used, with a clean handover.

## Show me

The reference register, [`admin.md`](../../labs/module-20/solution/workshop-kit/admin.md), shows one student clearing the blockers before signing Fabrikam:

```bash
cd labs/module-20
dotnet run --project tools/OfferCheck -- admin solution/workshop-kit/admin.md
```

```text
admin.md: engagement admin register
  (orientation only: the tool checks that each question was asked and answered in writing, not that the answer is right)
  jurisdiction: supplier in the student's home country; client in another; both named in the contract's governing-law clause

  E1 yes  employment contract: IP assignment clause read
  E2 yes  employment contract: outside-work, non-compete and confidentiality clauses read
  E3 yes  employer's written permission (or written confirmation none is needed)
  E4 yes  no employer time, equipment, accounts, code or data used
  E5 yes  client is not the employer's customer, competitor or supplier
  C1 yes  signed contract and SOW before any work starts
  ...
  invoices: 3, payment terms net 30

0 error(s), 0 warning(s)
```

Every "yes" points to evidence: a lawyer's note on clause 9, the head of engineering's email of 2026-09-10 approving outside training work "not for the employer's customers, competitors or suppliers", a check of Fabrikam against the customer list, the signed SOW. Application A03 (20.3) was referred elsewhere because of E5.

## Try it

Budget: about 75 minutes, plus waiting for answers.

1. **Read your employment contract (30 min).** Find the IP, outside-work, non-compete and confidentiality clauses. Copy the exact wording into your private notes.
2. **Ask (15 min).** Write to your manager or HR asking for written approval of the specific outside work, naming exclusions. If your first pitch is internal, ask instead whether the work is part of your role.
3. **Register (20 min).** Create `workshop-kit/admin.md` from the reference structure. Fill every row you can; leave the rest "no" with a note of who you will ask.
4. **Professional review (10 min to book).** Book a short consultation with an employment lawyer and an accountant where you live. Put the date in `Reviewed by:` when it happens.
5. **Check.** Run `admin`. Do not pitch anyone outside your employer until E1–E5 are clear.

<details>
<summary>Hint: my contract says everything I create while employed belongs to the employer</summary>

Do not guess whether that clause is enforceable where you live. Ask a lawyer, and ask your employer for a written carve-out for training material and methods unrelated to the employer's products, created on your own time and equipment. Many employers agree when asked early and plainly. Until then, your internal pitch (to your employer) is the safe first sale.
</details>

## Break it

[`break/20.5-moonlighting/admin.md`](../../labs/module-20/break/20.5-moonlighting/admin.md): a student three weeks into a side engagement. The contract "is coming", the manager "said sounds fine in the hallway", the demo ran on the work laptop with the team's real PR statistics, the client is "one of our biggest customers", the client's paper assigns all IP to the client, and two invoices numbered 1 and 3 are due 120 days after issue on a net-30 contract.

```bash
dotnet run --project tools/OfferCheck -- admin break/20.5-moonlighting/admin.md
```

Before running it: which single row would you fix first, and why?

## Fix it

**Diagnose.** Ten errors and thirteen warnings. Seven blockers: no contract clauses read, no written permission, employer equipment and confidential metrics used, a client who is the employer's customer, work started without a signed contract, and all IP, including the student's own method, assigned to the client. No jurisdiction, and "Reviewed by: me". Invoices without milestones or tax lines, numbered out of sequence, due four times later than the contract allows. Six items (liability, payment terms in the SOW, tax, worker status, insurance, invoice fields) never considered.

The first fix is E5 with E3: stop work for the employer's customer and tell your manager in writing, today. Everything else is repairable; that one threatens your job and your credibility with both organizations.

**Modify.** Stop, disclose, and get written guidance from the employer. If the employer approves the engagement, get a proper contract signed before any further work, with background IP retained and liability capped; reissue the invoices with sequential numbers, milestones, tax lines and the contract's terms; delete the employer's metrics from everything the client has; rebuild the demo on `brownfield-demo`. If the employer does not approve, end the engagement under whatever terms exist and refer the client elsewhere.

**Rerun.** `admin` clean on the repaired register, and a date in `Reviewed by:` that is not "me".

## How do I know it works?

- [ ] You have read, and copied into private notes, your contract's IP, outside-work, non-compete and confidentiality clauses.
- [ ] You have written permission for the kind of work you will sell, or a written statement that none is needed, or your first sale is internal and its status is clear.
- [ ] `OfferCheck admin` shows no blockers, and every "yes" has evidence.
- [ ] A lawyer and an accountant in your jurisdiction have looked at your set-up, or a date is booked.
- [ ] For each difficult-client pattern in the table, you can point to the document in your `workshop-kit` that answers it.

## Use / don't use

**Use** the register before every new client, not only the first; E5 changes whenever your employer signs a new customer. **Use** the documents in a difficult conversation; "here is what we both signed" lowers the temperature more than any phrasing.

**Don't** rely on this lesson, a template or a tool for legal or tax decisions. **Don't** start work on a promise that the contract is coming. **Don't** stay in an engagement that requires you to measure individuals, hide results or breach your employer's trust; the termination clause is part of the contract for a reason.

**Limitations.**

- The statutes cited are examples: California's § 2870 applies to California employees, Circular 30 to US copyright, the IRS test to US federal tax, IR35 to the UK, and the late-payment directive to EU member states as implemented nationally.
- The register records that questions were asked and answered in writing; it cannot judge the answers.
- Rules change. This lesson is marked for annual review (implementation volatility).

## Reflect

1. What does your employment contract actually say about work you create outside your job, and did anything in it surprise you?
2. Which difficult-client pattern are you most likely to give in to, and which document will stop you?
3. Who is the first lawyer or accountant you will ask, and what are your three questions?

## Sources

- [California Labor Code § 2870](https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=LAB&sectionNum=2870) — an assignment clause cannot cover inventions made entirely on the employee's own time without the employer's equipment, supplies, facilities or trade secrets, unless they relate to the employer's business or R&D or result from work for the employer.
- [U.S. Copyright Office — Circular 30: Works Made for Hire](https://www.copyright.gov/circs/circ30.pdf) — an employee's work within the scope of employment, or a specially commissioned work in one of nine categories with a written agreement; otherwise the contractor owns the copyright unless it is assigned.
- [IRS — Independent contractor (self-employed) or employee?](https://www.irs.gov/businesses/small-businesses-self-employed/independent-contractor-self-employed-or-employee) — behavioural control, financial control and type of relationship; no single decisive factor.
- [GOV.UK — Understanding off-payroll working (IR35)](https://www.gov.uk/guidance/understanding-off-payroll-working-ir35) — medium and large clients decide the worker's status and issue a status determination statement; for small clients the worker's intermediary decides.
- [Directive 2011/7/EU on late payment](https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=CELEX:32011L0007) — business-to-business payment periods of no more than 60 days unless expressly agreed and not grossly unfair; a fixed minimum of 40 euros compensation for recovery costs.
