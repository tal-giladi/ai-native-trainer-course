using Contoso.Billing.Collections;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

// Agent-written tests for the paste-and-go version of BILL-151. They pass.
// Note which invoice statuses they use, and where "now" comes from.
public class CollectionsSummaryServiceTests
{
    private sealed class Invoices(params Invoice[] rows) : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) =>
            Task.FromResult(rows.FirstOrDefault(r => r.InvoiceId == invoiceId));

        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(rows.Where(r => r.CustomerId == customerId).ToList());
    }

    private static Invoice Inv(long id, decimal amount, InvoiceStatus status, int dueInDays)
    {
        var now = DateTimeOffset.UtcNow;
        return new(id, 7, amount, now.AddDays(-30), now.AddDays(dueInDays), status);
    }

    [Fact]
    public async Task No_invoices_gives_zero()
    {
        var sut = new CollectionsSummaryService(new Invoices());
        var s = await sut.GetAsync(7, CancellationToken.None);
        Assert.Equal(0m, s.Owed);
        Assert.Equal(0m, s.Overdue);
    }

    [Fact]
    public async Task One_overdue_and_one_not_yet_due()
    {
        var sut = new CollectionsSummaryService(new Invoices(
            Inv(1, 100m, InvoiceStatus.Issued, -3),
            Inv(2, 40m, InvoiceStatus.Issued, 10)));
        var s = await sut.GetAsync(7, CancellationToken.None);
        Assert.Equal(140m, s.Owed);
        Assert.Equal(100m, s.Overdue);
    }

    [Fact]
    public async Task Paid_invoices_are_not_owed()
    {
        var sut = new CollectionsSummaryService(new Invoices(Inv(1, 100m, InvoiceStatus.Paid, -3)));
        var s = await sut.GetAsync(7, CancellationToken.None);
        Assert.Equal(0m, s.Owed);
    }
}
