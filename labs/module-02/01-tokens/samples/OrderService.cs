using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Contoso.Legacy.Orders
{
    /// <summary>
    /// Order lookups for the back-office screens. Do NOT switch to EF here:
    /// usp_GetOrdersByCustomer applies the tenant filter and soft-delete rules.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IDbConnectionFactory connectionFactory, ILogger<OrderService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IReadOnlyList<OrderSummaryDto> GetOrdersByCustomer(int tenantId, int customerId, DateTime? fromUtc)
        {
            using (IDbConnection conn = _connectionFactory.Create())
            {
                var rows = conn.Query<OrderSummaryDto>(
                    "dbo.usp_GetOrdersByCustomer",
                    new { TenantId = tenantId, CustomerId = customerId, FromUtc = fromUtc },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 30).AsList();

                _logger.LogInformation("Loaded {Count} orders for customer {CustomerId}", rows.Count, customerId);
                return rows;
            }
        }
    }
}
