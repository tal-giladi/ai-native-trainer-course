# Contoso Billing — topology map

On-demand context: referenced from `AGENTS.md` by path, never `@`-imported. Regenerate the tree with
`git ls-files` when folders change; review in the same PR.

```text
Contoso.Billing.sln                      the only solution (build + test entry point)
src/Contoso.Billing/
  Common/IClock.cs                       IClock + SystemClock; every "now" comes from here
  Invoices/Invoice.cs                    Invoice record, InvoiceStatus (Draft, Issued, Paid, Void)
  Invoices/IInvoiceRepository.cs         repository interface (GetAsync, ListByCustomerAsync)
  Invoices/InvoiceRepository.cs          Dapper implementation; Func<IDbConnection>; CancellationToken
  Invoices/InvoiceService.cs             OutstandingAsync, IsOverdue (overdue rule lives in code)
  Legacy/SqlHelper.cs                    [Obsolete] 2016 helper; DataSet-based
  Legacy/MonthlyRevenueReport.cs         last SqlHelper caller; migration owned by BILL-97
tests/Contoso.Billing.Tests/
  InvoiceServiceTests.cs                 FixedClock + in-memory repository pattern for unit tests
  ConventionTests.cs                     SqlHelper only in Legacy/; no DateTime.Now
db/migrations/                           V###__name.sql (+ U###__name.sql from V003 on)
docs/adr/0007-dapper-repositories.md     why Dapper replaced SqlHelper (2024)
docs/ARCHITECTURE.md                     STALE (2021); do not follow
docs/history-excerpt.txt                 dated history of conventions
tickets/                                 sample tickets (BILL-142)
```

## Where does X go?

| I need to… | Go to |
|---|---|
| add a query | a method on `IInvoiceRepository` + `InvoiceRepository` |
| add business logic about invoices | `InvoiceService`, with the clock injected |
| change the schema | new `V###` + `U###` pair in `db/migrations/` |
| test a service | copy the `FixedClock` / `InMemoryInvoices` pattern from `InvoiceServiceTests` |
