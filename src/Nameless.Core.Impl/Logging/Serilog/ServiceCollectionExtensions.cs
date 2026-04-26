using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;
using Serilog;

namespace Nameless.Logging.Serilog;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
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
