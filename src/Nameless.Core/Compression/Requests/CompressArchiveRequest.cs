using System.IO.Compression;
using Nameless.Helpers;

namespace Nameless.Compression.Requests;

/// <summary>
///     Represents a request to create a compressed archive
///     from a collection of files and directories.
/// </summary>
public class CompressArchiveRequest {
    private readonly HashSet<FileEntry> _files = [];

    /// <summary>
    ///     Gets or sets the full file system path
    ///     where the output file will be saved.
    /// </summary>
    public required string DestinationFilePath { get; set; }
    
    /// <summary>
    ///     Gets or sets the compression level to use when processing data.
    /// </summary>
    public CompressionLevel CompressionLevel { get; set; }

    /// <summary>
    ///     Gets the collection of files to be added to the archive.
    /// </summary>
    public IEnumerable<FileEntry> Files => _files;

    /// <summary>
    ///     Adds a file to the archive request, specifying its path and
    ///     optional directory path within the archive.
    /// </summary>
    /// <param name="path">
    ///     The file path to the file to include in the archive.
    /// </param>
    /// <param name="directoryPath">
    ///     The directory path within the archive where the file will be
    ///     placed. If <see langword="null"/> or white space, the file is
    ///     added to the root of the archive.
    /// </param>
    /// <returns>
    ///     The current <see cref="CompressArchiveRequest"/> instance with so
    ///     other actions can be chained.
    /// </returns>
    public CompressArchiveRequest IncludeFile(string path, string? directoryPath = null) {
        Guard.Against.NullOrWhiteSpace(path);

        var entry = new FileEntry(
            Path: PathHelper.Normalize(path),
            DirectoryPath: !string.IsNullOrWhiteSpace(directoryPath)
                ? PathHelper.Normalize(directoryPath)
                : null
        );

        _files.Add(entry);

        return this;
    }

    /// <summary>
    ///     Includes all files from the specified directory and its
    ///     subdirectories in the archive request.
    /// </summary>
    /// <param name="path">
    ///     The full path to the directory whose files are to be included.
    /// </param>
    /// <returns>
    ///     The current <see cref="CompressArchiveRequest"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">
    ///     Thrown if the directory specified by <paramref name="path"/>
    ///     does not exist.
    /// </exception>
    public CompressArchiveRequest IncludeDirectory(string path) {
        Guard.Against.NullOrWhiteSpace(path);

        path = PathHelper.Normalize(path);

        var directory = new DirectoryInfo(path);
        if (!directory.Exists) {
            throw new DirectoryNotFoundException($"Directory '{path}' not found.");
        }

        var root = directory.Parent is not null
            ? directory.Parent.FullName
            : directory.FullName;

        var files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files) {
            var relativeFilePath = Path.GetRelativePath(root, file.FullName);

            IncludeFile(
                path: file.FullName,
                directoryPath: Path.GetDirectoryName(relativeFilePath)
            );
        }

        return this;
    }
}