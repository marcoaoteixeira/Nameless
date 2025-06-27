using Serilog;
using Serilog.Configuration;

namespace Nameless.Identity.Web.Infrastructure;

/// <summary>
///     <see cref="LoggerEnrichmentConfiguration"/> extension methods."/>
/// </summary>
public static class LoggerEnrichmentConfigurationExtensions {
    /// <summary>
    ///     Configures the logger to include a correlation ID in log events.
    /// </summary>
    /// <param name="self">The current <see cref="LoggerEnrichmentConfiguration"/>.</param>
    /// <param name="key">When provided, use as the key of the correlation ID value.</param>
    /// <param name="useHeader">Whether it should use the HTTP header to store the correlation ID.</param>
    /// <returns>
    ///     The current <see cref="LoggerConfiguration"/> instance so other actions can be chained.
    /// </returns>
    /// <remarks>
    ///     This method enriches log events with a correlation ID, which is useful for
    ///     tracking requests across distributed systems. For this to work correctly,
    ///     it is 
    /// </remarks>
    public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration self, string? key = Constants.CORRELATION_ID_HEADER_KEY, bool useHeader = true) {
        return self.With(new CorrelationIdLogEventEnricher(key, useHeader));
    }
}
