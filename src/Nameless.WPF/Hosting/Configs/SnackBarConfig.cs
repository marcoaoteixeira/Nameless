using Microsoft.Extensions.Hosting;
using Nameless.WPF.SnackBar;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     SnackBar Configuration
/// </summary>
public static class SnackBarConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the SnackBar services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureSnackBar(WinHostSettings settings) {
            if (settings.DisableSnackBar) { return self; }

            self.ConfigureServices(
                services => services.RegisterSnackBar()
            );

            return self;
        }
    }
}
