using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.IO.FileSystem;
using Nameless.Lucene;
using Nameless.Lucene.ObjectModel;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.IO;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Lucene;

[IntegrationTest]
public class IndexTests : IDisposable {
    private readonly string _tempDir;
    private readonly Index _sut;

    public IndexTests() {
        _tempDir = Path.Combine(Path.GetTempPath(), $"lucene-index-tests-{Guid.NewGuid():N}");
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
    public void Insert_ThenSearch_ReturnsMatchingDocument() {
        // Arrange
        var collection = new DocumentCollection();
        var doc = new Document();
        doc.Add(new StringField("id", "001", Field.Store.YES));
        doc.Add(new StringField("title", "hello", Field.Store.YES));
        collection.Add(doc);

        var insertResult = _sut.Insert(collection);
        Assert.True(insertResult.Success);
        _sut.SaveChanges();

        var query = new TermQuery(new Term("id", "001"));

        // Act
        var results = _sut.Search(query, Sort.RELEVANCE, 10).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("001", ((Document)results[0]).GetField("id")?.GetStringValue());
    }

    [Fact]
    public void Delete_ByTerm_RemovesDocument() {
        // Arrange
        var collection = new DocumentCollection();
        var doc = new Document();
        doc.Add(new StringField("id", "002", Field.Store.YES));
        collection.Add(doc);

        _sut.Insert(collection);
        _sut.SaveChanges();

        var deleteQuery = new TermQuery(new Term("id", "002"));

        // Act
        var deleteResult = _sut.Delete(deleteQuery);
        _sut.SaveChanges();

        var countResult = _sut.Count(deleteQuery);

        // Assert
        Assert.True(deleteResult.Success);
        Assert.True(countResult.Success);
        Assert.Equal(0, countResult.Value);
    }

    [Fact]
    public void Update_ChangesFieldValue() {
        // Arrange
        var collection = new DocumentCollection();
        var doc = new Document();
        doc.Add(new StringField("id", "003", Field.Store.YES));
        doc.Add(new StringField("status", "pending", Field.Store.YES));
        collection.Add(doc);

        _sut.Insert(collection);
        _sut.SaveChanges();

        var updateTerm = new Term("id", "003");
        var updatedDoc = new Document();
        updatedDoc.Add(new StringField("id", "003", Field.Store.YES));
        updatedDoc.Add(new StringField("status", "active", Field.Store.YES));

        // Act
        var updateResult = _sut.Update(updateTerm, updatedDoc);
        _sut.SaveChanges();

        var searchQuery = new TermQuery(new Term("status", "active"));
        var results = _sut.Search(searchQuery, Sort.RELEVANCE, 10).ToList();

        // Assert
        Assert.True(updateResult.Success);
        Assert.Single(results);
        Assert.Equal("active", ((Document)results[0]).GetField("status")?.GetStringValue());
    }

    [Fact]
    public void Count_ReturnsCorrectCount() {
        // Arrange
        var collection = new DocumentCollection();
        for (var i = 0; i < 3; i++) {
            var doc = new Document();
            doc.Add(new StringField("tag", "countable", Field.Store.YES));
            collection.Add(doc);
        }

        _sut.Insert(collection);
        _sut.SaveChanges();

        var query = new TermQuery(new Term("tag", "countable"));

        // Act
        var result = _sut.Count(query);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void SaveChanges_PersistsChanges() {
        // Arrange
        var collection = new DocumentCollection();
        var doc = new Document();
        doc.Add(new StringField("id", "save-test", Field.Store.YES));
        collection.Add(doc);

        _sut.Insert(collection);

        // Act
        var saveResult = _sut.SaveChanges();

        // Assert
        Assert.True(saveResult.Success);

        var countResult = _sut.Count(new TermQuery(new Term("id", "save-test")));
        Assert.True(countResult.Success);
        Assert.Equal(1, countResult.Value);
    }

    [Fact]
    public void Rollback_DiscardsUncommittedChanges() {
        // Arrange
        var collection = new DocumentCollection();
        var doc = new Document();
        doc.Add(new StringField("id", "rollback-test", Field.Store.YES));
        collection.Add(doc);

        _sut.Insert(collection);

        // Act — rollback before saving
        var rollbackResult = _sut.Rollback();

        // Assert
        Assert.True(rollbackResult.Success);

        // after rollback a new writer is needed; re-open a new index over same dir to verify
        using var verifyIndex = CreateIndex(_tempDir);
        var countResult = verifyIndex.Count(new TermQuery(new Term("id", "rollback-test")));
        Assert.True(countResult.Success);
        Assert.Equal(0, countResult.Value);
    }

    [Fact]
    public void Dispose_DoesNotThrow() {
        // Arrange
        var indexDir = Path.Combine(Path.GetTempPath(), $"lucene-dispose-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(indexDir);
        var index = CreateIndex(indexDir);

        // Act
        var exception = Record.Exception(index.Dispose);

        // Assert
        Assert.Null(exception);

        if (Directory.Exists(indexDir)) {
            Directory.Delete(indexDir, recursive: true);
        }
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
            "test",
            options,
            logger
        );
    }
}
