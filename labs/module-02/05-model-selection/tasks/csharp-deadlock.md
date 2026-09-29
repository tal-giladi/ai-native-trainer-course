This ASP.NET (classic, .NET Framework 4.8) controller action intermittently hangs under load.
Rewrite ONLY this method so it cannot deadlock. Reply with the rewritten C# method and nothing else.

```csharp
public ActionResult CustomerOrders(int customerId)
{
    var orders = _orderApiClient.GetOrdersAsync(customerId).Result;
    return View(orders);
}
```
