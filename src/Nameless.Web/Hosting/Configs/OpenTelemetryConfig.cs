using Microsoft.AspNetCore.Builder;
using Nameless.Web.Observability;

namespace Nameless.Web.Hosting.Configs;

public static class OpenTelemetryConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures OpenTelemetry feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureOpenTelemetry(WebHostSettings settings) {
            if (settings.DisableOpenTelemetry) { return self; }

            self.RegisterOpenTelemetry(
                settings.OpenTelemetryRegistrationConfiguration
            );

            return self;
        }
    }
}
