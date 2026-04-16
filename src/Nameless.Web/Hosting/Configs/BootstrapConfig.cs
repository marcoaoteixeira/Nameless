using Microsoft.AspNetCore.Builder;
using Nameless.Bootstrap;

namespace Nameless.Web.Hosting.Configs;

public static class BootstrapConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the Bootstrap system.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureBootstrap(WebHostSettings settings) {
            if (settings.DisableBootstrap) { return self; }

            self.Services.RegisterBootstrap(
                WebHostSettingsHelper.Join(
                    settings.BootstrapRegistrationConfiguration,
                    settings.Assemblies
                ),
                self.Configuration
            );

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        public void Warmup(WebHostSettings settings) {
            self.WarmupAsync(settings).GetAwaiter().GetResult();
        }

        public Task WarmupAsync(WebHostSettings settings) {
            return settings.DisableBootstrap
                ? Task.CompletedTask
                : self.WarmupAsync(settings.BootstrapExecutionConfiguration);
        }
    }
}