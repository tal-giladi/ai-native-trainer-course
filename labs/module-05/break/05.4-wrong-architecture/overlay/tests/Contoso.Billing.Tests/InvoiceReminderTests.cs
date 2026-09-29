using Contoso.Billing.Common;
using Contoso.Billing.Data;
using Contoso.Billing.Invoices;
using Microsoft.EntityFrameworkCore;
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

    private static BillingDbContext Db(params DateTimeOffset[] sent)
    {
        var db = new BillingDbContext(new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        foreach (var s in sent) db.InvoiceReminders.Add(new InvoiceReminder { InvoiceId = 1, SentUtc = s });
        db.SaveChanges();
        return db;
    }

    private static Invoice Issued(int dueInDays) =>
        new(1, 7, 10m, Now.AddDays(-30), Now.AddDays(dueInDays), InvoiceStatus.Issued);

    [Fact]
    public async Task Not_overdue_is_refused()
    {
        using var db = Db();
        var sut = new InvoiceService(new OneInvoice(Issued(3)), new FixedClock(), db);
        Assert.False(await sut.RecordReminderAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task Overdue_without_earlier_reminder_is_recorded()
    {
        using var db = Db();
        var sut = new InvoiceService(new OneInvoice(Issued(-2)), new FixedClock(), db);
        Assert.True(await sut.RecordReminderAsync(1, CancellationToken.None));
        Assert.Equal(1, await db.InvoiceReminders.CountAsync());
    }

    [Theory]
    [InlineData(-3, false)]
    [InlineData(-8, true)]
    public async Task Second_reminder_within_7_days_is_refused(int lastSentDaysAgo, bool expected)
    {
        using var db = Db(Now.AddDays(lastSentDaysAgo));
        var sut = new InvoiceService(new OneInvoice(Issued(-20)), new FixedClock(), db);
        Assert.Equal(expected, await sut.RecordReminderAsync(1, CancellationToken.None));
    }
}
