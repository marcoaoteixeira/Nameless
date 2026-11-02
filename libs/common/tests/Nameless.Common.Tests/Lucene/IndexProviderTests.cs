using System.Collections.Concurrent;
using Lucene.Net.Documents;
using Microsoft.Extensions.Options;
using Nameless.IO.Physical;
using Nameless.Lucene.ObjectModel;
using Nameless.Testing.Tools.IO;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Lucene;

public class IndexProviderTests {
    private static IndexProvider CreateProvider(TempDirectory tempDirectory) {
        var fileManager = new PhysicalFileManager(tempDirectory.Path);
        var analyzerProvider = new AnalyzerProvider([]);
        var options = Options.Create(new LuceneOptions());
        var logger = new LoggerMocker<Index>().WithAnyLogLevel().Build();

        return new IndexProvider(analyzerProvider, fileManager, options, logger);
    }

    private static DocumentCollection CreateSampleDocuments() {
        var document = new Document {
            new StringField("id", "1", Field.Store.YES)
        };

        return new DocumentCollection([document]);
    }

    [Fact]
    [UnitTest]
    public void Get_AfterCloseAll_ReturnsDifferentWorkingInstance() {
        // arrange
        using var tempDirectory = new TempDirectory(nameof(IndexProviderTests));
        using var sut = CreateProvider(tempDirectory);

        var before = sut.Get("sample");

        // act
        sut.CloseAll();
        var after = sut.Get("sample");

        // assert
        Assert.NotSame(before, after);
        Assert.True(after.Count().Success);
    }

    [Fact]
    [UnitTest]
    public void OldInstance_AfterCloseAll_ThrowsObjectDisposedException() {
        // arrange
        using var tempDirectory = new TempDirectory(nameof(IndexProviderTests));
        using var sut = CreateProvider(tempDirectory);

        var index = sut.Get("sample");

        // act
        sut.CloseAll();

        // assert
        Assert.Throws<ObjectDisposedException>(() => index.Count());
    }

    [Fact]
    [UnitTest]
    public void CloseAll_AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        using var tempDirectory = new TempDirectory(nameof(IndexProviderTests));
        var sut = CreateProvider(tempDirectory);

        sut.Dispose();

        // act & assert
        Assert.Throws<ObjectDisposedException>(() => sut.CloseAll());
    }

    [Fact]
    [UnitTest]
    public void CloseAll_CalledTwice_IsNoOp() {
        // arrange
        using var tempDirectory = new TempDirectory(nameof(IndexProviderTests));
        using var sut = CreateProvider(tempDirectory);

        sut.Get("sample");
        sut.CloseAll();

        // act
        var exception = Record.Exception(() => sut.CloseAll());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public void CloseAll_CommittedWritesAreReadableAfterReopen() {
        // arrange
        using var tempDirectory = new TempDirectory(nameof(IndexProviderTests));
        using var sut = CreateProvider(tempDirectory);

        var index = sut.Get("sample");

        index.Insert(CreateSampleDocuments());
        index.SaveChanges();

        // act
        sut.CloseAll();
        var reopened = sut.Get("sample");
        var result = reopened.Count();

        // assert
        Assert.True(result.Success);
        Assert.Equal(1, result.Value);
    }

    // This race is only observable under real concurrency: RemoveFromCache's
    // old key-only TryRemove could evict a *new* instance cached under the
    // same name by a concurrent Get() racing a CloseAll()/Dispose() callback,
    // orphaning the new instance (never disposed, keeps write.lock) and
    // making the next Get() throw LockObtainFailedException. This stress
    // test exercises that window; with the key+value TryRemove fix it should
    // never throw, regardless of how many iterations run.
    [Fact]
    [IntegrationTest]
    public async Task CloseAll_ConcurrentWithGet_DoesNotOrphanIndexOrThrow() {
        // arrange
        using var tempDirectory = new TempDirectory(nameof(IndexProviderTests));
        using var sut = CreateProvider(tempDirectory);
        const string IndexName = "race-index";
        const int Iterations = 200;

        var exceptions = new ConcurrentBag<Exception>();
        using var cts = new CancellationTokenSource();
        var testCancellationToken = TestContext.Current.CancellationToken;

        var writerTask = Task.Run(() => {
            while (!cts.IsCancellationRequested) {
                try {
                    var index = sut.Get(IndexName);

                    index.Insert(CreateSampleDocuments());
                    index.SaveChanges();
                }
                catch (ObjectDisposedException) {
                    // Expected: the index obtained above may have been
                    // closed concurrently by CloseAll() before SaveChanges.
                }
                catch (Exception ex) {
                    exceptions.Add(ex);
                }
            }
        }, testCancellationToken);

        var closerTask = Task.Run(() => {
            for (var i = 0; i < Iterations; i++) {
                try { sut.CloseAll(); }
                catch (Exception ex) { exceptions.Add(ex); }
            }
        }, testCancellationToken);

        // act
        await closerTask;
        await cts.CancelAsync();
        await writerTask;

        // assert
        Assert.Empty(exceptions);

        var finalIndex = sut.Get(IndexName);
        var result = finalIndex.Count();
        Assert.True(result.Success);
    }
}
