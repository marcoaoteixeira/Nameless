using Microsoft.Extensions.Options;

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
    public string GetFullPath(string relativePath) {
        var path = Path.GetFullPath(relativePath, Options.Root);

        Options.EnsureRootDirectory(path);

        return path;
    }
}