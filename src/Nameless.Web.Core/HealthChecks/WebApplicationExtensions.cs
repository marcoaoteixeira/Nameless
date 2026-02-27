using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Web.HealthChecks.Reporting;
using MS_HealthCheckOptions = Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions;

namespace Nameless.Web.HealthChecks;

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
        public WebApplication UseHealthChecks() {
            // Adding health checks endpoints to applications in non-development
            // environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before
            // enabling these endpoints in non-development environments.
            if (!self.Environment.IsDevelopment()) {
                return self;
            }

            // All health checks must pass for app to be considered ready
            // to accept traffic after starting
            self.MapHealthChecks(
                pattern: "/health",
                new MS_HealthCheckOptions {
                    ResponseWriter = JsonReportWriter.WriteAsync
                });

            // Only health checks tagged with the "live" tag must pass for
            // app to be considered alive
            self.MapHealthChecks(
                pattern: "/alive",
                new MS_HealthCheckOptions {
                    Predicate = registration => registration.Tags.Contains("live"),
                    ResponseWriter = JsonReportWriter.WriteAsync
                });

            return self;
        }
    }
}