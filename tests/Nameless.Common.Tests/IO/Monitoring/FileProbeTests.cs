namespace Nameless.IO.Monitoring;

[Trait("Category", "Integration")]
public sealed class FileProbeTests : IDisposable
{
    private readonly string _dir = SysDirectory.CreateTempSubdirectory("probe-").FullName;
    private readonly FileProbe _sut = (FileProbe.Instance as FileProbe)!;

    public void Dispose()
    {
        SysDirectory.Delete(_dir, recursive: true);
    }

    [Fact]
    public void Probe_MissingPath_ReturnsNotFound()
    {
        Assert.Equal(FileProbeResult.NotFound, _sut.Probe(Path.Combine(_dir, "missing.txt")));
    }

    [Fact]
    public void Probe_MissingFolder_ReturnsNotFound()
    {
        Assert.Equal(FileProbeResult.NotFound, _sut.Probe(Path.Combine(_dir, "nope", "missing.txt")));
    }

    [Fact]
    public void Probe_Directory_ReturnsDirectory()
    {
        Assert.Equal(FileProbeResult.Directory, _sut.Probe(_dir));
    }

    [Fact]
    public void Probe_FreeFile_ReturnsAvailable()
    {
        var path = Path.Combine(_dir, "free.txt");
        File.WriteAllText(path, "x");

        Assert.Equal(FileProbeResult.Available, _sut.Probe(path));
    }

    [Fact]
    public void Probe_FreeFile_DoesNotKeepHandle()
    {
        var path = Path.Combine(_dir, "free.txt");
        File.WriteAllText(path, "x");

        _sut.Probe(path);

        File.Delete(path);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Probe_FileOpenedByWriter_ReturnsLocked()
    {
        var path = Path.Combine(_dir, "busy.txt");
        using var writer = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);

        Assert.Equal(FileProbeResult.Locked, _sut.Probe(path));
    }

    [Fact]
    public void Probe_FileOpenedByReader_ReturnsLocked()
    {
        var path = Path.Combine(_dir, "read.txt");
        File.WriteAllText(path, "x");
        using var reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        Assert.Equal(FileProbeResult.Locked, _sut.Probe(path));
    }
}
