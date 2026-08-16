using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless;

/// <summary>
///     Common log delegates.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGen)]
public static partial class CommonLog {

    #region Common

    /// <summary>
    ///     Produces an error log.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="exception">
    ///     The exception to log.
    /// </param>
    /// <param name="tag">
    ///     The tag to identify the log message.
    /// </param>
    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An unexpected error occurred. The exception has been logged for further investigation.")]
    public static partial void Failure(ILogger logger, Exception exception, string? tag = null);

    /// <summary>
    ///     Produces a warning log.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="reason">
    ///     The reason for the warning.
    /// </param>
    /// <param name="tag">
    ///     The tag to identify the log message.
    /// </param>
    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] The operation completed with warnings. No immediate action is required, but further investigation may be warranted. Reason: {Reason}")]
    public static partial void Warning(ILogger logger, string reason, string? tag = null);

    #endregion
}
