using System.Diagnostics.CodeAnalysis;
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
[ExcludeFromCodeCoverage]
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
                // Defines from where it should get its configurations.
                config.ReadFrom.Configuration(
                    provider.GetRequiredService<IConfiguration>(),
                    readerOptions: null
                );

                GetEnrichmentConfigurator(settings).Invoke(provider, config.Enrich);
                GetSinkConfigurator(settings).Invoke(provider, config.WriteTo);
                GetMinimumLevelConfigurator(settings).Invoke(provider, config.MinimumLevel);
            });

            return self.AddLogging(
                builder => builder.AddSerilog()
            );
        }
    }

    private static Action<IServiceProvider, LoggerEnrichmentConfiguration> GetEnrichmentConfigurator(SerilogRegistration registration) {
        return registration.OverrideEnrichmentConfiguration
            ? Throws.When.Null(registration.EnrichmentConfiguration)
            : (Action<IServiceProvider, LoggerEnrichmentConfiguration>)Delegate.Combine(
                DefaultEnrichmentConfiguration, registration.EnrichmentConfiguration
            );
    }

    private static Action<IServiceProvider, LoggerSinkConfiguration> GetSinkConfigurator(SerilogRegistration registration) {
        return registration.OverrideSinkConfiguration
            ? Throws.When.Null(registration.SinkConfiguration)
            : (Action<IServiceProvider, LoggerSinkConfiguration>)Delegate.Combine(
                DefaultSinkConfiguration, registration.SinkConfiguration
            );
    }

    private static Action<IServiceProvider, LoggerMinimumLevelConfiguration> GetMinimumLevelConfigurator(SerilogRegistration registration) {
        return registration.OverrideMinimumLevelConfiguration
            ? Throws.When.Null(registration.MinimumLevelConfiguration)
            : (Action<IServiceProvider, LoggerMinimumLevelConfiguration>)Delegate.Combine(
                DefaultMinimumLevelConfiguration, registration.MinimumLevelConfiguration
            );
    }

    private static void DefaultEnrichmentConfiguration(IServiceProvider _, LoggerEnrichmentConfiguration config) {
        // Enrich the log message with data from other locations.
        config.FromLogContext();
    }

    private static void DefaultSinkConfiguration(IServiceProvider provider, LoggerSinkConfiguration config) {
        // Write to console sink
        config.Console();

        // Write to file sink
        config.File(
            path: "app-.log",
            rollingInterval: RollingInterval.Hour,
            retainedFileCountLimit: 24
        );

        // Write to OpenTelemetry sink
        var configuration = provider.GetRequiredService<IConfiguration>();
        var exporterUrl = configuration[StaticData.OpenTelemetry.ExporterEndpointConfigKey];
        if (!string.IsNullOrWhiteSpace(exporterUrl)) {
            config.OpenTelemetry(opts => opts.Endpoint = exporterUrl);
        }
    }

    private static void DefaultMinimumLevelConfiguration(IServiceProvider _, LoggerMinimumLevelConfiguration config) {
        // Enrich the log message with data from other locations.
        config.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning);
        config.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning);
        config.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
    }
}
