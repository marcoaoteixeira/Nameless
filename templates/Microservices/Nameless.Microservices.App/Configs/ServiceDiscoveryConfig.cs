using Nameless.Web.Discoverability;

namespace Nameless.Microservices.App.Configs;

public static class ServiceDiscoveryConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureServiceDiscovery() {
            self.RegisterDiscoverability();

            return self;
        }
    }
}