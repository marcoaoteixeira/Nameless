using Nameless.Bootstrap;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Bootstrap Configuration
/// </summary>
public static class BootstrapConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Bootstrap service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureBootstrap(WinHostSettings settings) {
            if (settings.DisableBootstrap) { return self; }

            self.ConfigureServices((ctx, services) => services.RegisterBootstrap(
                WinHostSettingsHelper.Join(
                    settings.ConfigureBootstrapRegistration,
                    settings.Assemblies
                ),
                ctx.Configuration
            ));

            return self;
        }
    }
}