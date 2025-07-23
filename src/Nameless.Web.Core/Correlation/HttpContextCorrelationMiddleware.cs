using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Correlation;

/// <summary>
///     Correlation middleware that injects a correlation ID into the HTTP context.
/// </summary>
public class HttpContextCorrelationMiddleware {
    private readonly RequestDelegate _next;
    private readonly HttpContextCorrelationOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpContextCorrelationMiddleware"/>.
    /// </summary>
    /// <param name="next">
    ///     The next middleware in the request pipeline.
    /// </param>
    /// <param name="options">
    ///     The correlation options that configure the middleware behavior.
    /// </param>
    public HttpContextCorrelationMiddleware(RequestDelegate next, HttpContextCorrelationOptions options) {
        _next = Prevent.Argument.Null(next);
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
        var key = _options.Key;
        var useHeader = _options.UseHeader;

        if (!context.HasCorrelationID(key, useHeader)) {
            context.SetCorrelationID(Guid.NewGuid().ToString(), key, useHeader);
        }

        return _next(context);
    }
}
