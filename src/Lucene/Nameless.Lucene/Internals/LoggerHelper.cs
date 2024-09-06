using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Internals;

internal static class LoggerHelper {
    internal static Action<ILogger, Exception> IndexWriterOutOfMemory = LoggerMessage.Define(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Index writer out of memory, disposing current instance.",
        options: null
    );

    internal static Action<ILogger, Exception> IndexWriterCommitFailure = LoggerMessage.Define(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Error while commiting documents to the index.",
        options: null
    );

    internal static Action<ILogger, Exception> DeleteIndexDirectory = LoggerMessage.Define(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Error while trying to delete index directory.",
        options: null
    );
}
