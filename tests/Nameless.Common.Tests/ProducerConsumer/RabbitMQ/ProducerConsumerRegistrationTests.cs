using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Resilience;

namespace Nameless.ProducerConsumer.RabbitMQ;

[UnitTest]
public class ProducerConsumerRegistrationTests {
    private sealed class FakeConsumer : Consumer<string> {
        public override string Name => "fake";
        public override string Topic => "fake.queue";

        public FakeConsumer(IChannelFactory channelFactory, IMessageSerializer serializer, IRetryPipelineFactory retryPipelineFactory, ILogger<FakeConsumer> logger)
            : base(channelFactory, serializer, retryPipelineFactory, new ConsumerOptions(), logger) { }

        public override Task ConsumeAsync(string value, ConsumerContext context, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    // ── ProducerConsumerRegistration ─────────────────────────────────────────

    [Fact]
    public void WithConsumer_Generic_AddsType() {
        // arrange
        var sut = new ProducerConsumerRegistration().WithUseAssemblyScan(false);

        // act
        var returned = sut.WithConsumer<FakeConsumer, string>();

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.Contains(typeof(FakeConsumer), sut.Consumers)
        );
    }

    [Fact]
    public void WithConsumer_CalledTwiceWithSameType_AddsOnce() {
        // arrange
        var sut = new ProducerConsumerRegistration().WithUseAssemblyScan(false);

        // act
        sut.WithConsumer(typeof(FakeConsumer)).WithConsumer(typeof(FakeConsumer));

        // assert
        Assert.Single(sut.Consumers);
    }

    [Fact]
    public void WithConsumer_WithNonConsumerType_Throws() {
        // arrange
        var sut = new ProducerConsumerRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithConsumer(typeof(string)));
    }

    [Fact]
    public void WithConsumer_WithAbstractType_Throws() {
        // arrange
        var sut = new ProducerConsumerRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithConsumer(typeof(Consumer<string>)));
    }

    // ── RegisterProducerConsumer ─────────────────────────────────────────────

    private static ServiceProvider BuildProvider(IConfiguration? configuration, Action<ProducerConsumerRegistration>? configure = null, ServiceCollection? services = null) {
        services ??= [];
        services.AddLogging();
        services.AddSingleton(NullRetryPipelineFactory.Instance);
        services.RegisterProducerConsumer(configure ?? (r => r.WithUseAssemblyScan(false)), configuration);

        return services.BuildServiceProvider();
    }

    [Fact]
    public void RegisterProducerConsumer_BindsRabbitMQOptionsFromConfiguration() {
        // arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["RabbitMQ:Server:Hostname"] = "broker.local",
                ["RabbitMQ:Queues:0:Name"] = "orders",
                ["RabbitMQ:Queues:0:Durable"] = "true"
            })
            .Build();

        using var provider = BuildProvider(configuration);

        // act
        var options = provider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

        // assert
        Assert.Multiple(
            () => Assert.Equal("broker.local", options.Server.Hostname),
            () => Assert.Equal("orders", options.Queues.Single().Name),
            () => Assert.True(options.Queues.Single().Durable)
        );
    }

    [Fact]
    public async Task RegisterProducerConsumer_RegistersInfrastructureServices() {
        // arrange
        using var provider = BuildProvider(configuration: null);

        // act & assert
        Assert.Multiple(
            () => Assert.IsType<ConnectionManager>(provider.GetRequiredService<IConnectionManager>()),
            () => Assert.IsType<ChannelFactory>(provider.GetRequiredService<IChannelFactory>()),
            () => Assert.IsType<JsonMessageSerializer>(provider.GetRequiredService<IMessageSerializer>()),
            () => Assert.IsType<Producer>(provider.GetRequiredService<IProducer>())
        );

        await provider.DisposeAsync();
    }

    [Fact]
    public void RegisterProducerConsumer_RegistersConfiguredConsumersAsHostedServices() {
        // arrange
        var services = new ServiceCollection();

        // act
        using var _ = BuildProvider(null, r => r.WithUseAssemblyScan(false).WithConsumer<FakeConsumer, string>(), services);

        // assert
        Assert.Contains(services, d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(FakeConsumer));
    }

    [Fact]
    public void RegisterProducerConsumer_CalledTwice_KeepsSingleProducerRegistration() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NullRetryPipelineFactory.Instance);

        // act
        services.RegisterProducerConsumer(r => r.WithUseAssemblyScan(false));
        services.RegisterProducerConsumer(r => r.WithUseAssemblyScan(false));

        // assert
        Assert.Single(services, d => d.ServiceType == typeof(IProducer));
    }
}
