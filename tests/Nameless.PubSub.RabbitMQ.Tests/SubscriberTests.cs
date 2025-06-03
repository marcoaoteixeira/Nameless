using Nameless.PubSub.RabbitMQ.Utils;
using Nameless.Testing.Tools;

namespace Nameless.PubSub.RabbitMQ;

public class SubscriberTests {
    [Test]
    [Category(Categories.RunsOnDevMachine)]
    public async Task WhenSubscribeForTopic_ThenReturnsSubscription() {
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
        var args = new SubscriberArgs();

        args.SetQueueName(queueName);

        // act
        var subscription = await host.Subscriber.SubscribeAsync(exchangeName,
            (_, _) => Task.CompletedTask,
            args,
            CancellationToken.None);

        Assert.That(subscription, Is.Not.Null.Or.WhiteSpace);
    }

    [Test]
    [Category(Categories.RunsOnDevMachine)]
    public async Task WhenUnsubscribe_ThenReturnsTrue() {
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
        var args = new SubscriberArgs();

        args.SetQueueName(queueName);

        // act
        var subscription = await host.Subscriber.SubscribeAsync(exchangeName,
            (_, _) => Task.CompletedTask,
            args,
            CancellationToken.None);

        Assert.That(subscription, Is.Not.Null.Or.WhiteSpace);

        var unsubscribe = await host.Subscriber.UnsubscribeAsync(subscription,
            CancellationToken.None);

        Assert.That(unsubscribe, Is.True);
    }
}