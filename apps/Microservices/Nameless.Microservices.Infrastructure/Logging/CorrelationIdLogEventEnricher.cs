using Nameless.Web.Correlation;
using Serilog.Core;
using Serilog.Events;

namespace Nameless.Microservices.Infrastructure.Logging;

public class CorrelationIdLogEventEnricher : ILogEventEnricher {
    private const string CORRELATION_ID_PROPERTY_NAME = "CorrelationId";

    private readonly ICorrelationAccessor _correlationAccessor;

    public CorrelationIdLogEventEnricher(ICorrelationAccessor correlationAccessor) {
        _correlationAccessor = Prevent.Argument.Null(correlationAccessor);
    }

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