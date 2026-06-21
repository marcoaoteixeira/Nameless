using Microsoft.Extensions.Hosting;
using Nameless.Windows.Configuration;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Application Configuration Manager Configuration
/// </summary>
public static class AppConfigurationManagerConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Application Configuration Manager service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureAppConfigurationManager(WinHostSettings settings) {
            if (settings.DisableAppConfigurationManager) { return self; }

            self.ConfigureServices(
                services => services.RegisterAppConfigurationManager()
            );

            return self;
        }
    }
}