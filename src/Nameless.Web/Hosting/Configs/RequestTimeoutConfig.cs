using Microsoft.AspNetCore.Builder;
using Nameless.Web.RequestTimeout;

namespace Nameless.Web.Hosting.Configs;

public static class RequestTimeoutConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures request timeouts feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureRequestTimeout(WebHostSettings settings) {
            if (settings.DisableRequestTimeouts) { return self; }

            self.Services.RegisterRequestTimeout(
                self.Configuration
            );

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
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseRequestTimeout(WebHostSettings settings) {
            if (settings.DisableRequestTimeouts) { return self; }

            self.UseRequestTimeouts();

            return self;
        }
    }
}