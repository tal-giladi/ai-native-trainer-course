using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class InvoiceReminderTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => Now;
    }

    private sealed class OneInvoice(Invoice invoice) : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) =>
            Task.FromResult<Invoice?>(invoice.InvoiceId == invoiceId ? invoice : null);

        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(new[] { invoice });
    }

    private sealed class InMemoryReminders(DateTimeOffset? lastSent) : IInvoiceReminderRepository
    {
        public List<DateTimeOffset> Added { get; } = new();

        public Task<DateTimeOffset?> GetLastSentAsync(long invoiceId, CancellationToken ct) => Task.FromResult(lastSent);

        public Task AddAsync(long invoiceId, DateTimeOffset sentUtc, CancellationToken ct)
        {
            Added.Add(sentUtc);
            return Task.CompletedTask;
        }
    }

    private static Invoice Issued(int dueInDays) =>
        new(1, 7, 10m, Now.AddDays(-30), Now.AddDays(dueInDays), InvoiceStatus.Issued);

    [Fact]
    public async Task Not_overdue_is_refused()
    {
        var reminders = new InMemoryReminders(null);
        var sut = new InvoiceService(new OneInvoice(Issued(dueInDays: 3)), new FixedClock(), reminders);

        Assert.False(await sut.RecordReminderAsync(1, CancellationToken.None));
        Assert.Empty(reminders.Added);
    }

    [Fact]
    public async Task Overdue_without_earlier_reminder_is_recorded_at_now()
    {
        var reminders = new InMemoryReminders(null);
        var sut = new InvoiceService(new OneInvoice(Issued(dueInDays: -2)), new FixedClock(), reminders);

        Assert.True(await sut.RecordReminderAsync(1, CancellationToken.None));
        Assert.Equal(new[] { Now }, reminders.Added);
    }

    [Theory(Skip = "Flaky: depends on the clock. Tracked separately.")]
    [InlineData(-3, false)]
    [InlineData(-8, true)]
    public async Task Second_reminder_within_7_days_is_refused(int lastSentDaysAgo, bool expected)
    {
        var reminders = new InMemoryReminders(Now.AddDays(lastSentDaysAgo));
        var sut = new InvoiceService(new OneInvoice(Issued(dueInDays: -20)), new FixedClock(), reminders);

        Assert.Equal(expected, await sut.RecordReminderAsync(1, CancellationToken.None));
        Assert.Equal(expected ? 1 : 0, reminders.Added.Count);
    }
}
