using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Logging.Serilog;
using Serilog;

namespace Nameless.WPF.Hosting.Configs;

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
                registration.OverrideSerilogConfiguration = (provider, config) => {
                    var configuration = provider.GetRequiredService<IConfiguration>();

                    config
                        // Defines from where it should get its configurations.
                        .ReadFrom.Configuration(configuration)

                        // Enrich the log message with data from other locations.
                        .Enrich.FromLogContext()

                        // Write to file sink
                        .WriteTo.File(
                            path: "app-.log",
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 16_777_216,
                            retainedFileCountLimit: 3
                        )

                        // Write to console sink
                        .WriteTo.Console();
                };
            }
        }
    }
}