using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Nameless.Web.Correlation;

public class HttpRequestCorrelationDelegatingHandler : DelegatingHandler {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<HttpRequestCorrelationOptions> _options;

    public HttpRequestCorrelationDelegatingHandler(IHttpContextAccessor httpContextAccessor, IOptions<HttpRequestCorrelationOptions> options) {
        _httpContextAccessor = Throws.When.Null(httpContextAccessor);
        _options = Throws.When.Null(options);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var opts = _options.Value;

        if (_httpContextAccessor.HttpContext.TryGetCorrelationID(opts.HeaderKey, out var correlation)) {
            request.Headers.TryAddWithoutValidation(opts.HeaderKey, correlation);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
