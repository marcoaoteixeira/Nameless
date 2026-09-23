using System.Collections.Concurrent;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Nameless.IO.Monitoring;

[UnitTest]
public class FileMonitorTests {
    private sealed class Harness : IDisposable {
        public Mock<IFileSystemWatcherAdapter> Watcher { get; } = new();
        public Mock<IFileProbe> Probe { get; } = new();
        public FakeTimeProvider Time { get; } = new();
        public string Root { get; } = SysPath.Combine(SysPath.GetTempPath(), "fm-root");
        public FileMonitor Monitor { get; }
        public ConcurrentQueue<string> Events { get; } = new();
        public ConcurrentQueue<Exception> Errors { get; } = new();

        public Harness(string glob = "*.txt", FileMonitorOptions? options = null) {
            Probe.Setup(p => p.Probe(It.IsAny<string>())).Returns(FileProbeResult.Available);

            Monitor = new FileMonitor(Root, glob, Watcher.Object, Probe.Object, Time, options);
        }

        public string Path(string name) => SysPath.Combine(Root, name);

        public void Subscribe(Action<FileCreatedEvent>? onCreated = null) {
            Monitor.OnCreated(onCreated ?? (e => Events.Enqueue($"created:{e.Name}")));
            Monitor.OnChanged(e => Events.Enqueue($"changed:{e.Name}"));
            Monitor.OnDeleted(e => Events.Enqueue($"deleted:{e.Name}"));
            Monitor.OnRenamed(e => Events.Enqueue($"renamed:{e.PreviousName}>{e.Name}"));
            Monitor.OnError(e => Errors.Enqueue(e.Exception));
        }

        public void RaiseCreated(string name) {
            Watcher.Raise(w => w.Created += null, Watcher.Object,
                new FileSystemEventArgs(WatcherChangeTypes.Created, Root, name));
        }

        public void RaiseChanged(string name) {
            Watcher.Raise(w => w.Changed += null, Watcher.Object,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, Root, name));
        }

        public void RaiseDeleted(string name) {
            Watcher.Raise(w => w.Deleted += null, Watcher.Object,
                new FileSystemEventArgs(WatcherChangeTypes.Deleted, Root, name));
        }

        public void RaiseRenamed(string oldName, string newName) {
            Watcher.Raise(w => w.Renamed += null, Watcher.Object,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, Root, newName, oldName));
        }

        public void RaiseError(Exception exception) {
            Watcher.Raise(w => w.Error += null, Watcher.Object, new ErrorEventArgs(exception));
        }

        public void Advance(int milliseconds, int step = 50) {
            for (var elapsed = 0; elapsed < milliseconds; elapsed += step) {
                Time.Advance(TimeSpan.FromMilliseconds(step));
            }
        }

        public static async Task WaitUntil(Func<bool> condition) {
            var deadline = DateTime.UtcNow.AddSeconds(5);

            while (!condition()) {
                if (DateTime.UtcNow > deadline) { throw new TimeoutException("Condition not met."); }

                await Task.Delay(10);
            }
        }

        public static Task Settle() => Task.Delay(150);

        public void Dispose() => Monitor.Dispose();
    }

    // ── Construction ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_SetsRootAndGlob() {
        // arrange & act
        using var harness = new Harness("**/*.txt");

        // assert
        Assert.Multiple(
            () => Assert.Equal(SysPath.GetFullPath(harness.Root), harness.Monitor.Root),
            () => Assert.Equal("**/*.txt", harness.Monitor.Glob)
        );
    }

    [Fact]
    public void Constructor_WithBlankRoot_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => new FileMonitor(" ", "*", new Mock<IFileSystemWatcherAdapter>().Object,
            new Mock<IFileProbe>().Object, TimeProvider.System));
    }

    [Fact]
    public void Constructor_WithBlankGlob_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => new FileMonitor("root", " ", new Mock<IFileSystemWatcherAdapter>().Object,
            new Mock<IFileProbe>().Object, TimeProvider.System));
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws() {
        // arrange
        var watcher = new Mock<IFileSystemWatcherAdapter>().Object;
        var probe = new Mock<IFileProbe>().Object;

        // act & assert
        Assert.Multiple(
            () => Assert.Throws<ArgumentNullException>(() => new FileMonitor("root", "*", null!, probe, TimeProvider.System)),
            () => Assert.Throws<ArgumentNullException>(() => new FileMonitor("root", "*", watcher, null!, TimeProvider.System)),
            () => Assert.Throws<ArgumentNullException>(() => new FileMonitor("root", "*", watcher, probe, null!))
        );
    }

    // ── Handler registration ──────────────────────────────────────────────────

    [Fact]
    public void OnCreated_WithNullAction_Throws() {
        // arrange
        using var harness = new Harness();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => harness.Monitor.OnCreated(null!));
    }

    [Fact]
    public void OnCreated_CalledTwice_Throws() {
        // arrange
        using var harness = new Harness();
        harness.Monitor.OnCreated(_ => { });

        // act & assert
        Assert.Throws<InvalidOperationException>(() => harness.Monitor.OnCreated(_ => { }));
    }

    [Fact]
    public void OnChanged_AfterStart_Throws() {
        // arrange
        using var harness = new Harness();
        harness.Monitor.Start();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => harness.Monitor.OnChanged(_ => { }));
    }

    [Fact]
    public void OnDeleted_AfterDispose_Throws() {
        // arrange
        var harness = new Harness();
        harness.Dispose();

        // act & assert
        Assert.Throws<ObjectDisposedException>(() => harness.Monitor.OnDeleted(_ => { }));
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    [Fact]
    public void Start_ConfiguresAndEnablesWatcher() {
        // arrange
        using var harness = new Harness("**/*.txt");

        // act
        harness.Monitor.Start();

        // assert
        harness.Watcher.VerifySet(w => w.Path = harness.Monitor.Root, Times.Once);
        harness.Watcher.VerifySet(w => w.IncludeSubdirectories = true, Times.Once);
        harness.Watcher.VerifySet(w => w.InternalBufferSize = 64 * 1024, Times.Once);
        harness.Watcher.VerifySet(w => w.EnableRaisingEvents = true, Times.Once);
    }

    [Fact]
    public void Start_WithSimpleGlob_DoesNotIncludeSubdirectories() {
        // arrange
        using var harness = new Harness("*.txt");

        // act
        harness.Monitor.Start();

        // assert
        harness.Watcher.VerifySet(w => w.IncludeSubdirectories = false, Times.Once);
    }

    [Fact]
    public void Start_CalledTwice_ConfiguresWatcherOnce() {
        // arrange
        using var harness = new Harness();

        // act
        harness.Monitor.Start();
        harness.Monitor.Start();

        // assert
        harness.Watcher.VerifySet(w => w.EnableRaisingEvents = true, Times.Once);
    }

    [Fact]
    public void Start_AfterDispose_Throws() {
        // arrange
        var harness = new Harness();
        harness.Dispose();

        // act & assert
        Assert.Throws<ObjectDisposedException>(harness.Monitor.Start);
    }

    [Fact]
    public void Dispose_DisposesWatcherOnceAndIsIdempotent() {
        // arrange
        var harness = new Harness();
        harness.Monitor.Start();
        harness.RaiseCreated("a.txt");
        harness.RaiseDeleted("b.txt");

        // act
        harness.Dispose();
        harness.Dispose();

        // assert
        harness.Watcher.Verify(w => w.Dispose(), Times.Once);
    }

    // ── Created / Changed ─────────────────────────────────────────────────────

    [Fact]
    public async Task Created_AfterQuietPeriod_NotifiesCreated() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(600);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["created:a.txt"], harness.Events);
    }

    [Fact]
    public async Task Changed_AfterQuietPeriod_NotifiesChanged() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseChanged("a.txt");
        harness.Advance(600);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["changed:a.txt"], harness.Events);
    }

    [Fact]
    public async Task Created_FollowedByChanges_NotifiesCreatedOnce() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(200);
        harness.RaiseChanged("a.txt");
        harness.Advance(200);
        harness.RaiseChanged("a.txt");
        harness.Advance(700);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        await Harness.Settle();
        Assert.Equal(["created:a.txt"], harness.Events);
    }

    [Fact]
    public async Task Created_BeforeQuietPeriodElapses_DoesNotNotifyYet() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(200);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Created_WhenFileNotFoundOnProbe_NotifiesNothing() {
        // arrange
        using var harness = new Harness();
        harness.Probe.Setup(p => p.Probe(It.IsAny<string>())).Returns(FileProbeResult.NotFound);
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(700);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Created_WhileLocked_NotifiesOnceAvailable() {
        // arrange
        using var harness = new Harness(options: new FileMonitorOptions {
            ProbeInterval = TimeSpan.FromMilliseconds(100)
        });
        harness.Probe.SetupSequence(p => p.Probe(It.IsAny<string>()))
               .Returns(FileProbeResult.Locked)
               .Returns(FileProbeResult.Locked)
               .Returns(FileProbeResult.Available);
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(2000, step: 25);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["created:a.txt"], harness.Events);
    }

    [Fact]
    public async Task Created_LockedTooLong_ReportsErrorOnce() {
        // arrange
        using var harness = new Harness(options: new FileMonitorOptions {
            ProbeInterval = TimeSpan.FromMilliseconds(100),
            MaxProbeInterval = TimeSpan.FromMilliseconds(200),
            LockedTooLongAfter = TimeSpan.FromSeconds(1)
        });
        harness.Probe.Setup(p => p.Probe(It.IsAny<string>())).Returns(FileProbeResult.Locked);
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(4000, step: 25);

        // assert
        await Harness.WaitUntil(() => !harness.Errors.IsEmpty);
        await Harness.Settle();
        Assert.Multiple(
            () => Assert.Single(harness.Errors),
            () => Assert.IsType<FileLockedTooLongException>(harness.Errors.Single()),
            () => Assert.Empty(harness.Events)
        );
    }

    [Fact]
    public async Task Created_WhenProbeThrows_ReportsError() {
        // arrange
        using var harness = new Harness();
        harness.Probe.Setup(p => p.Probe(It.IsAny<string>())).Throws(new InvalidOperationException("probe"));
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(700);

        // assert
        await Harness.WaitUntil(() => !harness.Errors.IsEmpty);
        Assert.Multiple(
            () => Assert.IsType<InvalidOperationException>(harness.Errors.First()),
            () => Assert.Empty(harness.Events)
        );
    }

    // ── Filtering ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Created_WithNonMatchingFile_IsIgnored() {
        // arrange
        using var harness = new Harness("*.txt");
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.log");
        harness.Advance(700);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Created_WithExcludedFile_IsIgnored() {
        // arrange
        using var harness = new Harness("*");
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("~$doc.txt");
        harness.RaiseCreated("scratch.tmp");
        harness.Advance(700);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Created_OutsideRoot_IsIgnored() {
        // arrange
        using var harness = new Harness("**/*.txt");
        harness.Subscribe();
        harness.Monitor.Start();

        var outside = SysPath.Combine(SysPath.GetTempPath(), "fm-elsewhere");

        // act
        harness.Watcher.Raise(w => w.Created += null, harness.Watcher.Object,
            new FileSystemEventArgs(WatcherChangeTypes.Created, outside, "a.txt"));
        harness.Advance(700);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Created_InSubdirectory_MatchesRecursiveGlob() {
        // arrange
        using var harness = new Harness("**/*.txt");
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated(SysPath.Combine("sub", "a.txt"));
        harness.Advance(700);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["created:a.txt"], harness.Events);
    }

    // ── Deleted ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleted_WithoutGracePeriod_NotifiesImmediately() {
        // arrange
        using var harness = new Harness(options: new FileMonitorOptions { ReplaceGracePeriod = TimeSpan.Zero });
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseDeleted("a.txt");

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["deleted:a.txt"], harness.Events);
    }

    [Fact]
    public async Task Deleted_WithGracePeriod_NotifiesAfterGraceExpires() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseDeleted("a.txt");
        harness.Advance(100);
        await Harness.Settle();
        var beforeGrace = harness.Events.ToArray();

        harness.Advance(300);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Multiple(
            () => Assert.Empty(beforeGrace),
            () => Assert.Equal(["deleted:a.txt"], harness.Events)
        );
    }

    [Fact]
    public async Task Deleted_ThenCreatedWithinGrace_NotifiesChangedInstead() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseDeleted("a.txt");
        harness.Advance(100);
        harness.RaiseCreated("a.txt");
        harness.Advance(1000);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        await Harness.Settle();
        Assert.Equal(["changed:a.txt"], harness.Events);
    }

    [Fact]
    public async Task Created_ThenDeletedBeforeAnnounced_NotifiesNothing() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(100);
        harness.RaiseDeleted("a.txt");
        harness.Advance(1000);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    // ── Renamed ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Renamed_NotifiesRenamed() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseRenamed("old.txt", "new.txt");

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["renamed:old.txt>new.txt"], harness.Events);
    }

    [Fact]
    public async Task Renamed_WithoutGracePeriod_NotifiesRenamed() {
        // arrange
        using var harness = new Harness(options: new FileMonitorOptions { ReplaceGracePeriod = TimeSpan.Zero });
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseRenamed("old.txt", "new.txt");

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        Assert.Equal(["renamed:old.txt>new.txt"], harness.Events);
    }

    [Fact]
    public async Task Renamed_ToDirectory_IsIgnored() {
        // arrange
        using var harness = new Harness("*");
        harness.Probe.Setup(p => p.Probe(It.IsAny<string>())).Returns(FileProbeResult.Directory);
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseRenamed("old", "new");
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Renamed_ToNonMatchingName_IsIgnored() {
        // arrange
        using var harness = new Harness("*.txt");
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseRenamed("old.txt", "new.bak");
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }

    [Fact]
    public async Task Renamed_OntoRecentlyDeletedPath_NotifiesChanged() {
        // arrange (safe-save: delete target, rename temp file onto it)
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseDeleted("target.txt");
        harness.Advance(50);
        harness.RaiseRenamed("target.tmp", "target.txt");
        harness.Advance(1000);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        await Harness.Settle();
        Assert.Equal(["changed:target.txt"], harness.Events);
    }

    [Fact]
    public async Task Renamed_PendingFile_DiscardsPreviousAndNotifiesRename() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("old.txt");
        harness.RaiseRenamed("old.txt", "new.txt");
        harness.Advance(1000);

        // assert
        await Harness.WaitUntil(() => !harness.Events.IsEmpty);
        await Harness.Settle();
        Assert.Equal(["renamed:old.txt>new.txt"], harness.Events);
    }

    // ── Errors ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task WatcherError_IsForwardedToErrorHandler() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe();
        harness.Monitor.Start();
        var exception = new IOException("overflow");

        // act
        harness.RaiseError(exception);

        // assert
        await Harness.WaitUntil(() => !harness.Errors.IsEmpty);
        Assert.Same(exception, harness.Errors.Single());
    }

    [Fact]
    public async Task HandlerThatThrows_IsReportedToErrorHandler() {
        // arrange
        using var harness = new Harness();
        harness.Subscribe(onCreated: _ => throw new InvalidOperationException("handler"));
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.Advance(700);

        // assert
        await Harness.WaitUntil(() => !harness.Errors.IsEmpty);
        Assert.IsType<InvalidOperationException>(harness.Errors.Single());
    }

    [Fact]
    public async Task ErrorHandlerThatThrows_IsSwallowed() {
        // arrange
        using var harness = new Harness();
        var delivered = 0;
        harness.Monitor.OnError(_ => {
            Interlocked.Increment(ref delivered);
            throw new InvalidOperationException("error handler");
        });
        harness.Monitor.Start();

        // act
        harness.RaiseError(new IOException("one"));
        harness.RaiseError(new IOException("two"));

        // assert
        await Harness.WaitUntil(() => Volatile.Read(ref delivered) == 2);
    }

    [Fact]
    public async Task Events_WithoutRegisteredHandlers_AreIgnoredSafely() {
        // arrange
        using var harness = new Harness();
        harness.Monitor.Start();

        // act
        harness.RaiseCreated("a.txt");
        harness.RaiseChanged("b.txt");
        harness.RaiseDeleted("c.txt");
        harness.RaiseError(new IOException());
        harness.Advance(1000);
        await Harness.Settle();

        // assert
        Assert.Empty(harness.Events);
    }
}
