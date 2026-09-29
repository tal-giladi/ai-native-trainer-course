# DRAFT-01 — Days overdue on the customer statement

**Type:** Story · **Priority:** Medium · **Reporter:** Collections team (from PRD)

## Description

The customer statement (BILL-154) lists issued invoices with a running balance. Collections also
wants each line to show how many whole days the invoice is overdue, so customers see what to pay
first. Depends on BILL-154.

## Acceptance criteria

1. Each statement line has a whole number of days overdue, computed in UTC from `IClock.UtcNow`.
2. "Overdue" means what `InvoiceService.IsOverdue` already means; a line that is not overdue shows 0.
3. An invoice due exactly now shows 0 days (it is overdue from its due instant, 0 whole days).
4. Unit tests cover: not yet due (0), due 1 day and 1 hour ago (1), due exactly now (0).

## Source

`prd/PRD-collections-q4.md`, requirement 5.
