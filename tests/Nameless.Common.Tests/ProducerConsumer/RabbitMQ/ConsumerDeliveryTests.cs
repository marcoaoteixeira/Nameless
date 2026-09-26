using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Resilience;
using Nameless.Testing.Tools.Mockers.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

[UnitTest]
public class ConsumerDeliveryTests {
    public sealed class DeliveryConsumer : Consumer<string> {
        private readonly Func<string, ConsumerContext, Task> _handler;

        public override string Name => "delivery";
        public override string Topic => "delivery.queue";

        public DeliveryConsumer(IChannelFactory channelFactory, IMessageSerializer serializer, IRetryPipelineFactory retryFactory,
                                ConsumerOptions options, ILogger<DeliveryConsumer> logger, Func<string, ConsumerContext, Task> handler)
            : base(channelFactory, serializer, retryFactory, options, logger) {
            _handler = handler;
        }

        public override Task ConsumeAsync(string value, ConsumerContext context, CancellationToken cancellationToken)
            => _handler(value, context);
    }

    private sealed class Fixture {
        public Mock<IChannel> Channel { get; } = new();
        public AsyncEventingBasicConsumer? Consumer { get; private set; }
        public DeliveryConsumer Sut { get; }

        public Fixture(Func<string, ConsumerContext, Task> handler, RetryPolicyOptions? retry = null) {
            Channel.Setup(c => c.BasicConsumeAsync(
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object?>>(), It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object?>?, IAsyncBasicConsumer, CancellationToken>(
                    (_, _, _, _, _, _, consumer, _) => Consumer = (AsyncEventingBasicConsumer)consumer)
                .ReturnsAsync("tag");

            Channel.Setup(c => c.BasicAckAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                   .Returns(ValueTask.CompletedTask);
            Channel.Setup(c => c.BasicNackAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                   .Returns(ValueTask.CompletedTask);
            Channel.Setup(c => c.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            var factory = new Mock<IChannelFactory>();
            factory.Setup(f => f.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(Channel.Object);

            var logger = new LoggerMocker<DeliveryConsumer>().WithAnyLogLevel().Build();
            var retryFactory = new RetryPipelineFactory(new LoggerMocker<RetryPipelineFactory>().WithAnyLogLevel().Build());

            Sut = new DeliveryConsumer(factory.Object, new JsonMessageSerializer(), retryFactory,
                new ConsumerOptions { RetryPolicy = retry }, logger, handler);
        }

        public Task StartAsync() => ((IHostedService)Sut).StartAsync(CancellationToken.None);

        public Task DeliverAsync(string payload = "hello", ulong deliveryTag = 7) {
            var buffer = new JsonMessageSerializer().Serialize(payload, new ProducerContext {
                MessageId = "m-1", CorrelationId = "c-1"
            });

            return Consumer!.HandleBasicDeliverAsync("tag", deliveryTag, false, "ex", "rk", new BasicProperties(), buffer);
        }
    }

    [Fact]
    public async Task Delivery_WhenHandlerSucceeds_AcksMessageAndPopulatesContext() {
        // arrange
        ConsumerContext? seen = null;
        string? value = null;
        var fixture = new Fixture((v, ctx) => { value = v; seen = ctx; return Task.CompletedTask; });
        await fixture.StartAsync();

        // act
        await fixture.DeliverAsync("hello", deliveryTag: 7);

        // assert
        Assert.NotNull(seen);
        Assert.Multiple(
            () => Assert.Equal("hello", value),
            () => Assert.Equal(7UL, seen.DeliveryTag),
            () => Assert.Equal("m-1", seen.MessageId),
            () => Assert.Equal("c-1", seen.CorrelationId)
        );
        fixture.Channel.Verify(c => c.BasicAckAsync(7UL, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delivery_WhenHandlerThrows_NacksWithoutRequeueAndRethrows() {
        // arrange
        var fixture = new Fixture((_, _) => throw new InvalidOperationException("boom"));
        await fixture.StartAsync();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.DeliverAsync());
        fixture.Channel.Verify(c => c.BasicNackAsync(7UL, false, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delivery_WhenHandlerThrowsNoRetryWithRequeue_NacksWithRequeue() {
        // arrange
        var fixture = new Fixture((_, _) => throw new NoRetryException("stop", requeue: true));
        await fixture.StartAsync();

        // act & assert
        await Assert.ThrowsAsync<NoRetryException>(() => fixture.DeliverAsync());
        fixture.Channel.Verify(c => c.BasicNackAsync(7UL, false, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delivery_WhenHandlerIsCancelled_NacksWithRequeue() {
        // arrange
        var fixture = new Fixture((_, _) => throw new OperationCanceledException());
        await fixture.StartAsync();

        // act
        await fixture.DeliverAsync();

        // assert
        fixture.Channel.Verify(c => c.BasicNackAsync(7UL, false, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delivery_WithRetryPolicy_RetriesFailedHandler() {
        // arrange
        var attempts = 0;
        var fixture = new Fixture((_, _) => {
            attempts++;
            return attempts < 3 ? throw new InvalidOperationException("transient") : Task.CompletedTask;
        }, new RetryPolicyOptions {
            RetryCount = 3, InitialDelay = TimeSpan.FromMilliseconds(1), MaxDelay = TimeSpan.FromMilliseconds(5),
            BackoffType = BackoffType.Constant
        });
        await fixture.StartAsync();

        // act
        await fixture.DeliverAsync();

        // assert
        Assert.Equal(3, attempts);
        fixture.Channel.Verify(c => c.BasicAckAsync(7UL, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delivery_WhenAckFails_Rethrows() {
        // arrange
        var fixture = new Fixture((_, _) => Task.CompletedTask);
        fixture.Channel.Setup(c => c.BasicAckAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
               .Throws(new InvalidOperationException("ack"));
        await fixture.StartAsync();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.DeliverAsync());
    }

    [Fact]
    public async Task Delivery_WhenAckTimesOut_IsSwallowed() {
        // arrange
        var fixture = new Fixture((_, _) => Task.CompletedTask);
        fixture.Channel.Setup(c => c.BasicAckAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
               .Throws(new OperationCanceledException());
        await fixture.StartAsync();

        // act
        var exception = await Record.ExceptionAsync(() => fixture.DeliverAsync());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Delivery_WhenNackTimesOut_IsSwallowed() {
        // arrange
        var fixture = new Fixture((_, _) => throw new OperationCanceledException());
        fixture.Channel.Setup(c => c.BasicNackAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
               .Throws(new OperationCanceledException());
        await fixture.StartAsync();

        // act
        var exception = await Record.ExceptionAsync(() => fixture.DeliverAsync());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Delivery_WhenNackFails_Rethrows() {
        // arrange
        var fixture = new Fixture((_, _) => throw new OperationCanceledException());
        fixture.Channel.Setup(c => c.BasicNackAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
               .Throws(new InvalidOperationException("nack"));
        await fixture.StartAsync();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.DeliverAsync());
    }

    [Fact]
    public async Task Shutdown_IsLoggedWithoutThrowing() {
        // arrange
        var fixture = new Fixture((_, _) => Task.CompletedTask);
        await fixture.StartAsync();

        // act
        var exception = await Record.ExceptionAsync(() => fixture.Consumer!.HandleChannelShutdownAsync(
            null!, new ShutdownEventArgs(ShutdownInitiator.Library, 200, "bye")));

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task StopAsync_ClosesChannelAndDetachesHandlers() {
        // arrange
        var fixture = new Fixture((_, _) => Task.CompletedTask);
        await fixture.StartAsync();

        // act
        await ((IHostedService)fixture.Sut).StopAsync(CancellationToken.None);

        // assert
        fixture.Channel.Verify(c => c.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StopAsync_BeforeStart_CompletesImmediately() {
        // arrange
        var fixture = new Fixture((_, _) => Task.CompletedTask);

        // act & assert
        await ((IHostedService)fixture.Sut).StopAsync(CancellationToken.None);
    }
}
