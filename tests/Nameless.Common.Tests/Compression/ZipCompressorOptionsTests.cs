using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

public sealed class ZipCompressorOptionsTests : IDisposable
{
    private readonly TempDirectory _temp = new();

    public void Dispose() {
        _temp.Dispose();
    }

    private static IConfiguration Configuration(params (string Key, string Value)[] values) {
        return new ConfigurationBuilder()
               .AddInMemoryCollection(values.Select(pair => new KeyValuePair<string, string?>(pair.Key, pair.Value)))
               .Build();
    }

    [Fact]
    public void Defaults_Are_No_Hmac_And_Default_Limits()
    {
        var options = new ZipCompressorOptions();

        Assert.Null(options.HmacKey);
        Assert.Equal(new ExtractionLimits(), options.Limits);
    }

    [Fact]
    public void Null_Limits_Are_Rejected()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ZipCompressorOptions { Limits = null! });

        Assert.Equal(nameof(ZipCompressorOptions.Limits), ex.ParamName);
    }

    [Fact]
    public void ToString_Never_Reveals_The_Key()
    {
        var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var options = new ZipCompressorOptions { HmacKey = key };

        Assert.DoesNotContain(key, options.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Binds_From_Configuration()
    {
        var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var configuration = Configuration(
            ("Compression:HmacKey", key),
            ("Compression:Limits:MaxEntryCount", "5"));

        var options = configuration.GetSection("Compression").Get<ZipCompressorOptions>()!;

        Assert.Equal(key, options.HmacKey);
        Assert.Equal(5, options.Limits.MaxEntryCount);
        Assert.Equal(new ExtractionLimits().MaxTotalUncompressedBytes, options.Limits.MaxTotalUncompressedBytes);
    }

    [Fact]
    public void Binding_Limits_Never_Mutates_The_Shared_Defaults()
    {
        var configuration = Configuration(("Compression:Limits:MaxEntryCount", "5"));

        _ = configuration.GetSection("Compression").Get<ZipCompressorOptions>();

        Assert.Equal(10_000, new ExtractionLimits().MaxEntryCount);
        Assert.Equal(10_000, new ZipCompressorOptions().Limits.MaxEntryCount);
    }

    [Fact]
    public async Task Bound_Key_Is_Used_For_Hmac()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var configuration = Configuration(("Compression:HmacKey", Convert.ToBase64String(key)));
        var sut = ZipCompressorSut.Create(configuration.GetSection("Compression").Get<ZipCompressorOptions>());

        var result = await sut.CompressFilesAsync(
            [_temp.CreateFile("a.txt", "A")], _temp.Combine("a.zip"), CompressionLevel.Optimal, false, TestContext.Current.CancellationToken);

        Assert.Equal(ChecksumAlgorithm.HmacSha256, result.Algorithm);
        Assert.Equal(Convert.ToHexStringLower(HMACSHA256.HashData(key, await SysFile.ReadAllBytesAsync(result.FilePath, TestContext.Current.CancellationToken))), result.Checksum);
    }
}
