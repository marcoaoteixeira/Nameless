using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Nameless.Web.Correlation;

/// <summary>
///     Correlation middleware that injects a correlation ID into the HTTP context.
/// </summary>
public class CorrelationMiddleware : IMiddleware {
    private readonly IOptions<CorrelationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CorrelationMiddleware"/>.
    /// </summary>
    /// <param name="options">
    ///     The correlation options that configure the middleware behavior.
    /// </param>
    public CorrelationMiddleware(IOptions<CorrelationOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context, RequestDelegate next) {
        var key = _options.Value.Key;
        var useHeader = _options.Value.UseHeader;

        if (!context.HasCorrelationID(key, useHeader)) {
            context.SetCorrelationID(Guid.NewGuid().ToString(), key, useHeader);
        }

        return next(context);
    }
}
