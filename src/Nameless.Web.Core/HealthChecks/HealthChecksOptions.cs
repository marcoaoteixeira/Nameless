using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     Options for health checks.
/// </summary>
public sealed record HealthChecksOptions {
    private readonly HashSet<Action<IHealthChecksBuilder>> _healthChecks = [];

    /// <summary>
    ///     Gets the registered health checks.
    /// </summary>
    public IEnumerable<Action<IHealthChecksBuilder>> HealthChecks => _healthChecks;

    /// <summary>
    ///     Registers a health check with the specified options.
    /// </summary>
    /// <typeparam name="THealthCheck">Type of the health check.</typeparam>
    /// <param name="name">The name of the health check.</param>
    /// <param name="failureStatus">The failure status.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="timeout">The timeout for the health check.</param>
    /// <returns>
    ///     The current <see cref="HealthChecksOptions"/> instance so other actions can be chained.
    /// </returns>
    public HealthChecksOptions RegisterHealthCheck<THealthCheck>(string? name = null,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
        where THealthCheck : class, IHealthCheck {

        _healthChecks.Add(builder => builder.AddCheck<THealthCheck>(
            name ?? typeof(THealthCheck).Name,
            failureStatus,
            tags,
            timeout));

        return this;
    }
}