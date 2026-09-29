# BILL-161 — Show the latest note on each open invoice

**Type:** Story · **Priority:** Medium · **Reporter:** Collections team

## Description

When collections call a customer, they want to see the most recent note on each of the customer's
open invoices: what was said, who wrote it and when.

## Acceptance criteria

1. A repository method returns, for one customer, the latest note of each issued invoice:
   invoice id, note text, author and time (UTC).
2. Invoices without notes are not returned.
3. Data access follows ADR 0007 (Dapper repository behind an interface, `CancellationToken`).
4. Unit tests cover the service logic with a fake repository.
