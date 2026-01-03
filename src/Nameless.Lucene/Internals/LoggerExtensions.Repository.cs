using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Internals;

internal static class IndexLoggerExtensions {
    private static readonly Action<ILogger, Exception> CreateFSDirectoryFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create the Lucene File System (FS) Directory."
        );

    private static readonly Action<ILogger, Exception> GetIndexReaderFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create/retrieve Lucene Index Reader."
        );

    private static readonly Action<ILogger, Exception> InsertDocumentsFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to insert the documents."
        );

    private static readonly Action<ILogger, Exception> DeleteDocumentsFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to delete the documents."
        );

    private static readonly Action<ILogger, Exception> GetQueryCountFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to get the total count of documents for the query."
        );

    private static readonly Action<ILogger, Exception> SearchFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while executing document search."
        );

    extension(ILogger<Index> self) {
        internal void CreateFSDirectoryFailure(Exception exception) {
            CreateFSDirectoryFailureDelegate(self, exception);
        }

        internal void GetIndexReaderFailure(Exception exception) {
            GetIndexReaderFailureDelegate(self, exception);
        }

        internal void InsertDocumentsFailure(Exception exception) {
            InsertDocumentsFailureDelegate(self, exception);
        }

        internal void DeleteDocumentsFailure(Exception exception) {
            DeleteDocumentsFailureDelegate(self, exception);
        }

        internal void GetQueryCountFailure(Exception exception) {
            GetQueryCountFailureDelegate(self, exception);
        }

        internal void SearchFailure(Exception exception) {
            SearchFailureDelegate(self, exception);
        }
    }
}