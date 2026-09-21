namespace Nameless.IO.Monitoring;

// Options in the fixture: quiet 500ms, probe interval 200ms, replace grace 300ms.
// Editors rarely overwrite in place. They replace the file, which surfaces as delete/create/rename noise.
public class FileMonitorSafeSaveTests
{
    [Fact]
    public void DeleteThenCreate_SamePath_BecomesSingleChanged()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseDeleted("doc.txt");
        fx.Advance(100);
        fx.RaiseCreated("doc.txt");
        fx.RaiseChanged("doc.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("doc.txt"), Assert.Single(fx.Changed).CurrentPath),
            () => Assert.Empty(fx.Deleted),
            () => Assert.Empty(fx.Created)
        );
    }

    [Fact]
    public void DeleteThenCreate_AfterGracePeriod_RemainsDeletedThenCreated()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseDeleted("doc.txt");
        fx.Advance(300);
        fx.RaiseCreated("doc.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Single(fx.Deleted),
            () => Assert.Single(fx.Created),
            () => Assert.Empty(fx.Changed)
        );
    }

    [Fact]
    public void DeleteThenCreate_WaitsForFileToBeReleasedBeforeChanged()
    {
        using var fx = new FileMonitorFixture();
        fx.ProbeReturns("doc.txt", FileProbeResult.Locked, FileProbeResult.Available);

        fx.RaiseDeleted("doc.txt");
        fx.RaiseCreated("doc.txt");
        fx.Advance(699);
        Assert.Empty(fx.Changed);

        fx.Advance(1);

        Assert.Single(fx.Changed);
    }

    [Fact]
    public void DeleteThenRenameTempOverIt_BecomesSingleChanged()
    {
        using var fx = new FileMonitorFixture();
        fx.RaiseCreated("doc.txt.new");
        fx.RaiseChanged("doc.txt.new");
        fx.RaiseDeleted("doc.txt");

        fx.Advance(100);
        fx.RaiseRenamed("doc.txt.new", "doc.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("doc.txt"), Assert.Single(fx.Changed).CurrentPath),
            () => Assert.Empty(fx.Renamed),
            () => Assert.Empty(fx.Deleted),
            () => Assert.Empty(fx.Created)
        );
    }

    [Fact]
    public void RenameSwap_WithGlobExcludingTemps_BecomesSingleChanged()
    {
        using var fx = new FileMonitorFixture("*.docx");

        fx.RaiseRenamed("doc.docx", "~WRL0001.tmp");
        fx.RaiseRenamed("~WRD0002.tmp", "doc.docx");
        fx.RaiseDeleted("~WRL0001.tmp");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("doc.docx"), Assert.Single(fx.Changed).CurrentPath),
            () => Assert.Empty(fx.Renamed),
            () => Assert.Empty(fx.Deleted),
            () => Assert.Empty(fx.Created)
        );
    }

    [Fact]
    public void RenameSwap_WithMatchAllGlob_BecomesSingleChangedBecauseTempsAreExcluded()
    {
        using var fx = new FileMonitorFixture("**", register: false, start: false);
        var log = RegisterLog(fx);
        fx.Monitor.Start();

        RaiseWordSwap(fx);

        Assert.Equal(["changed:doc.docx"], log.ToArray());
    }

    [Fact]
    public void RenameSwap_WithExcludesCleared_ReportsBackupNoise()
    {
        using var fx = new FileMonitorFixture("**", register: false, start: false, configure: o => o.Excludes.Clear());
        var log = RegisterLog(fx);
        fx.Monitor.Start();

        RaiseWordSwap(fx);

        Assert.Equal(["renamed:doc.docx->~WRL0001.tmp", "deleted:~WRL0001.tmp", "changed:doc.docx"], log.ToArray());
    }

    private static List<string> RegisterLog(FileMonitorFixture fx)
    {
        var log = new List<string>();
        fx.Monitor.OnRenamed(e => log.Add($"renamed:{e.PreviousName}->{e.Name}"));
        fx.Monitor.OnChanged(e => log.Add($"changed:{e.Name}"));
        fx.Monitor.OnDeleted(e => log.Add($"deleted:{e.Name}"));
        fx.Monitor.OnCreated(e => log.Add($"created:{e.Name}"));

        return log;
    }

    // Sequence captured from a real Word session saving a document that stays open.
    private static void RaiseWordSwap(FileMonitorFixture fx)
    {
        fx.RaiseCreated("~WRD0002.tmp");
        fx.RaiseChanged("~WRD0002.tmp");
        fx.RaiseRenamed("doc.docx", "~WRL0001.tmp");
        fx.RaiseRenamed("~WRD0002.tmp", "doc.docx");
        fx.RaiseDeleted("~WRL0001.tmp");
        fx.Advance(10_000);
    }

    [Fact]
    public void RenameAway_NeverReplaced_StaysSilent()
    {
        using var fx = new FileMonitorFixture("*.docx");

        fx.RaiseRenamed("doc.docx", "doc.bak");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Empty(fx.Renamed),
            () => Assert.Empty(fx.Deleted),
            () => Assert.Empty(fx.Changed)
        );
    }

    [Fact]
    public void RenameAway_ReplacedAfterGracePeriod_ReportsRenameOfNewFile()
    {
        using var fx = new FileMonitorFixture("*.docx");

        fx.RaiseRenamed("doc.docx", "doc.bak");
        fx.Advance(300);
        fx.RaiseRenamed("other.tmp", "doc.docx");

        Assert.Multiple(
            () => Assert.Single(fx.Renamed),
            () => Assert.Empty(fx.Changed)
        );
    }

    [Fact]
    public void Deleted_OtherPathCreated_DoesNotCoalesce()
    {
        using var fx = new FileMonitorFixture();

        fx.RaiseDeleted("a.txt");
        fx.RaiseCreated("b.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Equal(fx.FullPath("a.txt"), Assert.Single(fx.Deleted).CurrentPath),
            () => Assert.Equal(fx.FullPath("b.txt"), Assert.Single(fx.Created).CurrentPath)
        );
    }

    [Fact]
    public void Deleted_WithZeroGracePeriod_DoesNotCoalesce()
    {
        using var fx = new FileMonitorFixture(configure: o => o.ReplaceGracePeriod = TimeSpan.Zero);

        fx.RaiseDeleted("doc.txt");
        fx.RaiseCreated("doc.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Single(fx.Deleted),
            () => Assert.Single(fx.Created),
            () => Assert.Empty(fx.Changed)
        );
    }

    [Fact]
    public void Dispose_CancelsHeldDeleted()
    {
        var fx = new FileMonitorFixture();
        fx.RaiseDeleted("doc.txt");

        fx.Dispose();
        fx.Advance(10_000);

        Assert.Empty(fx.Deleted);
    }
}
