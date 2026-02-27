namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for RabbitMQ prefetch.
/// </summary>
public class PrefetchOptions {
    /// <summary>
    ///     Gets or sets the prefetch size.
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    ///     Gets or sets the prefetch count.
    /// </summary>
    public ushort Count { get; set; } = 1;

    /// <summary>
    ///     Whether the prefetch settings are global.
    /// </summary>
    public bool Global { get; set; }
}