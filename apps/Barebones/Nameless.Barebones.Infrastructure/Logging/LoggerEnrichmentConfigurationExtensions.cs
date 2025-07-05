using Nameless.Web.Correlation;
using Serilog;
using Serilog.Configuration;

namespace Nameless.Barebones.Infrastructure.Logging;

/// <summary>
///     <see cref="LoggerEnrichmentConfiguration"/> extension methods."/>
/// </summary>
public static class LoggerEnrichmentConfigurationExtensions {
    /// <summary>
    ///     Configures the logger to include a correlation ID in log events.
    /// </summary>
    /// <param name="self">The current <see cref="LoggerEnrichmentConfiguration"/>.</param>
    /// <param name="correlationAccessor">
    ///     The correlation ID accessor.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerConfiguration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <remarks>
    ///     This method enriches log events with a correlation ID, which is
    ///     useful for tracking requests across distributed systems.
    /// </remarks>
    public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration self, ICorrelationAccessor correlationAccessor) {
        return self.With(new CorrelationLogEventEnricher(correlationAccessor));
    }
}
