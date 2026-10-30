using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Nameless.Logging.Serilog;

/// <summary>
///     Registers Serilog features.
/// </summary>
public class SerilogHostFeature : HostFeature<SerilogFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        // Captured now, like every other feature, so a later Configure call
        // has no effect.
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        builder.Services.AddSerilog((provider, config) => {
            ConfigureSettings(provider, config.ReadFrom, options);
            ConfigureEnrichment(provider, config.Enrich, options);
            ConfigureSink(provider, config.WriteTo, options);
            ConfigureMinimumLevel(provider, config.MinimumLevel, options);
        });

        builder.Services.AddLogging(
            logging => logging.AddSerilog()
        );
    }

    private static void ConfigureSettings(IServiceProvider provider, LoggerSettingsConfiguration config, SerilogFeatureOptions options) {
        if (!options.OverwriteSettingsConfiguration) {
            // Defines from where it should get its configurations.
            config.Configuration(
                provider.GetRequiredService<IConfiguration>(),
                readerOptions: null
            );
        }

        options.ConfigureSettings?.Invoke(provider, config);
    }

    private static void ConfigureEnrichment(IServiceProvider provider, LoggerEnrichmentConfiguration config, SerilogFeatureOptions options) {
        if (!options.OverwriteEnrichmentConfiguration) {
            // Enrich the log message with data from other locations.
            config.FromLogContext();
        }

        options.ConfigureEnrichment?.Invoke(provider, config);
    }

    private static void ConfigureSink(IServiceProvider provider, LoggerSinkConfiguration config, SerilogFeatureOptions options) {
        if (!options.OverwriteSinkConfiguration) {
            // Enrich the log message with data from other locations.
            // Write to console sink
            config.Console();
        }

        options.ConfigureSink?.Invoke(provider, config);
    }

    private static void ConfigureMinimumLevel(IServiceProvider provider, LoggerMinimumLevelConfiguration config, SerilogFeatureOptions options) {
        if (!options.OverwriteMinimumLevelConfiguration) {
            // Enrich the log message with data from other locations.
            config.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning);
            config.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning);
            config.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
        }

        options.ConfigureMinimumLevel?.Invoke(provider, config);
    }
}
