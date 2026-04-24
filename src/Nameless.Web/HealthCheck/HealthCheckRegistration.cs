using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Helpers;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     Options for health checks.
/// </summary>
public class HealthCheckRegistration {
    private readonly Dictionary<Type, HealthCheckOptions> _healthChecks = [];

    /// <summary>
    ///     Gets the registered health checks.
    /// </summary>
    internal IReadOnlyDictionary<Type, HealthCheckOptions> HealthChecks => _healthChecks;

    /// <summary>
    ///     Registers a health check with the specified options.
    /// </summary>
    /// <typeparam name="THealthCheck">
    ///     Type of the health check.
    /// </typeparam>
    /// <param name="configure">
    ///     The delegate to configure the health check.
    /// </param>
    /// <returns>
    ///     The current <see cref="HealthCheckRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public HealthCheckRegistration RegisterHealthCheck<THealthCheck>(Action<HealthCheckOptions>? configure = null)
        where THealthCheck : class, IHealthCheck {
        var opts = ActionHelper.FromDelegate(configure);

        _healthChecks[typeof(THealthCheck)] = opts;

        return this;
    }
}

/// <summary>
///     Represents a health check registration options.
/// </summary>
public record HealthCheckOptions {
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