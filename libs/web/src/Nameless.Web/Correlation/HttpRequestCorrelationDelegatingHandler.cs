using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nameless.Contracts;

namespace Nameless.Web.Correlation;

/// <summary>
///     HTTP request correlation delegate handler.
/// </summary>
public class HttpRequestCorrelationDelegatingHandler : DelegatingHandler {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<HttpRequestCorrelationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="HttpRequestCorrelationDelegatingHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor.
    /// </param>
    /// <param name="options">
    ///     The HTTP request correlation options.
    /// </param>
    public HttpRequestCorrelationDelegatingHandler(IHttpContextAccessor httpContextAccessor, IOptions<HttpRequestCorrelationOptions> options) {
        _httpContextAccessor = Throws.When.Null(httpContextAccessor);
        _options = Throws.When.Null(options);
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var opts = _options.Value;

        if (_httpContextAccessor.HttpContext.TryGetCorrelationID(opts.HeaderKey, out var correlation)) {
            request.Headers.TryAddWithoutValidation(opts.HeaderKey, correlation);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
