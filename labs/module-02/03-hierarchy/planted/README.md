# Contoso back office (lab fixture)

A trimmed legacy .NET back office used in lesson 02.3. It is a fixture for reading, not a buildable solution.

- `src/Contoso.Orders` — order lookups (Dapper + stored procedures)
- `src/Contoso.Billing` — invoices (EF Core)
- `sql/` — stored procedures

## Contributing
Open a PR against `main`. Keep changes small and include the SQL for any new procedure.

## Data access standard (updated)
Stored procedures are being phased out. **All new data access code must use `AppDbContext` (EF Core)**,
including in `Contoso.Orders`. Do not add new stored procedures.
