# Contoso Billing — Full Architecture (pasted from the wiki for the agent)

_Source: Confluence page "Billing Architecture", exported 2021-06-10, pasted here 2025-11 with small edits._

## 1. Overview

Contoso Billing is the system that creates, sends and tracks invoices for Contoso customers.
It consists of a .NET back end, a SQL Server database, a React billing portal, a nightly job host,
and integrations with the finance system and the e-mail service.

The system was designed in 2016 as a classic three-layer application:

1. Presentation (the portal, originally ASP.NET WebForms, now React).
2. Business logic (services).
3. Data access (`SqlHelper`).

Every layer only talks to the layer directly below it.

## 2. Components

### 2.1 Billing API

- ASP.NET Web API 2 (2016), migrated to ASP.NET Core in 2020.
- Hosts the REST endpoints used by the portal.
- Authentication is done by the company identity provider (OAuth 2.0).
- Endpoints are versioned in the URL: `/api/v1/...`.
- Swagger is available at `/swagger` in development.

### 2.2 Billing core library

- Contains the services and the domain model.
- `InvoiceService` is the main entry point for invoice operations.
- Business rules live in the services, not in the controllers.
- The library has no dependency on ASP.NET.

### 2.3 Data access

All database access goes through `SqlHelper.ExecuteDataSet`. Repositories return `DataSet`
objects and the caller reads the tables it needs. This keeps SQL in one place, makes logging
consistent and lets the DBA review every query in one file.

Example:

```csharp
var ds = SqlHelper.ExecuteDataSet(connectionString,
    "SELECT * FROM dbo.Invoice WHERE CustomerId = @c",
    new SqlParameter("@c", customerId));
foreach (DataRow row in ds.Tables[0].Rows) { /* ... */ }
```

Rules:

- Never open a `SqlConnection` directly outside `SqlHelper`.
- Never use an ORM.
- Every query must be reviewed by the DBA.
- `SELECT *` is acceptable for small tables.

### 2.4 Nightly jobs

- A Windows service runs nightly jobs: reminders, the monthly revenue report, data clean-up.
- Jobs are scheduled with Quartz.NET.
- Each job logs its start and end time.
- If a job fails, an e-mail is sent to the on-call engineer.

### 2.5 Billing portal

- React single-page application written in TypeScript.
- Built with webpack; served from a CDN.
- Uses the Billing API only; never talks to the database.
- State management: Redux (legacy screens) and React Query (new screens).
- Design system: the shared Contoso component library.

### 2.6 Integrations

- **Finance system:** invoices are exported every night as CSV over SFTP.
- **E-mail service:** invoices are sent as PDF attachments via the company SMTP relay.
- **Payment provider:** payment notifications arrive through a webhook and mark invoices as Paid.
- **Audit log:** every change to an invoice is written to MongoDB for the audit team.

## 3. Database

### 3.1 Server

- SQL Server 2019 Standard, one primary and one read replica.
- Database name: `ContosoBilling`.
- Collation: `Hebrew_CI_AS`.
- Backups: full nightly, log every 15 minutes.

### 3.2 Tables

| Table | Purpose |
|---|---|
| `dbo.Invoice` | One row per invoice |
| `dbo.Customer` | Customers (owned by the CRM, replicated nightly) |
| `dbo.InvoiceLine` | Invoice lines (planned) |
| `dbo.Payment` | Payments (planned) |
| `dbo.AuditLog` | Moved to MongoDB in 2023 |

### 3.3 Conventions

- Table names are singular.
- Primary keys are `<Table>Id`, `bigint IDENTITY`.
- Money is `decimal(19,4)`.
- Dates are `datetime` in local time (Israel Standard Time).
- Every table has `CreatedAt` and `UpdatedAt` columns (not yet on `dbo.Invoice`).
- Foreign keys are named `FK_<Child>_<Parent>`.
- Indexes are named `IX_<Table>_<Columns>`.

### 3.4 Migrations

- Migrations are plain SQL scripts in `db/migrations`, named `V###__description.sql`.
- The DBA runs them manually before each deployment.
- Scripts must be idempotent where possible.
- Never rename a column; add a new one and migrate the data.

## 4. Dates and time zones

Use `DateTime.Now` for timestamps. The database server and the application servers run in the same
time zone (Israel Standard Time), so local time is consistent everywhere. Convert to the customer's
time zone only for display.

## 5. Build

Open `Billing.sln` in Visual Studio 2019 and build. Tests are in `Billing.UnitTests`. The build
server uses the same solution file.

## 6. Deployment

- The API and the job host are deployed to Kubernetes with Helm (chart in `deploy/helm/billing`).
- The portal is deployed to the CDN by the front-end pipeline.
- Configuration comes from environment variables and Kubernetes secrets.
- Blue/green deployment is planned.

## 7. Non-functional requirements

- The API must answer 95% of requests within 300 ms.
- The monthly revenue report must finish within 10 minutes.
- Availability target: 99.5% during business hours.
- All personal data must stay in the EU or Israel.
- Invoices must be kept for 7 years.

## 8. Known issues (2021)

- `SqlHelper` opens a new connection per call; acceptable for current load.
- The monthly revenue report is slow for large customers.
- Some old invoices have no due date.
- The portal still has a few WebForms pages.
- Time zone handling for German customers is not correct around daylight-saving changes.

## 9. Future plans (2021)

- Move to a microservice architecture (2022).
- Replace `SqlHelper` with an ORM (maybe).
- Add credit notes.
- Add multi-currency reporting.
- Move the job host to Kubernetes CronJobs.
- Evaluate GraphQL for the portal API.

## 10. Contacts

- Architecture owner: Moshe (left the company in 2023).
- DBA: Rina (part-time).
- Front-end lead: Tamar.
- DevOps: the platform team (#platform on Slack).
