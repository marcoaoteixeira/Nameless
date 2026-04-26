namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for a RabbitMQ binding.
/// </summary>
public record BindingOptions {
    /// <summary>
    ///     Gets or sets the routing key for the binding.
    /// </summary>
    public string RoutingKey { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the binding arguments.
    /// </summary>
    public Dictionary<string, object?> Arguments { get; init; } = [];
}