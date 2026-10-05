using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

/// <summary>
/// Entries must never be written outside the destination: no other drive, no network share, no traversal,
/// no NTFS alternate data stream. Unsafe archives are rejected before anything is written.
/// </summary>
public sealed class ExtractionSafetyTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public void Dispose() {
        _temp.Dispose();
    }

    private string Out => _temp.Combine("out");

    private void AssertNothingExtracted() {
        Assert.True(!SysDirectory.Exists(Out) || !SysDirectory.EnumerateFileSystemEntries(Out).Any());
    }

    [Theory]
    [InlineData("C:/evil.txt")]
    [InlineData("C:\\evil.txt")]
    [InlineData("C:evil.txt")]
    [InlineData("/rooted.txt")]
    [InlineData("\\rooted.txt")]
    [InlineData("\\\\server\\share\\evil.txt")]
    [InlineData("//server/share/evil.txt")]
    [InlineData("a.txt:hidden-stream")]
    [InlineData("../escape.txt")]
    [InlineData("sub/../../escape.txt")]
    public async Task Unsafe_Entry_Is_Rejected_Before_Anything_Is_Extracted(string entryName)
    {
        var archive = _temp.CreateArchive(("innocent.txt", "ok"), (entryName, "pwned"));

        var ex = await Assert.ThrowsAsync<IOException>(() => _sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        Assert.Contains(entryName, ex.Message);
        AssertNothingExtracted();
    }

    [Fact]
    public async Task Traversal_That_Stays_Inside_Destination_Is_Allowed()
    {
        var archive = _temp.CreateArchive(("sub/../inside.txt", "fine"));

        await _sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken);

        Assert.Equal("fine", await SysFile.ReadAllTextAsync(SysPath.Combine(Out, "inside.txt"), TestContext.Current.CancellationToken));
    }

    [WindowsFact]
    public async Task Locked_Target_File_Throws_IOException()
    {
        var archive = _temp.CreateArchive(("a.txt", "new"));
        var target = _temp.CreateFile(SysPath.Combine("out", "a.txt"), "old");

        await using (new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            await Assert.ThrowsAsync<IOException>(() => _sut.DecompressAsync(archive, Out, null, true, TestContext.Current.CancellationToken));
        }

        Assert.Equal("old", await SysFile.ReadAllTextAsync(target, TestContext.Current.CancellationToken));
    }
}
