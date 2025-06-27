using Moq;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.Testing.Tools.Mockers;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Mockers;

public sealed class ConnectionManagerMocker : Mocker<IConnectionManager> {
    public ConnectionManagerMocker WithGetConnectionAsync(IConnection connection) {
        MockInstance.Setup(mock => mock.GetConnectionAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(connection);

        return this;
    }
}