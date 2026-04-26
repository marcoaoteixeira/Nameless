using Microsoft.Extensions.Configuration;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IChannelFactory"/> for creating RabbitMQ channels. 
/// </summary>
public sealed class ChannelFactory : IChannelFactory {
    private readonly IConfiguration _configuration;
    private readonly IConnectionManager _connectionManager;

    /// <summary>
    /// Initializes a new instance of <see cref="ChannelFactory"/>.
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <param name="connectionManager">The connection manager.</param>
    public ChannelFactory(IConfiguration configuration, IConnectionManager connectionManager) {
        _configuration = configuration;
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

        var options = GetQueueOptions(queueName);

        await ConfigureQueueAsync(channel, queueName, options, cancellationToken).SkipContextSync();
        await ConfigureQueueBindingsAsync(channel, queueName, options, cancellationToken).SkipContextSync();
        await ConfigurePrefetchAsync(channel, cancellationToken).SkipContextSync();
    }

    private QueueOptions GetQueueOptions(string queueName) {
        return _configuration.GetQueueOptions(queueName);
    }

    private static async Task ConfigureQueueAsync(IChannel channel, string queueName, QueueOptions options, CancellationToken cancellationToken) {
        var result = await channel.QueueDeclareAsync(
            queueName,
            options.Durable,
            options.Exclusive,
            options.AutoDelete,
            options.Arguments,
            cancellationToken: cancellationToken
        ).SkipContextSync();

        if (string.IsNullOrWhiteSpace(result.QueueName)) {
            throw new InvalidOperationException($"Unable to declare named queue '{queueName}'.");
        }
    }

    private static async Task ConfigureQueueBindingsAsync(IChannel channel, string queueName, QueueOptions options, CancellationToken cancellationToken) {
        foreach (var binding in options.Bindings) {
            await channel.QueueBindAsync(
                queueName,
                options.ExchangeName,
                binding.RoutingKey,
                binding.Arguments,
                cancellationToken: cancellationToken
            ).SkipContextSync();
        }
    }

    private Task ConfigurePrefetchAsync(IChannel channel, CancellationToken cancellationToken) {
        var prefetch = _configuration.GetPrefetchOptions();

        if (!prefetch.IsEnabled) { return Task.CompletedTask; }

        return channel.BasicQosAsync(
            prefetch.Size,
            prefetch.Count,
            prefetch.Global,
            cancellationToken
        );
    }
}