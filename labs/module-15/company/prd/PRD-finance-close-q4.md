# PRD — Finance close, Q4

**Owner:** Ruth Amar (Finance controller) · **Status:** Approved · **Date:** 2026-09-10

## Problem

Month-end close takes Finance three business days. One of them is spent waiting: the monthly
revenue report can only be run by a developer, from a console, with a connection string pasted in.

## Goals

- Finance runs the monthly revenue report from the new month-end reporting job, without a developer.
- The revenue number does not change as a side effect of any of this work.

## Requirements

1. The revenue report can be called by the reporting job through the same repository pattern as
   the rest of Billing (BILL-97).
2. Collections can see whether an invoice may still be voided before they try (BILL-180).
3. Later: revenue per currency (not this quarter).

## Non-goals

- Changing what counts as revenue. The report includes every invoice issued in the month, whatever
  its status; Finance adjusts voids in the ledger. Changing that is a Finance decision, not a ticket.

## Constraints

- Month-end freeze: no deployment that touches the revenue report on the first three business days
  of a month.
