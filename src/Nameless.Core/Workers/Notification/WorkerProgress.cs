namespace Nameless.Workers.Notification;

/// <summary>
///     A progress notification emitted by a <see cref="Worker" /> during
///     its execution cycle.
/// </summary>
public sealed record WorkerProgress {
    /// <summary>
    ///     Gets the name of the worker that produced this notification.
    /// </summary>
    public required string WorkerName { get; init; }

    /// <summary>
    ///     Gets the human-readable message describing the current state.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    ///     Gets the completion percentage, or <see langword="null" /> when
    ///     undetermined.
    /// </summary>
    public int? PercentageComplete { get; init; }

    /// <summary>
    ///     Gets the UTC timestamp at which this notification was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    ///     Gets any additional metadata attached to this notification.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    ///     Gets the type of this progress notification.
    /// </summary>
    public WorkerProgressType Type { get; init; }
}
