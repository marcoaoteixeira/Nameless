using Microsoft.AspNetCore.Builder;
using Nameless.Web.RateLimiter;

namespace Nameless.Web.Hosting.Configs;

public static class RateLimiterConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures RateLimiter feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureRateLimiter(WebHostSettings settings) {
            if (settings.DisableRateLimiter) { return self; }

            self.Services.RegisterRateLimiter(
                WebHostSettingsHelper.Join(
                    settings.RateLimiterRegistrationConfiguration,
                    settings.Assemblies
                )
            );

            return self;
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
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseRateLimiter(WebHostSettings settings) {
            if (settings.DisableRateLimiter) { return self; }

            self.UseRateLimiter();

            return self;
        }
    }
}