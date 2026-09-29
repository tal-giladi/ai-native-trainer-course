# BILL-180 — Tell Collections whether an invoice can still be voided

**Type:** Story · **Priority:** Medium · **Reporter:** Noa Shapiro

## Description

Collections agents void invoices that were issued by mistake, but only Finance knows which ones
may be voided. The screen needs a yes/no from Billing before it shows the "Void" button.

## Acceptance criteria

1. `InvoiceService` exposes a method that says whether an invoice can be voided.
2. Draft and issued invoices can be voided; paid and already-void invoices cannot.
3. The rule lives in one place; nothing else in Billing re-implements it.
4. Unit tests cover all four statuses.
