using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Internals;

internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        Exception> DeleteDocumentError
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while deleting document.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception> StoreDocumentError
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while storing document.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception> CommitChangesError
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while commiting changes to index.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception> IndexWriterOutOfMemory
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Index writer out of memory. Unfortunately all work will be lost and current instance will be disposed.",
                               options: null);
}
