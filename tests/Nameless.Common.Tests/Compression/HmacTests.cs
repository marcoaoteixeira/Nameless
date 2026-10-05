using System.IO.Compression;
using System.Security.Cryptography;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

public sealed class HmacTests : IDisposable
{
    private static readonly byte[] Key = RandomNumberGenerator.GetBytes(32);
    private static readonly byte[] OtherKey = RandomNumberGenerator.GetBytes(32);

    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create(Key);

    public void Dispose() {
        _temp.Dispose();
    }

    private string Out => _temp.Combine("out");

    private async Task<CompressionResult> CompressSampleAsync(ZipCompressor compressor) {
        return await compressor.CompressFilesAsync([_temp.CreateFile("a.txt", "content")], _temp.Combine("a.zip"),
            CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);
    }

    // ---- Construction -----------------------------------------------------

    [Theory]
    [InlineData(1)]
    [InlineData(16)]
    [InlineData(31)]
    public void Key_Decoding_To_Fewer_Than_32_Bytes_Is_Rejected(int length)
    {
        var ex = Assert.Throws<ArgumentException>(() => ZipCompressorSut.Create(new byte[length]));

        Assert.Equal(nameof(ZipCompressorOptions.HmacKey), ex.ParamName);
    }

    [Theory]
    [InlineData(32)]
    [InlineData(64)]
    public void Key_Decoding_To_32_Bytes_Or_More_Is_Accepted(int length)
    {
        _ = ZipCompressorSut.Create(RandomNumberGenerator.GetBytes(length));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\r\n")]
    public void Empty_Key_Is_Rejected_Instead_Of_Silently_Falling_Back_To_Sha256(string hmacKey)
    {
        var ex = Assert.Throws<ArgumentException>(() => ZipCompressorSut.Create(hmacKey));

        Assert.Equal(nameof(ZipCompressorOptions.HmacKey), ex.ParamName);
    }

    [Theory]
    [InlineData("MyCompanySecretPassword2026!!!!!")] // a passphrase, not Base64
    [InlineData("correct horse battery staple ok?")]
    [InlineData("q3Jx9vQ")] // truncated
    [InlineData("q3Jx9vQ-Zk8_q3Jx9vQ-Zk8_q3Jx9vQ-Zk8_q3Jx9vQ=")] // Base64Url alphabet
    public void Key_That_Is_Not_Base64_Is_Rejected(string hmacKey)
    {
        var ex = Assert.Throws<ArgumentException>(() => ZipCompressorSut.Create(hmacKey));

        Assert.Equal(nameof(ZipCompressorOptions.HmacKey), ex.ParamName);
    }

    [Theory]
    [InlineData("MyCompanySecretPassword2026!!!!!")]
    [InlineData("q3Jx9vQq3Jx9vQq3Jx9vQw==")] // valid Base64, but only 16 bytes
    public void Rejection_Never_Reveals_The_Key(string hmacKey)
    {
        var ex = Assert.Throws<ArgumentException>(() => ZipCompressorSut.Create(hmacKey));

        Assert.DoesNotContain(hmacKey, ex.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Surrounding_Whitespace_And_Line_Breaks_Are_Ignored()
    {
        // Secret files and environment variables often end with a newline.
        var sut = ZipCompressorSut.Create("  " + Convert.ToBase64String(Key) + "\n");

        var result = await CompressSampleAsync(sut);

        Assert.Equal(Convert.ToHexStringLower(HMACSHA256.HashData(Key, await SysFile.ReadAllBytesAsync(result.FilePath, TestContext.Current.CancellationToken))), result.Checksum);
    }

    // ---- Compress ---------------------------------------------------------

    [Fact]
    public async Task Without_Key_Checksum_Is_Plain_Sha256()
    {
        var result = await CompressSampleAsync(ZipCompressorSut.Create());

        Assert.Equal(ChecksumAlgorithm.Sha256, result.Algorithm);
        Assert.Equal(Convert.ToHexStringLower(SHA256.HashData(await SysFile.ReadAllBytesAsync(result.FilePath, TestContext.Current.CancellationToken))), result.Checksum);
    }

    [Fact]
    public async Task With_Key_Checksum_Is_HmacSha256_Of_Archive()
    {
        var result = await CompressSampleAsync(_sut);

        Assert.Equal(ChecksumAlgorithm.HmacSha256, result.Algorithm);
        Assert.Equal(Convert.ToHexStringLower(HMACSHA256.HashData(Key, await SysFile.ReadAllBytesAsync(result.FilePath, TestContext.Current.CancellationToken))), result.Checksum);
    }

    // ---- Decompress -------------------------------------------------------

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Valid_Hmac_Is_Accepted_Case_Insensitively(bool upperCase)
    {
        var archive = await CompressSampleAsync(_sut);
        var checksum = upperCase ? archive.Checksum.ToUpperInvariant() : archive.Checksum;

        await _sut.DecompressAsync(archive.FilePath, Out, checksum, false, TestContext.Current.CancellationToken);

        Assert.True(SysFile.Exists(ZipInspector.ExtractedPath(Out, _temp.Combine("a.txt"))));
    }

    [Fact]
    public async Task Tampered_Archive_Fails_Without_Revealing_The_Correct_Hmac()
    {
        var archive = await CompressSampleAsync(_sut);
        var bytes = await SysFile.ReadAllBytesAsync(archive.FilePath, TestContext.Current.CancellationToken);
        bytes[^1] ^= 0xFF;
        await SysFile.WriteAllBytesAsync(archive.FilePath, bytes, TestContext.Current.CancellationToken);
        var correctHmacOfTampered = Convert.ToHexStringLower(HMACSHA256.HashData(Key, bytes));

        var ex = await Assert.ThrowsAsync<ChecksumMismatchException>(
            () => _sut.DecompressAsync(archive.FilePath, Out, archive.Checksum, false, TestContext.Current.CancellationToken));

        Assert.Null(ex.Actual);
        Assert.DoesNotContain(correctHmacOfTampered, ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(SysDirectory.Exists(Out));
    }

    [Fact]
    public async Task Hmac_From_A_Different_Key_Is_Rejected()
    {
        var archive = await CompressSampleAsync(ZipCompressorSut.Create(OtherKey));

        await Assert.ThrowsAsync<ChecksumMismatchException>(
            () => _sut.DecompressAsync(archive.FilePath, Out, archive.Checksum, false, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Plain_Sha256_Is_Rejected_By_A_Keyed_Compressor()
    {
        var archive = await CompressSampleAsync(ZipCompressorSut.Create());

        await Assert.ThrowsAsync<ChecksumMismatchException>(
            () => _sut.DecompressAsync(archive.FilePath, Out, archive.Checksum, false, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Unkeyed_Mismatch_Still_Reports_Computed_Checksum()
    {
        var sut = ZipCompressorSut.Create();
        var archive = await CompressSampleAsync(sut);

        var ex = await Assert.ThrowsAsync<ChecksumMismatchException>(
            () => sut.DecompressAsync(archive.FilePath, Out, new string('0', 64), false, TestContext.Current.CancellationToken));

        Assert.Equal(archive.Checksum, ex.Actual);
    }
}
