using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Helpers;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers health checks for the application.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterHealthChecks(Action<HealthChecksRegistrationSettings> registration) {
            var setting = ActionHelper.FromDelegate(registration);

            // Adding health checks endpoints to applications in non-development
            var builder = self.Services
                              .AddHealthChecks()
                              // Add a default liveness check to ensure app is responsive
                              .AddCheck(
                                  name: "self",
                                  check: () => HealthCheckResult.Healthy(),
                                  tags: ["live"]
                              );

            // Add other health checks.
            foreach (var healthCheck in setting.HealthChecks) {
                healthCheck(builder);
            }

            return self;
        }
    }
}