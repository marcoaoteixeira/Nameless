using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless.Compression.Infrastructure;

/// <summary>
/// Builds the system under test the way a DI container would; silent logger unless one is given.
/// </summary>
internal static class ZipCompressorSut {
    public static ZipCompressor Create(byte[]? hmacKey, ILogger<ZipCompressor>? logger = null) {
        return Create(
            opts: new ZipCompressorOptions { HmacKey = hmacKey is not null ? Convert.ToBase64String(hmacKey) : null },
            logger: logger
        );
    }

    public static ZipCompressor Create(string? hmacKey, ILogger<ZipCompressor>? logger = null) {
        return Create(
            opts: new ZipCompressorOptions { HmacKey = hmacKey },
            logger: logger
        );
    }

    public static ZipCompressor Create(ZipCompressorOptions? opts = null, ILogger<ZipCompressor>? logger = null) {
        return new ZipCompressor(
            options: Options.Create(opts ?? new ZipCompressorOptions()),
            logger: logger ?? NullLogger<ZipCompressor>.Instance
        );
    }
}
