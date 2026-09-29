using System.Data;
using Dapper;

namespace Contoso.Billing.Invoices;

// BILL-150. Same pattern as InvoiceRepository (ADR 0007): Dapper, parameterized SQL,
// connection from a factory, CancellationToken on every async method.
public sealed class InvoiceReminderRepository : IInvoiceReminderRepository
{
    private readonly Func<IDbConnection> _connectionFactory;

    public InvoiceReminderRepository(Func<IDbConnection> connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<DateTimeOffset?> GetLastSentAsync(long invoiceId, CancellationToken ct)
    {
        using var connection = _connectionFactory();
        return await connection.QuerySingleAsync<DateTimeOffset?>(new CommandDefinition(
            "SELECT MAX(SentUtc) FROM dbo.InvoiceReminder WHERE InvoiceId = @invoiceId",
            new { invoiceId },
            cancellationToken: ct));
    }

    public async Task AddAsync(long invoiceId, DateTimeOffset sentUtc, CancellationToken ct)
    {
        using var connection = _connectionFactory();
        await connection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO dbo.InvoiceReminder (InvoiceId, SentUtc) VALUES (@invoiceId, @sentUtc)",
            new { invoiceId, sentUtc },
            cancellationToken: ct));
    }
}
