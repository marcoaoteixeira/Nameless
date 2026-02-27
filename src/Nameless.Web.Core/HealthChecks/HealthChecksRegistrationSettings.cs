using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Helpers;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     Options for health checks.
/// </summary>
public class HealthChecksRegistrationSettings {
    private readonly Dictionary<Type, Action<IHealthChecksBuilder>> _healthChecks = [];

    /// <summary>
    ///     Gets the registered health checks.
    /// </summary>
    internal IReadOnlyCollection<Action<IHealthChecksBuilder>> HealthChecks => _healthChecks.Values;

    /// <summary>
    ///     Registers a health check with the specified options.
    /// </summary>
    /// <typeparam name="THealthCheck">
    ///     Type of the health check.
    /// </typeparam>
    /// <param name="configure">
    ///     The configuration delegate.
    /// </param>
    /// <returns>
    ///     The current <see cref="HealthChecksRegistrationSettings"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public HealthChecksRegistrationSettings RegisterHealthCheck<THealthCheck>(Action<HealthCheckOptions>? configure = null)
        where THealthCheck : class, IHealthCheck {
        var info = ActionHelper.FromDelegate(configure);

        _healthChecks[typeof(THealthCheck)] = builder
            => builder.AddCheck<THealthCheck>(
                info.Name ?? typeof(THealthCheck).Name,
                info.FailureStatus,
                info.Tags,
                info.Timeout
            );

        return this;
    }
}