using Nameless.Attributes;

namespace Nameless.Bootstrap;

/// <summary>
///     Provides configuration options for registering and managing bootstrap
///     steps in an application startup sequence.
/// </summary>
[ConfigurationSectionName("Bootstrap")]
public record BootstrapOptions {
    /// <summary>
    ///     Whether it should execute the steps in parallel or not.
    /// </summary>
    public bool EnableParallelExecution { get; init; }

    /// <summary>
    ///     Gets or sets the maximum degree of parallelism.
    /// </summary>
    /// <remarks>
    ///     Set the value to <c>-1</c> to define it as unlimited.
    /// </remarks>
    public int MaxDegreeOfParallelism { get; init; } = Environment.ProcessorCount;
}