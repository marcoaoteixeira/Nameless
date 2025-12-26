using Nameless.Web.Correlation;
using Serilog.Core;
using Serilog.Events;

namespace Nameless.Microservices.App.Infrastructure.Serilog;

public class CorrelationIdLogEventEnricher : ILogEventEnricher {
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public CorrelationIdLogEventEnricher(IServiceProvider provider) {
        _httpContextAccessor = provider.GetService<IHttpContextAccessor>();
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
        if (_httpContextAccessor?.HttpContext is null) {
            return;
        }

        var correlationId = _httpContextAccessor.HttpContext.GetCorrelationID();
        var property = propertyFactory.CreateProperty("CorrelationId", correlationId);

        logEvent.AddOrUpdateProperty(property);
    }
}