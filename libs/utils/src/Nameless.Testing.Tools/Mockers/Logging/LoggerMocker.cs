using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Logging;

/// <summary>
///     Mocker for <see cref="ILogger{TCategoryName}"/>
/// </summary>
/// <typeparam name="TCategoryName">
///     Type of the log category.
/// </typeparam>
public class LoggerMocker<TCategoryName> : Mocker<ILogger<TCategoryName>> {
    /// <summary>
    ///     Enables <see cref="LogLevel.Trace"/>.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithLogTrace(Action<LogLevel, string>? callback = null) {
        return WithLog(LogLevel.Trace, callback);
    }

    /// <summary>
    ///     Enables <see cref="LogLevel.Debug"/>.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithLogDebug(Action<LogLevel, string>? callback = null) {
        return WithLog(LogLevel.Debug, callback);
    }

    /// <summary>
    ///     Enables <see cref="LogLevel.Information"/>.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithLogInformation(Action<LogLevel, string>? callback = null) {
        return WithLog(LogLevel.Information, callback);
    }

    /// <summary>
    ///     Enables <see cref="LogLevel.Warning"/>.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithLogWarning(Action<LogLevel, string>? callback = null) {
        return WithLog(LogLevel.Warning, callback);
    }

    /// <summary>
    ///     Enables <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithLogError(Action<LogLevel, string>? callback = null) {
        return WithLog(LogLevel.Error, callback);
    }

    /// <summary>
    ///     Enables <see cref="LogLevel.Critical"/>.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithLogCritical(Action<LogLevel, string>? callback = null) {
        return WithLog(LogLevel.Critical, callback);
    }

    /// <summary>
    ///     Enables all log levels.
    /// </summary>
    /// <param name="callback">
    ///     Callback that captures the log message.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> WithAnyLogLevel(Action<LogLevel, string>? callback = null) {
        foreach (var level in Enum.GetValues<LogLevel>()) {
            if (level == LogLevel.None) { continue; }

            WithLog(level, callback);
        }

        return this;
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Trace"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> VerifyTrace(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Trace, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Debug"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> VerifyDebug(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Debug, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Information"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> VerifyInformation(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Information, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Warning"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> VerifyWarning(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Warning, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Error"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> VerifyError(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Error, times, exactly);
    }

    /// <summary>
    ///     Verifies if a <see cref="LogLevel.Critical"/> message was captured.
    /// </summary>
    /// <param name="assertMessage">
    ///     Assert the message content.
    /// </param>
    /// <param name="times">
    ///     How many times the capture occured.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should be exactly number of times or at least.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerMocker{TCategoryName}"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerMocker<TCategoryName> VerifyCritical(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Critical, times, exactly);
    }
    
    private LoggerMocker<TCategoryName> WithLog(LogLevel level, Action<LogLevel, string>? callback = null) {
        MockInstance
            .Setup(mock => mock.IsEnabled(level))
            .Returns(value: true);

        var flow = MockInstance
            .Setup(mock => mock.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ));

        if (callback is not null) {
            flow.Callback(new InvocationAction(invocation => {
                var entry = CaptureEntryFromInvocation(invocation);

                callback.Invoke(entry.Level, entry.Message);
            }));
        }

        return this;
    }

    private LoggerMocker<TCategoryName> VerifyCallFor(Func<string, bool>? assertMessage, LogLevel level, int times, bool exactly) {
        var stateMessageMatch = (object value, Type type)
            => IsLogValueState(type) &&
               MatchMessage(assertMessage, value);

        MockInstance.Verify(mock => mock.Log(
            It.Is<LogLevel>(currentLogLevel => currentLogLevel == level),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((value, type) => stateMessageMatch(value!, type)),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), exactly ? Times.Exactly(times) : Times.AtLeast(times));

        return this;

        // [LoggerMessage] source generator emits a private nested struct whose
        // *closed* name contains "LogValues". We also accept any IReadOnlyList
        // of KVPs (the common interface), so the matcher is resilient to both
        // generated and hand-rolled callers.
        static bool IsLogValueState(Type type) {
            // Source-generated structs: __LogValues, LogValues, etc.
            return type.Name.Contains("LogValues", StringComparison.OrdinalIgnoreCase)
                   || type.Name.Contains("FormattedLogValues", StringComparison.OrdinalIgnoreCase)
                   || type.Name.Contains("LoggerMessage", StringComparison.OrdinalIgnoreCase)
                   // Fallback: any state that implements the structured-logging contract
                   || typeof(IReadOnlyList<KeyValuePair<string, object?>>).IsAssignableFrom(type);
        }

        static bool MatchMessage(Func<string, bool>? assertMessage, object value) {
            return (assertMessage ?? (_ => true)).Invoke(value.ToString() ?? string.Empty);
        }
    }
    
    private static Entry CaptureEntryFromInvocation(IInvocation invocation) {
        var level = (LogLevel)invocation.Arguments[index: 0]!;
        var state = invocation.Arguments[index: 2];
        var exception = invocation.Arguments[index: 3] as Exception;
        var formatter = invocation.Arguments[index: 4];
        var invoker = formatter!.GetType().GetMethod(name: "Invoke");
        var message = invoker?.Invoke(formatter, [state, exception])?.ToString()
               ?? state!.ToString()
               ?? string.Empty;

        return new Entry(level, message);
    }

    internal record Entry(LogLevel Level, string Message);
}