# Revenue export — notes

Ported from the Northwind Payments revenue export I wrote in 2022, simplified.

- Same idea as there: one CSV per month, one row per customer.
- The awkward case is Tailspin Toys: their invoices are split per site, so the export groups by
  parent customer. Rachel Cohen (rachel.cohen@northwindpayments.test) knows why.
- Their controller signs off the numbers on day 3 of the close.
