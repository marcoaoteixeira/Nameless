namespace Nameless.Services;

public sealed class SystemClock : ISystemClock {
    public DateTime GetUtcNow()
        => DateTime.UtcNow;

    public DateTimeOffset GetUtcNowOffset()
        => new(new DateTime(ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                            kind: DateTimeKind.Utc));
}