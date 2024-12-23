using Nameless.PubSub.RabbitMQ.Options;
using Nameless.PubSub.RabbitMQ.Utils;
using Shouldly;

namespace Nameless.PubSub.RabbitMQ;
public class SubscriberTests {
    [Test]
    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    public async Task WhenSubscribeForTopic_ThenReturnsSubscription() {
        // arrange
        var exchangeName = $"exchange_{Guid.NewGuid():N}";
        var queueName = $"queue_{Guid.NewGuid():N}";
        var routingKey = $"routing_key_{Guid.NewGuid():N}";
        using var host = new SimpleHost(opts => {
            opts.Exchanges = [new ExchangeSettings {
                Name = exchangeName,
                AutoDelete = true,
                Type = ExchangeType.Topic,
                Queues = [new QueueSettings {
                    Name = queueName,
                    Exclusive = true,
                    Bindings = [new BindingSettings {
                        RoutingKey = routingKey
                    }]
                }]
            }];
        });
        var args = new SubscriberArgs();

        args.SetQueueName(queueName);

        // act
        var subscription = await host.Subscriber.SubscribeAsync(exchangeName,
                                                                HandleAsync,
                                                                args,
                                                                CancellationToken.None);

        subscription.ShouldNotBeNullOrWhiteSpace();

        return;

        Task HandleAsync(object message, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
    }

    [Test]
    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    public async Task WhenUnsubscribe_ThenReturnsTrue() {
        // arrange
        var exchangeName = $"exchange_{Guid.NewGuid():N}";
        var queueName = $"queue_{Guid.NewGuid():N}";
        var routingKey = $"routing_key_{Guid.NewGuid():N}";
        using var host = new SimpleHost(opts => {
            opts.Exchanges = [new ExchangeSettings {
                Name = exchangeName,
                AutoDelete = true,
                Type = ExchangeType.Topic,
                Queues = [new QueueSettings {
                    Name = queueName,
                    Exclusive = true,
                    Bindings = [new BindingSettings {
                        RoutingKey = routingKey
                    }]
                }]
            }];
        });
        var args = new SubscriberArgs();

        args.SetQueueName(queueName);

        // act
        var subscription = await host.Subscriber.SubscribeAsync(exchangeName,
                                                                HandleAsync,
                                                                args,
                                                                CancellationToken.None);

        subscription.ShouldNotBeNullOrWhiteSpace();

        var unsubscribe = await host.Subscriber.UnsubscribeAsync(subscription,
                                                                 CancellationToken.None);

        unsubscribe.ShouldBeTrue();

        return;

        Task HandleAsync(object message, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
    }
}
