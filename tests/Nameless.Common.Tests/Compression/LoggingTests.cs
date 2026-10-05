using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

public sealed class LoggingTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ListLogger<ZipCompressor> _logger = new();

    public void Dispose() {
        _temp.Dispose();
    }

    private ZipCompressor CreateSut(ExtractionLimits? limits = null, byte[]? hmacKey = null) {
        return ZipCompressorSut.Create(
            new ZipCompressorOptions {
                Limits = limits ?? new ExtractionLimits(),
                HmacKey = hmacKey is null ? null : Convert.ToBase64String(hmacKey),
            },
            _logger);
    }

    private string Out => _temp.Combine("out");

    [Fact]
    public async Task Compression_Logs_Start_Entries_And_Completion()
    {
        var directory = SysPath.GetDirectoryName(_temp.CreateFile(SysPath.Combine("docs", "a.txt"), "content"))!;

        var result = await CreateSut().CompressDirectoryAsync(directory, null, _temp.Combine("docs.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        Assert.True(_logger.Contains(LogLevel.Debug, directory));
        Assert.True(_logger.Contains(LogLevel.Trace, "docs/a.txt"));
        Assert.True(_logger.Contains(LogLevel.Information, result.FilePath));
    }

    [Fact]
    public async Task Extraction_Logs_Plan_Entries_And_Completion()
    {
        var archive = _temp.CreateArchive(("a.txt", "A"), ("b.txt", "B"));

        await CreateSut().DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken);

        Assert.True(_logger.Contains(LogLevel.Debug, "2 entries"));
        Assert.True(_logger.Contains(LogLevel.Trace, "b.txt"));
        Assert.True(_logger.Contains(LogLevel.Information, Out));
    }

    [SymlinkFact]
    public async Task Skipped_Symlink_Is_Logged_At_Debug()
    {
        _temp.CreateFile(SysPath.Combine("docs", "real.txt"), "real");
        var link = _temp.Combine("docs", "link.txt");
        SysFile.CreateSymbolicLink(link, _temp.Combine("docs", "real.txt"));

        await CreateSut().CompressDirectoryAsync(_temp.Combine("docs"), null, _temp.Combine("docs.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        Assert.True(_logger.Contains(LogLevel.Debug, link));
    }

    [Fact]
    public async Task Unsafe_Entry_Is_Logged_As_Warning()
    {
        var archive = _temp.CreateArchive(("../escape.txt", "x"));

        await Assert.ThrowsAsync<IOException>(() => CreateSut().DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        Assert.True(_logger.Contains(LogLevel.Warning, "../escape.txt"));
    }

    [Fact]
    public async Task Limit_Violation_Is_Logged_As_Warning()
    {
        var archive = _temp.CreateArchive(("a.txt", "a"), ("b.txt", "b"));

        await Assert.ThrowsAsync<InvalidDataException>(
            () => CreateSut(new ExtractionLimits { MaxEntryCount = 1 }).DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        Assert.Contains(_logger.Records, r => r.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task Checksum_Mismatch_Is_Logged_As_Warning_Without_Any_Checksum_Value()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var sut = CreateSut(hmacKey: key);
        var archive = _temp.CreateArchive(("a.txt", "A"));
        var correctHmac = Convert.ToHexStringLower(HMACSHA256.HashData(key, await SysFile.ReadAllBytesAsync(archive, TestContext.Current.CancellationToken)));
        var expected = new string('0', 64);

        await Assert.ThrowsAsync<ChecksumMismatchException>(() => sut.DecompressAsync(archive, Out, expected, false, TestContext.Current.CancellationToken));

        Assert.Contains(_logger.Records, r => r.Level == LogLevel.Warning);
        Assert.All(_logger.Records, r =>
        {
            Assert.DoesNotContain(correctHmac, r.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(expected, r.Message, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task Hmac_Key_And_Checksums_Never_Appear_In_Logs()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var sut = CreateSut(hmacKey: key);

        var result = await sut.CompressFilesAsync([_temp.CreateFile("a.txt", "A")], _temp.Combine("a.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);
        await sut.DecompressAsync(result.FilePath, Out, result.Checksum, false, TestContext.Current.CancellationToken);

        var keyHex = Convert.ToHexString(key);
        var keyBase64 = Convert.ToBase64String(key);
        Assert.All(_logger.Records, r =>
        {
            Assert.DoesNotContain(result.Checksum, r.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(keyHex, r.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(keyBase64, r.Message, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task Works_With_NullLogger()
    {
        var file = _temp.CreateFile("a.txt", "content");

        var result = await ZipCompressorSut.Create().CompressFilesAsync([file], _temp.Combine("a.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        Assert.True(SysFile.Exists(result.FilePath));
    }
}
