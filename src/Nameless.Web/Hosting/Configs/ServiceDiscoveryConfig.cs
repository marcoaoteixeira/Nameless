using Microsoft.AspNetCore.Builder;
using Nameless.Web.ServiceDiscovery;

namespace Nameless.Web.Hosting.Configs;

public static class ServiceDiscoveryConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures ServiceDiscover feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureServiceDiscovery(WebHostSettings settings) {
            if (settings.DisableServiceDiscovery) { return self; }

            self.Services.RegisterServiceDiscovery(
                settings.ServiceDiscoveryRegistrationConfiguration
            );

            return self;
        }
    }
}
