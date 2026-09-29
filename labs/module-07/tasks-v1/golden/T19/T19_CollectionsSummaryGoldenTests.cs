// Golden test for task T19 (BILL-151). Copied into the working copy by EvalHarness after the agent
// has finished; the agent never sees it.
using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests.Golden;

public class T19_CollectionsSummaryGoldenTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock { public DateTimeOffset UtcNow => Now; }

    private sealed class Rows(params Invoice[] rows) : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) =>
            Task.FromResult(rows.FirstOrDefault(r => r.InvoiceId == invoiceId));
        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(rows.Where(r => r.CustomerId == customerId).ToList());
    }

    private static Invoice Inv(long id, decimal amount, InvoiceStatus status, int dueInDays, int customer = 7) =>
        new(id, customer, amount, Now.AddDays(-30), Now.AddDays(dueInDays), status);

    private static Task<CollectionsSummary> Summary(params Invoice[] rows) =>
        new InvoiceService(new Rows(rows), new FixedClock()).CollectionsSummaryAsync(7, CancellationToken.None);

    [Fact]
    public async Task No_invoices() => Assert.Equal(new CollectionsSummary(0m, 0m), await Summary());

    [Fact]
    public async Task Owed_includes_not_yet_due_overdue_does_not() =>
        Assert.Equal(new CollectionsSummary(140.25m, 100.25m), await Summary(
            Inv(1, 100.25m, InvoiceStatus.Issued, -3),
            Inv(2, 40m, InvoiceStatus.Issued, 10)));

    [Fact]
    public async Task Paid_void_draft_and_other_customers_are_excluded() =>
        Assert.Equal(new CollectionsSummary(5m, 5m), await Summary(
            Inv(1, 100m, InvoiceStatus.Paid, -3),
            Inv(2, 50m, InvoiceStatus.Void, -3),
            Inv(3, 25m, InvoiceStatus.Draft, -3),
            Inv(4, 5m, InvoiceStatus.Issued, -1),
            Inv(5, 999m, InvoiceStatus.Issued, -1, customer: 8)));
}
