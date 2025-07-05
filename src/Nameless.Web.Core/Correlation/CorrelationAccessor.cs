using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nameless.Web.Null;

namespace Nameless.Web.Correlation;

/// <summary>
///     Default implementation of <see cref="ICorrelationAccessor"/> that
///     retrieves the correlation ID.
/// </summary>
public class CorrelationAccessor : ICorrelationAccessor {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<CorrelationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CorrelationAccessor"/>.
    /// </summary>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="options">The correlation options.</param>
    public CorrelationAccessor(IHttpContextAccessor httpContextAccessor, IOptions<CorrelationOptions> options) {
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
        _options = Prevent.Argument.Null(options);
    }

    /// <inheritdoc />
    public string? GetID() {
        var key = _options.Value.Key;
        var useHeader = _options.Value.UseHeader;

        return GetHttpContext().GetCorrelationID(key, useHeader);
    }

    private HttpContext GetHttpContext() {
        return _httpContextAccessor.HttpContext ?? NullHttpContext.Instance;
    }
}
