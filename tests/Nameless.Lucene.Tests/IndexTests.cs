using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.IO.FileSystem;
using Nameless.Lucene.ObjectModel;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.IO;
using Index = Nameless.Lucene.Infrastructure.Implementations.Index;

namespace Nameless.Lucene;

[UnitTest]
public class IndexTests {
    private static string CreateDirectoryPath() {
        return Path.Combine(Path.GetTempPath(), $"{Guid.CreateVersion7():N}");
    }

    private static IFileSystem CreateFileSystem(string directoryPath) {
        var directory = new DirectoryMocker().WithPath(directoryPath).Build();

        return new FileSystemMocker().WithGetDirectory(directory).Build();
    }

    public static Index CreateSut(string directoryPath, ILogger<Index> logger = null) {
        return new Index(
            Defaults.Analyzer,
            CreateFileSystem(directoryPath),
            $"{Guid.CreateVersion7():N}",
            Options.Create(new LuceneOptions()),
            logger ?? NullLogger<Index>.Instance
        );
    }

    [Fact]
    public void WhenInsertDocuments_ThenIndexMustAcceptDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);
        var documents = new DocumentCollection([
            [new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES)],
            [new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES)],
            [new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES)],
            [new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES)],
            [new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES)]
        ]);

        // act
        var result = sut.Insert(documents);
        sut.SaveChanges();

        // assert
        Assert.Equal(documents.Count, result.Value);
    }

    [Fact]
    public void WhenDeletingDocuments_ThenIndexMustRemoveDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);
        var query = new TermQuery(new Term("__id__", $"{Guid.CreateVersion7():N}"));

        // act
        var result = sut.Delete(query);

        // assert
        Assert.True(result.Value);
    }

    [Fact]
    public void WhenSearchingDocuments_WhenThereAreDocumentsInIndex_ThenReturnsDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);

        var documents = new DocumentCollection([
            [
                new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES),
                new StringField("Name", "John Doe", Field.Store.YES),
                new StringField("Email", "john.doe@email.com", Field.Store.YES)
            ]
        ]);

        sut.Insert(documents);
        sut.SaveChanges();

        var query = new TermQuery(
            new Term("Name", "John Doe")
        );
        var collector = TopFieldCollector.Create(
            Sort.RELEVANCE,
            numHits: 10,
            fillFields: false,
            trackDocScores: true,
            trackMaxScore: false,
            docsScoredInOrder: true
        );

        // act
        var result = sut.Search(query, collector);

        // assert
        result.Match(
            onSuccess: Assert.NotEmpty,
            onFailure: failure => Assert.Fail(failure[0].Message)
        );
    }

    [Fact]
    public void WhenCountingDocuments_WhenThereAreDocumentsInIndex_ThenReturnsTotalNumberOfDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);

        var documents = new DocumentCollection([
            [
                new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES),
                new StringField("Name", "John Doe", Field.Store.YES),
                new StringField("Email", "john.doe@email.com", Field.Store.YES)
            ],
            [
                new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES),
                new StringField("Name", "John Doe", Field.Store.YES),
                new StringField("Email", "john.doe@email.com", Field.Store.YES)
            ],
            [
                new StringField("__id__", Guid.CreateVersion7().ToString(), Field.Store.YES),
                new StringField("Name", "John Doe", Field.Store.YES),
                new StringField("Email", "john.doe@email.com", Field.Store.YES)
            ]
        ]);

        sut.Insert(documents);
        sut.SaveChanges();

        // act
        var result = sut.Count(new MatchAllDocsQuery());

        // assert
        result.Match(
            onSuccess: value => Assert.Equal(3, value),
            onFailure: failure => Assert.Fail(failure[0].Message)
        );
    }
}
