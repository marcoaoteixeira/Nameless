using Nameless.ProducerConsumer;
using Nameless.ProducerConsumer.RabbitMQ;
using Nameless.Testing.Tools.Attributes;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Extensions;

[UnitTest]
public class ContextExtensionsTests {
    // ── Context extension properties ─────────────────────────────────────────

    [Fact]
    public void Context_AppId_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.AppId = "my-app";

        // assert
        Assert.Equal("my-app", ctx.AppId);
    }

    [Fact]
    public void Context_AppId_WhenNotSet_ReturnsNull() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Null(ctx.AppId);
    }

    [Fact]
    public void Context_ClusterId_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.ClusterId = "cluster-1";

        // assert
        Assert.Equal("cluster-1", ctx.ClusterId);
    }

    [Fact]
    public void Context_ContentEncoding_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.ContentEncoding = "utf-8";

        // assert
        Assert.Equal("utf-8", ctx.ContentEncoding);
    }

    [Fact]
    public void Context_ContentType_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.ContentType = "application/json";

        // assert
        Assert.Equal("application/json", ctx.ContentType);
    }

    [Fact]
    public void Context_CorrelationId_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.CorrelationId = "corr-abc";

        // assert
        Assert.Equal("corr-abc", ctx.CorrelationId);
    }

    [Fact]
    public void Context_DeliveryMode_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.DeliveryMode = DeliveryModes.Persistent;

        // assert
        Assert.Equal(DeliveryModes.Persistent, ctx.DeliveryMode);
    }

    [Fact]
    public void Context_DeliveryMode_WhenNotSet_ReturnsDefault() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Equal((DeliveryModes)0, ctx.DeliveryMode);
    }

    [Fact]
    public void Context_Expiration_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.Expiration = "60000";

        // assert
        Assert.Equal("60000", ctx.Expiration);
    }

    [Fact]
    public void Context_Headers_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();
        var headers = new Dictionary<string, object?> { ["x-custom"] = "value" };

        // act
        ctx.Headers = headers;

        // assert
        Assert.Equal("value", ctx.Headers["x-custom"]);
    }

    [Fact]
    public void Context_Headers_WhenNotSet_ReturnsEmptyDictionary() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Empty(ctx.Headers);
    }

    [Fact]
    public void Context_MessageId_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.MessageId = "msg-001";

        // assert
        Assert.Equal("msg-001", ctx.MessageId);
    }

    [Fact]
    public void Context_Persistent_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.Persistent = true;

        // assert
        Assert.True(ctx.Persistent);
    }

    [Fact]
    public void Context_Persistent_WhenNotSet_ReturnsFalse() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.False(ctx.Persistent);
    }

    [Fact]
    public void Context_Priority_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.Priority = 5;

        // assert
        Assert.Equal(5, ctx.Priority);
    }

    [Fact]
    public void Context_Priority_WhenNotSet_ReturnsZero() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Equal(0, ctx.Priority);
    }

    [Fact]
    public void Context_ReplyTo_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.ReplyTo = "reply-queue";

        // assert
        Assert.Equal("reply-queue", ctx.ReplyTo);
    }

    [Fact]
    public void Context_ReplyToAddress_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();
        var address = new PublicationAddress("direct", "my-exchange", "routing-key");

        // act
        ctx.ReplyToAddress = address;

        // assert
        Assert.Equal(address, ctx.ReplyToAddress);
    }

    [Fact]
    public void Context_ReplyToAddress_WhenNotSet_ReturnsNull() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Null(ctx.ReplyToAddress);
    }

    [Fact]
    public void Context_Timestamp_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();
        var ts = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        // act
        ctx.Timestamp = ts;

        // assert
        Assert.Equal(ts, ctx.Timestamp);
    }

    [Fact]
    public void Context_Type_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.Type = "order.created";

        // assert
        Assert.Equal("order.created", ctx.Type);
    }

    [Fact]
    public void Context_UserId_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.UserId = "user-42";

        // assert
        Assert.Equal("user-42", ctx.UserId);
    }

    // ── ConsumerContext extension properties ─────────────────────────────────

    [Fact]
    public void ConsumerContext_QueueName_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.QueueName = "my-queue";

        // assert
        Assert.Equal("my-queue", ctx.QueueName);
    }

    [Fact]
    public void ConsumerContext_QueueName_WhenNotSet_ReturnsDefault() {
        // arrange
        var ctx = new ConsumerContext();

        // act & assert
        Assert.Equal("q.default", ctx.QueueName);
    }

    [Fact]
    public void ConsumerContext_AckOnSuccess_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.AckOnSuccess = true;

        // assert
        Assert.True(ctx.AckOnSuccess);
    }

    [Fact]
    public void ConsumerContext_AckOnSuccess_WhenNotSet_ReturnsFalse() {
        // arrange
        var ctx = new ConsumerContext();

        // act & assert
        Assert.False(ctx.AckOnSuccess);
    }

    [Fact]
    public void ConsumerContext_AckMultiple_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.AckMultiple = true;

        // assert
        Assert.True(ctx.AckMultiple);
    }

    [Fact]
    public void ConsumerContext_NAckOnFailure_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.NAckOnFailure = true;

        // assert
        Assert.True(ctx.NAckOnFailure);
    }

    [Fact]
    public void ConsumerContext_NAckMultiple_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.NAckMultiple = true;

        // assert
        Assert.True(ctx.NAckMultiple);
    }

    [Fact]
    public void ConsumerContext_AutoAck_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.AutoAck = true;

        // assert
        Assert.True(ctx.AutoAck);
    }

    [Fact]
    public void ConsumerContext_RequeueOnFailure_GetSet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.RequeueOnFailure = true;

        // assert
        Assert.True(ctx.RequeueOnFailure);
    }

    // ── ProducerContext extension properties ─────────────────────────────────

    [Fact]
    public void ProducerContext_ExchangeName_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.ExchangeName = "orders";

        // assert
        Assert.Equal("orders", ctx.ExchangeName);
    }

    [Fact]
    public void ProducerContext_ExchangeName_WhenNotSet_ReturnsEmpty() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Equal(string.Empty, ctx.ExchangeName);
    }

    [Fact]
    public void ProducerContext_RoutingKeys_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();
        var keys = new[] { "key1", "key2" };

        // act
        ctx.RoutingKeys = keys;

        // assert
        Assert.Equal(keys, ctx.RoutingKeys);
    }

    [Fact]
    public void ProducerContext_RoutingKeys_WhenNotSet_ReturnsEmpty() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.Empty(ctx.RoutingKeys);
    }

    [Fact]
    public void ProducerContext_HasRoutingKeys_WhenRoutingKeysSet_ReturnsTrue() {
        // arrange
        var ctx = new ProducerContext();
        ctx.RoutingKeys = new[] { "key1" };

        // act & assert
        Assert.True(ctx.HasRoutingKeys);
    }

    [Fact]
    public void ProducerContext_HasRoutingKeys_WhenNotSet_ReturnsFalse() {
        // arrange
        var ctx = new ProducerContext();

        // act & assert
        Assert.False(ctx.HasRoutingKeys);
    }

    [Fact]
    public void ProducerContext_Mandatory_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.Mandatory = true;

        // assert
        Assert.True(ctx.Mandatory);
    }

    [Fact]
    public void ProducerContext_UsePrefetch_GetSet_RoundTrips() {
        // arrange
        var ctx = new ProducerContext();

        // act
        ctx.UsePrefetch = true;

        // assert
        Assert.True(ctx.UsePrefetch);
    }

    [Fact]
    public void ProducerContext_CreateBasicProperties_ReturnsPopulatedProperties() {
        // arrange
        var ctx = new ProducerContext();
        ctx.AppId = "test-app";
        ctx.ContentType = "application/json";
        ctx.MessageId = "msg-123";
        ctx.Persistent = true;

        // act
        var props = ctx.CreateBasicProperties();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(props);
            Assert.Equal("test-app", props.AppId);
            Assert.Equal("application/json", props.ContentType);
            Assert.Equal("msg-123", props.MessageId);
            Assert.True(props.Persistent);
        });
    }
}
