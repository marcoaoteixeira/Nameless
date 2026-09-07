using Nameless.Testing.Tools.Attributes;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class JsonMessageSerializerTests {
    private static JsonMessageSerializer CreateSut() {
        return new JsonMessageSerializer();
    }

    private static ProducerContext CreateContextWithMetadata(
        string messageId = "test-msg-id",
        string correlationId = "test-corr-id",
        long unixTimestamp = 1_700_000_000L) {

        var ctx = new ProducerContext {
            MessageId = messageId,
            CorrelationId = correlationId,
            Timestamp = new AmqpTimestamp(unixTimestamp)
        };

        return ctx;
    }

    [Fact]
    [UnitTest]
    public async Task SerializeAsync_ThenDeserializeAsync_RoundTrips() {
        // arrange
        var sut = CreateSut();
        var original = "Hello, RabbitMQ!";
        var producerCtx = CreateContextWithMetadata();
        var consumerCtx = new ConsumerContext();

        // act
        var buffer = await sut.SerializeAsync(original, producerCtx, CancellationToken.None);
        var deserialized = await sut.DeserializeAsync<string>(buffer, consumerCtx, CancellationToken.None);

        // assert
        Assert.Equal(original, deserialized);
    }

    [Fact]
    [UnitTest]
    public async Task SerializeAsync_SetsMessageIdInContext() {
        // arrange
        var sut = CreateSut();
        const string ExpectedMessageId = "msg-id-123";
        var producerCtx = CreateContextWithMetadata(messageId: ExpectedMessageId);
        var consumerCtx = new ConsumerContext();

        // act
        var buffer = await sut.SerializeAsync("payload", producerCtx, CancellationToken.None);
        await sut.DeserializeAsync<string>(buffer, consumerCtx, CancellationToken.None);

        // assert
        Assert.Equal(ExpectedMessageId, consumerCtx.MessageId);
    }

    [Fact]
    [UnitTest]
    public async Task SerializeAsync_SetsCorrelationIdInContext() {
        // arrange
        var sut = CreateSut();
        const string ExpectedCorrelationId = "corr-id-456";
        var producerCtx = CreateContextWithMetadata(correlationId: ExpectedCorrelationId);
        var consumerCtx = new ConsumerContext();

        // act
        var buffer = await sut.SerializeAsync("payload", producerCtx, CancellationToken.None);
        await sut.DeserializeAsync<string>(buffer, consumerCtx, CancellationToken.None);

        // assert
        Assert.Equal(ExpectedCorrelationId, consumerCtx.CorrelationId);
    }

    [Fact]
    [UnitTest]
    public async Task SerializeAsync_SetsTimestampInContext() {
        // arrange
        var sut = CreateSut();
        const long ExpectedUnixTime = 1_700_000_000L;
        var producerCtx = CreateContextWithMetadata(unixTimestamp: ExpectedUnixTime);
        var consumerCtx = new ConsumerContext();

        // act
        var buffer = await sut.SerializeAsync("payload", producerCtx, CancellationToken.None);
        await sut.DeserializeAsync<string>(buffer, consumerCtx, CancellationToken.None);

        // assert
        Assert.Equal(ExpectedUnixTime, consumerCtx.Timestamp.UnixTime);
    }

    [Fact]
    [UnitTest]
    public async Task SerializeAsync_ProducesNonEmptyBuffer() {
        // arrange
        var sut = CreateSut();
        var ctx = CreateContextWithMetadata();

        // act
        var buffer = await sut.SerializeAsync("some message", ctx, CancellationToken.None);

        // assert
        Assert.NotEmpty(buffer);
    }

    [Fact]
    [UnitTest]
    public async Task DeserializeAsync_InvalidBuffer_ThrowsInvalidOperationException() {
        // arrange
        var sut = CreateSut();
        var invalidBuffer = "not-json"u8.ToArray();
        var ctx = new ConsumerContext();

        // act & assert
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(
            () => sut.DeserializeAsync<string>(invalidBuffer, ctx, CancellationToken.None)
        );
    }
}
