using System.IO.Compression;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

public sealed class DecompressTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public void Dispose() {
        _temp.Dispose();
    }

    private string Out => _temp.Combine("out");

    private Task<string> DecompressAsync(
        string filePath,
        string? destinationDirectoryPath = null,
        bool overwrite = false) {
        return _sut.DecompressAsync(filePath, destinationDirectoryPath ?? Out, null, overwrite,
            TestContext.Current.CancellationToken);
    }

    // ---- Guard clauses ----------------------------------------------------

    [Fact]
    public async Task Throws_ArgumentNullException_When_FilePath_Is_Null()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => DecompressAsync(null!));

        Assert.Equal("filePath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Throws_ArgumentException_When_FilePath_Is_Empty_Or_WhiteSpace(string filePath)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => DecompressAsync(filePath));

        Assert.Equal("filePath", ex.ParamName);
    }

    [Fact]
    public async Task Throws_ArgumentNullException_When_Destination_Is_Null()
    {
        var archive = _temp.CreateArchive(("a.txt", "A"));

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.DecompressAsync(archive, null!, null, false, TestContext.Current.CancellationToken));

        Assert.Equal("destinationDirectoryPath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Throws_ArgumentException_When_Destination_Is_Empty_Or_WhiteSpace(string destination)
    {
        var archive = _temp.CreateArchive(("a.txt", "A"));

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => DecompressAsync(archive, destination));

        Assert.Equal("destinationDirectoryPath", ex.ParamName);
    }

    [Fact]
    public async Task Throws_FileNotFoundException_When_Archive_Does_Not_Exist()
    {
        var missing = _temp.Combine("missing.zip");

        var ex = await Assert.ThrowsAsync<FileNotFoundException>(() => DecompressAsync(missing));

        Assert.Equal(missing, ex.FileName);
    }

    [Fact]
    public async Task Throws_InvalidDataException_When_File_Is_Not_An_Archive()
    {
        var notAZip = _temp.CreateFile("fake.zip", "definitely not a zip");

        await Assert.ThrowsAsync<InvalidDataException>(() => DecompressAsync(notAZip));
    }

    [Fact]
    public async Task Throws_InvalidDataException_When_File_Is_Empty()
    {
        var empty = _temp.CreateFile("empty.zip");

        await Assert.ThrowsAsync<InvalidDataException>(() => DecompressAsync(empty));
    }

    // ---- Behavior ---------------------------------------------------------

    [Fact]
    public async Task Extracts_To_Destination_Creating_It_When_Missing()
    {
        var archive = _temp.CreateArchive(("docs/a.txt", "A"), ("docs/empty/", ""));
        var destination = _temp.Combine("out", "nested");

        var result = await DecompressAsync(archive, destination);

        Assert.Equal(destination, result);
        Assert.Equal("A", await SysFile.ReadAllTextAsync(SysPath.Combine(destination, "docs", "a.txt"), TestContext.Current.CancellationToken));
        Assert.True(SysDirectory.Exists(SysPath.Combine(destination, "docs", "empty")));
    }

    [Fact]
    public async Task Relative_Destination_Is_Resolved_And_Full_Path_Is_Returned()
    {
        var archive = _temp.CreateArchive(("a.txt", "A"));

        var result = await DecompressAsync(archive, SysPath.GetRelativePath(Environment.CurrentDirectory, Out));

        Assert.Equal(Out, result);
    }

    [Fact]
    public async Task Empty_Archive_Extracts_Nothing_And_Does_Not_Throw()
    {
        var archive = _temp.CreateArchive();

        await DecompressAsync(archive);

        Assert.Empty(SysDirectory.EnumerateFileSystemEntries(Out));
    }

    [Fact]
    public async Task Throws_IOException_And_Keeps_Existing_File_When_Overwrite_Is_False()
    {
        var archive = _temp.CreateArchive(("a.txt", "new"));
        var existing = _temp.CreateFile(SysPath.Combine("out", "a.txt"), "old");

        await Assert.ThrowsAsync<IOException>(() => DecompressAsync(archive, overwrite: false));

        Assert.Equal("old", await SysFile.ReadAllTextAsync(existing, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Conflict_Is_Detected_Before_Anything_Is_Extracted()
    {
        var archive = _temp.CreateArchive(("first.txt", "1"), ("second.txt", "2"));
        _temp.CreateFile(SysPath.Combine("out", "second.txt"), "old");

        await Assert.ThrowsAsync<IOException>(() => DecompressAsync(archive));

        Assert.False(SysFile.Exists(_temp.Combine("out", "first.txt")));
    }

    [Fact]
    public async Task Zip_Slip_Is_Detected_Before_Anything_Is_Extracted()
    {
        var archive = _temp.CreateArchive(("innocent.txt", "ok"), ("../escaped.txt", "pwned"));

        await Assert.ThrowsAsync<IOException>(() => DecompressAsync(archive));

        Assert.False(SysFile.Exists(_temp.Combine("out", "innocent.txt")));
        Assert.False(SysFile.Exists(_temp.Combine("escaped.txt")));
    }

    [Fact]
    public async Task Replaces_Existing_File_When_Overwrite_Is_True()
    {
        var archive = _temp.CreateArchive(("a.txt", "new"));
        var existing = _temp.CreateFile(SysPath.Combine("out", "a.txt"), "old");

        await DecompressAsync(archive, overwrite: true);

        Assert.Equal("new", await SysFile.ReadAllTextAsync(existing, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Cancelled_Token_Throws()
    {
        var archive = _temp.CreateArchive(("a.txt", "A"));
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _sut.DecompressAsync(archive, Out, null, false, cts.Token));

        Assert.False(SysFile.Exists(_temp.Combine("out", "a.txt")));
    }

    // ---- Round trip -------------------------------------------------------

    [Fact]
    public async Task Directory_Round_Trip_Restores_Tree()
    {
        _temp.CreateFile(SysPath.Combine("src", "docs", "a.txt"), "A");
        _temp.CreateFile(SysPath.Combine("src", "docs", "sub", "b.txt"), "B");
        _temp.CreateDirectory(SysPath.Combine("src", "docs", "empty"));

        var archive = await _sut.CompressDirectoryAsync(
            _temp.Combine("src", "docs"), null, _temp.Combine("docs.zip"), CompressionLevel.SmallestSize, false, TestContext.Current.CancellationToken);
        var result = await DecompressAsync(archive.FilePath, _temp.Combine("restored"));

        Assert.Equal("A", await SysFile.ReadAllTextAsync(SysPath.Combine(result, "docs", "a.txt"), TestContext.Current.CancellationToken));
        Assert.Equal("B", await SysFile.ReadAllTextAsync(SysPath.Combine(result, "docs", "sub", "b.txt"), TestContext.Current.CancellationToken));
        Assert.True(SysDirectory.Exists(SysPath.Combine(result, "docs", "empty")));
    }
}
