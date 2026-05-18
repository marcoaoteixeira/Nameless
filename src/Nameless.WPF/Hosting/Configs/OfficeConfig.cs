using Microsoft.Extensions.Hosting;
using Nameless.WPF.Office;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Office Services Configuration
/// </summary>
public static class OfficeConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Office Services service.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureOfficeServices(WinHostSettings settings) {
            if (settings.DisableOffice) { return self; }

            self.ConfigureServices(
                services => services.RegisterOffice()
            );

            return self;
        }
    }
}
