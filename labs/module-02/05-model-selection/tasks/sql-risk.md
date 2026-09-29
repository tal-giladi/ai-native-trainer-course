This stored procedure backs a screen that shows a customer's orders in a multi-tenant SQL Server database.
Which single line carries the biggest correctness risk under concurrent writes? Quote the line, then explain in at most two sentences.

```sql
CREATE OR ALTER PROCEDURE dbo.usp_GetOrdersByCustomer
    @TenantId INT, @CustomerId INT, @FromUtc DATETIME2(3) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.OrderId, o.OrderNumber, o.CreatedUtc, o.StatusCode,
           SUM(ol.Quantity * ol.UnitPrice) AS TotalAmount
    FROM   dbo.Orders AS o WITH (NOLOCK)
    JOIN   dbo.OrderLines AS ol ON ol.OrderId = o.OrderId
    WHERE  o.TenantId = @TenantId AND o.CustomerId = @CustomerId AND o.IsDeleted = 0
      AND  (@FromUtc IS NULL OR o.CreatedUtc >= @FromUtc)
    GROUP BY o.OrderId, o.OrderNumber, o.CreatedUtc, o.StatusCode;
END
```
