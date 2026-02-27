namespace Nameless.Web.OutputCache;

/// <summary>
///     Represents a policy that defines how long a cache is allowed
///     live.
/// </summary>
/// <param name="Name">The name associated with the policy.</param>
/// <param name="Duration">The duration</param>
public record OutputCachePolicy(string Name, TimeSpan Duration) {
    public static OutputCachePolicy OneSecond { get; } = new(
        Name: Defaults.OutputCachePolicies.ONE_SECOND,
        Duration: TimeSpan.FromSeconds(1)
    );

    public static OutputCachePolicy FiveSeconds { get; } = new(
        Name: Defaults.OutputCachePolicies.FIVE_SECONDS,
        Duration: TimeSpan.FromSeconds(5)
    );

    public static OutputCachePolicy FifteenSeconds { get; } = new(
        Name: Defaults.OutputCachePolicies.FIFTEEN_SECONDS,
        Duration: TimeSpan.FromSeconds(15)
    );

    public static OutputCachePolicy ThirtySeconds { get; } = new(
        Name: Defaults.OutputCachePolicies.THIRTY_SECONDS,
        Duration: TimeSpan.FromSeconds(30)
    );

    public static OutputCachePolicy OneMinute { get; } = new(
        Name: Defaults.OutputCachePolicies.ONE_MINUTE,
        Duration: TimeSpan.FromMinutes(1)
    );
}