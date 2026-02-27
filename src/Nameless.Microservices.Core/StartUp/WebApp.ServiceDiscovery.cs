using Microsoft.AspNetCore.Builder;
using Nameless.Web.ServiceDiscovery;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures ServiceDiscover feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ServiceDiscoveryConfig(WebAppSettings settings) {
            if (settings.DisableServiceDiscovery) { return self; }

            self.RegisterServiceDiscovery(settings.ConfigureServiceDiscovery ?? (_ => { }));

            return self;
        }
    }
}
