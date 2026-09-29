# Agent instructions — Contoso back office

## Data access
- `src/Contoso.Orders` reads and writes through SQL Server stored procedures called with Dapper
  (`conn.Query<T>("dbo.usp_...", ..., commandType: CommandType.StoredProcedure)`).
  Never use EF Core or `AppDbContext` in `src/Contoso.Orders`: the procedures apply the tenant filter
  and soft-delete rules, and EF queries would bypass both.
- `src/Contoso.Billing` uses EF Core (`AppDbContext`). Do not copy that pattern into Orders.
- Every new procedure goes in `sql/` as `CREATE OR ALTER PROCEDURE dbo.usp_<Verb><Noun>` with `SET NOCOUNT ON`.

## Style
- Keep the existing `using (...)` block style in legacy files; do not modernise unrelated code.
