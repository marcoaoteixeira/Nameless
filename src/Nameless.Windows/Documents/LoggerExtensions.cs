using Microsoft.Extensions.Logging;
using Nameless.Windows.Documents.Impl;

namespace Nameless.Windows.Documents;

internal static class LoggerExtensions {
    extension(ILogger<IDocumentReader> self) {
        internal void GetContentFailure(string filePath, Exception exception) {
            Log.DocumentReaderGetContentFailure(self, filePath, exception);
        }
    }

    extension(ILogger<XpsDocumentConverter> self) {
        internal void ConvertFailure(string filePath, Exception exception) {
            Log.XpsDocumentConverterConvertFailure(self, filePath, exception);
        }
    }
}
