using Microsoft.Extensions.Hosting;
using Nameless.WPF.Navigation;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Navigation Configuration
/// </summary>
public static class NavigationConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Navigation services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterNavigation(WinHostSettings settings) {
            if (settings.DisableNavigation) { return self; }

            self.ConfigureServices(services => services.RegisterNavigation(
                WinHostSettingsHelper.Join(
                    settings.ConfigureNavigationRegistration,
                    settings.Assemblies
                )
            ));

            return self;
        }
    }
}
