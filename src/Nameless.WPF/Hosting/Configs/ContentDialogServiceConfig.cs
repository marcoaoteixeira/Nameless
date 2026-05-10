using Microsoft.Extensions.Hosting;
using Nameless.WPF.Extensions;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Content Dialog Service Configuration
/// </summary>
public static class ContentDialogServiceConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Content Dialog services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterContentDialogService(WinHostSettings settings) {
            if (settings.DisableContentDialogService) { return self; }

            self.ConfigureServices(
                services => services.RegisterContentDialogService()
            );

            return self;
        }
    }
}
