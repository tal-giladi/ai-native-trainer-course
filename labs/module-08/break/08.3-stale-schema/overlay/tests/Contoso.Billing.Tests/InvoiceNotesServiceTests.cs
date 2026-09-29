using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class InvoiceNotesServiceTests
{
    private sealed class FakeNotes(params InvoiceNoteView[] rows) : IInvoiceNoteRepository
    {
        public Task<IReadOnlyList<InvoiceNoteView>> LatestByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<InvoiceNoteView>>(rows);
    }

    [Fact]
    public async Task Returns_notes_and_skips_blank_ones()
    {
        var sut = new InvoiceNotesService(new FakeNotes(
            new InvoiceNoteView(1, "Call back Monday", null, null),
            new InvoiceNoteView(2, " ", null, null)));

        var result = await sut.ForCollectionsAsync(1001, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(1, result[0].InvoiceId);
    }
}
