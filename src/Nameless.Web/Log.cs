using Microsoft.Extensions.Logging;
using Nameless.Web.ErrorHandling;
using Nameless.Web.Filters.Validation;
using Scalar.AspNetCore;

namespace Nameless.Web;

internal static partial class Log {
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] An problem has occurred while executing action '{ActionName}'. Reason: {Reason}")]
    internal static partial void Warning(ILogger logger, string tag, string actionName, string reason);

    #endregion

    #region GlobalExceptionHandler

    [LoggerMessage(level: LogLevel.Error, message: "Unhandled exception captured by global exception handler.")]
    internal static partial void GlobalExceptionHandlerCapture(ILogger<GlobalExceptionHandler> logger, Exception exception);

    #endregion

    #region ValidationFilterBase

    [LoggerMessage(level: LogLevel.Warning, message: "Validation service unavailable")]
    internal static partial void ValidationServiceUnavailable(ILogger<ValidationFilterBase> logger);

    #endregion
}
