namespace Nameless.EventSourcing.UpCasting;

[UnitTest]
public class EventTypeAttributeTests {
    [Fact]
    public void Constructor_SetsNameAndVersion() {
        // act
        var sut = new EventTypeAttribute("order.created", 2);

        // assert
        Assert.Multiple(
            () => Assert.Equal("order.created", sut.Name),
            () => Assert.Equal(2, sut.Version)
        );
    }

    [Fact]
    public void Constructor_WithNullOrWhiteSpaceName_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => new EventTypeAttribute(" ", 1));
    }

    [Fact]
    public void Constructor_WithVersionLowerThanOne_Throws() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new EventTypeAttribute("order.created", 0));
    }
}
