namespace Nameless.EventSourcing;

/// <summary>
///     Exception thrown when appending events to a stream whose current
///     version no longer matches the version the caller expected,
///     indicating another writer has already appended events since the
///     aggregate was loaded.
/// </summary>
public sealed class ConcurrencyConflictException : Exception {
    /// <summary>
    ///     Gets the identifier of the stream that could not be appended
    ///     to.
    /// </summary>
    public string StreamID { get; }

    /// <summary>
    ///     Gets the version the caller expected the stream to be at.
    /// </summary>
    public int ExpectedVersion { get; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ConcurrencyConflictException"/> class.
    /// </summary>
    /// <param name="streamID">
    ///     The identifier of the stream that could not be appended to.
    /// </param>
    /// <param name="expectedVersion">
    ///     The version the caller expected the stream to be at.
    /// </param>
    public ConcurrencyConflictException(string streamID, int expectedVersion)
        : base($"Stream '{streamID}' was expected to be at version '{expectedVersion}' but has since changed.") {
        StreamID = streamID;
        ExpectedVersion = expectedVersion;
    }
}