# BILL-153 — Store the customer's purchase-order number on every invoice

**Type:** Story · **Priority:** High · **Reporter:** Finance

## Description

Large customers reject invoices that do not quote their purchase-order (PO) number. Finance needs
every invoice to carry one, so the PO number can be printed and searched.

## Acceptance criteria

1. Every invoice has a PO number of up to 35 characters; it is never NULL in the database.
2. `Invoice` exposes the PO number, and `IInvoiceRepository.GetAsync` and `ListByCustomerAsync`
   return it.
3. Invoices that already exist keep working and are visible with a PO number.
4. Unit tests cover: an invoice read with its PO number, and a customer list that includes it.
