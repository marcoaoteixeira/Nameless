using System.Net;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web {
    /// <summary>
    /// <see cref="HttpContext"/> extension methods.
    /// </summary>
    public static class HttpContextExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the IP address (v4) from the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="self">The current instance that implements <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="string"/> representation of the IPv4.</returns>
        /// <remarks>If the IP address could not be parsed, a <see cref="IPAddress.None"/> will be returned instead.</remarks>
        public static string GetIPv4(this HttpContext self)
            => GetIPAddress(self).MapToIPv4().ToString();

        /// <summary>
        /// Retrieves the IP address (v4) from the <see cref="IHttpContextAccessor"/>.
        /// </summary>
        /// <param name="self">The current instance that implements <see cref="IHttpContextAccessor"/>.</param>
        /// <returns>A <see cref="string"/> representation of the IPv4.</returns>
        /// <remarks>If the IP address could not be parsed, a <see cref="IPAddress.None"/> will be returned instead.</remarks>
        public static string GetIPv4(this IHttpContextAccessor self)
            => GetIPAddress(self).MapToIPv4().ToString();

        /// <summary>
        /// Retrieves the IP address (v6) from the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="self">The current instance that implements <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="string"/> representation of the IPv6.</returns>
        /// <remarks>If the IP address could not be parsed, a <see cref="IPAddress.None"/> will be returned instead.</remarks>
        public static string GetIPv6(this HttpContext self)
            => GetIPAddress(self).MapToIPv6().ToString();

        /// <summary>
        /// Retrieves the IP address (v6) from the <see cref="IHttpContextAccessor"/>.
        /// </summary>
        /// <param name="self">The current instance that implements <see cref="IHttpContextAccessor"/>.</param>
        /// <returns>A <see cref="string"/> representation of the IPv4.</returns>
        /// <remarks>If the IP address could not be parsed, a <see cref="IPAddress.None"/> will be returned instead.</remarks>
        public static string GetIPv6(this IHttpContextAccessor self)
            => GetIPAddress(self).MapToIPv6().ToString();

        #endregion

        #region Private Static Methods

        private static IPAddress GetIPAddress(IHttpContextAccessor httpContextAccessor)
            => httpContextAccessor.HttpContext is not null
                ? GetIPAddress(httpContextAccessor.HttpContext)
                : IPAddress.None;

        private static IPAddress GetIPAddress(HttpContext httpContext)
            => httpContext.Request.Headers.ContainsKey(Root.HttpRequestHeaders.X_FORWARDED_FOR)
                ? IPAddress.Parse(httpContext.Request.Headers[Root.HttpRequestHeaders.X_FORWARDED_FOR].ToString())
                : httpContext.Connection.RemoteIpAddress ?? IPAddress.None;

        #endregion
    }
}
