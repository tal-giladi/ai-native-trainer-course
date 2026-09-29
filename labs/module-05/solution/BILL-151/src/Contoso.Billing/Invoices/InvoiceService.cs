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
        return Owed(invoices).Sum(i => i.Amount);
    }

    /// <summary>An invoice is overdue when it is issued and its due instant has passed (UTC).</summary>
    public bool IsOverdue(Invoice invoice) =>
        invoice.Status == InvoiceStatus.Issued && invoice.DueUtc < _clock.UtcNow;

    /// <summary>BILL-151: what the customer owes and the overdue part of it, with the same meanings as above.</summary>
    public async Task<CollectionsSummary> CollectionsSummaryAsync(int customerId, CancellationToken ct)
    {
        var owed = Owed(await _invoices.ListByCustomerAsync(customerId, ct)).ToList();
        return new CollectionsSummary(owed.Sum(i => i.Amount), owed.Where(IsOverdue).Sum(i => i.Amount));
    }

    private static IEnumerable<Invoice> Owed(IEnumerable<Invoice> invoices) =>
        invoices.Where(i => i.Status == InvoiceStatus.Issued);
}

public sealed record CollectionsSummary(decimal Owed, decimal Overdue);
