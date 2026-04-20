namespace Nameless;

public class SequentialGuidTests {
    // --- NewGuid ---

    [Fact]
    public void NewGuid_ReturnsNonEmptyGuid() {
        // act
        var guid = SequentialGuid.NewGuid();

        // assert
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Theory]
    [InlineData(SequentialGuid.SequentialType.AsString)]
    [InlineData(SequentialGuid.SequentialType.AsBinary)]
    [InlineData(SequentialGuid.SequentialType.AtEnd)]
    public void NewGuid_WithAllTypes_ReturnsNonEmptyGuid(SequentialGuid.SequentialType type) {
        // act
        var guid = SequentialGuid.NewGuid(type);

        // assert
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void NewGuid_WithRandomNumberGeneratorFalse_ReturnsNonEmptyGuid() {
        // act
        var guid = SequentialGuid.NewGuid(useRandomNumberGenerator: false);

        // assert
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void NewGuid_CalledTwice_ReturnsDifferentGuids() {
        // act
        var guid1 = SequentialGuid.NewGuid();
        var guid2 = SequentialGuid.NewGuid();

        // assert
        Assert.NotEqual(guid1, guid2);
    }
}
