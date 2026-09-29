namespace Contoso.Billing.Invoices;

public interface IInvoiceRepository
{
    Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct);
}
