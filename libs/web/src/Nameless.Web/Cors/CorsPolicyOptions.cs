using Nameless.Configuration;

namespace Nameless.Web.Cors;

/// <summary>
///     Represents a CORS policy.
/// </summary>
[ConfigurationSectionName("CorsPolicies")]
public record CorsPolicyOptions {
    /// <summary>
    ///     Gets or sets the CORS policy headers.
    /// </summary>
    public string? Headers { get; init; }
    /// <summary>
    ///     Gets or sets the CORS policy methods.
    /// </summary>
    public string? Methods { get; init; }
    /// <summary>
    ///     Gets or sets the CORS policy origins.
    /// </summary>
    public string? Origins { get; init; }
    /// <summary>
    ///     Whether it supports credentials.
    /// </summary>
    public bool SupportsCredentials { get; init; }
    /// <summary>
    ///     Gets or sets the CORS policy preflight maximum age.
    /// </summary>
    public TimeSpan? PreflightMaxAge { get; init; }
    /// <summary>
    ///     Whether the policy should be skipped.
    /// </summary>
    public bool Skip { get; init; }
}