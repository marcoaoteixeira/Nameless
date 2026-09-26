using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Correlation;
using Serilog.Core;
using Serilog.Events;

namespace Nameless.Web.Serilog;

public class CorrelationIdLogEventEnricher : ILogEventEnricher {
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public CorrelationIdLogEventEnricher(IServiceProvider provider) {
        _httpContextAccessor = provider.GetService<IHttpContextAccessor>();
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
        var context = _httpContextAccessor?.HttpContext;

        if (context is null || !context.TryGetCorrelationID(out var correlation)) {
            return;
        }

        logEvent.AddOrUpdateProperty(
            propertyFactory.CreateProperty("CorrelationId", correlation)
        );
    }
}