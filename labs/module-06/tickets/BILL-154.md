# BILL-154 — Customer statement

**Type:** Story · **Priority:** Medium · **Reporter:** Collections team

## Description

Collections sends customers a statement: their issued invoices, oldest due date first, with a
running balance after each line.

## Acceptance criteria

1. `InvoiceService` exposes a method that returns the statement lines for one customer.
2. Only issued invoices appear; "issued" means what it already means for `OutstandingAsync`.
3. Lines are ordered by due date, oldest first; the running balance is cumulative and uses `decimal`.
4. Unit tests cover: no invoices, three issued invoices out of order, a paid and a void invoice
   that do not appear.
