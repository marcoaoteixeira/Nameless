namespace Nameless.Localization.Json.Objects;

public class ResourceTests {
    [Fact]
    public void TryGetValue_Should_Return_True_When_Region_Found() {
        // arrange
        var sut = new Resource(string.Empty,
            string.Empty,
            [
                new Message("Message A", "Message A"),
                new Message("Message B", "Message B"),
                new Message("Message C", "Message C")
            ],
            true);

        // act
        var found = sut.TryGetMessage("Message C", out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.True(found);
            Assert.Equal("Message C", actual.Id);
        });
    }

    [Fact]
    public void TryGetValue_Should_Return_False_When_Region_Not_Found() {
        // arrange
        var sut = new Resource(string.Empty,
            string.Empty,
            [
                new Message("Message A", "Message A"),
                new Message("Message B", "Message B"),
                new Message("Message C", "Message C")
            ],
            true);

        // act
        var found = sut.TryGetMessage("Error", out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.False(found);
            Assert.Null(actual);
        });
    }

    [Theory]
    [InlineData("Region A", "Region A", true)]
    [InlineData("Region A", "Region B", false)]
    public void Equals_Should_Return_Corresponding_Result(string nameX, string nameY, bool expected) {
        // arrange
        var regionX = new Resource(nameX, nameX, [], true);
        var regionY = new Resource(nameY, nameY, [], true);

        // act
        var actual = regionX.Equals(regionY);

        // assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Region A", "Region A", true)]
    [InlineData("Region A", "Region B", false)]
    public void GetHashCode_Should_Return_Corresponding_Result(string nameX, string nameY, bool expected) {
        // arrange
        var regionX = new Resource(nameX, nameX, [], true);
        var regionY = new Resource(nameY, nameY, [], true);

        // act
        var hashCodeX = regionX.GetHashCode();
        var hashCodeY = regionY.GetHashCode();

        // assert
        Assert.Equal(expected, hashCodeX == hashCodeY);
    }
}