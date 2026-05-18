using Microsoft.Extensions.Hosting;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Messenger Configuration
/// </summary>
public static class MessengerConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Messenger services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureMessenger(WinHostSettings settings) {
            if (settings.DisableMessenger) { return self; }

            self.ConfigureServices(
                services => services.RegisterMessenger()
            );

            return self;
        }
    }
}
