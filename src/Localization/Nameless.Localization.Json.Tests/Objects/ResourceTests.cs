namespace Nameless.Localization.Json.Objects;

public class ResourceTests {
    [Test]
    public void TryGetValue_Should_Return_True_When_Region_Found() {
        // arrange
        var sut = new Resource(path: string.Empty,
                               culture: string.Empty,
                               messages: [
                                   new Message("Message A", "Message A"),
                                   new Message("Message B", "Message B"),
                                   new Message("Message C", "Message C")
                               ],
                               isAvailable: true);

        // act
        var found = sut.TryGetMessage("Message C", out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.That(found, Is.True);
            Assert.That(actual.Id, Is.EqualTo("Message C"));
        });
    }

    [Test]
    public void TryGetValue_Should_Return_False_When_Region_Not_Found() {
        // arrange
        var sut = new Resource(path: string.Empty,
                               culture: string.Empty,
                               messages: [
                                   new Message("Message A", "Message A"),
                                   new Message("Message B", "Message B"),
                                   new Message("Message C", "Message C")
                               ],
                               isAvailable: true);

        // act
        var found = sut.TryGetMessage("Error", out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.That(found, Is.False);
            Assert.That(actual, Is.Null);
        });
    }

    [TestCase("Region A", "Region A", true)]
    [TestCase("Region A", "Region B", false)]
    public void Equals_Should_Return_Corresponding_Result(string nameX, string nameY, bool expected) {
        // arrange
        var regionX = new Resource(nameX, nameX, [], isAvailable: true);
        var regionY = new Resource(nameY, nameY, [], isAvailable: true);

        // act
        var actual = regionX.Equals(regionY);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("Region A", "Region A", true)]
    [TestCase("Region A", "Region B", false)]
    public void GetHashCode_Should_Return_Corresponding_Result(string nameX, string nameY, bool expected) {
        // arrange
        var regionX = new Resource(nameX, nameX, [], isAvailable: true);
        var regionY = new Resource(nameY, nameY, [], isAvailable: true);

        // act
        var hashCodeX = regionX.GetHashCode();
        var hashCodeY = regionY.GetHashCode();

        // assert
        Assert.That(hashCodeX == hashCodeY, Is.EqualTo(expected));
    }
}