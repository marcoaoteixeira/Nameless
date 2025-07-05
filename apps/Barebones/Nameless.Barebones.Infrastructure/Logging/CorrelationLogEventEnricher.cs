using Nameless.Web.Correlation;
using Serilog.Core;
using Serilog.Events;

namespace Nameless.Barebones.Infrastructure.Logging;

/// <summary>
///     Correlation ID log event enricher.
/// </summary>
public class CorrelationLogEventEnricher : ILogEventEnricher {
    private const string CORRELATION_ID_PROPERTY_NAME = "CorrelationId";

    private readonly ICorrelationAccessor _correlationAccessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CorrelationLogEventEnricher"/> class.
    /// </summary>
    /// <param name="correlationAccessor"></param>
    public CorrelationLogEventEnricher(ICorrelationAccessor correlationAccessor) {
        _correlationAccessor = Prevent.Argument.Null(correlationAccessor);
    }

    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
        var correlation = _correlationAccessor.GetID();
        if (correlation is null) { return; }

        var logEventProperty = new LogEventProperty(
            CORRELATION_ID_PROPERTY_NAME,
            new ScalarValue(correlation)
        );

        logEvent.AddOrUpdateProperty(logEventProperty);
    }
}