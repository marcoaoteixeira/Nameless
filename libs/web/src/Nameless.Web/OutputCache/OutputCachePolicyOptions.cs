using Nameless.Configuration;

namespace Nameless.Web.OutputCache;

/// <summary>
///     Represents a policy that defines how long a cache is allowed
///     live.
/// </summary>
[ConfigurationSectionName("OutputCachePolicies")]
public record OutputCachePolicyOptions {
    /// <summary>
    ///     Gets or sets the duration.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    ///     Whether it should skip this policy.
    /// </summary>
    public bool Skip { get; init; }
}