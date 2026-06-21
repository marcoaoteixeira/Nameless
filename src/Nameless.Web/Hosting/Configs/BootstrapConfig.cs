using Microsoft.AspNetCore.Builder;
using Nameless.Bootstrap;
using Nameless.Registration;

namespace Nameless.Web.Hosting.Configs;

/// <summary>
///     Bootstrap configuration.
/// </summary>
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
                AssemblyScanAwareHelper.Join(
                    settings.ConfigureBootstrap,
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
        /// <summary>
        ///     Executes Bootstrap warmup.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> settings.
        /// </param>
        public void Warmup(WebHostSettings settings) {
            self.WarmupAsync(settings).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Asynchronous executes Bootstrap warmup.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> settings.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the method asynchronous
        ///     execution.
        /// </returns>
        public Task WarmupAsync(WebHostSettings settings) {
            return settings.DisableBootstrap
                ? Task.CompletedTask
                : self.WarmupAsync(settings.ConfigureBootstrapWarmup);
        }
    }
}