using Serilog;
using Serilog.Configuration;

namespace Nameless.Microservices.Infrastructure.Serilog;

public static class LoggerEnrichmentConfigurationExtensions {
    extension(LoggerEnrichmentConfiguration self) {
        public LoggerConfiguration WithCorrelationId(IServiceProvider provider) {
            return self.With(new CorrelationIdLogEventEnricher(provider));
        }
    }
}