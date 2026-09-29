namespace Contoso.Billing.Invoices;

public interface IInvoiceRepository
{
    Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct);
    Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct);
}
