using Microsoft.Extensions.Hosting;
using Nameless.Logging.Serilog;
using Nameless.Windows.Hosting.Wrappers;
using Serilog;

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
        public WinHostBuilder ConfigureLogging(WinHostSettings settings) {
            if (settings.DisableLogging) { return self; }

            self.ConfigureServices(services => services.RegisterSerilog(
                settings.ConfigureLoggingRegistration ?? ConfigureDefaults
            ));

            return self;

            static void ConfigureDefaults(SerilogRegistration registration) {
                registration.SinkConfiguration = (_, config) => {
                    // Write to file sink
                    config.File(
                        path: "app-.log",
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 16_777_216,
                        retainedFileCountLimit: 3
                    );
                };
            }
        }
    }
}