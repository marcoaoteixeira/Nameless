using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers health checks for the application.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type that implements <see cref="IHostApplicationBuilder"/>.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">The options configure action.</param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/>
    ///     instance so other actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterHealthChecks<THostApplicationBuilder>(
        this THostApplicationBuilder self, Action<HealthChecksOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new HealthChecksOptions();

        innerConfigure(options);

        // Adding health checks endpoints to applications in non-development
        var healthChecksBuilder = self.Services
                                      .AddHealthChecks()
                                      // Add a default liveness check to ensure app is responsive
                                      .AddCheck(name: "self", () => HealthCheckResult.Healthy(), ["live"]);

        // Add other health checks.
        foreach (var healthCheck in options.HealthChecks) {
            healthCheck(healthChecksBuilder);
        }

        return self;
    }
}