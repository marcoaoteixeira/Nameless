using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

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

            self.AddSerilog((provider, config) => {
                ConfigureSettings(provider, config.ReadFrom, settings);
                ConfigureEnrichment(provider, config.Enrich, settings);
                ConfigureSink(provider, config.WriteTo, settings);
                ConfigureMinimumLevel(provider, config.MinimumLevel, settings);
            });

            return self.AddLogging(
                builder => builder.AddSerilog()
            );
        }
    }

    private static void ConfigureSettings(IServiceProvider provider, LoggerSettingsConfiguration config, SerilogRegistration settings) {
        if (!settings.OverrideSettingsConfiguration) {
            // Defines from where it should get its configurations.
            config.Configuration(
                provider.GetRequiredService<IConfiguration>(),
                readerOptions: null
            );
        }

        settings.ConfigureSettings?.Invoke(provider, config);
    }

    private static void ConfigureEnrichment(IServiceProvider provider, LoggerEnrichmentConfiguration config, SerilogRegistration settings) {
        if (!settings.OverrideEnrichmentConfiguration) {
            // Enrich the log message with data from other locations.
            config.FromLogContext();
        }

        settings.ConfigureEnrichment?.Invoke(provider, config);
    }

    private static void ConfigureSink(IServiceProvider provider, LoggerSinkConfiguration config, SerilogRegistration settings) {
        if (!settings.OverrideSinkConfiguration) {
            // Enrich the log message with data from other locations.
            // Write to console sink
            config.Console();

            // Write to file sink
            config.File(
                path: "app-.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 16_777_216,
                retainedFileCountLimit: 3
            );

            // Write to OpenTelemetry sink
            var configuration = provider.GetRequiredService<IConfiguration>();
            var exporterUrl = configuration[StaticData.OpenTelemetry.ExporterEndpointConfigKey];
            if (!string.IsNullOrWhiteSpace(exporterUrl)) {
                config.OpenTelemetry(opts => opts.Endpoint = exporterUrl);
            }
        }

        settings.ConfigureSink?.Invoke(provider, config);
    }

    private static void ConfigureMinimumLevel(IServiceProvider provider, LoggerMinimumLevelConfiguration config, SerilogRegistration settings) {
        if (!settings.OverrideMinimumLevelConfiguration) {
            // Enrich the log message with data from other locations.
            config.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning);
            config.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning);
            config.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
        }

        settings.ConfigureMinimumLevel?.Invoke(provider, config);
    }
}
