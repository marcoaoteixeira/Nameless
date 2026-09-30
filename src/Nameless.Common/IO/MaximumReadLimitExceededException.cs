namespace Nameless.IO;

/// <summary>
///     Thrown when a <see cref="CappedReadStream"/> is asked to read more
///     bytes than its limit allows.
/// </summary>
/// <remarks>
///     Derives from <see cref="IOException"/> because, to the consumer, it
///     is a failure of the underlying read (the data source is bigger than
///     it was allowed to be).
/// </remarks>
public sealed class MaximumReadLimitExceededException : IOException {
    /// <summary>
    ///     Gets the maximum number of bytes that could be read.
    /// </summary>
    public long Limit { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="MaximumReadLimitExceededException"/> class.
    /// </summary>
    /// <param name="limit">
    ///     Maximum number of bytes that could be read.
    /// </param>
    public MaximumReadLimitExceededException(long limit)
        : this(limit, $"The maximum read limit of {limit} bytes was exceeded.") { }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="MaximumReadLimitExceededException"/> class.
    /// </summary>
    /// <param name="limit">
    ///     Maximum number of bytes that could be read.
    /// </param>
    /// <param name="message">
    ///     The message.
    /// </param>
    public MaximumReadLimitExceededException(long limit, string message)
        : base(message) { Limit = limit; }
}

