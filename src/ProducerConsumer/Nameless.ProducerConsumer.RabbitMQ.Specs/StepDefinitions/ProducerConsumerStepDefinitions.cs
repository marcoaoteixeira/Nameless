using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using NUnit.Framework;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Specs.StepDefinitions {
    [Ignore(reason: "This is an integration test. Must run in a specific environment.")]
    [Binding]
    public sealed class ProducerConsumerStepDefinitions {
        private const string EXCHANGE_NAME = "ex.default";
        private const string QUEUE_NAME = "q.default";

        private IChannelFactory? _channelFactory;
        private IModel? _channel;
        private IProducerService? _producerService;
        private IConsumerService? _consumerService;

        private string? _expected;
        private string? _message;

        [Given(@"that I have a ChannelFactory")]
        public void GivenThatIHaveAChannelFactory()
            => _channelFactory = new ChannelFactory();

        [Given(@"that I have a Channel")]
        public void GivenThatIHaveAChannel() {
            _channel = _channelFactory!.CreateChannel();

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
        }

        [Given(@"that I have a ProducerService")]
        public void GivenThatIHaveAProducerService()
            => _producerService = new ProducerService(_channel!);

        [Given(@"that I have a ConsumerService")]
        public void GivenThatIHaveAConsumerService()
            => _consumerService = new ConsumerService(_channel!);

        [Given(@"that I create a Registration using ConsumerService")]
        public void GivenThatICreateARegistrationUsingConsumerService() {
            var args = ConsumerArgs.Empty;

            args.SetAutoAck(true);

            _ = _consumerService!.Register<string>(
                topic: string.Empty,
                handler: Handler,
                args: args
            );
        }

        [When(@"I use the ProducerService to publish a message with content (.*)")]
        public async Task WhenIUseTheProducerServiceToPublishAMessageWithContent(string message) {
            _expected = message;

            var args = new ProducerArgs();
            args.SetExchangeName(EXCHANGE_NAME);

            await _producerService!.ProduceAsync(string.Empty, message, args);
        }

        [Then(@"the Handler associated with the Registration should capture the message")]
        public void ThenTheHandlerAssociatedWithTheRegistrationShouldCaptureTheMessage() {
            Thread.Sleep(250); // this should be smarter but it works as is.
            Assert.That(_expected, Is.EqualTo(_message));
        }

        private Task Handler(string message) {
            _message = message;
            return Task.CompletedTask;
        }
    }
}
