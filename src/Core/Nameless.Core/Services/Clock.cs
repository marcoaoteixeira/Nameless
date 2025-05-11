namespace Nameless.Services;

/// <summary>
/// Default implementation of <see cref="IClock"/>
/// </summary>
public sealed class Clock : IClock {
    /// <inheritdoc />
    public DateTime GetUtcNow()
        => DateTime.UtcNow;

    /// <inheritdoc />
    public DateTimeOffset GetUtcNowOffset()
        => new(new DateTime(ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                            kind: DateTimeKind.Utc));
}