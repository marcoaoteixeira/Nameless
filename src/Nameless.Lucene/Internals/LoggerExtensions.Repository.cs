using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Internals;
internal static class IndexLoggerExtensions {
    private static readonly Action<ILogger, Exception> CreateFSDirectoryFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create the Lucene File System (FS) Directory."
        );

    private static readonly Action<ILogger, Exception> GetIndexReaderFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create/retrieve Lucene Index Reader."
        );

    private static readonly Action<ILogger, Exception> InsertDocumentsFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to insert the documents."
        );

    private static readonly Action<ILogger, Exception> RemoveDocumentsFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to remove the documents."
        );

    private static readonly Action<ILogger, Exception> GetQueryCountFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to get the total count of documents for the query."
        );

    private static readonly Action<ILogger, Exception> SearchFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while executing document search."
        );

    internal static void CreateFSDirectoryFailure(this ILogger<Index> self, Exception exception) {
        CreateFSDirectoryFailureDelegate(self, exception);
    }

    internal static void GetIndexReaderFailure(this ILogger<Index> self, Exception exception) {
        GetIndexReaderFailureDelegate(self, exception);
    }

    internal static void InsertDocumentsFailure(this ILogger<Index> self, Exception exception) {
        InsertDocumentsFailureDelegate(self, exception);
    }

    internal static void RemoveDocumentsFailure(this ILogger<Index> self, Exception exception) {
        RemoveDocumentsFailureDelegate(self, exception);
    }

    internal static void GetQueryCountFailure(this ILogger<Index> self, Exception exception) {
        GetQueryCountFailureDelegate(self, exception);
    }

    internal static void SearchFailure(this ILogger<Index> self, Exception exception) {
        SearchFailureDelegate(self, exception);
    }
}
