# BILL-155 — Void an issued invoice

**Type:** Story · **Priority:** Medium · **Reporter:** Finance

## Description

Finance voids invoices that were issued by mistake. Today they ask a DBA to update the row.

## Acceptance criteria

1. `InvoiceService` exposes a method that voids one invoice and returns whether it did.
2. Only an issued invoice can be voided; a draft, paid or already void invoice is refused.
3. The status change is written through a repository method (no SQL in the service).
4. Unit tests cover: issued (voided), paid (refused), unknown id (refused).
