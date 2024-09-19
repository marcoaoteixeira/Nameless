namespace Nameless.Localization.Json.Objects;

public class TranslationTests {
    [Test]
    public void TryGetValue_Should_Return_True_When_Region_Found() {
        // arrange
        var sut = new Translation(string.Empty, [
            new Region("Test A", []),
            new Region("Test B", []),
            new Region("Test C", []),
        ]);

        // act
        var found = sut.TryGetRegion("Test B", out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.That(found, Is.True);
            Assert.That(actual.Name, Is.EqualTo("Test B"));
        });
    }

    [Test]
    public void TryGetValue_Should_Return_False_When_Region_Not_Found() {
        // arrange
        var sut = new Translation(string.Empty, [
            new Region("Test A", []),
            new Region("Test B", []),
            new Region("Test C", []),
        ]);

        // act
        var found = sut.TryGetRegion("Error", out var actual);

        // assert
        Assert.Multiple(() => {
            Assert.That(found, Is.False);
            Assert.That(actual, Is.Null);
        });
    }

    [TestCase("en-US", "en-US", true)]
    [TestCase("pt-BR", "en-US", false)]
    public void Equals_Should_Return_Corresponding_Result(string cultureX, string cultureY, bool expected) {
        // arrange
        var translationX = new Translation(cultureX, []);
        var translationY = new Translation(cultureY, []);

        // act
        var actual = translationX.Equals(translationY);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("en-US", "en-US", true)]
    [TestCase("pt-BR", "en-US", false)]
    public void GetHashCode_Should_Return_Corresponding_Result(string cultureX, string cultureY, bool expected) {
        // arrange
        var translationX = new Translation(cultureX, []);
        var translationY = new Translation(cultureY, []);

        // act
        var hashCodeX = translationX.GetHashCode();
        var hashCodeY = translationY.GetHashCode();

        // assert
        Assert.That(hashCodeX == hashCodeY, Is.EqualTo(expected));
    }
}