using Microsoft.AspNetCore.Builder;
using Nameless.Web.OutputCache;

namespace Nameless.Web.Hosting.Configs;

public static class OutputCacheConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures OutputCache feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureOutputCache(WebHostSettings settings) {
            if (settings.DisableOutputCache) { return self; }

            self.Services.RegisterOutputCache(
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
        ///     Uses the output cache service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseOutputCache(WebHostSettings settings) {
            if (settings.DisableOutputCache) { return self; }

            self.UseOutputCache();

            return self;
        }
    }
}