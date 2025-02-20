using Nameless.PubSub.RabbitMQ.Utils;
using Shouldly;

namespace Nameless.PubSub.RabbitMQ;
public class ConnectionManagerTests {
    [Test]
    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    public async Task WhenGetConnectionAsync_ThenConnectToRabbitMQ() {
        // arrange
        using var host = new SimpleHost(_ => { });

        // act
        await using var connection = await host.ConnectionManager.GetConnectionAsync(default);

        // assert
        connection.ShouldNotBeNull();
    }
}
