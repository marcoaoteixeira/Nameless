using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace Nameless.Compression;

/// <summary>
///     High-performance log messages (source generated). Checksums and keys
///     are never logged.
/// </summary>
internal static partial class Log {
    // ---- Compression (1xx) ------------------------------------------------

    [LoggerMessage(EventId = 100, Level = LogLevel.Debug, Message = "Compressing '{Source}' into '{Destination}' (level {CompressionLevel}).")]
    public static partial void CompressionStarted(ILogger logger, string source, string destination, CompressionLevel compressionLevel);

    [LoggerMessage(EventId = 101, Level = LogLevel.Debug, Message = "Glob '{GlobPattern}' matched {Count} files in '{Root}'.")]
    public static partial void GlobMatched(ILogger logger, string globPattern, int count, string root);

    [LoggerMessage(EventId = 102, Level = LogLevel.Debug, Message = "Ignoring link '{LinkPath}' inside '{Root}'.")]
    public static partial void LinkSkipped(ILogger logger, string linkPath, string root);

    [LoggerMessage(EventId = 103, Level = LogLevel.Trace, Message = "Adding entry '{EntryName}' from '{Source}'.")]
    public static partial void EntryAdded(ILogger logger, string entryName, string source);

    [LoggerMessage(EventId = 104, Level = LogLevel.Information, Message = "Created archive '{Destination}' with {EntryCount} entries ({Bytes} bytes, {Algorithm}) in {ElapsedMilliseconds:0} ms.")]
    public static partial void CompressionCompleted(ILogger logger, string destination, int entryCount, long bytes, ChecksumAlgorithm algorithm, double elapsedMilliseconds);

    [LoggerMessage(EventId = 105, Level = LogLevel.Debug, Message = "Compressing '{Source}' failed.")]
    public static partial void CompressionFailed(ILogger logger, Exception exception, string source);

    // ---- Extraction (2xx) -------------------------------------------------

    [LoggerMessage(EventId = 200, Level = LogLevel.Debug, Message = "Extracting into '{Destination}': {EntryCount} entries, {DeclaredBytes} bytes declared.")]
    public static partial void ExtractionPlanned(ILogger logger, string destination, int entryCount, long declaredBytes);

    [LoggerMessage(EventId = 201, Level = LogLevel.Trace, Message = "Extracting entry '{EntryName}' to '{Target}'.")]
    public static partial void EntryExtracted(ILogger logger, string entryName, string target);

    [LoggerMessage(EventId = 202, Level = LogLevel.Information, Message = "Extracted {EntryCount} entries ({Bytes} bytes) into '{Destination}' in {ElapsedMilliseconds:0} ms.")]
    public static partial void ExtractionCompleted(ILogger logger, int entryCount, long bytes, string destination, double elapsedMilliseconds);

    [LoggerMessage(EventId = 203, Level = LogLevel.Debug, Message = "Extraction into '{Destination}' failed.")]
    public static partial void ExtractionFailed(ILogger logger, Exception exception, string destination);

    // ---- Security rejections (3xx) ----------------------------------------

    [LoggerMessage(EventId = 300, Level = LogLevel.Warning, Message = "Rejected archive entry '{EntryName}': {Reason}.")]
    public static partial void UnsafeEntryRejected(ILogger logger, string entryName, string reason);

    [LoggerMessage(EventId = 301, Level = LogLevel.Warning, Message = "Rejected archive: {Reason}")]
    public static partial void ArchiveRejected(ILogger logger, string reason);

    [LoggerMessage(EventId = 302, Level = LogLevel.Warning, Message = "Archive checksum mismatch ({Algorithm}) for '{Source}'; nothing was extracted.")]
    public static partial void ChecksumMismatch(ILogger logger, ChecksumAlgorithm algorithm, string source);
}
