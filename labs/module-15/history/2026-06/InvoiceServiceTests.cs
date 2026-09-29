using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class InvoiceServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 6, 12, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => Now;
    }

    private sealed class NoInvoices : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) => Task.FromResult<Invoice?>(null);
    }

    private static Invoice Inv(InvoiceStatus status, int dueInDays) =>
        new(1, 7, 10m, Now.AddDays(-30), Now.AddDays(dueInDays), status);

    [Theory]
    [InlineData(InvoiceStatus.Issued, -1, true)]
    [InlineData(InvoiceStatus.Issued, 1, false)]
    [InlineData(InvoiceStatus.Paid, -1, false)]
    public void IsOverdue_uses_the_injected_clock(InvoiceStatus status, int dueInDays, bool expected)
    {
        var sut = new InvoiceService(new NoInvoices(), new FixedClock());
        Assert.Equal(expected, sut.IsOverdue(Inv(status, dueInDays)));
    }
}
