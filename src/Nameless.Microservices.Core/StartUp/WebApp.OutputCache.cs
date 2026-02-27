using Microsoft.AspNetCore.Builder;
using Nameless.Web.OutputCache;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures OutputCache feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder OutputCacheConfig(WebAppSettings settings) {
            if (settings.DisableOutputCache) { return self; }

            self.RegisterOutputCache(self.Configuration);

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
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseOutputCache(WebAppSettings settings) {
            if (settings.DisableOutputCache) { return self; }

            self.UseOutputCache();

            return self;
        }
    }
}