using Contoso.Billing.Common;

namespace Contoso.Billing.Invoices;

public sealed class InvoiceService
{
    private readonly IInvoiceRepository _invoices;
    private readonly IClock _clock;

    public InvoiceService(IInvoiceRepository invoices, IClock clock)
    {
        _invoices = invoices;
        _clock = clock;
    }

    /// <summary>An invoice is overdue when it is issued and its due instant has passed (UTC).</summary>
    public bool IsOverdue(Invoice invoice) =>
        invoice.Status == InvoiceStatus.Issued && invoice.DueUtc < _clock.UtcNow;
}
