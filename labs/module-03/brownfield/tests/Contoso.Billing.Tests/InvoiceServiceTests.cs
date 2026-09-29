using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class InvoiceServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => Now;
    }

    private sealed class InMemoryInvoices(params Invoice[] rows) : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) =>
            Task.FromResult(rows.FirstOrDefault(r => r.InvoiceId == invoiceId));

        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(rows.Where(r => r.CustomerId == customerId).ToList());
    }

    private static Invoice Inv(long id, decimal amount, InvoiceStatus status, int dueInDays) =>
        new(id, 7, amount, Now.AddDays(-30), Now.AddDays(dueInDays), status);

    [Fact]
    public async Task Outstanding_counts_only_issued_invoices()
    {
        var repo = new InMemoryInvoices(
            Inv(1, 100.10m, InvoiceStatus.Issued, 5),
            Inv(2, 50.00m, InvoiceStatus.Paid, -5),
            Inv(3, 20.00m, InvoiceStatus.Void, -5),
            Inv(4, 0.20m, InvoiceStatus.Issued, -1));
        var sut = new InvoiceService(repo, new FixedClock());

        Assert.Equal(100.30m, await sut.OutstandingAsync(7, CancellationToken.None));
    }

    [Theory]
    [InlineData(InvoiceStatus.Issued, -1, true)]
    [InlineData(InvoiceStatus.Issued, 1, false)]
    [InlineData(InvoiceStatus.Paid, -1, false)]
    public void IsOverdue_uses_the_injected_clock(InvoiceStatus status, int dueInDays, bool expected)
    {
        var sut = new InvoiceService(new InMemoryInvoices(), new FixedClock());
        Assert.Equal(expected, sut.IsOverdue(Inv(1, 10m, status, dueInDays)));
    }
}
