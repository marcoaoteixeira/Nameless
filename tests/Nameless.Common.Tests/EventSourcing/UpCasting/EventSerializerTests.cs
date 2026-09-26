using System.Text.Json.Nodes;
using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.UpCasting;

[UnitTest]
public class EventSerializerTests {
    private sealed class FakeUpCaster : IEventUpCaster {
        public string EventType => "order.created";
        public int FromVersion => 1;

        public JsonNode Upcast(JsonNode payload) {
            payload["Priority"] = "normal";
            return payload;
        }
    }

    [EventType("order.created", 2)]
    private sealed record OrderCreatedV2 : IEvent {
        public string OrderNumber { get; init; } = string.Empty;
        public string Priority { get; init; } = string.Empty;
    }

    [Fact]
    public void Serialize_ReturnsMetadataWithCurrentVersionAndPayload() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: []);

        var evt = new OrderCreatedV2 { OrderNumber = "SO-1", Priority = "high" };

        // act
        var metadata = sut.Serialize(evt);

        // assert
        Assert.Multiple(
            () => Assert.Equal("order.created", metadata.EventType),
            () => Assert.Equal(2, metadata.SchemaVersion),
            () => Assert.Contains("SO-1", metadata.Payload)
        );
    }

    [Fact]
    public void Serialize_WithNullEvent_Throws() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: []);

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.Serialize(null!));
    }

    [Fact]
    public void Deserialize_WithCurrentSchemaVersion_ReturnsEvent() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: []);

        var metadata = new EventMetadata("order.created", 2, """{"OrderNumber":"SO-1","Priority":"high"}""");

        // act
        var result = sut.Deserialize(metadata);

        // assert
        Assert.IsType<OrderCreatedV2>(result);
    }

    [Fact]
    public void Deserialize_WithOlderSchemaVersion_AppliesUpCaster() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: [new FakeUpCaster()]);

        var metadata = new EventMetadata("order.created", 1, """{"OrderNumber":"SO-1"}""");

        // act
        var result = (OrderCreatedV2)sut.Deserialize(metadata);

        // assert
        Assert.Equal("normal", result.Priority);
    }

    [Fact]
    public void Deserialize_WithOlderSchemaVersion_AndNoMatchingUpCaster_Throws() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: []);

        var metadata = new EventMetadata("order.created", 1, """{"OrderNumber":"SO-1"}""");

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Deserialize(metadata));
    }

    [Fact]
    public void Deserialize_WithEmptyEventType_Throws() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: []);

        var metadata = new EventMetadata(string.Empty, 1, "{}");

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.Deserialize(metadata));
    }

    [Fact]
    public void Deserialize_WithInvalidJsonPayload_Throws() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);
        var sut = new EventSerializer(catalog, upCasters: []);

        var metadata = new EventMetadata("order.created", 2, "not-json");

        // act & assert
        Assert.ThrowsAny<Exception>(() => sut.Deserialize(metadata));
    }

    [Fact]
    public void Constructor_WithNullTypeCatalog_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new EventSerializer(null!, upCasters: []));
    }

    [Fact]
    public void Constructor_WithNullUpCasters_Throws() {
        // arrange
        var catalog = new EventTypeCatalog([typeof(OrderCreatedV2)]);

        // act & assert
        Assert.Throws<ArgumentNullException>(() => new EventSerializer(catalog, null!));
    }
}
