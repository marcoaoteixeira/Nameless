namespace Nameless.Bootstrap.Notification;

/// <summary>
///     Report for step progress.
/// </summary>
public record StepProgress {
    /// <summary>
    ///     Gets or init the name of the step.
    /// </summary>
    public required string StepName { get; init; }

    /// <summary>
    ///     Gets or init the report message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    ///     Gets or init the percentage completion of the step.
    ///     It returns <see langword="null"/> if undetermined.
    /// </summary>
    public int? PercentComplete { get; init; }

    /// <summary>
    ///     Gets or init the timestamp. Default value is
    ///     <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    ///     Gets any additional metadata for the report.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; } = [];

    /// <summary>
    ///     Gets or init the progress type.
    /// </summary>
    public StepProgressType Type { get; init; }
}