using Microsoft.Extensions.Hosting;
using Nameless.Registration;
using Nameless.Windows.Hosting.Wrappers;
using Nameless.Windows.UI;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Window Factory Configuration
/// </summary>
public static class WindowFactoryConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Window Factory service.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureWindowFactory(WinHostSettings settings) {
            if (settings.DisableWindowsFactory) { return self; }

            self.ConfigureServices(
                services => services.RegisterWindowFactory(
                    AssemblyScanAwareHelper.Join(
                        settings.ConfigureWindowFactoryRegistration,
                        settings.Assemblies
                    )
                )
            );

            return self;
        }
    }
}
