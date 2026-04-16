using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Nameless.Web.Correlation;

/// <summary>
///     Correlation middleware that injects a correlation ID into the HTTP context.
/// </summary>
public class HttpRequestCorrelationMiddleware {
    private readonly RequestDelegate _next;
    private readonly IOptions<HttpRequestCorrelationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpRequestCorrelationMiddleware"/>.
    /// </summary>
    /// <param name="next">
    ///     The next middleware in the request pipeline.
    /// </param>
    /// <param name="options">
    ///     The correlation options that configure the middleware behavior.
    /// </param>
    public HttpRequestCorrelationMiddleware(RequestDelegate next, IOptions<HttpRequestCorrelationOptions> options) {
        _next = Throws.When.Null(next);
        _options = Throws.When.Null(options);
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
        var opts = _options.Value;

        // Try get from the request header.
        if (context.TryGetCorrelationID(opts.HeaderKey, out var correlation)) {
            // If it exists, assign it to the TraceIdentifier property.
            // We do not need to create one if it does not exist, since
            // ASP.NET Core automatically assigns a random value to the
            // property TraceIdentifier.
            context.TraceIdentifier = correlation;
        }

        if (opts.IncludeInResponse) {
            context.Response.OnStarting(() => {
                context.Response.Headers[opts.HeaderKey] = context.TraceIdentifier;

                return Task.CompletedTask;
            });
        }

        return _next(context);
    }
}