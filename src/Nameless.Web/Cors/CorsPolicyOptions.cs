using Nameless.Attributes;

namespace Nameless.Web.Cors;

/// <summary>
///     Represents a CORS policy.
/// </summary>
[ConfigurationSectionName("CorsPolicies")]
public record CorsPolicyOptions {
    public string? Headers { get; init; }
    public string? Methods { get; init; }
    public string? Origins { get; init; }
    public bool SupportsCredentials { get; init; }
    public TimeSpan? PreflightMaxAge { get; init; }
    public bool Skip { get; init; }
}