using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IChannelConfigurator"/> for configuring RabbitMQ channels.
/// </summary>
public sealed class ChannelConfigurator : IChannelConfigurator {
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly ILogger<ChannelConfigurator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelConfigurator"/> class.
    /// </summary>
    /// <param name="options">The RabbitMQ options.</param>
    /// <param name="logger">The logger.</param>
    public ChannelConfigurator(IOptions<RabbitMQOptions> options, ILogger<ChannelConfigurator> logger) {
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    public async Task ConfigureAsync(IChannel channel, string queueName, bool usePrefetch, CancellationToken cancellationToken) {
        Prevent.Argument.Null(channel);
        Prevent.Argument.NullOrWhiteSpace(queueName);

        if (!TryGetQueueSettings(queueName, out var queueSettings)) {
            _logger.QueueSettingsNotFound(queueName);

            return;
        }

        if (TryGetExchangeSettings(queueSettings.ExchangeName, out var exchangeSettings)) {
            await channel.ExchangeDeclareAsync(
                              exchangeSettings.Name,
                              exchangeSettings.Type.GetDescription(),
                              exchangeSettings.Durable,
                              exchangeSettings.AutoDelete,
                              exchangeSettings.Arguments,
                              passive: true,
                              cancellationToken: cancellationToken)
                         .ConfigureAwait(false);
        }

        var queueDeclareResult = await channel.QueueDeclareAsync(queueSettings.Name,
                                                   queueSettings.Durable,
                                                   queueSettings.Exclusive,
                                                   queueSettings.AutoDelete,
                                                   queueSettings.Arguments,
                                                   cancellationToken: cancellationToken)
                                              .ConfigureAwait(false);

        foreach (var binding in queueSettings.Bindings) {
            await channel.QueueBindAsync(queueDeclareResult.QueueName,
                              queueSettings.ExchangeName,
                              binding.RoutingKey,
                              binding.Arguments,
                              cancellationToken: cancellationToken)
                         .ConfigureAwait(false);
        }

        if (usePrefetch) {
            var prefetch = _options.Value.Prefetch;
            await channel.BasicQosAsync(prefetch.Size,
                              prefetch.Count,
                              prefetch.Global,
                              cancellationToken)
                         .ConfigureAwait(false);
        }
    }

    private bool TryGetExchangeSettings(string exchangeName, [NotNullWhen(returnValue: true)] out ExchangeSettings? output) {
        output = _options.Value.Exchanges.SingleOrDefault(Filter);

        return output is not null;

        bool Filter(ExchangeSettings exchange) {
            return string.Equals(exchange.Name, exchangeName, StringComparison.Ordinal);
        }
    }

    private bool TryGetQueueSettings(string queueName, [NotNullWhen(returnValue: true)] out QueueSettings? output) {
        output = _options.Value.Queues.SingleOrDefault(Filter);

        return output is not null;

        bool Filter(QueueSettings item) {
            return string.Equals(item.Name, queueName, StringComparison.Ordinal);
        }
    }
}
