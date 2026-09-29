# BILL-142 — List a customer's overdue invoices

**Type:** Story · **Priority:** Medium · **Reporter:** Collections team

## Description

Collections needs to see, for one customer, every invoice that is overdue right now, with the
amount and how many whole days it is overdue. Oldest due date first.

## Acceptance criteria

1. `InvoiceService` exposes a method that returns the overdue invoices for a customer.
2. "Overdue" means the same thing `InvoiceService.IsOverdue` already means.
3. Days overdue are computed in UTC.
4. The query only reads the customer's issued invoices from the database (do not load paid or
   void invoices into memory).
5. Unit tests cover: no invoices, one overdue, a paid invoice past its due date (not overdue).
