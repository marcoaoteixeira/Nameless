using Microsoft.AspNetCore.Builder;
using Nameless.Mediator;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Mediator feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder MediatorConfig(WebAppSettings settings) {
            if (settings.DisableMediator) { return self; }

            self.Services.RegisterMediator(settings.ConfigureMediator ?? DefaultConfiguration);

            return self;

            void DefaultConfiguration(MediatorRegistrationSettings opts) {
                opts.IncludeAssemblies(settings.Assemblies);
            }
        }
    }
}