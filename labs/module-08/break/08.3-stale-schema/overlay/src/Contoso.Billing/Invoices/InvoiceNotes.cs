using System.Data;
using Dapper;

namespace Contoso.Billing.Invoices;

// BILL-161. Written by the agent with the contoso-schema MCP server reading the nightly snapshot.
// "Author and time are not stored for invoice notes (dbo.Invoice has only Notes nvarchar(400) NULL),
//  so AuthorUpn and CreatedUtc are null until a follow-up ticket adds them."
public sealed record InvoiceNoteView(long InvoiceId, string Body, string? AuthorUpn, DateTimeOffset? CreatedUtc);

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
            "SELECT InvoiceId, Notes AS Body, NULL AS AuthorUpn, NULL AS CreatedUtc FROM dbo.Invoice " +
            "WHERE CustomerId = @customerId AND Status = 1 AND Notes IS NOT NULL",
            new { customerId },
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
