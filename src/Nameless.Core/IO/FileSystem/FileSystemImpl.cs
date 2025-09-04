using Microsoft.Extensions.Options;
using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

/// <summary>
///     Default implementation of <see cref="IFileSystem"/>.
/// </summary>
public class FileSystemImpl : IFileSystem {
    private readonly IOptions<FileSystemOptions> _options;

    private FileSystemOptions Options => _options.Value;

    /// <inheritdoc />
    public string Root => Options.Root;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="FileSystemImpl"/> class.
    /// </summary>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public FileSystemImpl(IOptions<FileSystemOptions> options) {
        _options = Guard.Against.Null(options);

        Initialize();
    }

    /// <inheritdoc />
    public IDirectory GetDirectory(string relativePath) {
        var path = GetFullPath(relativePath);
        var directory = new DirectoryInfo(path);

        return new DirectoryWrapper(directory, _options);
    }

    /// <inheritdoc />
    public IFile GetFile(string relativePath) {
        var path = GetFullPath(relativePath);
        var file = new FileInfo(path);

        return new FileWrapper(file, _options);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     If the <paramref name="relativePath"/> is rooted, then it will
    ///     return the full path given its root. Otherwise, the path root
    ///     will be related to the current <see cref="IFileSystem"/>.
    /// </remarks>
    public string GetFullPath(string relativePath) {
        var normalizeRelativePath = PathHelper.Normalize(relativePath);

        var path = Path.IsPathRooted(normalizeRelativePath)
            ? Path.GetFullPath(normalizeRelativePath)
            : Path.GetFullPath(normalizeRelativePath, Options.Root);

        _options.Value.EnsureRootDirectory(path);

        return path;
    }

    private void Initialize() {
        if (string.IsNullOrWhiteSpace(Options.Root)) {
            throw new InvalidOperationException("Root directory not provided.");
        }

        if (!Directory.Exists(Options.Root)) {
            throw new DirectoryNotFoundException("Root directory was not found.");
        }
    }
}