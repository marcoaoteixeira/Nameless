using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Nameless.Web.Cors;

internal static class CorsPolicyEntryExtensions {
    private const string SEPARATOR = Separators.COMMA;

    extension(CorsPolicyEntry self) {
        internal CorsPolicy ToPolicy() {
            var result = new CorsPolicy {
                SupportsCredentials = self.SupportsCredentials,
                PreflightMaxAge = self.PreflightMaxAge
            };

            var headers = self.Headers.Split(SEPARATOR);
            foreach (var value in headers) {
                result.Headers.Add(value);
            }

            var methods = self.Methods.Split(SEPARATOR);
            foreach (var value in methods) {
                result.Methods.Add(value);
            }

            var origins = self.Origins.Split(SEPARATOR);
            foreach (var value in origins) {
                result.Origins.Add(value);
            }

            return result;
        }
    }
}
