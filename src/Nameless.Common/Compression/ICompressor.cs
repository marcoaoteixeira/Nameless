using System.IO.Compression;

namespace Nameless.Compression;

/// <summary>
///     Compresses files and directories into ZIP archives, and extracts
///     them back.
/// </summary>
/// <remarks>
///     <para>
///         Entry names never carry drive letters or network (UNC) roots,
///         and extraction refuses any entry that would land outside the
///         destination directory (other drives, UNC paths, <c>..</c>
///         traversal, NTFS alternate data streams). On Windows, extraction
///         also refuses entries using reserved device names (<c>CON</c>,
///         <c>NUL</c>, <c>COM1</c>, ...); other operating systems extract
///         them normally.
///     </para>
///     <para>
///         Symbolic links and junctions found <em>inside</em> a source
///         directory are ignored (neither stored nor followed). Paths the
///         caller names explicitly — a listed file, the root directory —
///         are followed.
///     </para>
/// </remarks>
public interface ICompressor {
    /// <summary>
    ///     Compresses one or more files.
    /// </summary>
    /// <param name="filePaths">
    ///     Fully qualified paths to existing files.
    /// </param>
    /// <param name="destinationFilePath">
    ///     Archive path. Missing parent directories are created.
    /// </param>
    /// <param name="compressionLevel">
    ///     Compression level applied to every entry.
    /// </param>
    /// <param name="overwrite">
    ///     Whether an existing archive at the destination may be replaced.
    /// </param>
    /// <param name="cancellationToken">
    ///     Token to cancel the operation.
    /// </param>
    /// <returns>
    ///     The archive path and its checksum (SHA-256, or HMAC-SHA256 when
    ///     the compressor has a key).
    /// </returns>
    Task<CompressionResult> CompressFilesAsync(IEnumerable<string> filePaths, string destinationFilePath, CompressionLevel compressionLevel, bool overwrite, CancellationToken cancellationToken);

    /// <summary>
    ///     Compresses a directory. Entries keep their path relative to the
    ///     directory, under the directory's base folder name. Symbolic links
    ///     and junctions inside the directory are ignored.
    /// </summary>
    /// <param name="directoryPath">
    ///     Path to an existing directory.
    /// </param>
    /// <param name="globPattern">
    ///     Glob relative to <paramref name="directoryPath"/> (e.g.
    ///     <c>*.txt</c>, <c>**/*.cs</c>, <c>src/*/bin/*</c>); only matching
    ///     files are stored, and no matches produce an empty archive.
    ///     <see langword="null"/> stores the whole directory, empty
    ///     sub-directories included. The pattern cannot leave the directory:
    ///     rooted patterns and <c>..</c> segments are rejected.
    /// </param>
    /// <param name="destinationFilePath">
    ///     Archive path. Missing parent directories are created. An archive
    ///     placed inside the directory is never stored into itself.
    /// </param>
    /// <param name="compressionLevel">
    ///     Compression level applied to every entry.
    /// </param>
    /// <param name="overwrite">
    ///     Whether an existing archive at the destination may be replaced.
    /// </param>
    /// <param name="cancellationToken">
    ///     Token to cancel the operation.
    /// </param>
    /// <returns>
    ///     The archive path and its checksum (SHA-256, or HMAC-SHA256 when
    ///     the compressor has a key).
    /// </returns>
    Task<CompressionResult> CompressDirectoryAsync(string directoryPath, string? globPattern, string destinationFilePath, CompressionLevel compressionLevel, bool overwrite, CancellationToken cancellationToken);

    /// <summary>
    ///     Extracts a ZIP archive file.
    /// </summary>
    /// <param name="filePath">
    ///     Path to an existing archive.
    /// </param>
    /// <param name="destinationDirectoryPath">
    ///     Target directory (created if missing).
    /// </param>
    /// <param name="checksum">
    ///     Expected checksum of the archive (SHA-256, or HMAC-SHA256 when
    ///     the compressor has a key; hex, case-insensitive);
    ///     <see langword="null"/> skips verification.
    /// </param>
    /// <param name="overwrite">
    ///     Whether existing files in the destination may be replaced.
    /// </param>
    /// <param name="cancellationToken">
    ///     Token to cancel the operation.
    /// </param>
    /// <returns>
    ///     The full path of the destination directory.
    /// </returns>
    Task<string> DecompressAsync(string filePath, string destinationDirectoryPath, string? checksum, bool overwrite, CancellationToken cancellationToken);
}
