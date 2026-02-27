using Microsoft.AspNetCore.Builder;
using Nameless.Web.Observability;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures OpenTelemetry feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder OpenTelemetryConfig(WebAppSettings settings) {
            if (settings.DisableOpenTelemetry) { return self; }

            self.RegisterOpenTelemetry(settings.ConfigureOpenTelemetry ?? (_ => { }));

            return self;
        }
    }
}
