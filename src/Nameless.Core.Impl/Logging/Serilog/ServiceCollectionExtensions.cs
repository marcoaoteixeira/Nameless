using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;
using Serilog;

namespace Nameless.Logging.Serilog;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods for Serilog logging integration.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers Serilog as the logging provider, applying either the default
        ///     configuration (console, file, OpenTelemetry) or a custom override.
        /// </summary>
        /// <param name="registration">Optional delegate to configure Serilog options.</param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterSerilog(Action<SerilogRegistration>? registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.AddSerilog(
                settings.OverrideSerilogConfiguration ?? DefaultSerilogConfiguration
            );

            return self.AddLogging(
                builder => builder.AddSerilog()
            );
        }
    }

    private static void DefaultSerilogConfiguration(IServiceProvider provider, LoggerConfiguration config) {
        var configuration = provider.GetRequiredService<IConfiguration>();
        
        config
            // Defines from where it should get its configurations.
            .ReadFrom.Configuration(configuration, readerOptions: null)

            // Enrich the log message with data from other locations.
            .Enrich.FromLogContext()

            // Write to console sink
            .WriteTo.Console()

            // Write to file sink
            .WriteTo.File(
                path: "app-.log",
                rollingInterval: RollingInterval.Hour,
                retainedFileCountLimit: 24
            )

            // Write to OpenTelemetry sink
            .WriteTo.OpenTelemetry(opts => opts.Endpoint = configuration[
                CoreConstants.OpenTelemetry.ExporterEndpointConfigName
            ]);
    }
}
