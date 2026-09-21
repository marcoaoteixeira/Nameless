using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IChannelFactory"/> for creating RabbitMQ channels. 
/// </summary>
public sealed class ChannelFactory : IChannelFactory {
    private readonly Dictionary<string, QueueOptions> _queues;

    private readonly IConnectionManager _connectionManager;

    /// <summary>
    /// Initializes a new instance of <see cref="ChannelFactory"/>.
    /// </summary>
    /// <param name="options">The RabbitMQ options</param>
    /// <param name="connectionManager">The connection manager.</param>
    public ChannelFactory(IOptions<RabbitMQOptions> options, IConnectionManager connectionManager) {
        _queues = options.Value.Queues.ToDictionary(
            keySelector: queue => queue.Name,
            elementSelector: queue => queue
        );
        _connectionManager = connectionManager;
    }

    /// <inheritdoc />
    public async Task<IChannel> CreateAsync(string queueName, CancellationToken cancellationToken) {
        var channel = await InnerCreateChannelAsync(cancellationToken).SkipContextSync();

        await ConfigureChannelAsync(channel, queueName, cancellationToken).SkipContextSync();

        return channel;
    }

    private async Task<IChannel> InnerCreateChannelAsync(CancellationToken cancellationToken) {
        var connection = await _connectionManager.GetConnectionAsync(cancellationToken)
                                                 .SkipContextSync();

        var options = new CreateChannelOptions(
            publisherConfirmationTrackingEnabled: false,
            publisherConfirmationsEnabled: false,
            outstandingPublisherConfirmationsRateLimiter: null,
            consumerDispatchConcurrency: 1
        );

        return await connection.CreateChannelAsync(options, cancellationToken)
                               .SkipContextSync();
    }

    private async Task ConfigureChannelAsync(IChannel channel, string queueName, CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(queueName);

        if (!_queues.TryGetValue(queueName, out var queue)) {
            throw new MissingQueueConfigurationException(queueName);
        }

        await ConfigureQueueAsync(channel, queue, cancellationToken).SkipContextSync();
        await ConfigureQueueBindingsAsync(channel, queue, cancellationToken).SkipContextSync();
        await ConfigurePrefetchAsync(channel, queue.Prefetch, cancellationToken).SkipContextSync();
    }

    private static async Task ConfigureQueueAsync(IChannel channel, QueueOptions queue, CancellationToken cancellationToken) {
        var result = await channel.QueueDeclareAsync(
            queue.Name,
            queue.Durable,
            queue.Exclusive,
            queue.AutoDelete,
            queue.Arguments,
            cancellationToken: cancellationToken
        ).SkipContextSync();

        if (string.IsNullOrWhiteSpace(result.QueueName)) {
            throw new InvalidOperationException($"Unable to declare named queue '{queue.Name}'.");
        }
    }

    private static async Task ConfigureQueueBindingsAsync(IChannel channel, QueueOptions queue, CancellationToken cancellationToken) {
        foreach (var binding in queue.Bindings) {
            await channel.QueueBindAsync(
                queue.Name,
                queue.ExchangeName,
                binding.RoutingKey,
                binding.Arguments,
                cancellationToken: cancellationToken
            ).SkipContextSync();
        }
    }

    private static Task ConfigurePrefetchAsync(IChannel channel, PrefetchOptions? prefetch, CancellationToken cancellationToken) {
        if (prefetch is null || !prefetch.IsEnabled) { return Task.CompletedTask; }

        return channel.BasicQosAsync(
            prefetch.Size,
            prefetch.Count,
            prefetch.Global,
            cancellationToken
        );
    }
}