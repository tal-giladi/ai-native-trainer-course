using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class InvoiceNotesServiceTests
{
    private static readonly DateTimeOffset T0 = new(2026, 9, 20, 9, 0, 0, TimeSpan.Zero);

    private sealed class FakeNotes(params InvoiceNoteView[] rows) : IInvoiceNoteRepository
    {
        public Task<IReadOnlyList<InvoiceNoteView>> LatestByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<InvoiceNoteView>>(rows);
    }

    [Fact]
    public async Task No_notes_returns_empty()
    {
        var sut = new InvoiceNotesService(new FakeNotes());
        Assert.Empty(await sut.ForCollectionsAsync(1001, CancellationToken.None));
    }

    [Fact]
    public async Task Most_recent_first_with_author_and_time()
    {
        var sut = new InvoiceNotesService(new FakeNotes(
            new InvoiceNoteView(1, "Disputes July amount", "dana@contoso.example", T0),
            new InvoiceNoteView(2, "Promised payment Friday", "amir@contoso.example", T0.AddDays(3))));

        var result = await sut.ForCollectionsAsync(1001, CancellationToken.None);

        Assert.Equal([2L, 1L], result.Select(n => n.InvoiceId));
        Assert.Equal("amir@contoso.example", result[0].AuthorUpn);
    }
}
