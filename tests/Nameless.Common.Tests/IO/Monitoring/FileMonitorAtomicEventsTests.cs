namespace Nameless.IO.Monitoring;

public class FileMonitorAtomicEventsTests
{
    [Fact]
    public void Renamed_RaisedImmediatelyWithOldAndNewNames()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseRenamed("old.txt", "new.txt");

        var evt = Assert.Single(fx.Renamed);
        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("old.txt"), evt.PreviousPath),
            () => Assert.Equal(fx.FullPath("new.txt"), evt.CurrentPath),
            () => Assert.Equal("old.txt", evt.PreviousName),
            () => Assert.Equal("new.txt", evt.Name),
            () => Assert.Equal(fx.Root, evt.Directory)
        );
    }

    [Fact]
    public void Renamed_IntoAnotherFolder_KeepsBothFullPaths()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseRenamed("a.txt", Path.Combine("sub", "a.txt"));

        var evt = Assert.Single(fx.Renamed);
        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("a.txt"), evt.PreviousPath),
            () => Assert.Equal(Path.Combine(fx.Root, "sub"), evt.Directory)
        );
    }

    [Fact]
    public void Renamed_Directory_IsIgnored()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("folder", FileProbeResult.Directory);

        fx.RaiseRenamed("old", "folder");

        Assert.Empty(fx.Renamed);
    }

    [Fact]
    public void Renamed_CancelsPendingCreatedOfOldPath()
    {
        using var fx = new FileMonitorFixture();
        fx.RaiseCreated("download.part");

        fx.RaiseRenamed("download.part", "download.pdf");
        fx.Advance(5_000);

        Assert.Multiple(
            () => Assert.Single(fx.Renamed),
            () => Assert.Empty(fx.Created)
        );
    }

    [Fact]
    public void Deleted_RaisedAfterReplaceGracePeriod()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseDeleted("a.txt");
        fx.Advance(299);
        Assert.Empty(fx.Deleted);

        fx.Advance(1);

        Assert.Equal(fx.FullPath("a.txt"), Assert.Single(fx.Deleted).CurrentPath);
    }

    [Fact]
    public void Deleted_WithZeroGracePeriod_RaisedImmediately()
    {
        using var fx = new FileMonitorFixture(configure: o => o.ReplaceGracePeriod = TimeSpan.Zero);

        fx.RaiseDeleted("a.txt");

        Assert.Single(fx.Deleted);
    }

    [Fact]
    public void Deleted_DropsPendingChanged()
    {
        using var fx = new FileMonitorFixture();
        fx.RaiseChanged("a.txt");

        fx.RaiseDeleted("a.txt");
        fx.Advance(5_000);

        Assert.Multiple(
            () => Assert.Single(fx.Deleted),
            () => Assert.Empty(fx.Changed)
        );
    }

    [Fact]
    public void Deleted_WhileCreatedStillPending_SwallowsBoth()
    {
        using var fx = new FileMonitorFixture();
        fx.RaiseCreated("scratch.tmp");

        fx.RaiseDeleted("scratch.tmp");
        fx.Advance(5_000);

        Assert.Multiple(
            () => Assert.Empty(fx.Deleted),
            () => Assert.Empty(fx.Created)
        );
    }

    [Fact]
    public void Error_ForwardsWatcherException()
    {
        using var fx = new FileMonitorFixture();
        var failure = new InternalBufferOverflowException();

        fx.RaiseError(failure);

        Assert.Same(failure, Assert.Single(fx.Errors).Exception);
    }

    [Fact]
    public void HandlerThrowing_IsRoutedToErrorHandler()
    {
        using var fx = new FileMonitorFixture(register: false, start: false);
        var errors = new List<FileMonitorErrorEvent>();
        var boom = new InvalidOperationException("boom");
        fx.Monitor.OnDeleted(_ => throw boom);
        fx.Monitor.OnError(e => errors.Add(e));
        fx.Monitor.Start();

        fx.RaiseDeleted("a.txt");
        fx.Advance(300);

        Assert.Same(boom, Assert.Single(errors).Exception);
    }

    [Fact]
    public void ErrorHandlerThrowing_IsSwallowed()
    {
        using var fx = new FileMonitorFixture(register: false, start: false);
        fx.Monitor.OnError(_ => throw new InvalidOperationException());
        fx.Monitor.Start();

        fx.RaiseError(new IOException());
    }
}
