using System.Net;
using Microsoft.AspNetCore.Http;

namespace Nameless.AspNetCore {

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
        /// <remarks>If the IP address could not be parsed, a <see cref="string.Empty"/> will be returned instead.</remarks>
        public static string GetIPv4(this HttpContext self)
            => GetIPAddress(self).MapToIPv4().ToString();
        /// <summary>
        /// Retrieves the IP address (v6) from the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="self">The current instance that implements <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="string"/> representation of the IPv6.</returns>
        /// <remarks>If the IP address could not be parsed, a <see cref="string.Empty"/> will be returned instead.</remarks>
        public static string GetIPv6(this HttpContext self)
            => GetIPAddress(self).MapToIPv6().ToString();

        #endregion

        #region Private Static Methods

        private const string X_FORWARDED_FOR_KEY = "X-Forwarded-For";
        private static IPAddress GetIPAddress(HttpContext httpContext)
            => httpContext.Request.Headers.ContainsKey(X_FORWARDED_FOR_KEY)
                ? IPAddress.Parse(httpContext.Request.Headers[X_FORWARDED_FOR_KEY].ToString())
                : httpContext.Connection.RemoteIpAddress ?? IPAddress.None;

        #endregion
    }
}
