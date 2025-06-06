using Microsoft.Extensions.Logging;

namespace Nameless.Search.Lucene;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception> DeleteIndexDirectoryErrorDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occured while trying to delete the index {IndexName}");

    private static readonly Action<ILogger, Exception> DeleteDocumentsUnhandledErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occured while deleting documents.");

    private static readonly Action<ILogger, Exception> StoreDocumentsUnhandledErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occured while storing documents.");

    private static readonly Action<ILogger, Exception> CommitChangesUnhandledErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occured while commiting documents to the index.");

    private static readonly Action<ILogger,
        Exception> IndexWriterOutOfMemoryErrorHandler
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Index writer out of memory. Unfortunately all work will be lost and current instance will be disposed.");

    internal static void DeleteIndexDirectoryError(this ILogger self, string indexName, Exception exception) {
        DeleteIndexDirectoryErrorDelegate(self, indexName, exception);
    }

    internal static void DeleteDocumentsUnhandledError(this ILogger self, Exception exception) {
        DeleteDocumentsUnhandledErrorDelegate(self, exception);
    }

    internal static void StoreDocumentsUnhandledError(this ILogger self, Exception exception) {
        StoreDocumentsUnhandledErrorDelegate(self, exception);
    }

    internal static void CommitChangesUnhandledError(this ILogger self, Exception exception) {
        CommitChangesUnhandledErrorDelegate(self, exception);
    }

    internal static void IndexWriterOutOfMemoryError(this ILogger self, Exception exception) {
        IndexWriterOutOfMemoryErrorHandler(self, exception);
    }
}