using Microsoft.AspNetCore.Builder;
using Nameless.Registration;
using Nameless.Workers;

namespace Nameless.Web.Hosting.Configs;

public static class PeriodicWorkerConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Workers feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigurePeriodicWorkers(WebHostSettings settings) {
            if (settings.DisablePeriodicWorkers) { return self; }

            self.Services.RegisterPeriodicWorkers(
                AssemblyScanAwareHelper.Join(
                    settings.ConfigurePeriodicWorkers,
                    settings.Assemblies
                )
            );

            return self;
        }
    }
}