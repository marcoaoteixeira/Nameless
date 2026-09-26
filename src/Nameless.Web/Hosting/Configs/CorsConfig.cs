using Microsoft.AspNetCore.Builder;
using Nameless.Web.Cors;

namespace Nameless.Web.Hosting.Configs;

public static class CorsConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the CORS feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureCors(WebHostSettings settings) {
            if (settings.DisableCors) { return self; }

            self.Services.RegisterCors(self.Configuration);

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the CORS service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseCors(WebHostSettings settings) {
            if (settings.DisableCors) { return self; }

            self.UseCors();

            return self;
        }
    }
}
