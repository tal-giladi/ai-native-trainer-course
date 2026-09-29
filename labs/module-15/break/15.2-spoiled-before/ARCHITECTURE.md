# Contoso Billing — Architecture

_Last edited: 2026-09-22._

## Data access

New data access uses Dapper in a repository behind an interface (ADR 0007). `SqlHelper` is
obsolete and only `Legacy/` may call it.

## Dates

"Now" comes from `IClock.UtcNow`. Columns are `datetimeoffset(0)` in UTC.

## Build

`dotnet test Contoso.Billing.sln`.
