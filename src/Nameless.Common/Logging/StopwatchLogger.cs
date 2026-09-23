using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Nameless.Logging;

/// <summary>
///     A disposable scope stopwatch that emits structured logs entries.
/// </summary>
/// <remarks>
///     Use it calling
/// </remarks>
public sealed class StopwatchLogger : IDisposable {
    private readonly ILogger _logger;
    private readonly string _className;
    private readonly string _actionName;
    private readonly Stopwatch _sw;
    private readonly LogLevel _level;

    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of <see cref="StopwatchLogger"/>
    ///     class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="className">
    ///     The class name.
    /// </param>
    /// <param name="actionName">
    ///     The action name.
    /// </param>
    /// <param name="level">
    ///     The log level.
    /// </param>
    internal StopwatchLogger(ILogger logger, string className, string actionName, LogLevel level) {
        _logger = logger;
        _className = className;
        _actionName = actionName;
        _level = level;

        _sw = Stopwatch.StartNew();

        InnerWrite("Starting...");
    }

    /// <summary>
    ///     Emits a mid-execution checkpoint log entry with a custom message.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="overwriteLevel">
    ///     Overrides the default log level (Debug)
    /// </param>
    public void Write(string message, LogLevel? overwriteLevel = null) {
        if (_disposed) { return; }

        InnerWrite(message, overwriteLevel);
    }

    /// <inheritdoc />
    public void Dispose() {
        if (_disposed) { return; }

        _disposed = true;
        _sw.Stop();

        InnerWrite("Finished!");
    }

    private void InnerWrite(string message, LogLevel? overwriteLevel = null) {
        var level = overwriteLevel ?? _level;

        if (!_logger.IsEnabled(level)) { return; }

        _logger.Log(
            logLevel: level,
            message: "[{ClassName}] {ActionName} ({Duration}): {Message}",
            args: [_className, _actionName, _sw.Elapsed, message]
        );
    }
}
