using Microsoft.AspNetCore.Builder;
using Nameless.Web.RateLimiter;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures RateLimiter feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RateLimiterConfig(WebAppSettings settings) {
            if (settings.DisableRateLimiter) { return self; }

            self.RegisterRateLimiter(settings.ConfigureRateLimiter ?? DefaultConfiguration);

            return self;

            void DefaultConfiguration(RateLimiterRegistrationSettings opts) {
                opts.IncludeAssemblies(settings.Assemblies);
            }
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the rate limiter service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseRateLimiter(WebAppSettings settings) {
            if (settings.DisableRateLimiter) { return self; }

            self.UseRateLimiter();

            return self;
        }
    }
}