using System.Net;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web;

/// <summary>
///     <see cref="HttpContext" /> extension methods.
/// </summary>
public static class HttpContextExtensions {
    private const string X_FORWARDED_FOR_KEY = "X-Forwarded-For";

    /// <summary>
    ///     Retrieves the IP address (v4) from the <see cref="HttpContext" />.
    /// </summary>
    /// <param name="self">The current instance that implements <see cref="HttpContext" />.</param>
    /// <returns>A <see cref="string" /> representation of the IPv4.</returns>
    /// <remarks>If the IP address could not be parsed, a <see cref="IPAddress.None" /> will be returned instead.</remarks>
    public static string GetIPv4(this HttpContext self) {
        return GetIpAddress(self).MapToIPv4().ToString();
    }

    /// <summary>
    ///     Retrieves the IP address (v6) from the <see cref="HttpContext" />.
    /// </summary>
    /// <param name="self">The current instance that implements <see cref="HttpContext" />.</param>
    /// <returns>A <see cref="string" /> representation of the IPv6.</returns>
    /// <remarks>If the IP address could not be parsed, a <see cref="IPAddress.None" /> will be returned instead.</remarks>
    public static string GetIPv6(this HttpContext self) {
        return GetIpAddress(self).MapToIPv6().ToString();
    }

    private static IPAddress GetIpAddress(HttpContext httpContext) {
        return Prevent.Argument
                      .Null(httpContext)
                      .Request
                      .Headers
                      .TryGetValue(X_FORWARDED_FOR_KEY, out var xForwardedFor)
            ? IPAddress.Parse(xForwardedFor.ToString())
            : httpContext.Connection.RemoteIpAddress ?? IPAddress.None;
    }
}