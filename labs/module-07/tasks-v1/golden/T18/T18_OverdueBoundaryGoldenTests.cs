// Golden test for task T18 (BILL-152). Copied into the working copy by EvalHarness after the agent
// has finished; the agent never sees it. Namespace and class names start with Golden.T18 so the
// harness can run exactly these tests with --filter.
using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests.Golden;

public class T18_OverdueBoundaryGoldenTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock { public DateTimeOffset UtcNow => Now; }

    private sealed class NoInvoices : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) => Task.FromResult<Invoice?>(null);
        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(Array.Empty<Invoice>());
    }

    private static bool Overdue(InvoiceStatus status, TimeSpan dueFromNow) =>
        new InvoiceService(new NoInvoices(), new FixedClock())
            .IsOverdue(new Invoice(1, 7, 10m, Now.AddDays(-30), Now + dueFromNow, status));

    [Fact] public void Due_exactly_now_is_overdue() => Assert.True(Overdue(InvoiceStatus.Issued, TimeSpan.Zero));
    [Fact] public void Due_one_second_ago_is_overdue() => Assert.True(Overdue(InvoiceStatus.Issued, TimeSpan.FromSeconds(-1)));
    [Fact] public void Due_in_one_second_is_not_overdue() => Assert.False(Overdue(InvoiceStatus.Issued, TimeSpan.FromSeconds(1)));
    [Fact] public void Paid_invoice_due_now_is_not_overdue() => Assert.False(Overdue(InvoiceStatus.Paid, TimeSpan.Zero));
}
