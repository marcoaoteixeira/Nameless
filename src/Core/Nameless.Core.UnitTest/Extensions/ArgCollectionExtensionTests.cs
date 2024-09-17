using Nameless.Infrastructure;

namespace Nameless;

public class ArgCollectionExtensionTests {
    [Test]
    public void When_Convert_To_JSON_Then_Return_Valid_JSON() {
        // arrange
        var sut = new ArgCollection();
        const string expected = """[{"Name":"Nome","Value":"Valor"}]""";

        // act
        sut.Add(new Arg("Nome", "Valor"));
        var json = ArgCollectionExtension.ToJson(sut);

        // assert
        Assert.That(json, Is.EqualTo(expected));
    }
}
