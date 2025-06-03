using Nameless.PubSub.RabbitMQ.Utils;
using Nameless.Testing.Tools;

namespace Nameless.PubSub.RabbitMQ;

public class ChannelFactoryTests {
    [Test]
    [Category(Categories.RunsOnDevMachine)]
    public async Task WhenCreatingAChannel_ThenReturnsChannel() {
        // arrange
        var exchangeName = $"exchange_{Guid.NewGuid():N}";
        var queueName = $"queue_{Guid.NewGuid():N}";
        var routingKey = $"routing_key_{Guid.NewGuid():N}";
        using var host = new SimpleHost(opts => {
            opts.Exchanges = [
                new ExchangeSettings {
                    Name = exchangeName,
                    AutoDelete = true,
                    Type = ExchangeType.Topic,
                    Queues = [
                        new QueueSettings {
                            Name = queueName,
                            Exclusive = true,
                            Bindings = [new BindingSettings { RoutingKey = routingKey }]
                        }
                    ]
                }
            ];
        });

        // act
        await using var channel = await host.ChannelFactory.CreateChannelAsync(exchangeName, CancellationToken.None);

        // assert
        Assert.That(channel, Is.Not.Null);
    }
}