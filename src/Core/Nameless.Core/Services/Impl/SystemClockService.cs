namespace Nameless.Services.Impl;

/// <summary>
/// Singleton Pattern implementation for <see cref="ISystemClock" />.
/// See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class SystemClock : ISystemClock {
    /// <summary>
    /// Gets the unique instance of <see cref="SystemClock" />.
    /// </summary>
    public static ISystemClock Instance { get; } = new SystemClock();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static SystemClock() { }

    private SystemClock() { }

    public DateTime GetUtcNow()
        => DateTime.UtcNow;

    public DateTimeOffset GetUtcNowOffset()
        => new(new DateTime(ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                            kind: DateTimeKind.Utc));
}