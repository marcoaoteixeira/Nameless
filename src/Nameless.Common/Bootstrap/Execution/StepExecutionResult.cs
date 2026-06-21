namespace Nameless.Bootstrap.Execution;

/// <summary>
///     Contains the execution outcome for a single bootstrap step.
/// </summary>
public class StepExecutionResult {
    /// <summary>
    ///     Gets or sets the display name of the step.
    /// </summary>
    public required string StepName { get; set; }

    /// <summary>
    ///     Gets or sets the UTC time at which the step started.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    ///     Gets or sets the total elapsed time for the step.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    ///     Gets or sets the exception that occurred during the step, if any.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the step completed without error.
    /// </summary>
    public bool Success => Exception is null;
}