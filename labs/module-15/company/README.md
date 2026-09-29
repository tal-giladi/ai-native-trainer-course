# Contoso Ltd — Billing team handbook

> Contoso Ltd is a fictional company, built for a public demo. The code, people, tickets and numbers
> are invented. Addresses use the reserved `.example` domain.

Contoso sells maintenance contracts to about 1,400 business customers. **Billing** is the .NET and
SQL Server system that issues their invoices and tells Collections and Finance who owes what.

## Who uses Billing

- **Collections** chase overdue invoices every morning. They care about "owed" and "overdue".
- **Finance** closes the month and reconciles the monthly revenue report against the ledger. They
  care that the revenue number never moves by surprise.

## How work flows

1. Work starts as a ticket in the BILL project (exported to `tickets/` for the demo).
2. A pull request needs one approving review; changes to the AI layer need `@contoso/billing-leads`
   (see `.github/CODEOWNERS` once the layer exists).
3. `dotnet test` must pass in CI. The convention tests encode rules code review kept missing.
4. Month-end freeze: nothing that touches the revenue report deploys on the first three business
   days of a month.

## Definition of done

- Acceptance criteria met, each with a test that would fail without the change.
- No new warnings; no new packages without a reviewer's explicit yes.
- Anything you were unsure about is written in the PR, not decided silently.

More: [team](team.md) · [glossary](glossary.md) · [onboarding (wiki copy)](docs/onboarding.md)
