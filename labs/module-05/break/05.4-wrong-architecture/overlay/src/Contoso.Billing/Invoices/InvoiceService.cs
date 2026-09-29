using Contoso.Billing.Common;
using Contoso.Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Billing.Invoices;

public sealed class InvoiceService
{
    public static readonly TimeSpan ReminderInterval = TimeSpan.FromDays(7);

    private readonly IInvoiceRepository _invoices;
    private readonly IClock _clock;
    private readonly BillingDbContext? _db;

    public InvoiceService(IInvoiceRepository invoices, IClock clock, BillingDbContext? db = null)
    {
        _invoices = invoices;
        _clock = clock;
        _db = db;
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

    /// <summary>Records a reminder if the invoice is overdue and was not reminded in the last 7 days.</summary>
    public async Task<bool> RecordReminderAsync(long invoiceId, CancellationToken ct)
    {
        var db = _db ?? throw new InvalidOperationException("InvoiceService was created without a BillingDbContext.");
        var invoice = await _invoices.GetAsync(invoiceId, ct);
        if (invoice is null || !IsOverdue(invoice)) return false;

        var now = _clock.UtcNow;
        var last = await db.InvoiceReminders
            .Where(r => r.InvoiceId == invoiceId)
            .OrderByDescending(r => r.SentUtc)
            .Select(r => (DateTimeOffset?)r.SentUtc)
            .FirstOrDefaultAsync(ct);
        if (last is { } previous && now - previous < ReminderInterval) return false;

        db.InvoiceReminders.Add(new InvoiceReminder { InvoiceId = invoiceId, SentUtc = now });
        await db.SaveChangesAsync(ct);
        return true;
    }
}
