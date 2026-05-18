using Microsoft.Extensions.Hosting;
using Nameless.WPF.Dialogs.Message;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Message Dialog Configuration
/// </summary>
public static class MessageDialogConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Message Dialog services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureMessageDialog(WinHostSettings settings) {
            if (settings.DisableMessageDialog) { return self; }

            self.ConfigureServices(
                services => services.RegisterMessageDialog()
            );

            return self;
        }
    }
}
