using Serilog;
using Serilog.Configuration;

namespace Nameless.Web.Serilog;

public static class LoggerEnrichmentConfigurationExtensions {
    extension(LoggerEnrichmentConfiguration self) {
        public LoggerConfiguration WithCorrelationId(IServiceProvider provider) {
            return self.With(new CorrelationIdLogEventEnricher(provider));
        }
    }
}