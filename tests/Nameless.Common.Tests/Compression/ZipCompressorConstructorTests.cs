using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless.Compression;

public sealed class ZipCompressorConstructorTests
{
    private sealed class NullValueOptions : IOptions<ZipCompressorOptions>
    {
        public ZipCompressorOptions Value => null!;
    }

    [Fact]
    public void Throws_ArgumentNullException_When_Options_Is_Null()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ZipCompressor(null!, NullLogger<ZipCompressor>.Instance));

        Assert.Equal("options", ex.ParamName);
    }

    [Fact]
    public void Throws_ArgumentNullException_When_Options_Value_Is_Null()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ZipCompressor(new NullValueOptions(), NullLogger<ZipCompressor>.Instance));

        Assert.Equal("options", ex.ParamName);
    }

    [Fact]
    public void Throws_ArgumentNullException_When_Logger_Is_Null()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ZipCompressor(Options.Create(new ZipCompressorOptions()), null!));

        Assert.Equal("logger", ex.ParamName);
    }

    [Fact]
    public void Default_Options_Are_Accepted()
    {
        _ = new ZipCompressor(Options.Create(new ZipCompressorOptions()), NullLogger<ZipCompressor>.Instance);
    }
}
