namespace Nameless.IO.Monitoring;

[IntegrationTest]
public class FileProbeTests : IDisposable {
    private readonly string _root = SysPath.Combine(SysPath.GetTempPath(), $"probe-{Guid.NewGuid():N}");

    public FileProbeTests() {
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        SysDirectory.Delete(_root, recursive: true);
    }

    [Fact]
    public void Probe_WithMissingFile_ReturnsNotFound() {
        // act
        var actual = FileProbe.Instance.Probe(SysPath.Combine(_root, "missing.txt"));

        // assert
        Assert.Equal(FileProbeResult.NotFound, actual);
    }

    [Fact]
    public void Probe_WithMissingDirectory_ReturnsNotFound() {
        // act
        var actual = FileProbe.Instance.Probe(SysPath.Combine(_root, "nope", "missing.txt"));

        // assert
        Assert.Equal(FileProbeResult.NotFound, actual);
    }

    [Fact]
    public void Probe_WithDirectory_ReturnsDirectory() {
        // act
        var actual = FileProbe.Instance.Probe(_root);

        // assert
        Assert.Equal(FileProbeResult.Directory, actual);
    }

    [Fact]
    public void Probe_WithFreeFile_ReturnsAvailable() {
        // arrange
        var path = SysPath.Combine(_root, "free.txt");
        SysFile.WriteAllText(path, "content");

        // act
        var actual = FileProbe.Instance.Probe(path);

        // assert
        Assert.Equal(FileProbeResult.Available, actual);
    }

    [Fact]
    public void Probe_WithLockedFile_ReturnsLocked() {
        // arrange
        var path = SysPath.Combine(_root, "locked.txt");
        SysFile.WriteAllText(path, "content");

        using var handle = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        // act
        var actual = FileProbe.Instance.Probe(path);

        // assert
        Assert.Equal(FileProbeResult.Locked, actual);
    }
}
