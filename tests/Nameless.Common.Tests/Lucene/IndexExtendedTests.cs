using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Options;
using Nameless.Lucene.ObjectModel;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.IO;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Lucene;

[IntegrationTest]
public class IndexExtendedTests : IDisposable {
    private readonly string _tempDir;
    private readonly Index _sut;

    public IndexExtendedTests() {
        _tempDir = Path.Combine(Path.GetTempPath(), $"lucene-index-ext-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        _sut = CreateIndex(_tempDir);
    }

    public void Dispose() {
        _sut.Dispose();

        if (Directory.Exists(_tempDir)) {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Fact]
    public void RegisterDisposeCallback_CalledOnDispose() {
        // Arrange
        var indexDir = Path.Combine(Path.GetTempPath(), $"lucene-cb-{Guid.NewGuid():N}");
        Directory.CreateDirectory(indexDir);
        var index = CreateIndex(indexDir);

        Index? capturedIndex = null;
        index.RegisterDisposeCallback(idx => capturedIndex = idx);

        // Act
        index.Dispose();

        // Assert
        Assert.NotNull(capturedIndex);
        Assert.Same(index, capturedIndex);

        if (Directory.Exists(indexDir)) {
            Directory.Delete(indexDir, recursive: true);
        }
    }

    [Fact]
    public void UnregisterDisposeCallback_NotCalledAfterUnregister() {
        // Arrange
        var indexDir = Path.Combine(Path.GetTempPath(), $"lucene-uncb-{Guid.NewGuid():N}");
        Directory.CreateDirectory(indexDir);
        var index = CreateIndex(indexDir);

        var callCount = 0;
        Action<Index> callback = _ => callCount++;
        index.RegisterDisposeCallback(callback);
        index.UnregisterDisposeCallback(callback);

        // Act
        index.Dispose();

        // Assert
        Assert.Equal(0, callCount);

        if (Directory.Exists(indexDir)) {
            Directory.Delete(indexDir, recursive: true);
        }
    }

    [Fact]
    public void Count_AfterInsertThenDelete_ReturnsCorrectCount() {
        // Arrange
        var collection = new DocumentCollection();
        var doc = new Document();
        doc.Add(new StringField("id", "cnt-del-001", Field.Store.YES));
        collection.Add(doc);

        _sut.Insert(collection);
        _sut.SaveChanges();

        var query = new TermQuery(new Term("id", "cnt-del-001"));

        // confirm it's there first
        var countBefore = _sut.Count(query);
        Assert.True(countBefore.Success);
        Assert.Equal(1, countBefore.Value);

        // Act
        _sut.Delete(query);
        _sut.SaveChanges();

        var countAfter = _sut.Count(query);

        // Assert
        Assert.True(countAfter.Success);
        Assert.Equal(0, countAfter.Value);
    }

    [Fact]
    public void Search_WithNoResults_ReturnsEmpty() {
        // Arrange — query that matches nothing
        var query = new TermQuery(new Term("id", "nonexistent-xyz-9999"));

        // Act
        var results = _sut.Search(query, Sort.RELEVANCE, 100).ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Insert_MultipleDocuments_CountIsCorrect() {
        // Arrange
        const string Tag = "multi-insert-tag";
        var collection = new DocumentCollection();
        for (var i = 0; i < 5; i++) {
            var doc = new Document();
            doc.Add(new StringField("tag", Tag, Field.Store.YES));
            collection.Add(doc);
        }

        // Act
        var insertResult = _sut.Insert(collection);
        _sut.SaveChanges();

        var countResult = _sut.Count(new TermQuery(new Term("tag", Tag)));

        // Assert
        Assert.True(insertResult.Success);
        Assert.True(countResult.Success);
        Assert.Equal(5, countResult.Value);
    }

    [Fact]
    public void DocumentCollection_FromExistingSequence_HasCorrectCount() {
        // Arrange
        var docs = Enumerable.Range(0, 3).Select(i => {
            var d = new Document();
            d.Add(new StringField("seq", i.ToString(), Field.Store.YES));
            return d;
        }).ToList();

        // Act — exercises the IEnumerable<Document> constructor overload
        var collection = new DocumentCollection(docs);

        // Assert
        Assert.Equal(3, collection.Count);
    }

    private static Index CreateIndex(string tempDir) {
        var options = Options.Create(new LuceneOptions { DirectoryName = tempDir });
        var logger = new LoggerMocker<Index>().WithAnyLogLevel().Build();

        var directory = new DirectoryMocker()
            .WithPath(tempDir)
            .Build();

        var fileSystem = new FileSystemMocker()
            .WithGetDirectory(directory)
            .Build();

        return new Index(
            LuceneDefaults.Analyzer,
            fileSystem,
            "ext-test",
            options,
            logger
        );
    }
}
