using System.Data;
using Dapper;

namespace Contoso.Billing.Invoices;

// Pattern since ADR 0007 (2024): Dapper, parameterized SQL, one repository per aggregate,
// the connection comes from a factory so tests can substitute it.
public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly Func<IDbConnection> _connectionFactory;

    public InvoiceRepository(Func<IDbConnection> connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct)
    {
        using var connection = _connectionFactory();
        return await connection.QuerySingleOrDefaultAsync<Invoice>(new CommandDefinition(
            "SELECT InvoiceId, CustomerId, Amount, IssuedUtc, DueUtc, Status FROM dbo.Invoice WHERE InvoiceId = @invoiceId",
            new { invoiceId },
            cancellationToken: ct));
    }

    public async Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct)
    {
        using var connection = _connectionFactory();
        var rows = await connection.QueryAsync<Invoice>(new CommandDefinition(
            "SELECT InvoiceId, CustomerId, Amount, IssuedUtc, DueUtc, Status FROM dbo.Invoice WHERE CustomerId = @customerId",
            new { customerId },
            cancellationToken: ct));
        return rows.AsList();
    }
}
