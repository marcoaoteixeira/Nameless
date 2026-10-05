using Nameless.Compression.Infrastructure;
using Nameless.Compression.Internals;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

/// <summary>
/// Name matching is pure and runs on every OS; only extraction on Windows enforces it.
/// </summary>
public sealed class WindowsReservedNamesTests
{
    [Theory]
    [InlineData("CON")]
    [InlineData("con")]
    [InlineData("PRN")]
    [InlineData("AUX")]
    [InlineData("NUL")]
    [InlineData("nul.txt")]
    [InlineData("aux.tar.gz")]
    [InlineData("CON ")]
    [InlineData("CON .txt")]
    [InlineData("COM0")]
    [InlineData("COM1")]
    [InlineData("com9.log")]
    [InlineData("LPT1")]
    [InlineData("lpt9")]
    [InlineData("COM¹")]
    [InlineData("LPT³.txt")]
    [InlineData("CONIN$")]
    [InlineData("conout$")]
    public void Reserved_Segments_Are_Detected(string segment) {
        Assert.True(WindowsReservedNames.IsReserved(segment));
    }

    [Theory]
    [InlineData("CONSOLE.txt")]
    [InlineData("nullable.txt")]
    [InlineData("COM")]
    [InlineData("COM10")]
    [InlineData("LPTX")]
    [InlineData("a.con")]
    [InlineData(".nul")]
    [InlineData("readme.txt")]
    [InlineData("")]
    public void Ordinary_Segments_Are_Not_Reserved(string segment) {
        Assert.False(WindowsReservedNames.IsReserved(segment));
    }

    [Theory]
    [InlineData("CON", true)]
    [InlineData("docs/NUL/readme.txt", true)]
    [InlineData("docs\\aux\\x.txt", true)]
    [InlineData("docs/sub/com1.txt", true)]
    [InlineData("docs/console/readme.txt", false)]
    [InlineData("docs/", false)]
    public void Any_Segment_Of_An_Entry_Counts(string entryName, bool expected) {
        Assert.Equal(expected, WindowsReservedNames.ContainsReservedSegment(entryName));
    }
}

public sealed class ReservedNameExtractionTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public void Dispose() {
        _temp.Dispose();
    }

    private string Out => _temp.Combine("out");

    [WindowsFact]
    public async Task Reserved_Name_Is_Rejected_On_Windows_Before_Anything_Is_Extracted()
    {
        var archive = _temp.CreateArchive(("innocent.txt", "ok"), ("docs/NUL.txt", "device"));

        var ex = await Assert.ThrowsAsync<IOException>(() => _sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        Assert.Contains("docs/NUL.txt", ex.Message);
        Assert.True(!SysDirectory.Exists(Out) || !SysDirectory.EnumerateFileSystemEntries(Out).Any());
    }

    [WindowsFact]
    public async Task Reserved_Directory_Name_Is_Rejected_On_Windows()
    {
        var archive = _temp.CreateArchive(("con/readme.txt", "x"));

        await Assert.ThrowsAsync<IOException>(() => _sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));
    }

    [NonWindowsFact]
    public async Task Names_Reserved_On_Windows_Round_Trip_Elsewhere()
    {
        // Compression stores whatever exists; only Windows extraction refuses such entries.
        _temp.CreateFile(SysPath.Combine("docs", "con.txt"), "x");

        var result = await _sut.CompressDirectoryAsync(_temp.Combine("docs"), null, _temp.Combine("docs.zip"), System.IO.Compression.CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);
        await _sut.DecompressAsync(result.FilePath, Out, null, false, TestContext.Current.CancellationToken);

        Assert.Equal("x", await SysFile.ReadAllTextAsync(SysPath.Combine(Out, "docs", "con.txt"), TestContext.Current.CancellationToken));
    }
}
