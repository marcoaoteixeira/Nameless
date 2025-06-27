namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for a RabbitMQ binding.
/// </summary>
public sealed record BindingSettings {
    /// <summary>
    ///     Gets or sets the routing key for the binding.
    /// </summary>
    public string RoutingKey { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the binding arguments.
    /// </summary>
    public Dictionary<string, object?> Arguments { get; set; } = [];
}