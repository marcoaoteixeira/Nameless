using Microsoft.Extensions.Logging;

namespace Nameless.Compression;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> CompressStartingDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Starting compression of files..."
        );

    private static readonly Action<ILogger, long, Exception?> CompressFinishedDelegate
        = LoggerMessage.Define<long>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Compression finished in {ElapsedMilliseconds}ms."
        );

    private static readonly Action<ILogger, Exception> CompressFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while compressing the files."
        );

    private static readonly Action<ILogger, Exception?> DecompressStartingDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Starting decompression of files..."
        );

    private static readonly Action<ILogger, long, Exception?> DecompressFinishedDelegate
        = LoggerMessage.Define<long>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Decompression finished in {ElapsedMilliseconds}ms."
        );

    private static readonly Action<ILogger, Exception> DecompressFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while decompressing the files."
        );

    extension(ILogger<ZipFileService> self) {
        internal void CompressStarting() {
            CompressStartingDelegate(self, null /* exception */);
        }

        internal void CompressFinished(long elapsedMilliseconds) {
            CompressFinishedDelegate(self, elapsedMilliseconds, null /* exception */);
        }

        internal void CompressFailure(Exception ex) {
            CompressFailureDelegate(self, ex);
        }

        internal void DecompressStarting() {
            DecompressStartingDelegate(self, null /* exception */);
        }

        internal void DecompressFinished(long elapsedMilliseconds) {
            DecompressFinishedDelegate(self, elapsedMilliseconds, null /* exception */);
        }

        internal void DecompressFailure(Exception ex) {
            DecompressFailureDelegate(self, ex);
        }
    }
}
