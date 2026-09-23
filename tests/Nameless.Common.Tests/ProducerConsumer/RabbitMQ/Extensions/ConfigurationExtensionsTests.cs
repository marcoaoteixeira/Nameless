using Microsoft.Extensions.Configuration;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.ProducerConsumer.RabbitMQ.Extensions;

[UnitTest]
public class ConfigurationExtensionsTests {
    private static IConfiguration Create(Dictionary<string, string?> values) => ConfigurationHelper.CreateConfiguration(values);

    [Fact]
    public void GetServerOptions_WithSection_BindsValues() {
        // arrange
        var configuration = Create(new() { ["RabbitMQ:ServerOptions:Hostname"] = "broker", ["RabbitMQ:ServerOptions:Port"] = "1234" });

        // act
        var actual = configuration.GetServerOptions();

        // assert
        Assert.Multiple(
            () => Assert.Equal("broker", actual.Hostname),
            () => Assert.Equal(1234, actual.Port)
        );
    }

    [Fact]
    public void GetServerOptions_WithoutSection_Throws() {
        // arrange
        var configuration = Create(new());

        // act & assert
        Assert.Throws<InvalidOperationException>(configuration.GetServerOptions);
    }

    [Fact]
    public void GetQueueOptions_WithMatchingQueue_BindsValues() {
        // arrange
        var configuration = Create(new() { ["RabbitMQ:QueueOptions:orders:Durable"] = "true", ["RabbitMQ:QueueOptions:orders:Name"] = "orders" });

        // act
        var actual = configuration.GetQueueOptions("orders");

        // assert
        Assert.Multiple(
            () => Assert.True(actual.Durable),
            () => Assert.Equal("orders", actual.Name)
        );
    }

    [Fact]
    public void GetQueueOptions_WithUnknownQueue_Throws() {
        // arrange
        var configuration = Create(new() { ["RabbitMQ:QueueOptions:orders:Durable"] = "true" });

        // act & assert
        Assert.Throws<InvalidOperationException>(() => configuration.GetQueueOptions("other"));
    }

    [Fact]
    public void GetPrefetchOptions_WithSection_BindsValues() {
        // arrange
        var configuration = Create(new() { ["RabbitMQ:PrefetchOptions:IsEnabled"] = "true", ["RabbitMQ:PrefetchOptions:Count"] = "4" });

        // act
        var actual = configuration.GetPrefetchOptions();

        // assert
        Assert.Multiple(
            () => Assert.True(actual.IsEnabled),
            () => Assert.Equal((ushort)4, actual.Count)
        );
    }

    [Fact]
    public void GetPrefetchOptions_WithoutSection_Throws() {
        // arrange
        var configuration = Create(new());

        // act & assert
        Assert.Throws<InvalidOperationException>(configuration.GetPrefetchOptions);
    }
}
