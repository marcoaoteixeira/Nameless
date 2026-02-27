using Microsoft.AspNetCore.Builder;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Web.Bootstrap;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the Bootstrap system.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder BootstrapConfig(WebAppSettings settings) {
            if (settings.DisableBootstrap) { return self; }

            self.Services.RegisterBootstrap(
                settings.ConfigureBootstrap ?? DefaultConfiguration,
                self.Configuration
            );

            return self;

            void DefaultConfiguration(BootstrapRegistrationSettings opts) {
                opts.IncludeAssemblies(settings.Assemblies);
            }
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the bootstrap service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseBootstrap(WebAppSettings settings) {
            if (settings.DisableBootstrap) { return self; }

            self.UseBootstrap(_ => { });

            return self;
        }
    }
}