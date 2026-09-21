using Moq;

namespace Nameless.IO.Monitoring;

// Options in the fixture: quiet 500ms, probe interval 200ms (no back-off), locked-too-long after 2000ms.
public class FileMonitorSettlingTests
{
    [Fact]
    public void Created_NotRaisedBeforeQuietPeriod()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseCreated("a.txt");
        fx.Advance(499);

        Assert.Empty(fx.Created);
    }

    [Fact]
    public void Created_RaisedOnceAfterQuietPeriodWhenAvailable()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseCreated("a.txt");
        fx.Advance(500);
        fx.Advance(10_000);

        Assert.Equal(fx.FullPath("a.txt"), Assert.Single(fx.Created).CurrentPath);
    }

    [Fact]
    public void Created_ChangedEventsAreSuppressedAndExtendQuietPeriod()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseCreated("a.txt");
        fx.Advance(300);
        fx.RaiseChanged("a.txt");
        fx.Advance(300);
        fx.RaiseChanged("a.txt");
        fx.Advance(499);
        Assert.Empty(fx.Created);

        fx.Advance(1);

        Assert.Multiple(
            () => Assert.Single(fx.Created),
            () => Assert.Empty(fx.Changed)
        );
    }

    [Fact]
    public void Created_WhileLocked_RaisedWhenReleased()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("a.txt", FileProbeResult.Locked, FileProbeResult.Locked, FileProbeResult.Available);

        fx.RaiseCreated("a.txt");
        fx.Advance(899);
        Assert.Empty(fx.Created);

        fx.Advance(1);

        Assert.Single(fx.Created);
    }

    [Fact]
    public void Locked_BeyondThreshold_ReportsFileLockedTooLongOnce_AndKeepsWatching()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("a.txt", FileProbeResult.Locked);

        fx.RaiseCreated("a.txt");
        fx.Advance(2_499);
        Assert.Empty(fx.Errors);

        fx.Advance(1);

        var error = Assert.IsType<FileLockedTooLongException>(Assert.Single(fx.Errors).Exception);
        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("a.txt"), error.FilePath),
            () => Assert.Equal(TimeSpan.FromSeconds(2), error.LockedFor)
        );

        fx.Advance(10_000);
        Assert.Multiple(
            () => Assert.Single(fx.Errors),
            () => Assert.Empty(fx.Created)
        );

        fx.ProbeReturns("a.txt", FileProbeResult.Available);
        fx.Advance(200);

        Assert.Multiple(
            () => Assert.Single(fx.Created),
            () => Assert.Single(fx.Errors)
        );
    }

    [Fact]
    public void Locked_NewRawEventStartsNewEpisode_WarnsAgain()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("a.txt", FileProbeResult.Locked);

        fx.RaiseChanged("a.txt");
        fx.Advance(2_500);
        Assert.Single(fx.Errors);

        fx.RaiseChanged("a.txt");
        fx.Advance(2_500);

        Assert.Multiple(
            () => Assert.Equal(2, fx.Errors.Count),
            () => Assert.All(fx.Errors, e => Assert.IsType<FileLockedTooLongException>(e.Exception))
        );
    }

    [Fact]
    public void Locked_ProbeIntervalBacksOffUpToMax()
    {
        using var fx = new FileMonitorFixture(configure: o => o.MaxProbeInterval = TimeSpan.FromMilliseconds(800));
        // Probes land at 500 (quiet period), then +200, +400, +800, +800.
        fx.ProbeReturns("a.txt", FileProbeResult.Locked, FileProbeResult.Locked, FileProbeResult.Locked, FileProbeResult.Locked, FileProbeResult.Available);

        fx.RaiseCreated("a.txt");
        fx.Advance(2_699);
        Assert.Empty(fx.Created);

        fx.Advance(1);

        Assert.Single(fx.Created);
        fx.Probe.Verify(p => p.Probe(It.IsAny<string>()), Times.Exactly(5));
    }

    [Fact]
    public void Created_FileGoneBeforeProbe_IsDropped()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("a.txt", FileProbeResult.NotFound);

        fx.RaiseCreated("a.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Empty(fx.Created),
            () => Assert.Empty(fx.Errors)
        );
    }

    [Fact]
    public void Created_Directory_IsDropped()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("folder", FileProbeResult.Directory);

        fx.RaiseCreated("folder");
        fx.Advance(10_000);

        Assert.Empty(fx.Created);
    }

    [Fact]
    public void Created_FilesSettleIndependently()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseCreated("a.txt");
        fx.Advance(300);
        fx.RaiseCreated("b.txt");
        fx.Advance(200);

        Assert.Equal(fx.FullPath("a.txt"), Assert.Single(fx.Created).CurrentPath);

        fx.Advance(300);

        Assert.Equal(2, fx.Created.Count);
    }

    [Fact]
    public void Changed_ManyEventsProduceSingleNotificationAfterQuietPeriod()
    {
        using var fx = new FileMonitorFixture();

        for (var i = 0; i < 10; i++)
        {
            fx.RaiseChanged("a.txt");
            fx.Advance(100);
        }

        Assert.Empty(fx.Changed);

        fx.Advance(400);
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("a.txt"), Assert.Single(fx.Changed).CurrentPath),
            () => Assert.Empty(fx.Created)
        );
    }

    [Fact]
    public void Changed_WhileEditorHoldsFile_RaisedOnceWhenReleased()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("doc.docx", FileProbeResult.Locked, FileProbeResult.Locked, FileProbeResult.Locked, FileProbeResult.Available);

        fx.RaiseChanged("doc.docx");
        fx.Advance(1_099);
        Assert.Empty(fx.Changed);

        fx.Advance(1);
        fx.Advance(10_000);

        Assert.Single(fx.Changed);
    }

    [Fact]
    public void Changed_WhileProbing_RestartsQuietPeriod()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("doc.docx", FileProbeResult.Locked, FileProbeResult.Available);

        fx.RaiseChanged("doc.docx");
        fx.Advance(600);
        fx.RaiseChanged("doc.docx");
        fx.Advance(499);
        Assert.Empty(fx.Changed);

        fx.Advance(1);

        Assert.Single(fx.Changed);
    }

    [Fact]
    public void Changed_AfterNotification_StartsNewCycle()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseChanged("a.txt");
        fx.Advance(500);
        fx.RaiseChanged("a.txt");
        fx.Advance(500);

        Assert.Equal(2, fx.Changed.Count);
    }

    [Fact]
    public void Changed_FileGoneBeforeProbe_IsDropped()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("a.txt", FileProbeResult.NotFound);

        fx.RaiseChanged("a.txt");
        fx.Advance(10_000);

        Assert.Empty(fx.Changed);
    }
}
