using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;

namespace Nameless.Compression;

public sealed class ZipBombTests : IDisposable
{
    private readonly TempDirectory _temp = new();

    public void Dispose() {
        _temp.Dispose();
    }

    private string Out => _temp.Combine("out");

    private void AssertNothingExtracted() {
        Assert.True(!SysDirectory.Exists(Out) || !SysDirectory.EnumerateFileSystemEntries(Out).Any());
    }

    private static byte[] Zeros(int count) {
        return new byte[count];
    }

    private static byte[] Incompressible(int count)
    {
        var data = new byte[count];
        new Random(42).NextBytes(data);
        return data;
    }

    [Fact]
    public async Task Too_Many_Entries_Is_Rejected_Before_Extraction()
    {
        var sut = ZipCompressorSut.Create(new ZipCompressorOptions { Limits = new ExtractionLimits { MaxEntryCount = 2 } });
        var archive = _temp.CreateArchive(("a.txt", "a"), ("b.txt", "b"), ("c.txt", "c"));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        AssertNothingExtracted();
    }

    [Fact]
    public async Task Declared_Total_Size_Above_Limit_Is_Rejected_Before_Extraction()
    {
        var sut = ZipCompressorSut.Create(new ZipCompressorOptions { Limits = new ExtractionLimits { MaxTotalUncompressedBytes = 100 } });
        var archive = _temp.CreateBinaryArchive(("a.bin", Incompressible(80)), ("b.bin", Incompressible(80)));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        AssertNothingExtracted();
    }

    [Fact]
    public async Task Archive_Exactly_At_Limits_Is_Extracted()
    {
        var sut = ZipCompressorSut.Create(new ZipCompressorOptions { Limits = new ExtractionLimits { MaxEntryCount = 2, MaxTotalUncompressedBytes = 160 } });
        var archive = _temp.CreateBinaryArchive(("a.bin", Incompressible(80)), ("b.bin", Incompressible(80)));

        await sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken);

        Assert.Equal(2, SysDirectory.GetFiles(Out).Length);
    }

    [Fact]
    public async Task High_Compression_Ratio_Above_Threshold_Is_Rejected()
    {
        var sut = ZipCompressorSut.Create();
        var archive = _temp.CreateBinaryArchive(("bomb.bin", Zeros(2 * 1024 * 1024)));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken));

        AssertNothingExtracted();
    }

    [Fact]
    public async Task High_Compression_Ratio_Below_Threshold_Is_Allowed()
    {
        var sut = ZipCompressorSut.Create();
        var archive = _temp.CreateBinaryArchive(("zeros.bin", Zeros(100 * 1024)));

        await sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken);

        Assert.Equal(100 * 1024, new FileInfo(SysPath.Combine(Out, "zeros.bin")).Length);
    }

    [Fact]
    public async Task Zero_Threshold_Applies_Ratio_To_Every_Entry() {
        var opts = new ZipCompressorOptions {
            Limits = new ExtractionLimits {
                CompressionRatioThresholdBytes = 0
            }
        };
        var sut = ZipCompressorSut.Create(opts);
        var archive = _temp.CreateBinaryArchive(("zeros.bin", Zeros(100 * 1024)));
        
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task Entry_Larger_Than_Declared_Size_Is_Rejected_And_Partial_File_Removed()
    {
        var sut = ZipCompressorSut.Create();
        var honest = ZipInspector.CreateBinaryArchive(("liar.bin", Incompressible(64 * 1024)));
        var lying = ZipInspector.WithDeclaredUncompressedSize(honest, declaredSize: 10);

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.DecompressAsync(_temp.CreateFile("lying.zip", lying), Out, null, false, TestContext.Current.CancellationToken));

        Assert.False(SysFile.Exists(SysPath.Combine(Out, "liar.bin")));
    }

    [Fact]
    public async Task Entry_With_Wrong_Crc32_Is_Rejected_And_Partial_File_Removed()
    {
        var sut = ZipCompressorSut.Create();
        var honest = ZipInspector.CreateBinaryArchive(("corrupt.bin", Incompressible(1024)));
        var corrupt = ZipInspector.WithDeclaredCrc32(honest, crc32: 0xDEADBEEF);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(() => sut.DecompressAsync(_temp.CreateFile("corrupt.zip", corrupt), Out, null, false, TestContext.Current.CancellationToken));

        Assert.Contains("corrupt.bin", ex.Message);
        Assert.False(SysFile.Exists(SysPath.Combine(Out, "corrupt.bin")));
    }

    [Fact]
    public async Task Empty_Entry_Passes_Crc32_Verification()
    {
        var sut = ZipCompressorSut.Create();
        var archive = _temp.CreateBinaryArchive(("empty.bin", []));

        await sut.DecompressAsync(archive, Out, null, false, TestContext.Current.CancellationToken);

        Assert.Equal(0, new FileInfo(SysPath.Combine(Out, "empty.bin")).Length);
    }
}

public sealed class ExtractionLimitsTests
{
    [Fact]
    public void Defaults_Are_Sensible()
    {
        var limits = new ExtractionLimits();

        Assert.Equal(10_000, limits.MaxEntryCount);
        Assert.Equal(1L << 30, limits.MaxTotalUncompressedBytes);
        Assert.Equal(100, limits.MaxCompressionRatio);
        Assert.Equal(1L << 20, limits.CompressionRatioThresholdBytes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaxEntryCount_Must_Be_Positive(int value)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ExtractionLimits { MaxEntryCount = value });

        Assert.Equal(nameof(ExtractionLimits.MaxEntryCount), ex.ParamName);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void MaxTotalUncompressedBytes_Must_Be_Positive(long value)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ExtractionLimits { MaxTotalUncompressedBytes = value });

        Assert.Equal(nameof(ExtractionLimits.MaxTotalUncompressedBytes), ex.ParamName);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void MaxCompressionRatio_Must_Be_Finite_And_At_Least_One(double value)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ExtractionLimits { MaxCompressionRatio = value });

        Assert.Equal(nameof(ExtractionLimits.MaxCompressionRatio), ex.ParamName);
    }

    [Fact]
    public void CompressionRatioThresholdBytes_Must_Not_Be_Negative()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ExtractionLimits { CompressionRatioThresholdBytes = -1 });

        Assert.Equal(nameof(ExtractionLimits.CompressionRatioThresholdBytes), ex.ParamName);
    }

    [Fact]
    public async Task Default_Options_Apply_Default_Limits()
    {
        // Default ratio limit (100:1 above 1 MiB) must still apply.
        var sut = ZipCompressorSut.Create();
        using var temp = new TempDirectory();
        var archive = temp.CreateBinaryArchive(("bomb.bin", new byte[2 * 1024 * 1024]));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.DecompressAsync(archive, temp.Combine("out"), null, false, TestContext.Current.CancellationToken));
    }
}
