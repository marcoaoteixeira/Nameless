using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[Collection(nameof(RabbitContainerCollection))]
public class ConnectionManagerTests {
    private readonly RabbitContainer _container;

    public ConnectionManagerTests(RabbitContainer container) {
        _container = container;
    }

    [Fact]
    public async Task WhenGettingConnection_ThenReturnsRabbitMQConnection() {
        // arrange
        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions());
        var sut = new ConnectionManager(options);

        // act
        var actual = await sut.GetConnectionAsync(CancellationToken.None);

        // assert
        Assert.NotNull(actual);

        await sut.DisposeAsync();
    }
}
