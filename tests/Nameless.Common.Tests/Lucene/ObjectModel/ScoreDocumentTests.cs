using Lucene.Net.Documents;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.ObjectModel;

[UnitTest]
public class ScoreDocumentTests {
    [Fact]
    public void Add_FieldAndEnumerate_YieldsField() {
        // Arrange
        var sut = new ScoreDocument();
        var field = new StringField("id", "abc", Field.Store.YES);

        // Act
        sut.Add(field);
        var fields = sut.ToList();

        // Assert
        Assert.Single(fields);
        Assert.Equal("id", fields[0].Name);
    }

    [Fact]
    public void Score_ReturnsConstructedValue() {
        // Arrange
        const float Expected = 0.75f;
        var sut = new ScoreDocument([], Expected);

        // Act
        var actual = sut.Score;

        // Assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void ImplicitConversion_FromDocument_PreservesFields() {
        // Arrange
        var document = new Document();
        document.Add(new StringField("name", "Alice", Field.Store.YES));

        // Act
        ScoreDocument scoreDoc = document;
        var fields = scoreDoc.ToList();

        // Assert
        Assert.Single(fields);
        Assert.Equal("name", fields[0].Name);
        Assert.Equal(0F, scoreDoc.Score);
    }

    [Fact]
    public void ImplicitConversion_ToDocument_PreservesFields() {
        // Arrange
        var scoreDoc = new ScoreDocument();
        scoreDoc.Add(new StringField("city", "Lisbon", Field.Store.YES));

        // Act
        Document document = scoreDoc;
        var field = document.GetField("city");

        // Assert
        Assert.NotNull(field);
        Assert.Equal("Lisbon", field.GetStringValue());
    }
}
