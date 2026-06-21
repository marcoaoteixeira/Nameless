using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Nameless.Windows;

[ExcludeFromCodeCoverage]
internal static partial class Log {
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    #endregion
}
