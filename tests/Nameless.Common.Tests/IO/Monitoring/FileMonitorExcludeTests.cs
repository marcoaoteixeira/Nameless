namespace Nameless.IO.Monitoring;

public class FileMonitorExcludeTests
{
    [Theory]
    [InlineData("~$report.docx")]
    [InlineData("~WRL0002.tmp")]
    [InlineData("192807EF.TMP")]
    [InlineData("sub/~$report.docx")]
    [InlineData("sub/deep/scratch.tmp")]
    public void DefaultExcludes_TemporaryFilesAreNeverReported(string relative)
    {
        using var fx = new FileMonitorFixture("**");
        var path = relative.Replace('/', Path.DirectorySeparatorChar);

        fx.RaiseCreated(path);
        fx.RaiseChanged(path);
        fx.RaiseDeleted(path);
        fx.RaiseRenamed(path, "kept.txt");
        fx.Advance(10_000);

        Assert.Multiple(
            () => Assert.Empty(fx.Created),
            () => Assert.Empty(fx.Changed),
            () => Assert.Empty(fx.Deleted),
            () => Assert.Single(fx.Renamed)
        );
    }

    [Fact]
    public void DefaultExcludes_RenamedIntoTemporaryName_IsNotReported()
    {
        using var fx = new FileMonitorFixture("**");

        fx.RaiseRenamed("a.txt", "a.txt.tmp");

        Assert.Empty(fx.Renamed);
    }

    [Fact]
    public void DefaultExcludes_DoNotHideOrdinaryFiles()
    {
        using var fx = new FileMonitorFixture("**");

        fx.RaiseCreated("report.docx");
        fx.RaiseCreated(Path.Combine("sub", "notes.txt"));
        fx.Advance(500);

        Assert.Equal(2, fx.Created.Count);
    }

    [Fact]
    public void ClearingExcludes_ReportsTemporaryFiles()
    {
        using var fx = new FileMonitorFixture("**", configure: o => o.Excludes.Clear());

        fx.RaiseCreated("~$report.docx");
        fx.RaiseCreated("scratch.tmp");
        fx.Advance(500);

        Assert.Equal(2, fx.Created.Count);
    }

    [Fact]
    public void CustomExclude_IsAppliedOnTopOfDefaults()
    {
        using var fx = new FileMonitorFixture("**", configure: o => o.Excludes.Add("**/*.bak"));

        fx.RaiseCreated("a.bak");
        fx.RaiseCreated("a.tmp");
        fx.RaiseCreated("a.txt");
        fx.Advance(500);

        Assert.Equal(fx.FullPath("a.txt"), Assert.Single(fx.Created).CurrentPath);
    }

    [Fact]
    public void ReplacingExcludes_DropsTheDefaults()
    {
        using var fx = new FileMonitorFixture("**", configure: o => o.Excludes = ["**/*.bak"]);

        fx.RaiseCreated("a.bak");
        fx.RaiseCreated("a.tmp");
        fx.Advance(500);

        Assert.Equal(fx.FullPath("a.tmp"), Assert.Single(fx.Created).CurrentPath);
    }
}
