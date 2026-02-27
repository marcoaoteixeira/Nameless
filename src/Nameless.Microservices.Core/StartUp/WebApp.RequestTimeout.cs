using Microsoft.AspNetCore.Builder;
using Nameless.Web.RequestTimeouts;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures RequestTimeouts feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RequestTimeoutsConfig(WebAppSettings settings) {
            if (settings.DisableRequestTimeouts) { return self; }

            self.RegisterRequestTimeouts();

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the request timeouts service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseRequestTimeouts(WebAppSettings settings) {
            if (settings.DisableRequestTimeouts) { return self; }

            self.UseRequestTimeouts();

            return self;
        }
    }
}