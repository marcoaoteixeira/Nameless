using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for RabbitMQ prefetch.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.PocoStructure)]
public record PrefetchOptions {
    /// <summary>
    ///     Whether it should configure prefetch.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    ///     Gets or sets the prefetch size.
    /// </summary>
    public uint Size { get; init; }

    /// <summary>
    ///     Gets or sets the prefetch count.
    /// </summary>
    public ushort Count { get; init; } = 1;

    /// <summary>
    ///     Whether the prefetch settings are global.
    /// </summary>
    public bool Global { get; init; }
}