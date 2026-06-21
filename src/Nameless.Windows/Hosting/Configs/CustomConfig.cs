using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Additional Configuration config.
/// </summary>
public static class CustomConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures Additional Configurations.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureAdditionalServices(WinHostSettings settings) {
            if (settings.ConfigureAdditionalServices is null) { return self; }

            self.ConfigureServices((ctx, services) => {
                settings.ConfigureAdditionalServices(
                    services,
                    ctx.Configuration,
                    ctx.HostingEnvironment
                );
            });

            return self;
        }
    }
}
