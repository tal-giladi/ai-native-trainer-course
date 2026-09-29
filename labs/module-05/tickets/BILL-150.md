# BILL-150 — Record that a payment reminder was sent

**Type:** Story · **Priority:** High · **Reporter:** Collections team

## Description

Collections sends payment reminders by email from their own tool. They need Billing to record
each reminder, so that nobody reminds the same customer about the same invoice twice in one week.

## Acceptance criteria

1. `InvoiceService` exposes a method that records a reminder for one invoice and returns whether
   it was recorded.
2. A reminder can only be recorded for an invoice that is overdue (same meaning as
   `InvoiceService.IsOverdue`).
3. A second reminder for the same invoice within 7 days of the previous one is refused.
4. Reminders are stored in the database with the instant they were sent (UTC). Existing invoice
   rows and existing columns do not change.
5. Unit tests cover: not overdue (refused), overdue with no earlier reminder (recorded), overdue
   with a reminder 3 days ago (refused), overdue with a reminder 8 days ago (recorded).
