using System.Data;
using Dapper;

namespace Contoso.Billing.Invoices;

// BILL-161. Notes live in dbo.InvoiceNote since V006 (BILL-158); the procedure returns the latest note
// of each issued invoice of a customer, with author and time.
public sealed record InvoiceNoteView(long InvoiceId, string Body, string AuthorUpn, DateTimeOffset CreatedUtc);

public interface IInvoiceNoteRepository
{
    Task<IReadOnlyList<InvoiceNoteView>> LatestByCustomerAsync(int customerId, CancellationToken ct);
}

public sealed class InvoiceNoteRepository : IInvoiceNoteRepository
{
    private readonly Func<IDbConnection> _connectionFactory;

    public InvoiceNoteRepository(Func<IDbConnection> connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<InvoiceNoteView>> LatestByCustomerAsync(int customerId, CancellationToken ct)
    {
        using var connection = _connectionFactory();
        var rows = await connection.QueryAsync<InvoiceNoteView>(new CommandDefinition(
            "dbo.usp_InvoiceNote_LatestByCustomer",
            new { customerId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct));
        return rows.AsList();
    }
}

public sealed class InvoiceNotesService(IInvoiceNoteRepository notes)
{
    /// <summary>Latest note per open invoice, most recent first; invoices without a note are not returned.</summary>
    public async Task<IReadOnlyList<InvoiceNoteView>> ForCollectionsAsync(int customerId, CancellationToken ct) =>
        (await notes.LatestByCustomerAsync(customerId, ct))
            .Where(n => !string.IsNullOrWhiteSpace(n.Body))
            .OrderByDescending(n => n.CreatedUtc)
            .ToList();
}
