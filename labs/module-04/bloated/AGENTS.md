# AGENTS.md — Contoso Billing

_Created 2025-03 for Copilot and Cursor users. Claude users: see CLAUDE.md (it has more)._

## Project

Contoso Billing: invoices for Contoso customers. .NET 8 back end, SQL Server, React portal.

## Build and test

- Build: `dotnet build Contoso.Billing.sln`
- Test: `dotnet test Contoso.Billing.sln`
- Front end: `npm ci && npm test` in `portal/`

## Architecture

- Services in `src/Contoso.Billing`, one folder per aggregate.
- New data access uses Dapper repositories behind interfaces (`IInvoiceRepository`).
- Do not use `SqlHelper` in new code.
- The current time comes from `IClock`.
- Money is `decimal`.

## Database

- Migrations in `db/migrations`, `V###__name.sql`.
- Money is `decimal(19,4)`.
- Dates are `datetimeoffset(0)` in UTC.

## Style

- 4 spaces, file-scoped namespaces, nullable enabled.
- XML doc comments on public members.
- Async all the way; pass `CancellationToken`.
- Keep methods short.
- Use `var` when the type is obvious.
- Prefer records for immutable data.
- Seal classes that are not designed for inheritance.

## Testing

- xUnit.
- Arrange-Act-Assert.
- Inject the clock in tests.
- Do not call the database in unit tests.

## Pull requests

- Link the Jira ticket.
- Keep pull requests small.
- Squash-merge.
- Get one approval.

## Front-end

- React + TypeScript + Tailwind.
- Functional components and hooks.
- Storybook story for every component.
- React Query for server state.

## Deployment

- Kubernetes + Helm, chart in `deploy/helm/billing`.
- No deployments on Friday.

## Security

- No secrets in the repository.
- Parameterized SQL only.
- Never log personal data.

## Performance

- Avoid N+1 queries.
- Page large result sets.
- Measure before optimizing.

## Logging

- `ILogger<T>` with message templates.
- Include the invoice id in invoice log lines.

## Error handling

- Catch specific exceptions.
- Guard clauses for arguments.
- No exceptions for control flow.

## Domain

- Statuses: Draft, Issued, Paid, Void.
- Outstanding = sum of issued invoices.
- Overdue = issued and past due.

## Contacts

- Team lead: Avi.
- DBA: Rina.
- Front-end lead: Tamar.
