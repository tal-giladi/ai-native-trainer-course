# BILL-161 session excerpt (illustrative, abridged)

Reconstructed from a real-shaped run for the lesson; tool output is exactly what the server returns for
`samples/catalog-nightly.json` with no `--migrations` configured.

**User:** Implement BILL-161 (tickets/BILL-161.md). Use the schema server for table shapes.

**Tool call** `mcp__contoso-schema__describe_table` `{"table":"dbo.Invoice"}`

```text
[schema] source=snapshot:catalog-nightly.json version=V005 captured-age=41.6d repo-head=unknown status=UNKNOWN
WARNING: no migrations folder configured, so the server cannot tell whether this schema is current.
dbo.Invoice
  InvoiceId      bigint             NOT NULL
  CustomerId     int                NOT NULL
  Amount         decimal(19,4)      NOT NULL
  IssuedUtc      datetimeoffset(0)  NOT NULL
  DueUtc         datetimeoffset(0)  NOT NULL
  Status         tinyint            NOT NULL
  Notes          nvarchar(400)      NULL
```

**Assistant:** Notes are stored in `dbo.Invoice.Notes`. The schema has no author or timestamp for notes, so
`AuthorUpn` and `CreatedUtc` will be null for now; I'll note this as a follow-up. Creating
`InvoiceNotes.cs` with a Dapper repository and a service, plus tests.

**Tool calls** `Write` src/Contoso.Billing/Invoices/InvoiceNotes.cs, `Write` tests/…/InvoiceNotesServiceTests.cs,
`Bash` `dotnet test` → `Passed! - Failed: 0, Passed: 7`

**Assistant:** BILL-161 is implemented and all 7 tests pass. Author and time are not available in the
database; I suggest a follow-up ticket to add them.

*(The agent never listed `db/migrations/`. V006__invoice_note_table.sql, merged two weeks earlier, dropped
`dbo.Invoice.Notes` and added `dbo.InvoiceNote` with author and time and a procedure that returns exactly
what BILL-161 asks for. The first call in production fails with "Invalid column name 'Notes'".)*
