using Microsoft.Extensions.Logging;
using Moq;
using Nameless.ProducerConsumer.RabbitMQ.Mockers;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class ChannelConfiguratorTests {
    [Fact]
    public async Task WhenConfiguring_WithQueueSettings_ThenConfigureChannelWithQueue() {
        // arrange
        const string QueueName = "test-queue";
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Queues = [
                new QueueSettings {
                    Name = QueueName,
                    Durable = true,
                    Exclusive = false,
                    AutoDelete = false,
                    Bindings = []
                }
            ];
        });
        var channelMocker = new ChannelMocker().WithQueueDeclareAsync();

        var sut = new ChannelConfigurator(options, Quick.Mock<ILogger<ChannelConfigurator>>());

        // act
        await sut.ConfigureAsync(channelMocker.Build(), QueueName, usePrefetch: false, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            channelMocker.VerifyExchangeDeclareAsync(Times.Never());
            channelMocker.VerifyQueueDeclareAsync();
            channelMocker.VerifyQueueBindAsync(Times.Never());
            channelMocker.VerifyBasicQosAsync(Times.Never());
        });
    }

    [Fact]
    public async Task WhenConfiguring_WithExchangeSettings_ThenConfigureChannelWithExchange() {
        // arrange
        const string QueueName = "test-queue";
        const string ExchangeName = "test-exchange";
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Exchanges = [
                new ExchangeSettings {
                    Name = ExchangeName,
                    Type = ExchangeType.Direct,
                    Durable = true,
                    AutoDelete = false
                }
            ];

            opts.Queues = [
                new QueueSettings {
                    Name = QueueName,
                    ExchangeName = ExchangeName,
                    Durable = true,
                    Exclusive = false,
                    AutoDelete = false,
                    Bindings = []
                }
            ];
        });
        var channelMocker = new ChannelMocker().WithQueueDeclareAsync();

        var sut = new ChannelConfigurator(options, Quick.Mock<ILogger<ChannelConfigurator>>());

        // act
        await sut.ConfigureAsync(channelMocker.Build(), QueueName, usePrefetch: false, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            channelMocker.VerifyExchangeDeclareAsync();
            channelMocker.VerifyQueueDeclareAsync();
            channelMocker.VerifyQueueBindAsync(Times.Never());
            channelMocker.VerifyBasicQosAsync(Times.Never());
        });
    }

    [Fact]
    public async Task WhenConfiguring_WhenQueueHaveBindings_ThenConfigureChannelWithQueueBindings() {
        // arrange
        const string QueueName = "test-queue";
        const string RoutingKey = "test-routing-key";
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Queues = [
                new QueueSettings {
                    Name = QueueName,
                    Durable = true,
                    Exclusive = false,
                    AutoDelete = false,
                    Bindings = [new BindingSettings { RoutingKey = RoutingKey }]
                }
            ];
        });
        var channelMocker = new ChannelMocker().WithQueueDeclareAsync();

        var sut = new ChannelConfigurator(options, Quick.Mock<ILogger<ChannelConfigurator>>());

        // act
        await sut.ConfigureAsync(channelMocker.Build(), QueueName, usePrefetch: false, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            channelMocker.VerifyExchangeDeclareAsync(Times.Never());
            channelMocker.VerifyQueueDeclareAsync();
            channelMocker.VerifyQueueBindAsync();
            channelMocker.VerifyBasicQosAsync(Times.Never());
        });
    }

    [Fact]
    public async Task WhenConfiguring_WhenQueueUsePrefetch_ThenConfigureChannelWithPrefetchSettings() {
        // arrange
        const string QueueName = "test-queue";
        const bool UsePrefetch = true;
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Queues = [new QueueSettings { Name = QueueName, }];
        });
        var channelMocker = new ChannelMocker().WithQueueDeclareAsync();

        var sut = new ChannelConfigurator(options, Quick.Mock<ILogger<ChannelConfigurator>>());

        // act
        await sut.ConfigureAsync(channelMocker.Build(), QueueName, UsePrefetch, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            channelMocker.VerifyExchangeDeclareAsync(Times.Never());
            channelMocker.VerifyQueueDeclareAsync();
            channelMocker.VerifyQueueBindAsync(Times.Never());
            channelMocker.VerifyBasicQosAsync();
        });
    }

    [Fact]
    public async Task WhenConfiguring_WhenQueueSettingsNotFound_ThenDoNotConfigureChannel() {
        // arrange
        const string QueueName = "test-queue";
        const bool UsePrefetch = true;
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Queues = [new QueueSettings { Name = QueueName, }];
        });
        var channelMocker = new ChannelMocker().WithQueueDeclareAsync();
        var loggerMocker = new LoggerMocker<ChannelConfigurator>().WithAnyLogLevel();

        var sut = new ChannelConfigurator(options, loggerMocker.Build());

        // act
        await sut.ConfigureAsync(channelMocker.Build(), "other-test-queue", UsePrefetch, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            channelMocker.VerifyExchangeDeclareAsync(Times.Never());
            channelMocker.VerifyQueueDeclareAsync(Times.Never());
            channelMocker.VerifyQueueBindAsync(Times.Never());
            channelMocker.VerifyBasicQosAsync(Times.Never());
            loggerMocker.VerifyWarningCall();
        });
    }
}
