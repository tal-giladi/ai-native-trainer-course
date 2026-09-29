# Contoso Billing — Architecture

_Last edited: 2021-06-10 (copied from the team wiki)._

## Data access

All database access goes through `SqlHelper.ExecuteDataSet`. Repositories return `DataSet`
objects and the caller reads the tables it needs. This keeps SQL in one place.

## Dates

Use `DateTime.Now` for timestamps; the database server and the app servers run in the same
time zone (Israel Standard Time).

## Build

Open `Billing.sln` in Visual Studio 2019 and build. Tests are in `Billing.UnitTests`.
