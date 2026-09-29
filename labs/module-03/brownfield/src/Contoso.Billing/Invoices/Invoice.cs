namespace Contoso.Billing.Invoices;

public enum InvoiceStatus : byte
{
    Draft = 0,
    Issued = 1,
    Paid = 2,
    Void = 3
}

/// <summary>Money is decimal(19,4) in SQL Server and decimal in C#. Never double.</summary>
public sealed record Invoice(
    long InvoiceId,
    int CustomerId,
    decimal Amount,
    DateTimeOffset IssuedUtc,
    DateTimeOffset DueUtc,
    InvoiceStatus Status);
