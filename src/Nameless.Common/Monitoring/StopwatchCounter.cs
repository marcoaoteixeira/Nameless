using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Nameless.Monitoring;

/// <summary>
///     A disposable scope stopwatch that emits structured logs entries.
/// </summary>
/// <remarks>
///     Use it calling
/// </remarks>
public sealed class StopwatchCounter : IDisposable {
    private readonly ILogger _logger;
    private readonly string _className;
    private readonly string _actionName;
    private readonly Stopwatch _sw;
    private readonly LogLevel _level;

    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of <see cref="StopwatchCounter"/>
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
    internal StopwatchCounter(ILogger logger, string className, string actionName, LogLevel level) {
        _logger = logger;
        _className = className;
        _actionName = actionName;
        _level = level;

        _sw = Stopwatch.StartNew();

        Write("starting", overrideLevel: null);
    }

    /// <summary>
    ///     Emits a mid-execution checkpoint log entry with a custom message.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="overrideLevel">
    ///     Overrides the default log level (Debug)
    /// </param>
    public void Echo(string message, LogLevel? overrideLevel = null) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Write($"says '{message}'", overrideLevel);
    }

    /// <inheritdoc />
    public void Dispose() {
        if (_disposed) { return; }

        _disposed = true;
        _sw.Stop();

        Write("finished", overrideLevel: null);
    }

    private void Write(string message, LogLevel? overrideLevel) {
        var level = overrideLevel ?? _level;

        if (!_logger.IsEnabled(level)) { return; }

        _logger.Log(
            logLevel: level,
            message: "[{ClassName}] Action '{ActionName}' {Message}: {Duration}ms",
            args: [_className, _actionName, message, _sw.ElapsedMilliseconds]
        );
    }
}

/// <summary>
///     <see cref="ILogger"/> extension methods
/// </summary>
public static class LoggerExtensions {
    /// <param name="self">
    ///     The current <see cref="ILogger"/> instance.
    /// </param>
    extension(ILogger self) {
        /// <summary>
        ///     Starts a new <see cref="StopwatchCounter"/> that logs elapsed
        ///     time at each checkpoint and automatically logs a "finished"
        ///     entry on disposal.
        /// </summary>
        /// <param name="level">
        ///     The log level for all entries in the scope.
        ///     Defaults to <see cref="LogLevel.Debug"/>
        /// </param>
        /// <param name="caller">
        ///     Automatically populates with the calling member's name via
        ///     <see cref="CallerMemberNameAttribute"/>. Do not pass this
        ///     argument manually.
        /// </param>
        /// <returns>
        ///     A <see cref="StopwatchCounter"/> that must be disposed to emit
        ///     the final "finished" entry. Use a <see langword="using"/>
        ///     declaration or statement.
        /// </returns>
        public StopwatchCounter StartStopwatchCounter(LogLevel level = LogLevel.Debug, [CallerMemberName] string caller = "") {
            if (!LoggerCategoryExtractor.TryGetClassName(self, out var className)) {
                className = "Unknown";
            }

            return new StopwatchCounter(self, className, actionName: caller, level);
        }
    }
}

internal static class LoggerCategoryExtractor {
    /// <summary>
    ///     Returns the short class name from the logger's category.
    /// </summary>
    /// <param name="logger">
    ///     The logger instance.
    /// </param>
    /// <param name="className">
    ///     The short class name; or <see langword="null"/> if unavailable.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the short class name is present;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool TryGetClassName(ILogger logger, [NotNullWhen(returnValue: true)] out string? className) {
        className = null;

        // ILogger<T> : ILogger<TCategoryName> : ILogger
        // The non-generic ILogger<TCategoryName> exposes no members, but
        // the concrete Logger<T> carries the category via its string
        // representation. We reach it through the internal
        // ICategoryNameAccessor pattern MEL uses.
        if (logger is not ILogger<object> && logger.GetType() is { IsGenericType: true } type) {
            // Logger<T> - the category is the fully qualified name of T
            var category = type.GetGenericArguments().FirstOrDefault();
            if (category is not null) {
                className = category.Name;

                return true;
            }
        }

        // Fallback: try to read the category from the ToString()
        // representation that MEL logger exposes,
        // e.g.: "Microsoft.Extensions.Logger[Namespace.Class]"
        if (logger.GetType().GetProperty("CategoryName")?.GetValue(logger) is not string raw) {
            return false;
        }

        className = raw.Split('.')[^1];

        return true;
    }
}