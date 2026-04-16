using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Nameless.Web.Cors;

internal static class CorsPolicyOptionsExtensions {
    private const string SEPARATOR = CoreConstants.Separators.Comma;

    extension(CorsPolicyOptions self) {
        internal CorsPolicy ToPolicy() {
            var result = new CorsPolicy {
                SupportsCredentials = self.SupportsCredentials,
                PreflightMaxAge = self.PreflightMaxAge
            };

            var headers = (self.Headers ?? string.Empty).Split(SEPARATOR);
            foreach (var value in headers) {
                result.Headers.Add(value);
            }

            var methods = (self.Methods ?? string.Empty).Split(SEPARATOR);
            foreach (var value in methods) {
                result.Methods.Add(value);
            }

            var origins = (self.Origins ?? string.Empty).Split(SEPARATOR);
            foreach (var value in origins) {
                result.Origins.Add(value);
            }

            return result;
        }
    }
}
