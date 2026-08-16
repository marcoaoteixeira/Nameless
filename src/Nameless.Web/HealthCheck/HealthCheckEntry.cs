using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     Represents a health check registration options.
/// </summary>
public record HealthCheckEntry {
    /// <summary>
    ///     Gets the health check name, if not provided the health check type
    ///     name will be used.
    /// </summary>
    public string? Name { get; init; }
    /// <summary>
    ///     Gets the <see cref="HealthStatus"/> that should be reported when
    ///     the health check reports a failure. If the provided value is
    ///     <see langword="null"/>, then <see cref="HealthStatus.Unhealthy"/>
    ///     will be reported.
    /// </summary>
    public HealthStatus? FailureStatus { get; init; }
    /// <summary>
    ///     Gets the list of tags that can be used to filter health checks.
    /// </summary>
    public string[]? Tags { get; init; }
    /// <summary>
    ///     Gets the optional <see cref="TimeSpan"/> representing the timeout
    ///     of the check.
    /// </summary>
    public TimeSpan? Timeout { get; init; }
}