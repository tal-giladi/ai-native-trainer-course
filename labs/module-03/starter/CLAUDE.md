# Contoso Billing — agent rules

You are a senior .NET developer working on Contoso Billing.

## General

- Write clean, maintainable code that follows SOLID principles.
- Always think step by step before writing code.
- Be careful with performance.

## Build and test

- Build with `dotnet build Billing.sln`.
- Run tests from `Billing.UnitTests`.

## Data access

- All data access goes through `SqlHelper.ExecuteDataSet`; repositories return a `DataSet` and the caller reads the tables it needs.
- Keep SQL in one place.

## Dates

- Use `DateTime.Now` for timestamps; all servers run in Israel Standard Time.

## Style

- Use 4 spaces for indentation.
- Use `var` when the type is obvious.
- Add XML doc comments to public methods.
