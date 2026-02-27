using Microsoft.AspNetCore.Builder;
using Nameless.Web.Cors;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the CORS feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder CorsConfig(WebAppSettings settings) {
            return settings.DisableCors
                ? self
                : self.RegisterCors();
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
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseCors(WebAppSettings settings) {
            if (settings.DisableCors) { return self; }

            self.UseCors();

            return self;
        }
    }
}
