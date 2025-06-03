using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IChannelConfigurator"/> for configuring RabbitMQ channels.
/// </summary>
public class ChannelConfigurator : IChannelConfigurator {
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

    public async Task ConfigureChannelAsync(IChannel channel, string exchangeName, CancellationToken cancellationToken) {
        // get the exchange object from options
        var exchange = _options.Value.Exchanges.SingleOrDefault(exchange
            => string.Equals(exchange.Name,
                exchangeName,
                StringComparison.Ordinal));

        // if the exchange object was not found, log and return.
        if (exchange is null) {
            _logger.MissingExchangeOptions(exchangeName);

            return;
        }

        // exchange found. let's declare our exchange
        await channel.ExchangeDeclareAsync(exchange.Name,
                          exchange.Type.GetDescription(),
                          exchange.Durable,
                          exchange.AutoDelete,
                          exchange.Arguments,
                          cancellationToken: cancellationToken)
                     .ConfigureAwait(false);

        foreach (var queue in exchange.Queues) {
            // let's declare our queues
            await channel.QueueDeclareAsync(queue.Name,
                              queue.Durable,
                              queue.Exclusive,
                              queue.AutoDelete,
                              queue.Arguments,
                              cancellationToken: cancellationToken)
                         .ConfigureAwait(false);

            // let's declare our bindings
            foreach (var binding in queue.Bindings) {
                await channel.QueueBindAsync(queue.Name,
                                  exchange.Name,
                                  binding.RoutingKey,
                                  binding.Arguments,
                                  cancellationToken: cancellationToken)
                             .ConfigureAwait(false);
            }
        }
    }
}