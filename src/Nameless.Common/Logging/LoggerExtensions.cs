using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Nameless.Logging;

/// <summary>
///     <see cref="ILogger"/> extension methods
/// </summary>
public static class LoggerExtensions {
    /// <param name="self">
    ///     The current <see cref="ILogger"/> instance.
    /// </param>
    extension(ILogger self) {
        /// <summary>
        ///     Starts a new <see cref="StopwatchLogger"/> that logs elapsed
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
        ///     A <see cref="StopwatchLogger"/> that must be disposed to emit
        ///     the final "finished" entry. Use a <see langword="using"/>
        ///     declaration or statement.
        /// </returns>
        public StopwatchLogger StartStopwatchLogger(LogLevel level = LogLevel.Debug, [CallerMemberName] string caller = "") {
            if (!TryGetClassName(self, out var className)) {
                className = "Unknown";
            }

            return new StopwatchLogger(self, className, actionName: caller, level);
        }
    }

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
    private static bool TryGetClassName(ILogger logger, [NotNullWhen(returnValue: true)] out string? className) {
        className = null;

        // ILogger<TCategoryName> declares TCategoryName as `out` (covariant)
        // so an `is ILogger<object>` check can't be used to detect a generic
        // logger. Every Logger<TCategoryName> satisfies it regardless of
        // TCategoryName. Check the concrete runtime type instead.
        if (logger.GetType() is { IsGenericType: true } type && type.GetGenericTypeDefinition() == typeof(Logger<>)) {
            className = type.GetGenericArguments()[0].Name;

            return true;
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