// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;

namespace Nameless.Lucene;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, string, string, Exception> IndexActionFailureDelegate
        = LoggerMessage.Define<string, string, string>(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to execute '{Action}' on index '{IndexName}': {Reason}"
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
            IndexActionFailureDelegate(self, nameof(IIndex.Insert), indexName, exception.Message, exception);
        }

        internal void DeleteDocumentsFailure(string indexName, Exception exception) {
            IndexActionFailureDelegate(self, nameof(IIndex.Delete), indexName, exception.Message, exception);
        }

        internal void UpdateDocumentFailure(string indexName, Exception exception) {
            IndexActionFailureDelegate(self, nameof(IIndex.Update), indexName, exception.Message, exception);
        }

        internal void SearchDocumentsFailure(string indexName, Exception exception) {
            IndexActionFailureDelegate(self, nameof(IIndex.Search), indexName, exception.Message, exception);
        }

        internal void CountDocumentsFailure(string indexName, Exception exception) {
            IndexActionFailureDelegate(self, nameof(IIndex.Count), indexName, exception.Message, exception);
        }

        internal void RollbackFailure(string indexName, Exception exception) {
            IndexActionFailureDelegate(self, nameof(IIndex.Rollback), indexName, exception.Message, exception);
        }

        internal void SaveChangesFailure(string indexName, Exception exception) {
            IndexActionFailureDelegate(self, nameof(IIndex.SaveChanges), indexName, exception.Message, exception);
        }

        internal void CreateFSDirectoryFailure(string indexName, Exception exception) {
            CreateFSDirectoryFailureDelegate(self, indexName, exception);
        }

        internal void GetIndexReaderFailure(string indexName, Exception exception) {
            GetIndexReaderFailureDelegate(self, indexName, exception);
        }
    }
}
