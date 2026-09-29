# BILL-151 — Collections summary for one customer

**Type:** Story · **Priority:** Medium · **Reporter:** Collections team

## Description

The collections dashboard needs two numbers for a customer: the total amount still owed, and
the part of it that is overdue right now.

## Acceptance criteria

1. One call returns both numbers for a customer.
2. "Owed" and "overdue" mean exactly what they already mean elsewhere in Billing.
3. Unit tests cover: a customer with no invoices, a customer with one overdue and one not-yet-due
   issued invoice, and a customer whose only invoices are paid or void.
