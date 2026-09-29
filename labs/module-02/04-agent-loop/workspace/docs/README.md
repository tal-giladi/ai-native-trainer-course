# Orders module notes

Order lookups for the back-office screens live in `src/OrderService.cs`.
All reads go through stored procedures in `sql/`; the procedures enforce tenant isolation and soft deletes.
