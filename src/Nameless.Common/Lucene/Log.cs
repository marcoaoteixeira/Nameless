using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Lucene;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGen)]
internal static partial class Log {
    private const string TAG = "LUCENE";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while inserting the documents into index '{Index}'.")]
    internal static partial void InsertFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while removing documents from index '{Index}'.")]
    internal static partial void DeleteFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while updating documents in index '{Index}'.")]
    internal static partial void UpdateFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while searching documents in index '{Index}'.")]
    internal static partial void SearchFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while counting documents in index '{Index}'.")]
    internal static partial void CountFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while rolling back changes in index '{Index}'.")]
    internal static partial void RollbackFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while commiting changes in index '{Index}'.")]
    internal static partial void SaveChangesFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while creating the Lucene File System Directory (FSDirectory) for index '{Index}'.")]
    internal static partial void CreateFSDirectoryFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while creating an instance of the IndexReader for index '{Index}'.")]
    internal static partial void GetIndexReaderFailure(ILogger<Index> logger, string index, Exception exception, string tag = TAG);
}