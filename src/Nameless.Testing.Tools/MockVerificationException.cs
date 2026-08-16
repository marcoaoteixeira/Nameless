namespace Nameless.Testing.Tools;

/// <summary>
///     Exception thrown when a mock verification fails to match the
///     expected criteria.
/// </summary>
public class MockVerificationException : Exception
{
    /// <summary>
    ///     Gets the expected number of log calls.
    /// </summary>
    public int ExpectedCallsNumber { get; }

    /// <summary>
    ///     Gets the actual number of log calls that matched the predicate.
    /// </summary>
    public int ActualCallsNumber { get; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="MockVerificationException"/> class.
    /// </summary>
    /// <param name="expectedCallsNumber">
    ///     The expected number of calls.
    /// </param>
    /// <param name="actualCallsNumber">
    ///     The actual number of matching calls.
    /// </param>
    public MockVerificationException(int expectedCallsNumber, int actualCallsNumber)
        : base(FormatMessage(expectedCallsNumber, actualCallsNumber))
    {
        ExpectedCallsNumber = expectedCallsNumber;
        ActualCallsNumber = actualCallsNumber;
    }

    private static string FormatMessage(int expectedCallsNumber, int actualCallsNumber)
    {
        return $"""
                Mock verification failed.

                Expected: At least {expectedCallsNumber} call(s) matching the predicate
                Actual:   {actualCallsNumber} matching call(s) found
                """;
    }
}