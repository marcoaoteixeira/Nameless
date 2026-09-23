using System.Collections;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.IO;
using Nameless.Lucene.Collections;
using Nameless.Lucene.ObjectModel;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Lucene;

[UnitTest]
public class IndexFailureAndEnumeratorTests {
    private static Index CreateFailingIndex(Exception failure) {
        var fileProvider = new Mock<IFileProvider>();
        fileProvider.Setup(p => p.GetDirectory(It.IsAny<string>())).Throws(failure);

        return new Index(
            LuceneDefaults.Analyzer,
            fileProvider.Object,
            "failing",
            Microsoft.Extensions.Options.Options.Create(new LuceneOptions { DirectoryName = "lucene" }),
            new LoggerMocker<Index>().WithAnyLogLevel().Build()
        );
    }

    [Fact]
    public void Operations_WhenDirectoryCannotBeOpened_ReturnFailures() {
        // arrange
        using var sut = CreateFailingIndex(new IOException("no disk"));
        var term = new Term("id", "1");

        // act
        var results = new[] {
            sut.Insert(new DocumentCollection([new Document()])),
            sut.Delete(new MatchAllDocsQuery()),
            sut.Update(term, new Document()),
            sut.Rollback(),
            sut.SaveChanges()
        };
        var count = sut.Count(new MatchAllDocsQuery());

        // assert
        Assert.Multiple(
            () => Assert.All(results, result => Assert.False(result.Success)),
            () => Assert.False(count.Success)
        );
    }

    [Fact]
    public void Search_WhenDirectoryCannotBeOpened_Throws() {
        // arrange
        using var sut = CreateFailingIndex(new IOException("no disk"));

        // act & assert
        Assert.Throws<IOException>(() => sut.Search(new MatchAllDocsQuery(), Sort.RELEVANCE, 10));
    }

    [Fact]
    public void WriteOperations_WhenOutOfMemory_DestroyWriterAndReturnFailures() {
        // arrange
        using var sut = CreateFailingIndex(new OutOfMemoryException());

        // act
        var results = new[] {
            sut.Insert(new DocumentCollection([new Document()])),
            sut.Delete(new MatchAllDocsQuery()),
            sut.Update(new Term("id", "1"), new Document())
        };

        // assert
        Assert.All(results, result => Assert.False(result.Success));
    }

    [Fact]
    public void Operations_AfterDispose_Throw() {
        // arrange
        var sut = CreateFailingIndex(new IOException());
        sut.Dispose();

        // act & assert
        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => sut.Insert(new DocumentCollection())),
            () => Assert.Throws<ObjectDisposedException>(() => sut.RegisterDisposeCallback(_ => { })),
            () => Assert.Throws<ObjectDisposedException>(() => sut.UnregisterDisposeCallback(_ => { }))
        );
    }

    [Fact]
    public void NonGenericEnumerators_ReturnTheSameSequences() {
        // arrange
        var documents = new DocumentCollection([new Document(), new Document()]);
        var scoreDocument = new ScoreDocument([new StringField("id", "1", Field.Store.YES)], 1f);

        // act
        var documentEnumerator = ((IEnumerable)documents).GetEnumerator();
        var scoreEnumerator = ((IEnumerable)scoreDocument).GetEnumerator();

        // assert
        Assert.Multiple(
            () => Assert.True(documentEnumerator.MoveNext()),
            () => Assert.True(scoreEnumerator.MoveNext())
        );
    }

    [Fact]
    public void SearchEnumerable_NonGenericEnumerator_IsUsable() {
        // arrange
        using var directory = new global::Lucene.Net.Store.RAMDirectory();
        using (var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneDefaults.Version, LuceneDefaults.Analyzer))) {
            writer.AddDocument(new Document { new StringField("id", "1", Field.Store.YES) });
            writer.Commit();
        }

        using var reader = DirectoryReader.Open(directory);
        var sut = new SearchEnumerable(new IndexSearcher(reader), new MatchAllDocsQuery(), Sort.RELEVANCE, 10);

        // act
        var enumerator = ((IEnumerable)sut).GetEnumerator();
        var moved = enumerator.MoveNext();
        var current = ((IEnumerator)enumerator).Current;

        // assert
        Assert.Multiple(
            () => Assert.True(moved),
            () => Assert.IsType<ScoreDocument>(current)
        );
        (enumerator as IDisposable)?.Dispose();
    }

    [Fact]
    public void SearchEnumerator_ResetAndDispose_Behave() {
        // arrange
        using var directory = new global::Lucene.Net.Store.RAMDirectory();
        using (var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneDefaults.Version, LuceneDefaults.Analyzer))) {
            writer.AddDocument(new Document { new StringField("id", "1", Field.Store.YES) });
            writer.Commit();
        }

        using var reader = DirectoryReader.Open(directory);
        var sut = new SearchEnumerator(new IndexSearcher(reader), new MatchAllDocsQuery(), Sort.RELEVANCE, 10);

        // act
        var first = sut.MoveNext();
        var exhausted = !sut.MoveNext();
        sut.Reset();
        var afterReset = sut.MoveNext();
        sut.Dispose();

        // assert
        Assert.Multiple(
            () => Assert.True(first),
            () => Assert.True(exhausted),
            () => Assert.True(afterReset),
            () => Assert.Throws<ObjectDisposedException>(() => sut.MoveNext()),
            () => Assert.Throws<ObjectDisposedException>(sut.Reset)
        );
    }

    [Fact]
    public void SearchEnumerator_WithInvalidLimit_Throws() {
        // arrange
        using var directory = new global::Lucene.Net.Store.RAMDirectory();
        using (var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneDefaults.Version, LuceneDefaults.Analyzer))) {
            writer.Commit();
        }

        using var reader = DirectoryReader.Open(directory);

        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SearchEnumerator(new IndexSearcher(reader), new MatchAllDocsQuery(), Sort.RELEVANCE, 0));
    }
}
