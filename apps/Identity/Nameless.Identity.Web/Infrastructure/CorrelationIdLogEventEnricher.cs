using Serilog.Core;
using Serilog.Events;

namespace Nameless.Identity.Web.Infrastructure;

public class CorrelationIdLogEventEnricher : ILogEventEnricher {
    private const string CORRELATION_ID_PROPERTY_NAME = "CorrelationId";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly bool _useHeader;
    private readonly string _key;

    public CorrelationIdLogEventEnricher(string? key = null, bool useHeader = true)
        : this(new HttpContextAccessor(), key, useHeader) { }

    public CorrelationIdLogEventEnricher(IHttpContextAccessor httpContextAccessor, string? key = null, bool useHeader = true) {
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
        _key = key.WithFallback(Constants.CORRELATION_ID_HEADER_KEY);
        _useHeader = useHeader;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
        var correlationId = GetCorrelationId();
        var logEventProperty = new LogEventProperty(
            CORRELATION_ID_PROPERTY_NAME,
            new ScalarValue(correlationId)
        );

        logEvent.AddOrUpdateProperty(logEventProperty);

        SetCorrelationId(correlationId);
    }

    private HttpContext GetHttpContext() {
        return _httpContextAccessor.HttpContext ??
               throw new InvalidOperationException("HttpContext is not available.");
    }

    private string GetCorrelationId() {
        string? result = null;

        if (_useHeader) {
            if (GetHttpContext().Request.Headers.TryGetValue(_key, out var headerValue) ||
                GetHttpContext().Response.Headers.TryGetValue(_key, out headerValue)) {
                result = headerValue.ToString();
            }
        }
        else {
            if (GetHttpContext().Items.TryGetValue(_key, out var contextValue)) {
                result = contextValue?.ToString();
            }
        }

        return string.IsNullOrWhiteSpace(result) ? Guid.NewGuid().ToString() : result;
    }

    private void SetCorrelationId(string value) {
        if (_useHeader) { GetHttpContext().Request.Headers[_key] = value; }
        else { GetHttpContext().Items[_key] = value; }
    }
}