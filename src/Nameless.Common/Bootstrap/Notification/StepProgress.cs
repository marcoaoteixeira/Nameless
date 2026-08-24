using System.Text.Json;

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
    public int? PercentageComplete { get; init; }

    /// <summary>
    ///     Gets or init the timestamp. Default value is
    ///     <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    ///     Gets any additional metadata for the report.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    ///     Gets or init the progress type.
    /// </summary>
    public StepProgressType Type { get; init; }

    internal string ToText() {
        return $"""
               ----------
               Step name:  {StepName}
               Timestamp:  {Timestamp:yyyy-MM-dd HH:mm:ss:fff}
               Message:    {Message}
               Percentage: {(PercentageComplete is not null ? $"{PercentageComplete:P2}" : string.Empty)}
               Type:       {Type}
               Metadata:   {JsonSerializer.Serialize(Metadata)}
               """;
    }
}