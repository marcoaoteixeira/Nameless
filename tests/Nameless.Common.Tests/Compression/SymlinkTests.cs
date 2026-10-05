using System.IO.Compression;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

/// <summary>
/// Links inside a source directory are ignored; paths the caller names explicitly are followed.
/// </summary>
public sealed class SymlinkTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public SymlinkTests()
    {
        _temp.CreateFile(SysPath.Combine("docs", "real.txt"), "real");
        _temp.CreateFile(SysPath.Combine("elsewhere", "secret.txt"), "secret");
    }

    public void Dispose() {
        _temp.Dispose();
    }

    private string Docs => _temp.Combine("docs");

    private Task<CompressionResult> CompressDirectoryAsync(string directoryPath, string? globPattern = null) {
        return _sut.CompressDirectoryAsync(directoryPath, globPattern, _temp.Combine("archive.zip"),
            CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);
    }

    [SymlinkFact]
    public async Task File_Symlink_Inside_Directory_Is_Ignored()
    {
        SysFile.CreateSymbolicLink(_temp.Combine("docs", "file-link.txt"), _temp.Combine("elsewhere", "secret.txt"));

        var result = await CompressDirectoryAsync(Docs);

        var entries = await ZipInspector.ReadEntriesAsync(result.FilePath);
        Assert.Equal(["docs/real.txt"], entries.Keys);
    }

    [SymlinkFact]
    public async Task Directory_Symlink_Inside_Directory_Is_Neither_Stored_Nor_Followed()
    {
        SysDirectory.CreateSymbolicLink(_temp.Combine("docs", "dir-link"), _temp.Combine("elsewhere"));

        var result = await CompressDirectoryAsync(Docs);

        Assert.Equal(["docs/real.txt"], (await ZipInspector.ReadEntriesAsync(result.FilePath)).Keys);
    }

    [SymlinkFact]
    public async Task Symlink_Loop_Does_Not_Recurse_Forever()
    {
        SysDirectory.CreateSymbolicLink(_temp.Combine("docs", "loop"), Docs);

        var result = await CompressDirectoryAsync(Docs);

        Assert.Equal(["docs/real.txt"], (await ZipInspector.ReadEntriesAsync(result.FilePath)).Keys);
    }

    [SymlinkFact]
    public async Task Directory_Containing_Only_Links_Is_Stored_As_Empty()
    {
        var holder = _temp.CreateDirectory(SysPath.Combine("docs", "holder"));
        SysFile.CreateSymbolicLink(SysPath.Combine(holder, "link.txt"), _temp.Combine("elsewhere", "secret.txt"));

        var result = await CompressDirectoryAsync(Docs);

        Assert.Equal(["docs/holder/", "docs/real.txt"], (await ZipInspector.ReadEntriesAsync(result.FilePath)).Keys.Order());
    }

    [SymlinkFact]
    public async Task Glob_Ignores_Symlinks()
    {
        SysFile.CreateSymbolicLink(_temp.Combine("docs", "file-link.txt"), _temp.Combine("elsewhere", "secret.txt"));
        SysDirectory.CreateSymbolicLink(_temp.Combine("docs", "dir-link"), _temp.Combine("elsewhere"));

        var result = await CompressDirectoryAsync(Docs, "**/*.txt");

        Assert.Equal(["docs/real.txt"], (await ZipInspector.ReadEntriesAsync(result.FilePath)).Keys);
    }

    [WindowsFact]
    public async Task Junction_Inside_Directory_Is_Ignored()
    {
        Links.CreateJunction(_temp.Combine("docs", "junction"), _temp.Combine("elsewhere"));

        var result = await CompressDirectoryAsync(Docs);

        Assert.Equal(["docs/real.txt"], (await ZipInspector.ReadEntriesAsync(result.FilePath)).Keys);
    }

    [SymlinkFact]
    public async Task Root_Directory_Symlink_Named_By_Caller_Is_Followed()
    {
        var link = _temp.Combine("docs-link");
        SysDirectory.CreateSymbolicLink(link, Docs);

        var result = await CompressDirectoryAsync(link);

        Assert.Equal(
            new Dictionary<string, string?> { ["docs-link/real.txt"] = "real" },
            await ZipInspector.ReadEntriesAsync(result.FilePath));
    }

    [SymlinkFact]
    public async Task Listed_File_Symlink_Named_By_Caller_Is_Followed()
    {
        var link = _temp.Combine("file-link.txt");
        SysFile.CreateSymbolicLink(link, _temp.Combine("docs", "real.txt"));

        var result = await _sut.CompressFilesAsync([link], _temp.Combine("list.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        Assert.Equal(
            new Dictionary<string, string?> { [ZipInspector.EntryFor(link)] = "real" },
            await ZipInspector.ReadEntriesAsync(result.FilePath));
    }
}
