using Serilog;
using Serilog.Configuration;

namespace Nameless.Web.Logging;

/// <summary>
///     Serilog enrichment configuration extension methods.
/// </summary>
public static class LoggerEnrichmentConfigurationExtensions {
    extension(LoggerEnrichmentConfiguration self) {
        /// <summary>
        ///     Includes HTTP request correlation ID to the log.
        /// </summary>
        /// <param name="provider">
        ///     The service provider.
        /// </param>
        /// <returns>
        ///     The current Serilog logger configuration instance.
        /// </returns>
        public LoggerConfiguration WithHttpRequestCorrelation(IServiceProvider provider) {
            return self.With(new HttpRequestCorrelationLogEventEnricher(provider));
        }
    }
}