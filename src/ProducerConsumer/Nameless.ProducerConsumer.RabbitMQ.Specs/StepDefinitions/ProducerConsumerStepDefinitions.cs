using Microsoft.Extensions.Logging.Abstractions;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using NUnit.Framework;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Specs.StepDefinitions {
    [Binding]
    public class ProducerConsumerStepDefinitions {
        private const string EXCHANGE_NAME = "ex.default";
        private const string QUEUE_NAME = "q.default";

        private IChannelFactory? _channelFactory;
        private IModel? _channel;
        private IProducerService? _producerService;
        private IConsumerService? _consumerService;

        private string? _actual;
        private string? _expected;

        [Given(@"that I have the correct infrastructure for sending messages")]
        public void GivenThatIHaveTheCorrectInfrastructureForSendingMessages() {
            _channelFactory = new ChannelFactory(
                options: new RabbitMQOptions(),
                logger: NullLogger<ChannelFactory>.Instance);
            _channel = _channelFactory.CreateChannel();

            // assert exchange/queue
            _channel.ExchangeDeclare(
                exchange: EXCHANGE_NAME,
                type: "direct",
                durable: false,
                autoDelete: true,
                arguments: new Dictionary<string, object>()
            );
            _channel.QueueDeclare(
                queue: QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: new Dictionary<string, object>()
            );
            _channel.QueueBind(
                queue: QUEUE_NAME,
                exchange: EXCHANGE_NAME,
                routingKey: string.Empty,
                arguments: new Dictionary<string, object>()
            );

            _producerService = new ProducerService(_channel, NullLogger<ProducerService>.Instance);
            _consumerService = new ConsumerService(_channel, NullLogger<ConsumerService>.Instance);

            var args = ConsumerArgs.Empty;

            args.SetAutoAck(true);

            _ = _consumerService.Register<string>(
                topic: string.Empty,
                handler: Handler,
                args: args
            );
        }

        [Given(@"the message value will be ""([^""]*)""")]
        public void GivenTheMessageValueWillBe(string message)
            => _expected = message;

        [When(@"I call ProducerService ProduceAsync with that said message")]
        public async Task WhenICallProducerServiceProduceAsyncWithThatSaidMessage() {
            var args = new ProducerArgs();
            args.SetExchangeName(EXCHANGE_NAME);

            await _producerService!.ProduceAsync(string.Empty, _expected!, args, CancellationToken.None);
        }

        [Then(@"the handler created by ConsumerService Register should capture the message")]
        public void ThenTheHandlerCreatedByConsumerServiceRegisterShouldCaptureTheMessage() {
            Thread.Sleep(250); // this should be smarter but it works as is.
            Assert.That(_actual, Is.EqualTo(_expected));
        }

        private Task Handler(string message) {
            _actual = message;
            return Task.CompletedTask;
        }
    }
}
