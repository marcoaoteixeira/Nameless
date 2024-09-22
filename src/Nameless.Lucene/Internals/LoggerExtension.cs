using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Internals;

internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception> DeleteDocumentErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while deleting document.",
                               options: null);

    internal static void DeleteDocumentError(this ILogger self, Exception exception)
        => DeleteDocumentErrorHandler(self, exception);

    private static readonly Action<ILogger,
        Exception> StoreDocumentErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while storing document.",
                               options: null);

    internal static void StoreDocumentError(this ILogger self, Exception exception)
        => StoreDocumentErrorHandler(self, exception);

    private static readonly Action<ILogger,
        Exception> CommitChangesErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while commiting changes to index.",
                               options: null);

    internal static void CommitChangesError(this ILogger self, Exception exception)
        => CommitChangesErrorHandler(self, exception);

    private static readonly Action<ILogger,
        Exception> IndexWriterOutOfMemoryHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Index writer out of memory. Unfortunately all work will be lost and current instance will be disposed.",
                               options: null);

    internal static void IndexWriterOutOfMemory(this ILogger self, Exception exception)
        => IndexWriterOutOfMemoryHandler(self, exception);
}
