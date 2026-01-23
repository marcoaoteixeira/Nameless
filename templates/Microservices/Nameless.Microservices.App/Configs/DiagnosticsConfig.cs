using Nameless.Diagnostics;

namespace Nameless.Microservices.App.Configs;

public static class DiagnosticsConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureDiagnostics() {
            self.Services.RegisterActivitySourceProvider();

            return self;
        }
    }
}