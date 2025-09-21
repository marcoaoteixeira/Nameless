using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class DocumentTests {
    [Fact]
    public void WhenConstructing_WhenProvidingIdentifier_ThenDocumentHasIdentifierField() {
        // arrange
        var expected = Guid.CreateVersion7().ToString("N");
        var document = new Document(expected);

        // act
        var actual = document.SingleOrDefault(field => field.Name.Equals(Document.RESERVED_ID_NAME));

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Value);
        });
    }
}
