using System.Collections;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.Monitoring;

[UnitTest]
public class SmartFileSystemWatcherTests {
    private readonly Mock<IFileProvider> _fileProvider = new();
    private readonly Mock<ILogger<SmartFileSystemWatcher>> _logger = new();

    private SmartFileSystemWatcherOptions DefaultOptions => new() {
        Filter = "*.txt",
        LockProbeDelay = TimeSpan.FromMilliseconds(10),
        LockProbeMaxAttempts = 3
    };

    private SmartFileSystemWatcher CreateSut(SmartFileSystemWatcherOptions? options = null) {
        return new SmartFileSystemWatcher(_fileProvider.Object, retryFactory: null, Options.Create(options ?? DefaultOptions), _logger.Object);
    }

    private static Mock<IFileInfo> CreateFileInfo(
        string name,
        long length = 100L,
        DateTimeOffset? lastModified = null,
        string? physicalPath = null) {
        var mock = new Mock<IFileInfo>();
        mock.SetupGet(f => f.Name).Returns(name);
        mock.SetupGet(f => f.Length).Returns(length);
        mock.SetupGet(f => f.LastModified).Returns(lastModified ?? DateTimeOffset.UtcNow);
        mock.SetupGet(f => f.IsDirectory).Returns(false);
        mock.SetupGet(f => f.PhysicalPath).Returns(physicalPath);
        return mock;
    }

    /// <summary>
    ///     Sets up Watch() to capture the registered callback each time it is called.
    ///     Returns a holder whose Value will be set after StartAsync is called.
    /// </summary>
    private CallbackHolder SetupWatchCapture() {
        var holder = new CallbackHolder();
        _fileProvider
            .Setup(p => p.Watch(It.IsAny<string>()))
            .Returns(() => {
                var token = new Mock<IChangeToken>();
                token.SetupGet(t => t.ActiveChangeCallbacks).Returns(true);
                token.Setup(t => t.RegisterChangeCallback(
                        It.IsAny<Action<object?>>(),
                        It.IsAny<object?>()))
                    .Callback<Action<object?>, object?>((cb, _) => holder.Value = cb)
                    .Returns(Mock.Of<IDisposable>());
                return token.Object;
            });
        return holder;
    }

    private void SetupDirectorySequence(params IDirectoryContents[] sequence) {
        var queue = new Queue<IDirectoryContents>(sequence);
        _fileProvider
            .Setup(p => p.GetDirectoryContents(It.IsAny<string>()))
            .Returns(() => queue.Count > 0 ? queue.Dequeue() : sequence[^1]);
    }

    // ── Constructor guards ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenFileProviderIsNull_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(
            () => new SmartFileSystemWatcher(null!, retryFactory: null, Options.Create(DefaultOptions), _logger.Object));
    }

    [Fact]
    public void Constructor_WhenOptionsIsNull_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(
            () => new SmartFileSystemWatcher(_fileProvider.Object, retryFactory: null, null!, _logger.Object));
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(
            () => new SmartFileSystemWatcher(_fileProvider.Object, retryFactory: null, Options.Create(DefaultOptions), null!));
    }

    // ── OnXxx registration ────────────────────────────────────────────────────

    [Fact]
    public void OnCreate_WhenCallbackIsNull_ThrowsArgumentNullException() {
        var sut = CreateSut();
        Assert.Throws<ArgumentNullException>(() => sut.OnCreated(null!));
    }

    [Fact]
    public void OnDelete_WhenCallbackIsNull_ThrowsArgumentNullException() {
        var sut = CreateSut();
        Assert.Throws<ArgumentNullException>(() => sut.OnDeleted(null!));
    }

    [Fact]
    public void OnRename_WhenCallbackIsNull_ThrowsArgumentNullException() {
        var sut = CreateSut();
        Assert.Throws<ArgumentNullException>(() => sut.OnRenamed(null!));
    }

    [Fact]
    public void OnChange_WhenCallbackIsNull_ThrowsArgumentNullException() {
        var sut = CreateSut();
        Assert.Throws<ArgumentNullException>(() => sut.OnChanged(null!));
    }

    [Fact]
    public void OnError_WhenCallbackIsNull_ThrowsArgumentNullException() {
        var sut = CreateSut();
        Assert.Throws<ArgumentNullException>(() => sut.OnError(null!));
    }

    // ── StartAsync – invalid token ─────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WhenTokenHasNoActiveCallbacks_ThrowsSmartFileSystemWatcherException() {
        // arrange
        var inactiveToken = new Mock<IChangeToken>();
        inactiveToken.SetupGet(t => t.ActiveChangeCallbacks).Returns(false);
        _fileProvider.Setup(p => p.Watch(It.IsAny<string>())).Returns(inactiveToken.Object);
        _fileProvider.Setup(p => p.GetDirectoryContents(It.IsAny<string>()))
                     .Returns(new FakeDirectoryContents());

        var sut = CreateSut();

        // act & assert
        await Assert.ThrowsAsync<SmartFileSystemWatcherException>(
            () => sut.StartAsync(TestContext.Current.CancellationToken));
    }

    // ── OnDelete callback ─────────────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WhenFileIsDeleted_FiresOnDeleteCallback() {
        // arrange
        var holder = SetupWatchCapture();
        var fileA = CreateFileInfo("fileA.txt");
        SetupDirectorySequence(
            new FakeDirectoryContents(fileA.Object),
            new FakeDirectoryContents());

        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        var sut = CreateSut();
        sut.OnDeleted((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal("fileA.txt", result.CurrentFilePath);
        Assert.Null(result.PreviousFilePath);
    }

    // ── OnDelete overwrites ───────────────────────────────────────────────

    [Fact]
    public async Task OnDelete_CalledTwice_SecondCallbackOverwritesFirst() {
        // arrange
        var holder = SetupWatchCapture();
        var fileA = CreateFileInfo("fileA.txt");
        SetupDirectorySequence(
            new FakeDirectoryContents(fileA.Object),
            new FakeDirectoryContents());

        var firstCalled = false;
        var secondTcs = new TaskCompletionSource<bool>();

        var sut = CreateSut();
        sut.OnDeleted((_, _) => { firstCalled = true; return ValueTask.CompletedTask; });
        sut.OnDeleted((_, _) => { secondTcs.TrySetResult(true); return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);

        // assert
        await secondTcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.False(firstCalled, "First callback should have been overwritten by second");
    }

    // ── OnCreate callback (no physical path → fires immediately) ─────────

    [Fact]
    public async Task StartAsync_WhenFileIsCreated_FiresOnCreateCallback() {
        // arrange
        var holder = SetupWatchCapture();
        var fileA = CreateFileInfo("fileA.txt", physicalPath: null);
        SetupDirectorySequence(
            new FakeDirectoryContents(),
            new FakeDirectoryContents(fileA.Object));

        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        var sut = CreateSut();
        sut.OnCreated((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal("fileA.txt", result.CurrentFilePath);
        Assert.Null(result.PreviousFilePath);
    }

    // ── OnRename callback ─────────────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WhenFileIsRenamed_FiresOnRenameCallback() {
        // arrange
        var holder = SetupWatchCapture();
        var fileA = CreateFileInfo("fileA.txt");
        var fileB = CreateFileInfo("fileB.txt");
        SetupDirectorySequence(
            new FakeDirectoryContents(fileA.Object),
            new FakeDirectoryContents(fileB.Object));

        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        var sut = CreateSut();
        sut.OnRenamed((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal("fileB.txt", result.CurrentFilePath);
        Assert.Equal("fileA.txt", result.PreviousFilePath);
    }

    // ── OnChange callback (no physical path → fires immediately) ─────────

    [Fact]
    public async Task StartAsync_WhenFileIsModified_FiresOnChangeCallback() {
        // arrange
        var holder = SetupWatchCapture();
        var t0 = DateTimeOffset.UtcNow;
        var fileV1 = CreateFileInfo("fileA.txt", length: 100L, lastModified: t0, physicalPath: null);
        var fileV2 = CreateFileInfo("fileA.txt", length: 200L, lastModified: t0.AddSeconds(1), physicalPath: null);
        SetupDirectorySequence(
            new FakeDirectoryContents(fileV1.Object),
            new FakeDirectoryContents(fileV2.Object));

        var tcs = new TaskCompletionSource<FileWatcherEventArgs>();
        var sut = CreateSut();
        sut.OnChanged((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.Equal("fileA.txt", result.CurrentFilePath);
        Assert.Null(result.PreviousFilePath);
    }

    // ── Lock probe exhaustion → OnError ───────────────────────────────────

    [Fact]
    public async Task StartAsync_WhenLockProbeExhausted_FiresOnErrorCallback() {
        // arrange
        var holder = SetupWatchCapture();

        // Non-existent physical path → File.Open always throws FileNotFoundException (IOException)
        var physPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid():N}.txt");
        var fileA = CreateFileInfo("fileA.txt", physicalPath: physPath);
        SetupDirectorySequence(
            new FakeDirectoryContents(),
            new FakeDirectoryContents(fileA.Object));

        var options = new SmartFileSystemWatcherOptions {
            Filter = "*.txt",
            LockProbeDelay = TimeSpan.FromMilliseconds(1),
            LockProbeMaxAttempts = 2
        };

        var tcs = new TaskCompletionSource<FileWatcherErrorEventArgs>();
        var sut = CreateSut(options);
        sut.OnCreated((_, _) => ValueTask.CompletedTask);
        sut.OnError((args, _) => { tcs.TrySetResult(args); return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);

        // assert
        var result = await tcs.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(result.Error);
    }

    // ── Filter matching ───────────────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WhenFileDoesNotMatchFilter_IgnoresFile() {
        // arrange
        var holder = SetupWatchCapture();
        var nonMatchingFile = CreateFileInfo("fileA.json");
        SetupDirectorySequence(
            new FakeDirectoryContents(),
            new FakeDirectoryContents(nonMatchingFile.Object));

        var createCalled = false;
        var sut = CreateSut(); // filter = *.txt
        sut.OnCreated((_, _) => { createCalled = true; return ValueTask.CompletedTask; });

        // act
        await sut.StartAsync(TestContext.Current.CancellationToken);
        holder.Value!.Invoke(null);
        await Task.Delay(200, TestContext.Current.CancellationToken);

        // assert
        Assert.False(createCalled, "Non-matching file should not trigger OnCreate");
    }

    // ── StopAsync cancels probe ───────────────────────────────────────────

    [Fact]
    public async Task StopAsync_WhileRunning_CompletesGracefully() {
        // arrange
        SetupWatchCapture();
        _fileProvider.Setup(p => p.GetDirectoryContents(It.IsAny<string>()))
                     .Returns(new FakeDirectoryContents());

        var sut = CreateSut();
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // act & assert (should not throw)
        await sut.StopAsync(TestContext.Current.CancellationToken);
    }

    // ── Nested helpers ────────────────────────────────────────────────────

    private sealed class CallbackHolder {
        public Action<object?>? Value { get; set; }
    }

    private sealed class FakeDirectoryContents : IDirectoryContents {
        private readonly IFileInfo[] _entries;

        public bool Exists => true;

        public FakeDirectoryContents(params IFileInfo[] entries) { _entries = entries; }

        public IEnumerator<IFileInfo> GetEnumerator() => ((IEnumerable<IFileInfo>)_entries).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
    }
}
