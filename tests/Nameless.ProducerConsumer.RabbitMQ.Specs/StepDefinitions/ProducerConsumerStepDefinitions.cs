using Microsoft.Extensions.Logging.Abstractions;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using Nameless.ProducerConsumer.RabbitMQ.Specs.Fixtures;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Nameless.ProducerConsumer.RabbitMQ.StepDefinitions;

[Binding]
public class ProducerConsumerStepDefinitions {
    private const string EXCHANGE_NAME = "ex.default";
    private const string QUEUE_NAME = "q.default";

    private IChannelFactory _channelFactory;
    private IModel _channel;
    private IProducerService _producerService;
    private IConsumerService _consumerService;

    private OrderStartedEventProduced _expected;
    private OrderStartedEventReceived _actual;
    

    [Given(@"there is a RabbitMQ instance configured")]
    public void GivenThereIsARabbitMQInstanceConfigured() {
        _channelFactory = new ChannelFactory(options: new RabbitMQOptions(),
                                             logger: NullLogger<ChannelFactory>.Instance);
        try {
            _channel = _channelFactory.CreateChannel();
        } catch (BrokerUnreachableException) {
            Assert.Inconclusive("Broker not available");
            return;
        }

        // assert exchange/queue
        _channel.ExchangeDeclare(exchange: EXCHANGE_NAME,
                                 type: "direct",
                                 durable: false,
                                 autoDelete: true,
                                 arguments: new Dictionary<string, object>());

        _channel.QueueDeclare(queue: QUEUE_NAME,
                              durable: false,
                              exclusive: false,
                              autoDelete: true,
                              arguments: new Dictionary<string, object>());

        _channel.QueueBind(queue: QUEUE_NAME,
                           exchange: EXCHANGE_NAME,
                           routingKey: string.Empty,
                           arguments: new Dictionary<string, object>());

        _producerService = new ProducerService(_channel, NullLogger<ProducerService>.Instance);
        _consumerService = new ConsumerService(_channel, NullLogger<ConsumerService>.Instance);

        var args = ConsumerArgs.Empty;

        args.SetAutoAck(true);

        _ = _consumerService.Register<OrderStartedEventReceived>(topic: string.Empty,
                                                                 handler: Handler,
                                                                 args: args);
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

    private Task Handler(OrderStartedEventReceived message) {
        _actual = message;
        return Task.CompletedTask;
    }
}