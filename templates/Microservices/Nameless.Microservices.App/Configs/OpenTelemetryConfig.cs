using Nameless.Bootstrap;
using Nameless.Web.Observability;
using OpenTelemetry.Resources;

namespace Nameless.Microservices.App.Configs;

public static class OpenTelemetryConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureOpenTelemetry() {
            self.RegisterOpenTelemetry(opts => {
                opts.ActivitySources = [BootstrapExecutor.ActivitySourceName];
                //opts.ConfigureResources = resourceBuilder => {
                    
                //    resourceBuilder.AddService(self.Environment.ApplicationName);
                //};
            });

            return self;
        }
    }
}