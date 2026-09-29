namespace Contoso.Billing.Invoices;

public sealed class InvoiceReminder
{
    public long InvoiceReminderId { get; set; }
    public long InvoiceId { get; set; }
    public DateTimeOffset SentUtc { get; set; }
}
