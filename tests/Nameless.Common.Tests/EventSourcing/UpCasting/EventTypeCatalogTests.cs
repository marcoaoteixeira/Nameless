namespace Nameless.EventSourcing.UpCasting;

[UnitTest]
public class EventTypeCatalogTests {
    [EventType("order.created", 2)]
    private sealed record OrderCreated;

    private sealed record UndecoratedEvent;

    [Fact]
    public void Resolve_WithRegisteredEventType_ReturnsClrType() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated)]);

        // act
        var actual = sut.Resolve("order.created");

        // assert
        Assert.Equal(typeof(OrderCreated), actual);
    }

    [Fact]
    public void Resolve_WithUnregisteredEventType_Throws() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated)]);

        // act & assert
        Assert.Throws<KeyNotFoundException>(() => sut.Resolve("unknown"));
    }

    [Fact]
    public void GetCurrentVersion_WithRegisteredEventType_ReturnsVersion() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated)]);

        // act
        var actual = sut.GetCurrentVersion("order.created");

        // assert
        Assert.Equal(2, actual);
    }

    [Fact]
    public void GetEventTypeName_WithRegisteredClrType_ReturnsName() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated)]);

        // act
        var actual = sut.GetEventTypeName(typeof(OrderCreated));

        // assert
        Assert.Equal("order.created", actual);
    }

    [Fact]
    public void GetEventTypeName_WithUndecoratedType_Throws() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated)]);

        // act & assert
        Assert.Throws<KeyNotFoundException>(() => sut.GetEventTypeName(typeof(UndecoratedEvent)));
    }

    [Fact]
    public void GetEventTypeName_WithNullClrType_Throws() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated)]);

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetEventTypeName(null!));
    }

    [Fact]
    public void Constructor_WithNullEventTypes_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new EventTypeCatalog(null!));
    }

    [Fact]
    public void Constructor_IgnoresUndecoratedTypes() {
        // arrange
        var sut = new EventTypeCatalog([typeof(OrderCreated), typeof(UndecoratedEvent)]);

        // act & assert
        Assert.Throws<KeyNotFoundException>(() => sut.GetEventTypeName(typeof(UndecoratedEvent)));
    }
}
