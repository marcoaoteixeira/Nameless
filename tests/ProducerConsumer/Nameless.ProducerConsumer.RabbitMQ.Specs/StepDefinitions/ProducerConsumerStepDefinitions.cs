using Microsoft.Extensions.Logging.Abstractions;
using Nameless.ProducerConsumer.RabbitMQ.Fixtures;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Services;

namespace Nameless.ProducerConsumer.RabbitMQ.StepDefinitions;

[Binding]
public class ProducerConsumerStepDefinitions {
    private const string EXCHANGE_NAME = "ex.default";
    private const string QUEUE_NAME = "q.default";

    private IConnectionManager _connectionManager;
    private IChannelProvider _channelProvider;
    private IProducer _producerService;
    private IConsumer _consumerService;

    private OrderStartedEventProduced _expected;
    private OrderStartedEventReceived _actual;

    [Given(@"there is a RabbitMQ instance configured")]
    public async Task GivenThereIsARabbitMQInstanceConfigured() {
        var rabbitMQOptions = new RabbitMQOptions {
            Exchanges = [new ExchangeSettings {
                Name = EXCHANGE_NAME,
                Queues = [new QueueSettings {
                    Name = QUEUE_NAME,
                    Bindings = [new BindingSettings {
                        RoutingKey = QUEUE_NAME
                    }]
                }]
            }]

        };
        var options = Microsoft.Extensions.Options.Options.Create(rabbitMQOptions);

        _connectionManager = new ConnectionManager(NullLogger<ConnectionManager>.Instance, options);
        _channelProvider = new ChannelProvider(_connectionManager, options);
        _producerService = new Producer(_channelProvider, new SystemClock(), NullLogger<Producer>.Instance);
        _consumerService = new Consumer(_channelProvider, NullLogger<Consumer>.Instance);

        var args = ConsumerArgs.Empty;

        args.SetAutoAck(true);

        try {
            _ = await _consumerService.RegisterAsync<OrderStartedEventReceived>(topic: EXCHANGE_NAME,
                                                                                messageHandler: Handler,
                                                                                args: args,
                                                                                cancellationToken: CancellationToken.None);
        } catch (Exception ex) { Assert.Inconclusive(ex.Message); }
    }

    [When(@"we produce a OrderStartedEvent message with and ID and Date")]
    public async Task WhenWeProduceAOrderStartedEventMessageWithAndIDAndDate() {
        var days = Random.Shared.Next(-5000, 5000);

        _expected = new OrderStartedEventProduced {
            Id = Guid.NewGuid(),
            Date = DateTime.Now.AddDays(days)
        };

        var args = new ProducerArgs();
        args.SetExchangeName(EXCHANGE_NAME);

        await _producerService!.ProduceAsync(string.Empty, _expected!, args, CancellationToken.None);
    }

    [Then(@"the ConsumerService should receive the message and call its handler")]
    public void ThenTheConsumerServiceShouldReceiveTheMessageAndCallItsHandler() {
        Thread.Sleep(250); // this should be smarter but it works as is.
        Assert.Multiple(() => {
            Assert.That(_actual.Id, Is.EqualTo(_expected.Id));
            Assert.That(_actual.Date, Is.EqualTo(_expected.Date));
        });
    }

    private Task Handler(OrderStartedEventReceived message, CancellationToken cancellationToken) {
        _actual = message;
        return Task.CompletedTask;
    }
}