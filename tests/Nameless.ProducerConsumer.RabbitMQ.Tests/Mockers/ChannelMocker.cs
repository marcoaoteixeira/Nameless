using Moq;
using Nameless.Testing.Tools.Mockers;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Mockers;

public class ChannelMocker : Mocker<IChannel> {
    public ChannelMocker WithQueueDeclareAsync(QueueDeclareOk result = null) {
        MockInstance.Setup(mock => mock.QueueDeclareAsync(
                        It.IsAny<string>(), // queue name
                        It.IsAny<bool>(), // durable
                        It.IsAny<bool>(), // exclusive
                        It.IsAny<bool>(), // autoDelete
                        It.IsAny<IDictionary<string, object>>(), // arguments
                        It.IsAny<bool>(), // passive
                        It.IsAny<bool>(), // no wait
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(result ?? new QueueDeclareOk(string.Empty, messageCount: 0, consumerCount: 0));

        return this;
    }

    public void VerifyQueueDeclareAsync(Times? times = null) {
        MockInstance.Verify(mock => mock.QueueDeclareAsync(
                It.IsAny<string>(), // queue name
                It.IsAny<bool>(), // durable
                It.IsAny<bool>(), // exclusive
                It.IsAny<bool>(), // autoDelete
                It.IsAny<IDictionary<string, object>>(), // arguments
                It.IsAny<bool>(), // passive
                It.IsAny<bool>(), // no wait
                It.IsAny<CancellationToken>()),
            times ?? Times.Once());
    }

    public ChannelMocker WithExchangeDeclareAsync() {
        MockInstance.Setup(mock => mock.ExchangeDeclareAsync(
                        It.IsAny<string>(), // exchange name
                        It.IsAny<string>(), // exchange type
                        It.IsAny<bool>(), // durable
                        It.IsAny<bool>(), // autoDelete
                        It.IsAny<IDictionary<string, object>>(), // arguments
                        It.IsAny<bool>(), // passive
                        It.IsAny<bool>(), // no wait
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        return this;
    }

    public void VerifyExchangeDeclareAsync(Times? times = null) {
        MockInstance.Verify(mock => mock.ExchangeDeclareAsync(
                It.IsAny<string>(), // exchange name
                It.IsAny<string>(), // exchange type
                It.IsAny<bool>(), // durable
                It.IsAny<bool>(), // autoDelete
                It.IsAny<IDictionary<string, object>>(), // arguments
                It.IsAny<bool>(), // passive
                It.IsAny<bool>(), // no wait
                It.IsAny<CancellationToken>()),
            times ?? Times.Once());
    }

    public ChannelMocker WithQueueBindAsync() {
        MockInstance.Setup(mock => mock.QueueBindAsync(
                        It.IsAny<string>(), // queue name
                        It.IsAny<string>(), // exchange name
                        It.IsAny<string>(), // routing key
                        It.IsAny<IDictionary<string, object>>(), // arguments
                        It.IsAny<bool>(), // no wait
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        return this;
    }

    public void VerifyQueueBindAsync(Times? times = null) {
        MockInstance.Verify(mock => mock.QueueBindAsync(
                It.IsAny<string>(), // queue name
                It.IsAny<string>(), // exchange name
                It.IsAny<string>(), // routing key
                It.IsAny<IDictionary<string, object>>(), // arguments
                It.IsAny<bool>(), // no wait
                It.IsAny<CancellationToken>()),
            times ?? Times.Once());
    }

    public ChannelMocker WithBasicQosAsync() {
        MockInstance.Setup(mock => mock.BasicQosAsync(
                        It.IsAny<uint>(), // prefetch size
                        It.IsAny<ushort>(), // prefetch count
                        It.IsAny<bool>(), // global
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        return this;
    }

    public void VerifyBasicQosAsync(Times? times = null) {
        MockInstance.Verify(mock => mock.BasicQosAsync(
                It.IsAny<uint>(), // prefetch size
                It.IsAny<ushort>(), // prefetch count
                It.IsAny<bool>(), // global
                It.IsAny<CancellationToken>()),
            times ?? Times.Once());
    }
}