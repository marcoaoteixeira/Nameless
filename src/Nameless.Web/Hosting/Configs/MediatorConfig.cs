using Microsoft.AspNetCore.Builder;
using Nameless.Mediator;

namespace Nameless.Web.Hosting.Configs;

public static class MediatorConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Mediator feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureMediator(WebHostSettings settings) {
            if (settings.DisableMediator) { return self; }

            self.Services.RegisterMediator(
                WebHostSettingsHelper.Join(
                    settings.MediatorRegistrationConfiguration,
                    settings.Assemblies
                )
            );

            return self;
        }
    }
}