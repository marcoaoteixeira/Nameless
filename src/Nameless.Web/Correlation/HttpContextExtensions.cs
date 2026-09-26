using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Correlation;

public static class HttpContextExtensions {
    extension(HttpContext? self) {
        /// <summary>
        ///     Retrieves the correlation ID from the HTTP context request.
        /// </summary>
        /// <param name="output">
        ///     The correlation value, if exists.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the correlation ID exists in the
        ///     HTTP context request header;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetCorrelationID([NotNullWhen(returnValue: true)] out string? output) {
            return self.TryGetCorrelationID(HttpRequestCorrelationDefaults.HeaderKey, out output);
        }

        /// <summary>
        ///     Retrieves the correlation ID from the HTTP context request.
        /// </summary>
        /// <param name="key">
        ///     The correlation ID key.
        /// </param>
        /// <param name="output">
        ///     The correlation value, if exists.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the correlation ID exists in the
        ///     HTTP context request header;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetCorrelationID(string key, [NotNullWhen(returnValue: true)]out string? output) {
            output = null;
            
            if (self is null) { return false; }
            
            Throws.When.NullOrWhiteSpace(key);

            output = self.Request.Headers.TryGetValue(key, out var value)
                ? value.ToString()
                : null;

            return output is not null;
        }
    }
}