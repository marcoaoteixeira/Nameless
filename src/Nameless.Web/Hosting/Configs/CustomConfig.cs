using Microsoft.AspNetCore.Builder;

namespace Nameless.Web.Hosting.Configs;

public static class CustomConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureAdditionalServices(WebHostSettings settings) {
            settings.AdditionalServicesConfiguration?.Invoke(
                self.Services,
                self.Configuration,
                self.Environment
            );

            return self;
        }
    }

    extension(WebApplication self) {
        public WebApplication UseBeforeStartup(WebHostSettings settings) {
            settings.UseBeforeStartup?.Invoke(self);

            return self;
        }
    }
}
