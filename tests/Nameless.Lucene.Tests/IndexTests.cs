using J2N.Text;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Nameless.IO.FileSystem;
using Nameless.Lucene.ObjectModel;
using Nameless.Results;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.IO;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Lucene;

[UnitTest]
public class IndexTests {
    private const string FAKE_ERROR_MESSAGE = "Fake Error";

    private static string CreateDirectoryPath() {
        return Path.Combine(Path.GetTempPath(), $"{Guid.CreateVersion7():N}");
    }

    private static IFileSystem CreateFileSystem(string directoryPath) {
        var directory = new DirectoryMocker().WithPath(directoryPath).Build();

        return new FileSystemMocker().WithGetDirectory(directory).Build();
    }

    public static Index CreateSut(string directoryPath, string indexName = null, ILogger<Index> logger = null) {
        return CreateSut(CreateFileSystem(directoryPath), indexName, logger);
    }

    public static Index CreateSut(IFileSystem fileSystem, string indexName = null, ILogger<Index> logger = null) {
        return new Index(
            Defaults.Analyzer,
            fileSystem,
            indexName ?? $"{Guid.CreateVersion7():N}",
            Options.Create(new LuceneOptions()),
            logger ?? NullLogger<Index>.Instance
        );
    }

    [Fact]
    public void WhenInsertDocuments_ThenIndexMustInsertDocuments() {
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

        // assert
        result.Match(
            onSuccess: Assert.True,
            onFailure: errors => Assert.Fail($"Should not happen: {errors[0].Message}")
        );
    }

    [Fact]
    public void WhenDeletingDocuments_ThenIndexMustDeleteDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);
        var query = new TermQuery(new Term("__id__", $"{Guid.CreateVersion7():N}"));

        // act
        var result = sut.Delete(query);

        // assert
        result.Match(
            onSuccess: Assert.True,
            onFailure: errors => Assert.Fail($"Should not happen: {errors[0].Message}")
        );
    }

    [Fact]
    public void WhenUpdatingDocuments_ThenIndexMustUpdateDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);

        const string ID = "1d587757-a642-43f4-afa6-e01c0613b588";
        var term = new Term("__id__", ID);
        Document document = [
            new StringField("__id__", ID, Field.Store.YES),
            new StringField("name", "John Doe", Field.Store.YES)
        ];

        // act
        var result = sut.Update(term, document);

        // assert
        result.Match(
            onSuccess: Assert.True,
            onFailure: errors => Assert.Fail($"Should not happen: {errors[0].Message}")
        );
    }

    [Fact]
    public void WhenSearchingDocuments_ThenIndexMustReturnsDocuments() {
        // arrange
        var directoryPath = CreateDirectoryPath();
        var sut = CreateSut(directoryPath);

        const string ID = "196d6134-30d5-4190-9cf5-38c7c5281890";
        var documents = new DocumentCollection([
            [
                new StringField("__id__", ID, Field.Store.YES),
                new StringField("Name", "John Doe", Field.Store.YES),
                new StringField("Email", "john.doe@email.com", Field.Store.YES)
            ]
        ]);

        sut.Insert(documents);
        sut.SaveChanges();

        var query = new TermQuery(
            new Term("Name", "John Doe")
        );

        // act
        var actual = sut.Search(query).ToArray();

        // assert
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void WhenCountingDocuments_ThenIndexMustReturnsTotalNumberOfDocuments() {
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
        var result = sut.Count();

        // assert
        result.Match(
            onSuccess: value => Assert.Equal(3, value),
            onFailure: failure => Assert.Fail(failure[0].Message)
        );
    }

    // *** TEST SCENARIOS FOR CODE COVERAGE ***
    [Theory]
    [InlineData(typeof(InvalidOperationException))]
    [InlineData(typeof(OutOfMemoryException))]
    public void WhenInsert_WhenExceptionIsThrown_ThenCaptureExceptionAndReturnError(Type exceptionType) {
        // arrange
        var indexName = $"{Guid.CreateVersion7():N}";
        var fileSystem = new FileSystemMocker().WithGetDirectory(() => (Exception)Activator.CreateInstance(exceptionType, exceptionType.Name))
                                               .Build();
        var loggerMocker = new LoggerMocker<Index>().WithAnyLogLevel();
        var index = CreateSut(fileSystem, indexName, loggerMocker.Build());

        // act
        var actual = index.Insert([]);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Failure);
            Assert.Equal(exceptionType.Name, actual.Errors[0].Message);

            loggerMocker.VerifyError(message =>
                message.Contains(nameof(Index.Insert)) &&
                message.Contains(indexName) &&
                message.Contains(exceptionType.Name)
            );
        });
    }

    [Theory]
    [InlineData(typeof(InvalidOperationException))]
    [InlineData(typeof(OutOfMemoryException))]
    public void WhenDelete_WhenExceptionIsThrown_ThenCaptureExceptionAndReturnError(Type exceptionType) {
        // arrange
        var indexName = $"{Guid.CreateVersion7():N}";
        var fileSystem = new FileSystemMocker().WithGetDirectory(() => (Exception)Activator.CreateInstance(exceptionType, exceptionType.Name))
                                               .Build();
        var loggerMocker = new LoggerMocker<Index>().WithAnyLogLevel();
        var index = CreateSut(fileSystem, indexName, loggerMocker.Build());

        // act
        var actual = index.Delete(new MatchAllDocsQuery());

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Failure);
            Assert.Equal(exceptionType.Name, actual.Errors[0].Message);

            loggerMocker.VerifyError(message =>
                message.Contains(nameof(Index.Delete)) &&
                message.Contains(indexName) &&
                message.Contains(exceptionType.Name)
            );
        });
    }

    [Theory]
    [InlineData(typeof(InvalidOperationException))]
    [InlineData(typeof(OutOfMemoryException))]
    public void WhenUpdate_WhenExceptionIsThrown_ThenCaptureExceptionAndReturnError(Type exceptionType) {
        // arrange
        var indexName = $"{Guid.CreateVersion7():N}";
        var fileSystem = new FileSystemMocker().WithGetDirectory(() => (Exception)Activator.CreateInstance(exceptionType, exceptionType.Name))
                                               .Build();
        var loggerMocker = new LoggerMocker<Index>().WithAnyLogLevel();
        var index = CreateSut(fileSystem, indexName, loggerMocker.Build());

        // act
        var actual = index.Update(new Term("ID"), []);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Failure);
            Assert.Equal(exceptionType.Name, actual.Errors[0].Message);

            loggerMocker.VerifyError(message =>
                message.Contains(nameof(Index.Update)) &&
                message.Contains(indexName) &&
                message.Contains(exceptionType.Name)
            );
        });
    }
}
