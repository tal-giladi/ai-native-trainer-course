// Golden test for task T20. Copied into the working copy by EvalHarness after the agent has
// finished; the agent never sees it. Uses the IsOverdue meaning of the unmodified repository.
using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests.Golden;

public class T20_DaysOverdueGoldenTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock { public DateTimeOffset UtcNow => Now; }

    private sealed class NoInvoices : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) => Task.FromResult<Invoice?>(null);
        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(Array.Empty<Invoice>());
    }

    private static int Days(InvoiceStatus status, TimeSpan dueFromNow) =>
        new InvoiceService(new NoInvoices(), new FixedClock())
            .DaysOverdue(new Invoice(1, 7, 10m, Now.AddDays(-60), Now + dueFromNow, status));

    [Fact] public void Three_days_and_five_hours_late_is_three() => Assert.Equal(3, Days(InvoiceStatus.Issued, -(TimeSpan.FromDays(3) + TimeSpan.FromHours(5))));
    [Fact] public void Twelve_hours_late_is_zero_whole_days() => Assert.Equal(0, Days(InvoiceStatus.Issued, TimeSpan.FromHours(-12)));
    [Fact] public void Not_yet_due_is_zero() => Assert.Equal(0, Days(InvoiceStatus.Issued, TimeSpan.FromDays(4)));
    [Fact] public void Paid_long_ago_due_is_zero() => Assert.Equal(0, Days(InvoiceStatus.Paid, TimeSpan.FromDays(-40)));
    [Fact] public void Offset_of_the_due_instant_does_not_matter() =>
        Assert.Equal(2, new InvoiceService(new NoInvoices(), new FixedClock()).DaysOverdue(
            new Invoice(1, 7, 10m, Now.AddDays(-60), (Now.AddDays(-2).AddHours(-1)).ToOffset(TimeSpan.FromHours(3)), InvoiceStatus.Issued)));
}
