using Nameless.Attributes;

namespace Nameless.Workers;

/// <summary>
///     Worker options.
/// </summary>
[ConfigurationSectionName("Workers")]
public record WorkerOptions {
    /// <summary>
    ///     Whether the worker is enabled or not.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    ///     Gets the execution interval.
    /// </summary>
    public TimeSpan Interval { get; init; }
}