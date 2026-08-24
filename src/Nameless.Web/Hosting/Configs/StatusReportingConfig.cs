using Microsoft.AspNetCore.Builder;
using Nameless.Reporting;

namespace Nameless.Web.Hosting.Configs;

/// <summary>
///     Configure reporting services.
/// </summary>
public static class StatusReportingConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures reporting feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureStatusReporting(WebHostSettings settings) {
            if (settings.DisableStatusReporting) { return self; }

            self.Services.RegisterStatusReporting(
                settings.ConfigureStatusReporting
            );

            return self;
        }
    }
}
