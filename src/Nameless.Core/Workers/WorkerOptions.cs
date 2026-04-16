using Nameless.Attributes;

namespace Nameless.Workers;

[ConfigurationSectionName("Workers")]
public record WorkerOptions {
    public bool IsEnabled { get; init; }

    public TimeSpan Interval { get; init; }
}