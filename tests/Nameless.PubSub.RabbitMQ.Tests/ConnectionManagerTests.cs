using Nameless.PubSub.RabbitMQ.Utils;
using Nameless.Testing.Tools;

namespace Nameless.PubSub.RabbitMQ;

public class ConnectionManagerTests {
    [Test]
    [Category(Categories.RunsOnDevMachine)]
    public async Task WhenGetConnectionAsync_ThenConnectToRabbitMQ() {
        // arrange
        using var host = new SimpleHost(_ => { });

        // act
        await using var connection = await host.ConnectionManager.GetConnectionAsync(CancellationToken.None);

        // assert
        Assert.That(connection, Is.Not.Null);
    }
}