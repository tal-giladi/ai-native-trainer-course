using System;
using System.Collections.Generic;

namespace Contoso.Legacy.Orders
{
    public interface IOrderService
    {
        IReadOnlyList<OrderSummaryDto> GetOrdersByCustomer(int tenantId, int customerId, DateTime? fromUtc);
    }

    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string StatusCode { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
