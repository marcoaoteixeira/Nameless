using Microsoft.Extensions.Logging;

namespace Nameless.Search.Lucene;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception> DeleteIndexDirectoryErrorDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: Events.DeleteIndexDirectoryErrorEvent,
            formatString: "An error occured while trying to delete the index {IndexName}");

    private static readonly Action<ILogger, Exception> DeleteDocumentsUnhandledErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.DeleteDocumentsUnhandledErrorEvent,
            formatString: "An error occured while deleting documents.");

    private static readonly Action<ILogger, Exception> StoreDocumentsUnhandledErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.StoreDocumentsUnhandledErrorEvent,
            formatString: "An error occured while storing documents.");

    private static readonly Action<ILogger, Exception> CommitChangesUnhandledErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.CommitChangesUnhandledErrorEvent,
            formatString: "An error occured while commiting documents to the index.");

    private static readonly Action<ILogger, Exception> IndexWriterOutOfMemoryErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.IndexWriterOutOfMemoryErrorEvent,
            formatString: "Index writer out of memory. Unfortunately all work will be lost and current instance will be disposed.");

    private static readonly Action<ILogger, string, Exception?> IndexRemovedFromCacheDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: Events.IndexRemovedFromCacheEvent,
            formatString: "Index '{IndexName}' has been removed from cache and disposed.");

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
        IndexWriterOutOfMemoryErrorDelegate(self, exception);
    }

    internal static void IndexRemovedFromCache(this ILogger self, string indexName) {
        IndexRemovedFromCacheDelegate(self, indexName, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId DeleteIndexDirectoryErrorEvent = new(9001, nameof(DeleteIndexDirectoryError));
        internal static readonly EventId DeleteDocumentsUnhandledErrorEvent = new(9002, nameof(DeleteDocumentsUnhandledError));
        internal static readonly EventId StoreDocumentsUnhandledErrorEvent = new(9003, nameof(StoreDocumentsUnhandledError));
        internal static readonly EventId CommitChangesUnhandledErrorEvent = new(9004, nameof(CommitChangesUnhandledError));
        internal static readonly EventId IndexWriterOutOfMemoryErrorEvent = new(9005, nameof(IndexWriterOutOfMemoryError));
        internal static readonly EventId IndexRemovedFromCacheEvent = new(9006, nameof(IndexRemovedFromCache));
    }
}