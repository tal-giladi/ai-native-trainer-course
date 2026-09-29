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

    /// <summary>Total still owed by a customer: issued, not paid, not void.</summary>
    public async Task<decimal> OutstandingAsync(int customerId, CancellationToken ct)
    {
        var invoices = await _invoices.ListByCustomerAsync(customerId, ct);
        return invoices.Where(i => i.Status == InvoiceStatus.Issued).Sum(i => i.Amount);
    }

    /// <summary>An invoice is overdue when it is issued and its due instant has passed (UTC).</summary>
    public bool IsOverdue(Invoice invoice) =>
        invoice.Status == InvoiceStatus.Issued && invoice.DueUtc < _clock.UtcNow;
}
