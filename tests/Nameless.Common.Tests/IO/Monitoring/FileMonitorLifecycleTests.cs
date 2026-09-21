using Moq;

namespace Nameless.IO.Monitoring;

public class FileMonitorLifecycleTests
{
    [Fact]
    public void Ctor_ExposesRootAndGlob()
    {
        using var fx = new FileMonitorFixture("**/*.txt", register: false, start: false);

        Assert.Multiple(
            () => Assert.Equal(fx.Root, fx.Monitor.Root),
            () => Assert.Equal("**/*.txt", fx.Monitor.Glob)
        );
    }

    [Theory]
    [InlineData("", "**")]
    [InlineData("   ", "**")]
    public void Ctor_BlankRoot_Throws(string root, string glob)
    {
        var fx = new FileMonitorFixture(register: false, start: false);

        Assert.Throws<ArgumentException>(() => new FileMonitor(root, glob, fx.Watcher.Object, fx.Probe.Object, fx.Time));
    }

    [Fact]
    public void Ctor_BlankGlob_Throws()
    {
        var fx = new FileMonitorFixture(register: false, start: false);

        Assert.Throws<ArgumentException>(() => new FileMonitor(fx.Root, " ", fx.Watcher.Object, fx.Probe.Object, fx.Time));
    }

    [Fact]
    public void Start_ConfiguresAndEnablesWatcher()
    {
        using var fx = new FileMonitorFixture(start: false);

        fx.Monitor.Start();

        Assert.Multiple(
            () => Assert.Equal(fx.Root, fx.Watcher.Object.Path),
            () => Assert.Equal(fx.Options.InternalBufferSize, fx.Watcher.Object.InternalBufferSize),
            () => Assert.True(fx.Watcher.Object.EnableRaisingEvents)
        );
    }

    [Theory]
    [InlineData("**", true)]
    [InlineData("**/*.txt", true)]
    [InlineData("sub/*.txt", true)]
    [InlineData("sub\\*.txt", true)]
    [InlineData("*.txt", false)]
    [InlineData("report.docx", false)]
    public void Start_IncludesSubdirectoriesOnlyWhenGlobCanReachThem(string glob, bool expected)
    {
        using var fx = new FileMonitorFixture(glob, start: false);

        fx.Monitor.Start();

        Assert.Equal(expected, fx.Watcher.Object.IncludeSubdirectories);
    }

    [Fact]
    public void Start_CalledTwice_EnablesWatcherOnce()
    {
        using var fx = new FileMonitorFixture(start: false);

        fx.Monitor.Start();
        fx.Monitor.Start();

        fx.Watcher.VerifySet(w => w.EnableRaisingEvents = true, Times.Once);
    }

    [Fact]
    public void Start_WithoutHandlers_DoesNotThrowWhenEventsArrive()
    {
        using var fx = new FileMonitorFixture(register: false);

        fx.RaiseCreated("a.txt");
        fx.RaiseDeleted("a.txt");
        fx.RaiseRenamed("a.txt", "b.txt");
        fx.RaiseError(new InvalidOperationException());
        fx.Advance(5_000);
    }

    [Fact]
    public void Handlers_AfterStart_Throw()
    {
        using var fx = new FileMonitorFixture(register: false);

        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnCreated(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnChanged(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnDeleted(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnRenamed(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnError(_ => { }));
    }

    [Fact]
    public void Handlers_SameEventTwice_Throw()
    {
        using var fx = new FileMonitorFixture(register: false, start: false);
        fx.Monitor.OnCreated(_ => { });
        fx.Monitor.OnChanged(_ => { });
        fx.Monitor.OnDeleted(_ => { });
        fx.Monitor.OnRenamed(_ => { });
        fx.Monitor.OnError(_ => { });

        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnCreated(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnChanged(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnDeleted(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnRenamed(_ => { }));
        Assert.Throws<InvalidOperationException>(() => fx.Monitor.OnError(_ => { }));
    }

    [Fact]
    public void Handlers_Null_Throw()
    {
        using var fx = new FileMonitorFixture(register: false, start: false);

        Assert.Throws<ArgumentNullException>(() => fx.Monitor.OnCreated(null!));
        Assert.Throws<ArgumentNullException>(() => fx.Monitor.OnChanged(null!));
        Assert.Throws<ArgumentNullException>(() => fx.Monitor.OnDeleted(null!));
        Assert.Throws<ArgumentNullException>(() => fx.Monitor.OnRenamed(null!));
        Assert.Throws<ArgumentNullException>(() => fx.Monitor.OnError(null!));
    }

    [Fact]
    public void Dispose_DisposesWatcher_AndIsIdempotent()
    {
        var fx = new FileMonitorFixture();

        fx.Dispose();
        fx.Dispose();

        fx.Watcher.Verify(w => w.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_CancelsPendingNotifications()
    {
        var fx = new FileMonitorFixture();
        fx.RaiseCreated("a.txt");

        fx.Dispose();
        fx.Advance(5_000);

        Assert.Empty(fx.Created);
    }

    [Fact]
    public void Dispose_StopsListeningToWatcher()
    {
        var fx = new FileMonitorFixture();
        fx.Dispose();

        fx.RaiseDeleted("a.txt");

        Assert.Empty(fx.Deleted);
    }

    [Fact]
    public void AfterDispose_StartAndHandlers_Throw()
    {
        var fx = new FileMonitorFixture(register: false, start: false);
        fx.Dispose();

        Assert.Throws<ObjectDisposedException>(() => fx.Monitor.Start());
        Assert.Throws<ObjectDisposedException>(() => fx.Monitor.OnCreated(_ => { }));
    }
}
