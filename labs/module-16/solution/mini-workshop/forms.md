# Pre/post forms A and B — Shadow rule

> Reference answer for Module 16, **illustrative**. Two parallel forms for [session.md](session.md): same items, same objectives, different surface (form A uses *overdue* and *balance*, form B uses *disputed* and *VAT*). Half the group takes A before and B after, half the reverse, so neither the form nor memory of the pre-test can produce the gain. Item rules: [pre/post assessment template](../../../../templates/pre-post-assessment.md).

Instructions printed on both forms: *"This is not a test of you; it tells me what to teach. Answer from what you know now. 'I don't know' is a fine answer. Four minutes."*

Code on the forms comes from the lab's Contoso Billing repository (C# service layer, SQL Server stored procedures).

## Form A

**I1 (O1, remember).** Which description matches a *shadow rule*?
a) Code the agent copied from a public example without a licence
b) A second implementation of a business rule that already exists, with a slightly different meaning, living beside the first
c) A rule in the rules file that the agent ignored during the run
d) A test the agent wrote so that its own code would pass

**I2 (O1, analyze).** The agent's diff for "send reminders for late invoices" adds this to `InvoiceReminderJob.cs`. `InvoiceService.IsOverdue(Invoice, DateTime asOfUtc)` already exists and applies a 3-day grace period. Is this a shadow rule? If yes, name what it duplicates.

```csharp
private static bool IsLate(Invoice i) => i.DueUtc < DateTime.UtcNow && i.PaidUtc == null;
```

**I3 (O1, analyze).** The agent's diff changes the existing method's signature to `IsOverdue(Invoice i, DateTime asOfUtc, bool includeDisputed = true)`, passes `includeDisputed: false` from the new caller, and adds two tests; existing callers and tests are unchanged. Is this a shadow rule? Why?

**I4 (O2, apply).** Before the agent plans, which one search finds every existing implementation of "overdue"?
a) Search `*.cs` files for `IsOverdue`
b) `git grep -n -i -E "overdue|DueUtc" -- "*.cs" "*.sql"`
c) Ask the agent "is there already an overdue rule?"
d) Search the team wiki for "overdue"

**I5 (O2, apply).** The search returns the three lines below. Where does the overdue rule live?

```text
src/Billing/InvoiceService.cs:88:        public bool IsOverdue(Invoice i, DateTime asOfUtc)
db/procs/usp_GetOverdueInvoices.sql:12:  WHERE i.DueUtc < DATEADD(day, -3, @AsOfUtc) AND i.PaidUtc IS NULL
tests/Billing.Tests/InvoiceServiceTests.cs:40:  public void IsOverdue_after_grace_period()
```

a) Only in `InvoiceService.IsOverdue`
b) Only in `usp_GetOverdueInvoices`
c) In two places already: the C# method and the stored procedure; the test is not an implementation
d) In three places: the method, the procedure and the test

**I6 (O3, create).** Ticket BILL-240: "Skip reminder emails for invoices overdue by more than 90 days." Write the one line you would add to the research brief.

**I7 (O3, create).** Ticket BILL-251: "Show the customer's balance on the statement PDF." Write the one line you would add to the research brief.

**I8 (O1, analyze).** A migration in the agent's diff adds a view. `usp_GetCustomerBalance` exists and `BalanceService` calls it. Is this a shadow rule? If yes, name what it duplicates.

```sql
CREATE VIEW dbo.vw_CustomerBalance AS
SELECT CustomerId, SUM(Amount) - SUM(PaidAmount) AS Balance FROM dbo.Invoice GROUP BY CustomerId;
```

## Form B

**I1 (O1, remember).** Which description matches a *shadow rule*?
a) A test the agent wrote so that its own code would pass
b) A rule in the rules file that the agent ignored during the run
c) A second implementation of a business rule that already exists, with a slightly different meaning, living beside the first
d) Code the agent copied from a public example without a licence

**I2 (O1, analyze).** The agent's diff for "pause dunning for disputed invoices" adds this to `DunningJob.cs`. `InvoiceService.IsDisputed(Invoice)` already exists and also checks for an open row in `InvoiceDispute`. Is this a shadow rule? If yes, name what it duplicates.

```csharp
private static bool Disputed(Invoice i) => i.Status == "D";
```

**I3 (O1, analyze).** The agent's diff adds an overload `VatCalculator.Calculate(CreditNote note)` that maps the credit note to lines and calls the existing `Calculate(IEnumerable<Line>, DateTime)`, plus two tests; existing callers and tests are unchanged. Is this a shadow rule? Why?

**I4 (O2, apply).** Before the agent plans, which one search finds every existing implementation of "disputed"?
a) Ask the agent "is there already a dispute rule?"
b) Search the team wiki for "dispute"
c) Search `*.cs` files for `IsDisputed`
d) `git grep -n -i -E "disput|Status = 'D'" -- "*.cs" "*.sql"`

**I5 (O2, apply).** The search returns the three lines below. Where does the VAT rounding rule live?

```text
src/Billing/VatCalculator.cs:31:        return Math.Round(net * rate, 2, MidpointRounding.AwayFromZero);
db/procs/usp_InvoiceTotals.sql:27:       ROUND(l.Net * l.VatRate, 2) AS Vat
docs/billing/vat.md:5:                   VAT is rounded per line, half away from zero.
```

a) In two places already: the C# calculator and the stored procedure; the doc is not an implementation
b) Only in `VatCalculator`
c) Only in `usp_InvoiceTotals`
d) In three places: the calculator, the procedure and the doc

**I6 (O3, create).** Ticket BILL-262: "Exclude disputed invoices from dunning letters." Write the one line you would add to the research brief.

**I7 (O3, create).** Ticket BILL-270: "Show VAT per line on the invoice export." Write the one line you would add to the research brief.

**I8 (O1, analyze).** The agent's diff changes `usp_ExportInvoices` to compute VAT inline. `VatCalculator` exists and the invoice screen uses it. Is this a shadow rule? If yes, name what it duplicates.

```sql
SELECT l.InvoiceId, l.Net, CAST(l.Net * l.VatRate AS decimal(18,2)) AS Vat FROM dbo.InvoiceLine l;
```

## Key (instructor only; do not hand out)

| Item | Form A | Form B | Scoring |
|---|---|---|---|
| I1 | b | c | 1 if correct |
| I2 | yes, `InvoiceService.IsOverdue` (no grace period, reads the clock) | yes, `InvoiceService.IsDisputed` (ignores open dispute rows) | 1 only with the existing implementation named |
| I3 | no: one implementation, extended; callers and tests unchanged | no: the overload delegates to the one implementation | 1 only with the reason |
| I4 | b | d | 1 if correct |
| I5 | c | a | 1 if correct |
| I6 | e.g. "Before planning, name the existing overdue implementations (`InvoiceService.IsOverdue`, `usp_GetOverdueInvoices`) and reuse one; do not write a new overdue check." | e.g. "Before planning, find and name the existing disputed-invoice rule (`InvoiceService.IsDisputed`) and reuse it; no new dispute check." | 1 if it (a) names the term, (b) names the existing implementation or requires the agent to find and name it before planning, (c) forbids a new one |
| I7 | same rubric for *balance* (`usp_GetCustomerBalance`, `BalanceService`) | same rubric for *VAT* (`VatCalculator`) | as I6 |
| I8 | yes, `usp_GetCustomerBalance` | yes, `VatCalculator` (and the rounding differs: `CAST` to `decimal(18,2)` is not "half away from zero") | 1 only with the existing implementation named |

Note on I5: in both forms the lab repository *already* contains a shadow rule (the procedure and the C# method implement the same rule). A learner who says so has understood the concept: mark it correct and use it in the reflection.
