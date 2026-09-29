# BILL-152 — Invoice due exactly now is overdue

**Type:** Bug · **Priority:** Low · **Reporter:** Finance

## Description

An invoice whose due instant is exactly the current instant is reported as not overdue. Finance
says an invoice is overdue from its due instant onward.

## Acceptance criteria

1. `InvoiceService.IsOverdue` returns true when the due instant equals `IClock.UtcNow` for an
   issued invoice.
2. A test covers the boundary.
