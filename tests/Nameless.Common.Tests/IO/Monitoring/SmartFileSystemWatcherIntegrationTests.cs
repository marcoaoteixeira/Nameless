using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.Monitoring;

[IntegrationTest]
public class SmartFileSystemWatcherIntegrationTests : IDisposable {
    private readonly string _root;

    public SmartFileSystemWatcherIntegrationTests() {
        _root = Path.Combine(Path.GetTempPath(), $"nameless-watcher-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (Directory.Exists(_root)) {
            Directory.Delete(_root, recursive: true);
        }
    }

    private SmartFileSystemWatcher CreateSut(string filter = "*.txt", string subPath = "") {
        var fileProvider = new PhysicalFileProvider(_root);
        var options = new SmartFileSystemWatcherOptions {
            SubPath = subPath,
            Filter = filter,
            LockProbeDelay = TimeSpan.FromMilliseconds(50),
            LockProbeMaxAttempts = 20
        };
        var logger = new Mock<ILogger<SmartFileSystemWatcher>>().Object;
        return new SmartFileSystemWatcher(fileProvider, retryFactory: null, Options.Create(options), logger);
    }

    private static string WriteFile(string path, string content = "hello") {
        File.WriteAllText(path, content);
        return path;
    }

    // ── Created ───────────────────────────────────────────────────────────

    [Fact]
    public async Task WhenFileIsCreated_FiresOnCreateCallbackAfterWriteIsComplete() {
        // arrange
        var sut = CreateSut();
        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        sut.OnCreated((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act
        var filePath = Path.Combine(_root, "created.txt");
        WriteFile(filePath);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal(filePath, result.CurrentFilePath);
        Assert.Null(result.PreviousFilePath);
        Assert.True(File.Exists(result.CurrentFilePath));

        await sut.StopAsync(TestContext.Current.CancellationToken);
    }

    // ── Deleted ───────────────────────────────────────────────────────────

    [Fact]
    public async Task WhenFileIsDeleted_FiresOnDeleteCallback() {
        // arrange
        var filePath = WriteFile(Path.Combine(_root, "to-delete.txt"));
        var sut = CreateSut();
        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        sut.OnDeleted((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act
        File.Delete(filePath);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Contains("to-delete.txt", result.CurrentFilePath);
        Assert.Null(result.PreviousFilePath);

        await sut.StopAsync(TestContext.Current.CancellationToken);
    }

    // ── Renamed ───────────────────────────────────────────────────────────

    [Fact]
    public async Task WhenFileIsRenamed_FiresOnRenameCallback() {
        // arrange
        var oldPath = WriteFile(Path.Combine(_root, "old-name.txt"));
        var newPath = Path.Combine(_root, "new-name.txt");
        var sut = CreateSut();
        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        sut.OnRenamed((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act
        File.Move(oldPath, newPath);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Contains("new-name.txt", result.CurrentFilePath);
        Assert.Contains("old-name.txt", result.PreviousFilePath!);

        await sut.StopAsync(TestContext.Current.CancellationToken);
    }

    // ── Changed ───────────────────────────────────────────────────────────

    [Fact]
    public async Task WhenFileIsModified_FiresOnChangeCallbackAfterWriteIsComplete() {
        // arrange
        var filePath = WriteFile(Path.Combine(_root, "to-modify.txt"), "initial content");
        var sut = CreateSut();
        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        sut.OnChanged((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act — overwrite the file with different content
        await Task.Delay(100, TestContext.Current.CancellationToken); // slight delay so LastModified differs
        File.WriteAllText(filePath, "modified content");

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal(filePath, result.CurrentFilePath);
        Assert.Null(result.PreviousFilePath);
        Assert.Equal("modified content", File.ReadAllText(result.CurrentFilePath));

        await sut.StopAsync(TestContext.Current.CancellationToken);
    }

    // ── Multiple concurrent files ─────────────────────────────────────────

    [Fact]
    public async Task WhenMultipleFilesCreatedConcurrently_AllFireIndependently() {
        // arrange
        const int fileCount = 5;
        var sut = CreateSut();
        var received = new System.Collections.Concurrent.ConcurrentBag<string>();
        var allReceived = new TaskCompletionSource<bool>();

        sut.OnCreated((args, _) => {
            received.Add(args.CurrentFilePath);
            if (received.Count >= fileCount) {
                allReceived.TrySetResult(true);
            }

            return ValueTask.CompletedTask;
        });

        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act — create all files concurrently
        var tasks = Enumerable.Range(0, fileCount)
            .Select(i => Task.Run(
                () => WriteFile(Path.Combine(_root, $"concurrent-{i}.txt")),
                TestContext.Current.CancellationToken));
        await Task.WhenAll(tasks);

        // assert
        await allReceived.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal(fileCount, received.Count);

        await sut.StopAsync(TestContext.Current.CancellationToken);
    }

    // ── SubPath scoping ───────────────────────────────────────────────────

    [Fact]
    public async Task WhenSubPathSet_OnlyWatchesFilesInSubDirectory() {
        // arrange
        var subDir = Path.Combine(_root, "watched");
        Directory.CreateDirectory(subDir);
        var outsideDir = Path.Combine(_root, "outside");
        Directory.CreateDirectory(outsideDir);

        var sut = CreateSut(filter: "*.txt", subPath: "watched");
        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        sut.OnCreated((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act — write to outside dir first (should not trigger), then inside
        WriteFile(Path.Combine(outsideDir, "ignored.txt"));
        await Task.Delay(200, TestContext.Current.CancellationToken);
        WriteFile(Path.Combine(subDir, "watched-file.txt"));

        // assert — callback fires for the watched subdirectory file
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Contains("watched-file.txt", result.CurrentFilePath);

        await sut.StopAsync(TestContext.Current.CancellationToken);
    }
}
