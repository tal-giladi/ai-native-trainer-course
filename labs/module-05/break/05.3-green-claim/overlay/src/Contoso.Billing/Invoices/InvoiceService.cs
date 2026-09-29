using Contoso.Billing.Common;

namespace Contoso.Billing.Invoices;

public sealed class InvoiceService
{
    /// <summary>BILL-150: at most one reminder per invoice per 7 days.</summary>
    public static readonly TimeSpan ReminderInterval = TimeSpan.FromDays(7);

    private readonly IInvoiceRepository _invoices;
    private readonly IClock _clock;
    private readonly IInvoiceReminderRepository? _reminders;

    // The reminder repository is optional so existing callers and tests keep compiling (BILL-150 plan, Approach).
    public InvoiceService(IInvoiceRepository invoices, IClock clock, IInvoiceReminderRepository? reminders = null)
    {
        _invoices = invoices;
        _clock = clock;
        _reminders = reminders;
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
        var reminders = _reminders ?? throw new InvalidOperationException("InvoiceService was created without a reminder repository.");
        var invoice = await _invoices.GetAsync(invoiceId, ct);
        if (invoice is null || !IsOverdue(invoice)) return false;

        var now = _clock.UtcNow;
        var last = await reminders.GetLastSentAsync(invoiceId, ct);
        if (last is { } previous && previous > now.Add(ReminderInterval)) return false;

        await reminders.AddAsync(invoiceId, now, ct);
        return true;
    }
}
