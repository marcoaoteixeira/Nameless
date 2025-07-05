using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Nameless.Web.Correlation;

/// <summary>
///     Correlation middleware that injects a correlation ID into the HTTP context.
/// </summary>
public class CorrelationMiddleware {
    private readonly RequestDelegate _next;
    private readonly IOptions<CorrelationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CorrelationMiddleware"/>.
    /// </summary>
    /// <param name="next">
    ///     The next middleware in the request pipeline.
    /// </param>
    /// <param name="options">
    ///     The correlation options that configure the middleware behavior.
    /// </param>
    public CorrelationMiddleware(RequestDelegate next, IOptions<CorrelationOptions> options) {
        _next = next;
        _options = Prevent.Argument.Null(options);
    }

    /// <summary>
    ///     Invokes the middleware to set a correlation ID in the
    ///     HTTP context if it does not already exist.
    /// </summary>
    /// <param name="context">
    ///     The <see cref="HttpContext"/> for the current request.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public Task InvokeAsync(HttpContext context) {
        var key = _options.Value.Key;
        var useHeader = _options.Value.UseHeader;

        if (!context.HasCorrelationID(key, useHeader)) {
            context.SetCorrelationID(Guid.NewGuid().ToString(), key, useHeader);
        }

        return _next(context);
    }
}
