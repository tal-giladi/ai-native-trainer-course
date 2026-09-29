namespace Contoso.Billing.Invoices;

public interface IInvoiceReminderRepository
{
    /// <summary>The most recent reminder for the invoice, or null if none was ever sent.</summary>
    Task<DateTimeOffset?> GetLastSentAsync(long invoiceId, CancellationToken ct);

    Task AddAsync(long invoiceId, DateTimeOffset sentUtc, CancellationToken ct);
}
