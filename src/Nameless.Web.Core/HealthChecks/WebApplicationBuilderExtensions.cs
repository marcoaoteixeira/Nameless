using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods for registering health checks.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <summary>
    ///     Registers health checks for the application.
    /// </summary>
    /// <param name="self">The current <see cref="WebApplicationBuilder"/>.</param>
    /// <param name="configure">The options configure action.</param>
    /// <returns>
    ///     The current <see cref="WebApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    public static WebApplicationBuilder RegisterHealthChecks(this WebApplicationBuilder self, Action<HealthChecksOptions>? configure = null) {
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
