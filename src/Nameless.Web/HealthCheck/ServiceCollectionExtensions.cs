using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Helpers;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers health checks for the application.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterHealthCheck(Action<HealthCheckRegistration>? registration = null) {
            var setting = ActionHelper.FromDelegate(registration);

            // Adding health checks endpoints to applications in non-development
            var builder = self.AddHealthChecks()
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