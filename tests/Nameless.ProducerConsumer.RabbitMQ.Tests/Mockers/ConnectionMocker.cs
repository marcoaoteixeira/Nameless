using Moq;
using Nameless.Testing.Tools.Mockers;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Mockers;

public sealed class ConnectionMocker : Mocker<IConnection> {
    public ConnectionMocker WithCreateChannelAsync(IChannel result = null) {
        result ??= Mock.Of<IChannel>();

        MockInstance.Setup(mock =>
                        mock.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(result);

        return this;
    }
}