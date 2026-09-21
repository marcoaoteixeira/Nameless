namespace Nameless.IO.Monitoring;

public class FileMonitorGlobTests
{
    [Fact]
    public void Created_NotMatchingGlob_IsIgnored()
    {
        using var fx = new FileMonitorFixture("*.txt");

        fx.RaiseCreated("a.log");
        fx.Advance(5_000);

        Assert.Empty(fx.Created);
    }

    [Fact]
    public void Created_MatchingGlob_IsReported()
    {
        using var fx = new FileMonitorFixture("*.txt");

        fx.RaiseCreated("a.txt");
        fx.Advance(500);

        Assert.Single(fx.Created);
    }

    [Fact]
    public void DoubleStar_MatchesNestedFolders()
    {
        using var fx = new FileMonitorFixture();
        var nested = Path.Combine("sub", "deep", "a.txt");

        fx.RaiseCreated(nested);
        fx.Advance(500);

        Assert.Equal(fx.FullPath(nested), Assert.Single(fx.Created).CurrentPath);
    }

    [Fact]
    public void SingleStar_DoesNotMatchNestedFolders()
    {
        using var fx = new FileMonitorFixture("*.txt");

        fx.RaiseCreated(Path.Combine("sub", "a.txt"));
        fx.Advance(5_000);

        Assert.Empty(fx.Created);
    }

    [Fact]
    public void FolderScopedGlob_MatchesOnlyThatFolder()
    {
        using var fx = new FileMonitorFixture("docs/**/*.md");

        fx.RaiseCreated(Path.Combine("docs", "guide", "a.md"));
        fx.RaiseCreated(Path.Combine("src", "b.md"));
        fx.Advance(500);

        Assert.Equal(fx.FullPath(Path.Combine("docs", "guide", "a.md")), Assert.Single(fx.Created).CurrentPath);
    }

    [Fact]
    public void ExactFileName_MatchesOnlyThatFile()
    {
        using var fx = new FileMonitorFixture("report.docx");

        fx.RaiseCreated("report.docx");
        fx.RaiseCreated("other.docx");
        fx.Advance(500);

        Assert.Equal(fx.FullPath("report.docx"), Assert.Single(fx.Created).CurrentPath);
    }

    [Fact]
    public void Deleted_NotMatchingGlob_IsIgnored()
    {
        using var fx = new FileMonitorFixture("*.txt");

        fx.RaiseDeleted("a.log");

        Assert.Empty(fx.Deleted);
    }

    [Fact]
    public void Renamed_ReportedWhenNewPathMatches()
    {
        using var fx = new FileMonitorFixture("*.txt");

        fx.RaiseRenamed("a.tmp", "a.txt");

        Assert.Single(fx.Renamed);
    }

    [Fact]
    public void Renamed_IgnoredWhenNewPathDoesNotMatch()
    {
        using var fx = new FileMonitorFixture("*.txt");

        fx.RaiseRenamed("a.txt", "a.bak");

        Assert.Empty(fx.Renamed);
    }
}
