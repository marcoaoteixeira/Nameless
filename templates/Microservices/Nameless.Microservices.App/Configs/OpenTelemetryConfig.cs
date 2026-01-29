using Nameless.Bootstrap;
using Nameless.Web.Observability;

namespace Nameless.Microservices.App.Configs;

public static class OpenTelemetryConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureOpenTelemetry() {
            self.RegisterOpenTelemetry();

            return self;
        }
    }
}