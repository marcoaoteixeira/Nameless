using Microsoft.Extensions.Logging;

namespace Nameless.Compression.Zip;

internal static partial class Log {
    private const string TAG = "ZIP_COMPRESSOR";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while executing compression.")]
    internal static partial void CompressAsyncFailure(ILogger<ICompressor> logger, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while executing decompression.")]
    internal static partial void DecompressAsyncFailure(ILogger<ICompressor> logger, Exception exception, string tag = TAG);
}