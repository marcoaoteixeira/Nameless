using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     <see cref="IHostApplicationBuilder"/> extension methods for registering health checks.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers health checks for the application.
    /// </summary>
    /// <typeparam name="TApplicationBuilder">Type of the application builder.</typeparam>
    /// <param name="self">The current <see cref="TApplicationBuilder"/>.</param>
    /// <param name="configure">The options configure action.</param>
    /// <returns>
    ///     The current <see cref="TApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    public static TApplicationBuilder RegisterHealthChecks<TApplicationBuilder>(this TApplicationBuilder self, Action<HealthChecksOptions>? configure = null)
        where TApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new HealthChecksOptions();

        innerConfigure(options);

        // Adding health checks endpoints to applications in non-development
        var healthChecksBuilder = self.Services
                                      .AddHealthChecks()
                                      // Add a default liveness check to ensure app is responsive
                                      .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        // Add other health checks.
        foreach (var healthCheck in options.HealthChecks) {
            healthCheck(healthChecksBuilder);
        }

        return self;
    }
}
