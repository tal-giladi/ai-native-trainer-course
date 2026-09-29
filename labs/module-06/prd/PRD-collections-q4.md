# PRD — Collections improvements, Q4

**Owner:** Head of Collections · **Status:** Draft for ticketing · **Date:** 2026-09-20

## Problem

Collections works from spreadsheets exported by a DBA every Monday. By Wednesday the numbers are
stale, customers are chased for invoices they already paid, and nobody can see who was reminded.

## Goals

- Collections sees, per customer, what is owed and what is overdue, from Billing itself.
- Customers are not reminded about the same invoice twice in one week.
- Finance can hand a customer a statement without a DBA.

## Requirements

1. A per-customer view of the amount owed and the part that is overdue.
2. Record each payment reminder; refuse a second reminder for the same invoice within 7 days.
3. A customer statement: issued invoices, oldest due first, running balance.
4. The statement should be fast and easy to read.
5. Show on the statement how many days each invoice is overdue.
6. Later: send the reminder emails from Billing (not this quarter).

## Non-goals

- Changing how invoices are created or paid.
- Any change to the monthly revenue report (BILL-97 owns it).

## Definitions

"Owed", "overdue" and "issued" mean what they already mean in Billing. Ask Finance if unsure.
