using Microsoft.Extensions.Hosting;
using Nameless.WPF.Mvvm;
using Nameless.WPF.Windows;

namespace Nameless.WPF.Hosting.Configs;

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
        public WinHostBuilder RegisterWindowFactory(WinHostSettings settings) {
            if (settings.DisableWindowsFactory) { return self; }

            self.ConfigureServices(
                services => services.RegisterWindowFactory(
                    WinHostSettingsHelper.Join(
                        settings.ConfigureWindowFactoryRegistration,
                        settings.Assemblies
                    )
                )
            );

            return self;
        }
    }
}
