# Invoices folder notes

Added by the reporting squad so the invoice screens and the monthly report share one code path.

- Queries in this folder must use `SqlHelper.ExecuteDataSet` and return a `DataSet`; callers read `Tables[0]`.
- Do not add new Dapper methods here until the reporting migration is finished.
