using Microsoft.AspNetCore.Builder;
using Nameless.Workers;

namespace Nameless.Web.Hosting.Configs;

public static class WorkerConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Workers feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureWorkers(WebHostSettings settings) {
            if (settings.DisableWorkers) { return self; }

            self.Services.RegisterWorkers(
                WebHostSettingsHelper.Join(
                    settings.WorkerRegistrationConfiguration,
                    settings.Assemblies
                )
            );

            return self;
        }
    }
}