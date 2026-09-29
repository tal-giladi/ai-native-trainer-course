# Draft tickets — PRD-collections-q4

Produced by `spec` 1.0.0 from `prd/PRD-collections-q4.md`. Nothing here is committed to the backlog until the PRD owner has read the questions.

## Drafts

| Draft | PRD requirement | Depends on |
|---|---|---|
| `DRAFT-01.md` Days overdue on the customer statement | 5 | BILL-154 |

## Already covered by an existing ticket (no draft)

| PRD requirement | Existing ticket |
|---|---|
| 1 Owed and overdue per customer | BILL-151 |
| 2 Record reminders, refuse within 7 days | BILL-150 |
| 3 Customer statement with running balance | BILL-154 |

## Not ticketed

| PRD requirement | Why |
|---|---|
| 4 "fast and easy to read" | Not testable as written; see question 1. |
| 6 Send reminder emails from Billing | "Later: not this quarter." |
| Non-goal: monthly revenue report | Owned by BILL-97. |

## Questions for the PRD owner

1. Requirement 4: what does "fast" mean (a response time for how many invoices?) and who judges "easy to read"? Without numbers it cannot be accepted or rejected.
2. Requirement 5 and BILL-152: an invoice due exactly now is overdue with 0 whole days. Is showing "0 days overdue" acceptable to Collections?

## Build order

BILL-151 → BILL-150 → BILL-154 → DRAFT-01.
