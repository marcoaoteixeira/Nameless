using Microsoft.Extensions.Logging;

namespace Nameless.Web.ErrorHandling;

internal static class LoggerExtensions {
    extension(ILogger<GlobalExceptionHandler> logger) {
        internal void CaptureException(Exception exception) {
            Log.GlobalExceptionHandlerCapture(logger, exception);
        }
    }
}
