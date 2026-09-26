namespace Nameless.IO;

[IntegrationTest]
public class DirectoryHelperTests : IDisposable {
    private readonly string _root = SysPath.Combine(SysPath.GetTempPath(), $"dirhelper-{Guid.NewGuid():N}");

    public DirectoryHelperTests() {
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        SysDirectory.Delete(_root, recursive: true);
    }

    [Fact]
    public void CopyDirectory_CopiesFilesAndNestedDirectories() {
        // arrange
        var source = SysPath.Combine(_root, "src");
        var nested = SysPath.Combine(source, "nested");
        SysDirectory.CreateDirectory(nested);
        SysFile.WriteAllText(SysPath.Combine(source, "a.txt"), "a");
        SysFile.WriteAllText(SysPath.Combine(nested, "b.txt"), "b");
        var destination = SysPath.Combine(_root, "dst");

        // act
        DirectoryHelper.CopyDirectory(source, destination, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Equal("a", SysFile.ReadAllText(SysPath.Combine(destination, "a.txt"))),
            () => Assert.Equal("b", SysFile.ReadAllText(SysPath.Combine(destination, "nested", "b.txt")))
        );
    }

    [Fact]
    public void CopyDirectory_OverwritesExistingFiles() {
        // arrange
        var source = SysPath.Combine(_root, "src");
        var destination = SysPath.Combine(_root, "dst");
        SysDirectory.CreateDirectory(source);
        SysDirectory.CreateDirectory(destination);
        SysFile.WriteAllText(SysPath.Combine(source, "a.txt"), "new");
        SysFile.WriteAllText(SysPath.Combine(destination, "a.txt"), "old");

        // act
        DirectoryHelper.CopyDirectory(source, destination, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal("new", SysFile.ReadAllText(SysPath.Combine(destination, "a.txt")));
    }

    [Fact]
    public void CopyDirectory_WithMissingSource_Throws() {
        // act & assert
        Assert.Throws<DirectoryNotFoundException>(() => DirectoryHelper.CopyDirectory(SysPath.Combine(_root, "none"), SysPath.Combine(_root, "dst"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public void CopyDirectory_WithCancelledToken_Throws() {
        // arrange
        var source = SysPath.Combine(_root, "src");
        SysDirectory.CreateDirectory(source);
        SysFile.WriteAllText(SysPath.Combine(source, "a.txt"), "a");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // act & assert
        Assert.Throws<OperationCanceledException>(() => DirectoryHelper.CopyDirectory(source, SysPath.Combine(_root, "dst"), cts.Token));
    }
}
