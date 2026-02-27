// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Infrastructure.Implementations;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception> InsertDocumentsFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to insert documents into index '{IndexName}'."
        );

    private static readonly Action<ILogger, string, Exception> DeleteDocumentsFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to delete documents from index '{IndexName}'."
        );

    private static readonly Action<ILogger, string, Exception> SearchDocumentsFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while executing search documents from index '{IndexName}'."
        );

    private static readonly Action<ILogger, string, Exception> CountDocumentsFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to retrieve the total number of documents from index '{IndexName}'."
        );

    private static readonly Action<ILogger, string, Exception> SaveChangesFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to save changes done to index '{IndexName}'."
        );

    private static readonly Action<ILogger, string, Exception> CreateFSDirectoryFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create the Lucene File System (FS) Directory for index '{IndexName}'."
        );

    private static readonly Action<ILogger, string, Exception> GetIndexReaderFailureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create/retrieve the Lucene Index Reader object for index '{IndexName}'."
        );

    extension(ILogger<Index> self) {
        internal void InsertDocumentsFailure(string indexName, Exception exception) {
            InsertDocumentsFailureDelegate(self, indexName, exception);
        }

        internal void DeleteDocumentsFailure(string indexName, Exception exception) {
            DeleteDocumentsFailureDelegate(self, indexName, exception);
        }

        internal void SearchDocumentsFailure(string indexName, Exception exception) {
            SearchDocumentsFailureDelegate(self, indexName, exception);
        }

        internal void CountDocumentsFailure(string indexName, Exception exception) {
            CountDocumentsFailureDelegate(self, indexName, exception);
        }

        internal void SaveChangesFailure(string indexName, Exception exception) {
            SaveChangesFailureDelegate(self, indexName, exception);
        }

        internal void CreateFSDirectoryFailure(string indexName, Exception exception) {
            CreateFSDirectoryFailureDelegate(self, indexName, exception);
        }

        internal void GetIndexReaderFailure(string indexName, Exception exception) {
            GetIndexReaderFailureDelegate(self, indexName, exception);
        }
    }
}
