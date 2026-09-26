using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Helpers;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     Options for health checks.
/// </summary>
public class HealthCheckRegistration {
    private readonly Dictionary<Type, HealthCheckEntry> _healthChecks = [];

    /// <summary>
    ///     Gets the registered health checks.
    /// </summary>
    internal IReadOnlyDictionary<Type, HealthCheckEntry> HealthChecks => _healthChecks;

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
    public HealthCheckRegistration RegisterHealthCheck<THealthCheck>(Action<HealthCheckEntry>? configure = null)
        where THealthCheck : class, IHealthCheck {
        var opts = ActionHelper.FromDelegate(configure);

        _healthChecks[typeof(THealthCheck)] = opts;

        return this;
    }
}