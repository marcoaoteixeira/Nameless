using Nameless.Bootstrap;
using Nameless.Registration;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

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
        public WinHostBuilder ConfigureBootstrapFeature(WinHostSettings settings) {
            if (settings.DisableBootstrap) { return self; }

            self.ConfigureServices((_, services) => services.RegisterBootstrap(
                AssemblyScanAwareHelper.Join(
                    settings.ConfigureBootstrap,
                    settings.Assemblies
                )
            ));

            return self;
        }
    }
}