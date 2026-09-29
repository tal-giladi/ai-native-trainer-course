# Billing glossary

Copied from the Confluence space. Where the code already defines a term, the code wins.

| Term | Meaning | Where it lives |
|---|---|---|
| Issued | Sent to the customer and not yet paid or voided (`InvoiceStatus.Issued`) | `Invoices/Invoice.cs` |
| Owed / outstanding | Sum of issued invoices for a customer | `InvoiceService.OutstandingAsync` |
| Overdue | Issued, and the due instant (UTC) has passed | `InvoiceService.IsOverdue` |
| Void | Cancelled after issue; kept for audit, never deleted | `InvoiceStatus.Void` |
| Monthly revenue | Sum of invoice amounts by customer for invoices issued in the month, **whatever their status**; Finance adjusts voids in the ledger | `Legacy/MonthlyRevenueReport.cs` |
| Month-end freeze | First three business days of a month: nothing that touches the revenue report deploys | this handbook |
