using Contoso.Billing.Invoices;

namespace Contoso.Billing.Collections;

// Written by an agent from the BILL-151 ticket text alone ("paste-and-go"). It compiles,
// its own tests pass and the existing convention tests stay green. Compare it with
// InvoiceService before you read the lesson's Fix it section.
public sealed record CollectionsSummary(int CustomerId, decimal Owed, decimal Overdue);

public sealed class CollectionsSummaryService
{
    private readonly IInvoiceRepository _invoices;

    public CollectionsSummaryService(IInvoiceRepository invoices) => _invoices = invoices;

    public async Task<CollectionsSummary> GetAsync(int customerId, CancellationToken ct)
    {
        var invoices = await _invoices.ListByCustomerAsync(customerId, ct);
        var unpaid = invoices.Where(i => i.Status != InvoiceStatus.Paid).ToList();
        var owed = unpaid.Sum(i => i.Amount);
        var overdue = unpaid.Where(i => i.DueUtc < DateTimeOffset.UtcNow).Sum(i => i.Amount);
        return new CollectionsSummary(customerId, owed, overdue);
    }
}
