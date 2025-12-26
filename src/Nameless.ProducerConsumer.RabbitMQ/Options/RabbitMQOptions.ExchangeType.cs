using System.ComponentModel;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Types of RabbitMQ exchanges.
/// </summary>
public enum ExchangeType {
    /// <summary>
    ///     Direct exchange type, where messages are routed to queues based on
    ///     the routing key.
    /// </summary>
    [Description(description: "direct")]
    Direct = 0,

    /// <summary>
    ///     Topic exchange type, where messages are routed to queues based on
    ///     wildcard matches of the routing key.
    /// </summary>
    [Description(description: "topic")]
    Topic = 1,

    /// <summary>
    ///     Queue exchange type, where messages are routed to a single queue.
    /// </summary>
    [Description(description: "queue")]
    Queue = 2,

    /// <summary>
    ///     Fanout exchange type, where messages are broadcast to all bound
    ///     queues.
    /// </summary>
    [Description(description: "fanout")]
    Fanout = 4,

    /// <summary>
    ///     Headers exchange type, where messages are routed based on header
    ///     attributes instead of the routing key.
    /// </summary>
    [Description(description: "headers")]
    Headers = 8,

    /// <summary>
    ///     Consistent hash exchange type, where messages are routed to queues
    ///     based on a consistent hashing algorithm.
    /// </summary>
    [Description(description: "x-consistent-hash")]
    ConsistentHash = 16
}