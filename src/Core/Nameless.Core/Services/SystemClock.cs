namespace Nameless.Services;

public sealed class SystemClock : IClock {
    public DateTime GetUtcNow()
        => DateTime.UtcNow;

    public DateTimeOffset GetUtcNowOffset()
        => new(new DateTime(ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                            kind: DateTimeKind.Utc));
}