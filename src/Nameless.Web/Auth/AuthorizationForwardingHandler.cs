using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Auth;

/// <summary>
///     DelegatingHandler that forwards the incoming HTTP request's
///     authorization information to outgoing HTTP requests made by an
///     HttpClient that uses this handler.
/// </summary>
/// <remarks>
///     This handler attempts to obtain an access token from the current
///     <see cref="HttpContext"/> via <see cref="AuthenticationHttpContextExtensions.GetTokenAsync(HttpContext, string)"/>
///     using the "access_token" token name. If an access token is found, it
///     is attached to the outgoing request using the
///     <see cref="JwtBearerDefaults.AuthenticationScheme"/> scheme.
///
///     If no access token is available, the handler falls back to copying the
///     raw "Authorization" header from the incoming request to the outgoing
///     request (without validation).
///
///     The handler requires an <see cref="IHttpContextAccessor"/> to access
///     the current request context. If no <see cref="HttpContext"/> is
///     available, the handler simply calls the base implementation.
/// </remarks>
public sealed class AuthorizationForwardingHandler : DelegatingHandler {
    /// <summary>
    ///     Accessor used to obtain the current <see cref="HttpContext"/> when
    ///     processing outgoing requests.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="AuthorizationForwardingHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor used to read the incoming request context.
    /// </param>
    public AuthorizationForwardingHandler(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Sends an HTTP request to the inner handler after attempting to
    ///     attach an authorization header copied from the current incoming
    ///     HTTP request.
    /// </summary>
    /// <param name="request">
    ///     The outgoing <see cref="HttpRequestMessage"/> to send.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to cancel the operation.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{HttpResponseMessage}"/> which completes with the
    ///     response returned by the inner handler.
    /// </returns>
    /// <remarks>
    ///     If no <see cref="HttpContext"/> is available (for example when
    ///     running outside of an HTTP request), the outgoing request is sent
    ///     without modification.
    /// </remarks>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null) {
            return await base.SendAsync(request, cancellationToken);
        }

        await SetAuthorizationTokenAsync(request, httpContext);

        return await base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    ///     Attempts to set the Authorization header on the outgoing request
    ///     by:
    ///     <para>
    ///         1. Reading the "access_token" from the
    ///         <paramref name="httpContext"/> tokens and using the JWT bearer
    ///         scheme if present.
    ///     </para>
    ///     <para>
    ///         2. Falling back to copying the raw incoming "Authorization"
    ///         header if no token was found.
    ///     </para>
    /// </summary>
    /// <param name="request">
    ///     The outgoing <see cref="HttpRequestMessage"/> to modify.
    /// </param>
    /// <param name="httpContext">
    ///     The current <see cref="HttpContext"/> containing the incoming request
    ///     and authentication tokens.
    /// </param>
    /// <returns>
    ///     A completed <see cref="Task"/> when the header has been set (or not).
    /// </returns>
    private static async Task SetAuthorizationTokenAsync(HttpRequestMessage request, HttpContext httpContext) {
        // Forward the Authorization header from the incoming request
        var token = await httpContext.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(token)) {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                scheme: JwtBearerDefaults.AuthenticationScheme,
                parameter: token
            );

            return;
        }

        // Fallback: forward the raw Authorization header if present
        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader)) {
            request.Headers.TryAddWithoutValidation(
                name: "Authorization",
                value: authHeader
            );
        }
    }
}
