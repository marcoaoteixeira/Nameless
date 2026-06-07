using Microsoft.AspNetCore.Builder;

namespace Nameless.Web.Hosting.Configs;

public static class CustomConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureAdditionalServices(WebHostSettings settings) {
            settings.ConfigureAdditionalServices?.Invoke(
                self.Services,
                self.Configuration,
                self.Environment
            );

            return self;
        }
    }

    extension(WebApplication self) {
        public WebApplication ExecuteBeforeStartup(WebHostSettings settings) {
            settings.ExecuteBeforeStartup?.Invoke(self);

            return self;
        }
    }
}
