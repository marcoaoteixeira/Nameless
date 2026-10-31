using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Logging.Serilog;
using Nameless.Web.Logging;
using Serilog;
using Serilog.Configuration;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Serilog Web Host Feature
/// </summary>
public class SerilogWebHostFeature : SerilogHostFeature, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        Configure(options => options.WithEnrichmentConfiguration(ConfigureEnrichment)
                                    .WithSinkConfiguration(ConfigureSink));

        base.Register(builder, context);
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseSerilogRequestLogging();
    }

    private static void ConfigureEnrichment(IServiceProvider provider, LoggerEnrichmentConfiguration enrich) {
        enrich.WithHttpRequestCorrelation(provider);
    }

    private static void ConfigureSink(IServiceProvider provider, LoggerSinkConfiguration sink) {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var exporterUrl = configuration[WebConstants.OpenTelemetry.ExporterEndpointConfigKey];
        if (!string.IsNullOrWhiteSpace(exporterUrl)) {
            sink.OpenTelemetry(opts => opts.Endpoint = exporterUrl);
        }
    }
}
