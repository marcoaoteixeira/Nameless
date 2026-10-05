using System.IO.Compression;
using System.Security.Cryptography;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

public sealed class ChecksumTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public void Dispose() {
        _temp.Dispose();
    }

    private string Out => _temp.Combine("out");

    private string Archive => _temp.Combine("archive.zip");

    private static async Task<string> Sha256OfAsync(string path) {
        return Convert.ToHexStringLower(
            SHA256.HashData(await SysFile.ReadAllBytesAsync(path, TestContext.Current.CancellationToken)));
    }

    private static readonly string WrongChecksum = new('0', 64);

    public static TheoryData<string> MalformedChecksums => new()
    {
        "",
        "   ",
        "xyz",
        new string('a', 63),
        new string('a', 65),
        new string('g', 64),
    };

    // ---- Compress ---------------------------------------------------------

    [Fact]
    public async Task Path_Overload_Returns_Lowercase_Sha256_Of_Archive()
    {
        var file = _temp.CreateFile("a.txt", "content");

        var result = await _sut.CompressFilesAsync([file], Archive, CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        Assert.Matches("^[0-9a-f]{64}$", result.Checksum);
        Assert.Equal(await Sha256OfAsync(result.FilePath), result.Checksum);
    }

    [Fact]
    public async Task Every_Compress_Method_Returns_Checksum_Of_Its_Archive()
    {
        var file = _temp.CreateFile(SysPath.Combine("dir", "a.txt"), "content");

        CompressionResult[] results =
        [
            await _sut.CompressDirectoryAsync(_temp.Combine("dir"), null, _temp.Combine("1.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken),
            await _sut.CompressDirectoryAsync(_temp.Combine("dir"), "*.txt", _temp.Combine("2.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken),
            await _sut.CompressFilesAsync([file], _temp.Combine("3.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken),
        ];

        foreach (var result in results)
        {
            Assert.Equal(await Sha256OfAsync(result.FilePath), result.Checksum);
        }
    }

    // ---- Decompress -------------------------------------------------------

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Matching_Checksum_Is_Accepted_Case_Insensitively(bool upperCase)
    {
        var file = _temp.CreateFile("a.txt", "content");
        var archive = await _sut.CompressFilesAsync([file], Archive, CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);
        var checksum = upperCase ? archive.Checksum.ToUpperInvariant() : archive.Checksum;

        await _sut.DecompressAsync(archive.FilePath, Out, checksum, false, TestContext.Current.CancellationToken);

        Assert.Equal("content", await SysFile.ReadAllTextAsync(ZipInspector.ExtractedPath(Out, file), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Wrong_Checksum_Throws_And_Extracts_Nothing()
    {
        var file = _temp.CreateFile("a.txt", "content");
        var archive = await _sut.CompressFilesAsync([file], Archive, CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        var ex = await Assert.ThrowsAsync<ChecksumMismatchException>(
            () => _sut.DecompressAsync(archive.FilePath, Out, WrongChecksum, false, TestContext.Current.CancellationToken));

        Assert.Equal(WrongChecksum, ex.Expected);
        Assert.Equal(archive.Checksum, ex.Actual);
        Assert.False(SysDirectory.Exists(Out));
    }

    [Fact]
    public async Task Tampered_Archive_Is_Detected()
    {
        var file = _temp.CreateFile("a.txt", "content");
        var archive = await _sut.CompressFilesAsync([file], Archive, CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);
        var bytes = await SysFile.ReadAllBytesAsync(archive.FilePath, TestContext.Current.CancellationToken);
        bytes[^1] ^= 0xFF;
        await SysFile.WriteAllBytesAsync(archive.FilePath, bytes, TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<ChecksumMismatchException>(
            () => _sut.DecompressAsync(archive.FilePath, Out, archive.Checksum, false, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task No_Checksum_Skips_Verification()
    {
        var archive = _temp.CreateArchive(("a.txt", "A"));

        await _sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken);

        Assert.True(SysFile.Exists(SysPath.Combine(Out, "a.txt")));
    }

    [Theory]
    [MemberData(nameof(MalformedChecksums))]
    public async Task Malformed_Checksum_Is_Rejected(string checksum)
    {
        var archive = _temp.CreateArchive(("a.txt", "A"));

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.DecompressAsync(archive, Out, checksum, false, TestContext.Current.CancellationToken));

        Assert.Equal("checksum", ex.ParamName);
    }
}
