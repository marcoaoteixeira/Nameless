using Microsoft.Extensions.Logging;

namespace Nameless.Compression.Zip;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class LoggerExtensions {
    extension(ILogger<ZipCompressor> self) {
        internal void CompressFailure(Exception ex) {
            Log.Failure(self, "ZIP_COMPRESS", nameof(ICompressor.CompressAsync), ex);
        }

        internal void DecompressFailure(Exception ex) {
            Log.Failure(self, "ZIP_COMPRESS", nameof(ICompressor.DecompressAsync), ex);
        }
    }
}
