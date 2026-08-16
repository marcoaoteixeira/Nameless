using Microsoft.Extensions.Logging;

namespace Nameless.Testing.Tools.Mockers.Logging;

/// <summary>
///     Exception thrown when a mock logger verification fails to match the
///     expected criteria.
/// </summary>
    public class LoggerMockVerificationException : Exception
{
    /// <summary>
    ///     Gets the expected log level for the verification.
    /// </summary>
    public LogLevel ExpectedLogLevel { get; }

    /// <summary>
    ///     Gets the expected number of log calls.
    /// </summary>
    public int ExpectedCallsNumber { get; }

    /// <summary>
    ///     Gets the actual number of log calls that matched the predicate.
    /// </summary>
    public int ActualCallsNumber { get; }

    /// <summary>
    ///     Gets the messages that were captured at the expected log level.
    /// </summary>
    public IReadOnlyList<string> Messages { get; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="LoggerMockVerificationException"/> class.
    /// </summary>
    /// <param name="expectedLogLevel">
    ///     The expected log level.
    /// </param>
    /// <param name="expectedCallsNumber">
    ///     The expected number of calls.
    /// </param>
    /// <param name="actualCallsNumber">
    ///     The actual number of matching calls.
    /// </param>
    /// <param name="messages">
    ///     The captured log messages at the expected level.
    /// </param>
    public LoggerMockVerificationException(LogLevel expectedLogLevel, int expectedCallsNumber, int actualCallsNumber, IReadOnlyList<string> messages)
        : base(FormatMessage(expectedLogLevel, expectedCallsNumber, actualCallsNumber, messages))
    {
        ExpectedLogLevel = expectedLogLevel;
        ExpectedCallsNumber = expectedCallsNumber;
        ActualCallsNumber = actualCallsNumber;
        Messages = messages;
    }

    private static string FormatMessage(LogLevel expectedLogLevel, int expectedCallsNumber, int actualCallsNumber, IReadOnlyList<string> messages)
    {
        var messageList = messages.Count > 0
            ? string.Join("\n        - ", messages)
            : "(no messages captured at this level)";

        return $"""
                Mock logger verification failed.

                Expected: At least {expectedCallsNumber} {expectedLogLevel} log(s) matching the predicate
                Actual:   {actualCallsNumber} matching log(s) found

                Captured {expectedLogLevel} messages:
                    - {messageList}
                """;
    }
}