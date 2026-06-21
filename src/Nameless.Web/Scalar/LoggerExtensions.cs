using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;

namespace Nameless.Web.Scalar;

internal static class LoggerExtensions {
    extension(ILogger<ScalarHttpSecurityScheme> self) {
        internal void Failure(string actionName, Exception exception) {
            Log.Failure(
                logger: self,
                tag: "USE_SCALAR",
                actionName,
                exception
            );
        }

        internal void UnableRetrieveAccessToken(string actionName, string reason) {
            Log.Warning(
                logger: self,
                tag: "USE_SCALAR",
                actionName,
                reason
            );
        }
    }
}
