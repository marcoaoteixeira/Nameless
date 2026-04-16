using Microsoft.Extensions.Logging;
using Nameless.Web.ErrorHandling;

namespace Nameless.Web;

internal static partial class Log {
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    [LoggerMessage(level: LogLevel.Error, message: "Unhandled exception captured by global exception handler.")]
    internal static partial void GlobalExceptionHandlerCapture(ILogger<GlobalExceptionHandler> logger, Exception exception);

    #endregion
}
