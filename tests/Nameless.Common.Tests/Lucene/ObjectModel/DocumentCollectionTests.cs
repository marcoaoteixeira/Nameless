using Lucene.Net.Documents;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.ObjectModel;

[UnitTest]
public class DocumentCollectionTests {
    [Fact]
    public void Add_IncrementsCount() {
        // Arrange
        var sut = new DocumentCollection();

        // Act
        sut.Add(new Document());
        sut.Add(new Document());

        // Assert
        Assert.Equal(2, sut.Count);
    }

    [Fact]
    public void GetEnumerator_YieldsAddedDocuments() {
        // Arrange
        var doc1 = new Document();
        doc1.Add(new StringField("id", "1", Field.Store.YES));

        var doc2 = new Document();
        doc2.Add(new StringField("id", "2", Field.Store.YES));

        var sut = new DocumentCollection();
        sut.Add(doc1);
        sut.Add(doc2);

        // Act
        var list = sut.ToList();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.Contains(list, d => d.GetField("id")?.GetStringValue() == "1");
        Assert.Contains(list, d => d.GetField("id")?.GetStringValue() == "2");
    }
}
