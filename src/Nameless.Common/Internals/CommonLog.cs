using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless;

/// <summary>
///     Common log delegates.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGenCode)]
public static partial class CommonLog {
    /// <summary>
    ///     Produces a debug log.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="tag">
    ///     The tag to identify the log message.
    /// </param>
    [LoggerMessage(level: LogLevel.Debug, message: "[{Tag}] {Message}")]
    public static partial void Debug(ILogger logger, string message, string? tag = null);

    /// <summary>
    ///     Produces an information log.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="tag">
    ///     The tag to identify the log message.
    /// </param>
    [LoggerMessage(level: LogLevel.Information, message: "[{Tag}] {Message}")]
    public static partial void Info(ILogger logger, string message, string? tag = null);

    /// <summary>
    ///     Produces a warning log.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="message">
    ///     The message for the warning.
    /// </param>
    /// <param name="tag">
    ///     The tag to identify the log message.
    /// </param>
    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] {Message}")]
    public static partial void Warning(ILogger logger, string message, string? tag = null);

    /// <summary>
    ///     Produces an error log.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="exception">
    ///     The exception to log.
    /// </param>
    /// <param name="tag">
    ///     The tag to identify the log message.
    /// </param>
    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] {Message}")]
    public static partial void Error(ILogger logger, string message, Exception? exception = null, string? tag = null);
}