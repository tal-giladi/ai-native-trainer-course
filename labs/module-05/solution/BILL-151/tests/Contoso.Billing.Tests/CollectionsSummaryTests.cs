using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class CollectionsSummaryTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => Now;
    }

    private sealed class Invoices(params Invoice[] rows) : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) =>
            Task.FromResult(rows.FirstOrDefault(r => r.InvoiceId == invoiceId));

        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(rows.Where(r => r.CustomerId == customerId).ToList());
    }

    private static Invoice Inv(long id, decimal amount, InvoiceStatus status, int dueInDays) =>
        new(id, 7, amount, Now.AddDays(-30), Now.AddDays(dueInDays), status);

    private static Task<CollectionsSummary> Summary(params Invoice[] rows) =>
        new InvoiceService(new Invoices(rows), new FixedClock()).CollectionsSummaryAsync(7, CancellationToken.None);

    [Fact]
    public async Task No_invoices_gives_zero() =>
        Assert.Equal(new CollectionsSummary(0m, 0m), await Summary());

    [Fact]
    public async Task One_overdue_and_one_not_yet_due() =>
        Assert.Equal(new CollectionsSummary(140m, 100m), await Summary(
            Inv(1, 100m, InvoiceStatus.Issued, -3),
            Inv(2, 40m, InvoiceStatus.Issued, 10)));

    [Fact]
    public async Task Paid_void_and_draft_invoices_are_not_owed() =>
        Assert.Equal(new CollectionsSummary(0m, 0m), await Summary(
            Inv(1, 100m, InvoiceStatus.Paid, -3),
            Inv(2, 50m, InvoiceStatus.Void, -3),
            Inv(3, 25m, InvoiceStatus.Draft, -3)));
}
