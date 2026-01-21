using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Nameless.Compression;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> CompressArchiveFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while compressing the files."
        );

    private static readonly Action<ILogger, Exception> DecompressArchiveFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while decompressing the files."
        );

    extension(ILogger<ZipArchiveService> self) {
        internal void CompressArchiveFailure(Exception ex) {
            CompressArchiveFailureDelegate(self, ex);
        }

        internal void DecompressArchiveFailure(Exception ex) {
            DecompressArchiveFailureDelegate(self, ex);
        }
    }
}
