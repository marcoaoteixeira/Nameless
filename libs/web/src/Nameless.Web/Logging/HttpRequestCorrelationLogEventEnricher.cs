using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Correlation;
using Serilog.Core;
using Serilog.Events;

namespace Nameless.Web.Logging;

/// <summary>
///     HTTP request correlation ID log event enricher.
/// </summary>
public class HttpRequestCorrelationLogEventEnricher : ILogEventEnricher {
    private readonly IHttpContextAccessor? _httpContextAccessor;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="HttpRequestCorrelationLogEventEnricher"/> class.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    public HttpRequestCorrelationLogEventEnricher(IServiceProvider provider) {
        _httpContextAccessor = provider.GetService<IHttpContextAccessor>();
    }

    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
        var context = _httpContextAccessor?.HttpContext;

        if (context is null || !context.TryGetCorrelationID(out var correlation)) {
            return;
        }

        logEvent.AddOrUpdateProperty(
            propertyFactory.CreateProperty("Correlation", correlation)
        );
    }
}