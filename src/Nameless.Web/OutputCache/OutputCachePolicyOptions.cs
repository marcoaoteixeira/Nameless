using Nameless.Attributes;

namespace Nameless.Web.OutputCache;

/// <summary>
///     Represents a policy that defines how long a cache is allowed
///     live.
/// </summary>
[ConfigurationSectionName("OutputCachePolicies")]
public record OutputCachePolicyOptions {
    public TimeSpan Duration { get; init; }
    public bool Skip { get; init; }
}