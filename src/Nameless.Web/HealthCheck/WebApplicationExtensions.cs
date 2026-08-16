using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.HealthCheck.Reporting;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     <see cref="WebApplication"/> extension methods for configure
///     health checks endpoints.
/// </summary>
public static class WebApplicationExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplication"/>.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses health checks endpoints in the application.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplication"/> instance so other
        ///     actions can be chained.
        /// </returns>
        /// <remarks>
        ///     Call this method after <c>UseRouting</c>.
        /// </remarks>
        public WebApplication UseHealthCheck() {
            var healthChecks = self.MapGroup(string.Empty);
            var config = self.Configuration.GetOptions<HealthCheckConfiguration>();

            // Basic action to ensure security of the health check endpoints.
            healthChecks
                .WithRequestTimeout(HealthCheckConfiguration.TimeoutPolicyName)
                .CacheOutput(HealthCheckConfiguration.TimeoutPolicyName);

            // Liveness: no checks at all, just "process is alive" (cheap, safe)
            healthChecks.MapHealthChecks(
                pattern: config.LivenessPath,
                options: new HealthCheckOptions {
                    Predicate = registration => registration.Tags.Contains("alive"),
                    ResponseWriter = JsonReportWriter.WriteAsync
                }
            );

            // Readiness: real dependency checks, tagged
            healthChecks.MapHealthChecks(
                pattern: config.ReadinessPath,
                options: new HealthCheckOptions {
                    Predicate = registration => registration.Tags.Overlaps(config.ReadinessTags),
                    ResponseWriter = JsonReportWriter.WriteAsync
                }
            );

            return self;
        }
    }
}