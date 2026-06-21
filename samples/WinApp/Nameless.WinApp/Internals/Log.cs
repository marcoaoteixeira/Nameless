using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.WinApp.Views.Windows;

namespace Nameless.WinApp.Internals;

[ExcludeFromCodeCoverage]
internal static class LoggerExtensions {
    #region MainWindow

    extension(ILogger<MainWindow> self) {
        internal void Failure(string actionName, Exception exception) {
            Log.Failure(
                logger: self,
                tag: "MAIN_WINDOW",
                actionName,
                exception
            );
        }
    }

    #endregion
}

[ExcludeFromCodeCoverage]
internal static partial class Log {
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] An problem has occurred while executing action '{ActionName}'. Reason: {Reason}")]
    internal static partial void Warning(ILogger logger, string tag, string actionName, string reason);

    #endregion
}
