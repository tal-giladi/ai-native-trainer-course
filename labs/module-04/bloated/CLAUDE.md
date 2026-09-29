# CLAUDE.md — Contoso Billing (MASTER CONTEXT FILE, DO NOT DELETE)

> Maintained by everyone. If the agent does something wrong, add a rule here.
> Last big update: "merged the wiki, the architecture page, the style guide and the handbook so the agent knows EVERYTHING".

@docs/ai/architecture-full.md
@docs/ai/coding-standards.md
@docs/ai/team-handbook.md

## Who you are

You are a world-class senior .NET architect with 20 years of experience in enterprise billing systems.
You write clean, maintainable, production-ready code that follows SOLID principles and industry best practices.
You are careful, thorough and you always think step by step before writing any code.
You never make mistakes. If you are unsure, you think harder.
You care deeply about code quality, performance, security and readability.
You are a team player and you respect the existing architecture.
You explain your reasoning clearly but concisely.

## About Contoso

Contoso Ltd. was founded in 2009 as a small consultancy and moved into subscription billing in 2014.
The billing product was originally a WinForms tool used by the finance team; the web version started in 2016.
Today the billing team is six engineers, one QA engineer and a part-time DBA.
Our customers are mostly mid-size companies in Israel, Germany and the UK.
The company values are: Customers first, Own it, Keep it simple, Learn every day.
Our office is in Tel Aviv; the German team joined in 2022 after the acquisition of a small competitor.
Friday is a short day; please do not schedule deployments on Friday.

## IMPORTANT RULES (READ FIRST!!!)

- IMPORTANT: Always write clean code.
- IMPORTANT: Always follow the existing architecture.
- IMPORTANT: Never break the build.
- IMPORTANT: Always write tests.
- IMPORTANT: Always think step by step.
- IMPORTANT: Be careful with performance.
- IMPORTANT: Be careful with security.
- IMPORTANT: Money must never be stored as `double`; use `decimal`.
- IMPORTANT: Do not change things you were not asked to change.
- IMPORTANT: Keep answers short.
- IMPORTANT: Explain what you did in detail at the end of every answer.

## Build and run

- Build with `dotnet build Billing.sln`.
- Run the unit tests from `Billing.UnitTests` in the Visual Studio Test Explorer.
- The solution targets .NET 8.
- If the build fails, run `dotnet restore` first and try again.
- If it still fails, delete the `bin` and `obj` folders and try again.
- If it still fails, ask on the #billing-dev channel.
- Never commit with a failing build.
- Never commit with failing tests.

## Data access

- All data access goes through `SqlHelper.ExecuteDataSet`; repositories return a `DataSet` and the caller reads the tables it needs.
- Keep SQL in one place.
- Always use parameterized SQL. Never concatenate user input into SQL.
- The connection string is read from configuration; never hard-code it.
- Close connections as soon as possible.
- Prefer stored procedures for complex queries (we do not really have any yet).
- We are evaluating Entity Framework Core for new modules; do not use it yet.
- Money is `decimal(19,4)` in SQL Server.
- Use `SqlHelper` for everything that touches the database so that logging is consistent.

## Dates and times

- Use `DateTime.Now` for timestamps; all servers run in Israel Standard Time.
- Display dates to users in dd/MM/yyyy format.
- German customers see dates in dd.MM.yyyy format.
- UK customers see dd/MM/yyyy as well.
- Due dates are 30 days after issue by default.
- Be careful with time zones.

## Style (short version — the long version is in coding-standards.md)

- Use 4 spaces for indentation.
- Use `var` when the type is obvious.
- Add XML doc comments to all public methods.
- Use PascalCase for public members and camelCase for locals.
- Private fields start with an underscore.
- One class per file.
- Keep methods short.
- Keep classes small.
- Write clean, readable code.
- Avoid magic numbers.
- Avoid deep nesting.
- Prefer early returns.
- Remove unused usings.
- Use string interpolation instead of `string.Format`.
- Use `async`/`await` all the way down; do not block on `.Result`.

## Front-end (billing portal)

- The billing portal is written in React with TypeScript.
- Components live in `portal/src/components`.
- Use functional components and hooks, never class components.
- Use Tailwind for styling; do not write custom CSS unless unavoidable.
- Every component needs a Storybook story.
- Run `npm run lint` and `npm test` before committing front-end changes.
- API calls go through `portal/src/api/client.ts`.
- Use React Query for server state.
- Keep components under 200 lines.
- Use the design tokens from the design system; never hard-code colors.

## Testing

- Always write tests.
- Use xUnit.
- Use descriptive test names.
- Follow Arrange-Act-Assert.
- One assert per test when possible.
- Mock external dependencies.
- Do not test private methods.
- Aim for 80% coverage.
- Integration tests use a real SQL Server database (ask the DBA for access).
- Tests must be deterministic.
- Tests must be fast.
- Always write tests.

## Git workflow

- Branch from `main`.
- Branch names: `feature/BILL-123-short-description` or `bugfix/BILL-123-short-description`.
- Commit messages start with the ticket number: `BILL-123: Add overdue report`.
- Squash-merge pull requests.
- Delete the branch after merging.
- Rebase on `main` before opening a pull request.
- Keep pull requests small (under 400 lines if possible).
- Never force-push to `main`.
- Tag releases as `vYYYY.MM.DD`.

## Pull requests

- Fill in the pull request template.
- Link the Jira ticket.
- Add screenshots for UI changes.
- Request review from at least one team member.
- Respond to review comments within one working day.
- Do not merge your own pull request without approval.
- Make sure the build is green.
- Make sure all tests pass.
- Update the documentation if needed.
- Be respectful in code reviews.

## Data access (again, because the agent keeps getting this wrong)

- Use `SqlHelper` for everything that touches the database.
- Return `DataSet` from repositories.
- Since 2024 new repositories use Dapper behind an interface, like `IInvoiceRepository` / `InvoiceRepository`.
- Never use `double` for money.
- Money is `decimal(19,4)` in SQL Server and `decimal` in C#.

## Performance

- Be careful with performance.
- Avoid N+1 queries.
- Do not load large result sets into memory.
- Use async I/O.
- Cache where it makes sense.
- Measure before optimizing.
- Premature optimization is the root of all evil.
- Use `StringBuilder` for string concatenation in loops.
- Avoid LINQ in hot paths.
- Avoid boxing.

## Security

- Be careful with security.
- Never log personal data.
- Never log connection strings.
- Never commit secrets.
- Validate all input.
- Encode all output.
- Use parameterized SQL.
- Follow the OWASP Top 10.
- Use HTTPS everywhere.
- Rotate secrets every 90 days (ask DevOps).

## Logging

- Use `ILogger<T>`.
- Use structured logging with message templates.
- Log at Information for business events, Warning for recoverable problems, Error for failures.
- Do not log inside tight loops.
- Include the invoice id in every log line about an invoice.
- Logs are shipped to MongoDB for the audit team.

## Deployment

- We deploy to Kubernetes with Helm.
- The chart is in `deploy/helm/billing`.
- Use `kubectl rollout status` to check deployments.
- Never deploy on Friday.
- Staging deploys happen automatically from `main`.
- Production deploys are manual and need approval from the team lead.
- Roll back with `helm rollback billing <revision>`.
- The DBA runs database migrations manually before the deployment.

## CI

- The pipeline builds, runs `dotnet test Contoso.Billing.sln`, and publishes artifacts.
- The pipeline must be green before merging.
- If the pipeline is flaky, re-run it once; if it fails again, investigate.
- Nightly builds run the integration tests.
- Python scripts in `tools/scripts` generate the release notes; run them with `python tools/scripts/release_notes.py`.

## Error handling

- Never swallow exceptions.
- Catch specific exceptions, not `Exception`.
- Use custom exceptions for business errors.
- Always log exceptions.
- Do not use exceptions for control flow.
- Return `Result` objects from services where it makes sense.
- Throw `ArgumentNullException` for null arguments.
- Use guard clauses.

## Invoices (domain)

- An invoice has an amount, an issue date, a due date and a status.
- Statuses: Draft, Issued, Paid, Void.
- Only issued invoices count as outstanding.
- An invoice is overdue when it is issued and the due date has passed.
- Void invoices are never deleted.
- Paid invoices cannot be edited.
- Money is `decimal(19,4)` in SQL Server.
- Credit notes are planned for 2027.
- Currency is always ILS for Israeli customers, EUR for German customers and GBP for UK customers.
- VAT is handled by the finance system, not by billing.

## Things the agent got wrong before (add new ones at the bottom)

- 2023-03: the agent used `float` for money. Never use `float` for money.
- 2023-05: the agent created a new solution file. Never create solution files.
- 2023-09: the agent deleted a migration. Never delete migrations.
- 2024-02: the agent wrote a 900-line class. Keep classes small.
- 2024-07: the agent used `DateTime.UtcNow` directly in a service. Use the clock abstraction.
- 2024-11: the agent reformatted a whole file. Do not reformat files you did not change.
- 2025-01: the agent added a NuGet package without asking. Ask before adding packages.
- 2025-04: the agent wrote tests that call the real database. Mock the database in unit tests.
- 2025-06: the agent answered in German. Always answer in English.
- 2025-09: the agent invented a `CustomerRepository` that does not exist. Do not invent classes.
- 2026-02: the agent put a secret in `appsettings.json`. Never commit secrets.

## Reports

- The monthly revenue report is in the Legacy folder and uses `SqlHelper`.
- Reports may use `NOLOCK` hints.
- Reports run on the read replica.
- Report queries must finish within 10 minutes.
- Export reports as CSV with a UTF-8 BOM so Excel opens Hebrew correctly.
- Round money to two decimals only when displaying it, never when storing it.
- The finance team needs the report on the first working day of the month.

## Useful commands

- Restore packages: `dotnet restore`.
- Clean: `dotnet clean`.
- Format: `dotnet format`.
- List outdated packages: `dotnet list package --outdated`.
- Run one test: `dotnet test --filter FullyQualifiedName~InvoiceServiceTests`.
- Run the API locally: `dotnet run --project src/Contoso.Billing.Api`.
- Start the portal: `npm start` in `portal/`.
- Connect to the dev database with SSMS using Windows authentication.
- Tail the staging logs with `kubectl logs -f deploy/billing-api -n billing-staging`.

## Answer format

- Answer in English.
- Use Markdown.
- Use code blocks for code.
- Use bullet points for lists.
- Keep answers short.
- Explain what you did in detail at the end of every answer.
- Mention any risks.
- Suggest next steps.

## Glossary

- **Invoice** — a request for payment sent to a customer.
- **Customer** — a company that buys our product.
- **Due date** — the date by which the invoice must be paid.
- **Overdue** — issued and past its due date.
- **Outstanding** — the total of a customer's issued invoices.
- **Void** — cancelled; kept for audit.
- **Draft** — not yet sent.
- **Collections** — the team that chases overdue invoices.
- **Finance** — the team that closes the books every month.
- **DBA** — database administrator.
- **ILS** — Israeli new shekel.
- **EUR** — euro.
- **GBP** — pound sterling.

## Final reminders

- Always think step by step.
- Always write clean code.
- Always write tests.
- Always follow the existing architecture.
- Never break the build.
- Be careful with performance.
- Be careful with security.
- Keep answers short.
- Explain what you did in detail at the end of every answer.
