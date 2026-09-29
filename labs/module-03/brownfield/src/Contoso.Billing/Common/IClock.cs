namespace Contoso.Billing.Common;

/// <summary>All "now" values come from here so tests can freeze time.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
