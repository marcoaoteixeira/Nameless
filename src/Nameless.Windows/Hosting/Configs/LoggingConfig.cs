using Microsoft.Extensions.Hosting;
using Nameless.Logging.Serilog;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Logging Configuration
/// </summary>
public static class LoggingConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Logging service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureLoggingFeature(WinHostSettings settings) {
            if (settings.DisableLogging) { return self; }

            self.ConfigureServices(services => services.RegisterSerilog(
                settings.ConfigureLogging
            ));

            return self;
        }
    }
}