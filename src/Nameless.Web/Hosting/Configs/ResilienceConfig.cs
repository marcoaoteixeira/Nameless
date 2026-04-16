using Microsoft.AspNetCore.Builder;
using Nameless.Resilience;

namespace Nameless.Web.Hosting.Configs;

public static class ResilienceConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the Resilience feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureResilience(WebHostSettings settings) {
            if (settings.DisableResilience) { return self; }

            self.Services.RegisterRetryPipelineFactory();

            return self;
        }
    }
}
