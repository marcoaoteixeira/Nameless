using Microsoft.AspNetCore.Builder;
using Nameless.Web.HealthChecks;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures health check endpoints for the application,
        ///     including a liveness probe.
        /// </summary>
        /// <remarks>
        ///     This method adds a health check named "self" that always
        ///     reports a healthy status and is tagged with "live" for use with
        ///     liveness probes. Additional health checks, such as database
        ///     connectivity checks, may also be registered within this
        ///     configuration.
        /// </remarks>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder HealthChecksConfig(WebAppSettings settings) {
            // Adds a health check named "self" that always reports a healthy status,
            // tagged with "live" for liveness probes.
            // This is just to check if the microservice still running.
            return settings.DisableHealthChecks
                ? self
                : self.RegisterHealthChecks(settings.ConfigureHealthChecks ?? (_ => { }));
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the health checks service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseHealthChecks(WebAppSettings settings) {
            if (settings.DisableHealthChecks) { return self; }

            self.UseHealthChecks();

            return self;
        }
    }
}
